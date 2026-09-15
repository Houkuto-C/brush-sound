using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace BrushSound
{
    public class HookManager : IDisposable
    {
        private IKeyboardMouseEvents hook;
        private readonly HashSet<string> pressedKeys = new HashSet<string>();
        private readonly HashSet<string> pressedMouse = new HashSet<string>();
        private readonly HashSet<string> activeCombos = new HashSet<string>();

        private readonly Dictionary<string, CancellationTokenSource> pendingModifiers
            = new Dictionary<string, CancellationTokenSource>();

        public int ModifierDelayMs { get; set; } = 120;
        public bool Capturing { get; set; } = false;

        public Func<int, int, bool> MouseLocationFilter { get; set; }

        public event Action<string> KeyPressed;
        public event Action<string> KeyReleased;
        public event Action<string> Captured;

        private static readonly HashSet<string> Modifiers
            = new HashSet<string> { "ctrl", "shift", "alt", "win" };

        private readonly object sync = new object();

        // 捕获模式下，记录当前组合
        private string captureLastCombo = null;

        public void Start()
        {
            hook = Hook.GlobalEvents();
            hook.KeyDown += OnKeyDown;
            hook.KeyUp += OnKeyUp;
            hook.MouseDown += OnMouseDown;
            hook.MouseUp += OnMouseUp;
        }

        public void Stop()
        {
            if (hook != null)
            {
                hook.KeyDown -= OnKeyDown;
                hook.KeyUp -= OnKeyUp;
                hook.MouseDown -= OnMouseDown;
                hook.MouseUp -= OnMouseUp;
                hook.Dispose();
                hook = null;
            }
            lock (sync)
            {
                pressedKeys.Clear();
                pressedMouse.Clear();
                activeCombos.Clear();
                captureLastCombo = null;
            }
            CancelAllPendingModifiers();
        }

        public void Dispose() { Stop(); }

        /// <summary>
        /// 开始捕获，重置内部状态
        /// </summary>
        public void StartCapture()
        {
            lock (sync)
            {
                captureLastCombo = null;
                Capturing = true;
            }
        }

        // ==================== 键盘 ====================
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            string name = NormalizeKey(e);
            if (name == null) return;

            lock (sync) pressedKeys.Add(name);

            // 捕获模式下：不触发延迟，只更新待捕获组合
            if (Capturing)
            {
                string combo = BuildComboForCapture();
                if (combo != null) captureLastCombo = combo;
                return;
            }

            if (Modifiers.Contains(name))
            {
                StartPendingModifier(name);
                return;
            }

            CancelAllPendingModifiers();

            string keyCombo = BuildKeyCombo();
            if (keyCombo == null) return;

            lock (sync) activeCombos.Add(keyCombo);
            KeyPressed?.Invoke(keyCombo);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            string name = NormalizeKey(e);
            if (name == null) return;

            lock (sync) pressedKeys.Remove(name);

            // 捕获模式下：所有键都松开时才算捕获完成
            if (Capturing)
            {
                if (pressedKeys.Count == 0 && pressedMouse.Count == 0 && captureLastCombo != null)
                {
                    string combo = captureLastCombo;
                    captureLastCombo = null;
                    Capturing = false;
                    Captured?.Invoke(combo);
                }
                return;
            }

            if (Modifiers.Contains(name)) CancelPendingModifier(name);
            CheckReleasedCombos();
        }

        private string BuildComboForCapture()
        {
            var mods = new List<string>();
            foreach (var m in new[] { "ctrl", "shift", "alt", "win" })
                if (pressedKeys.Contains(m)) mods.Add(m);
            mods.Sort();

            string main = null;
            foreach (var k in pressedKeys)
                if (!Modifiers.Contains(k)) { main = k; break; }

            if (main != null)
            {
                var parts = new List<string>(mods);
                parts.Add(main);
                return string.Join("+", parts);
            }
            // 只有修饰键
            if (mods.Count > 0) return string.Join("+", mods);
            return null;
        }

        private void CheckReleasedCombos()
        {
            List<string> released = null;
            lock (sync)
            {
                foreach (var combo in activeCombos)
                {
                    if (!IsComboStillPressed(combo))
                    {
                        if (released == null) released = new List<string>();
                        released.Add(combo);
                    }
                }
                if (released != null)
                    foreach (var c in released) activeCombos.Remove(c);
            }
            if (released != null)
                foreach (var c in released)
                    KeyReleased?.Invoke(c);
        }

        private bool IsComboStillPressed(string combo)
        {
            var parts = combo.Split('+');
            foreach (var p in parts)
            {
                if (p.StartsWith("mouse_"))
                {
                    if (!pressedMouse.Contains(p)) return false;
                }
                else
                {
                    if (!pressedKeys.Contains(p)) return false;
                }
            }
            return true;
        }

        // ==================== 修饰键延迟 ====================
        private void StartPendingModifier(string name)
        {
            CancellationTokenSource cts;
            lock (pendingModifiers)
            {
                if (pendingModifiers.TryGetValue(name, out var old))
                {
                    try { old.Cancel(); } catch { }
                    try { old.Dispose(); } catch { }
                }
                cts = new CancellationTokenSource();
                pendingModifiers[name] = cts;
            }

            int delay = ModifierDelayMs;
            var token = cts.Token;
            Task.Delay(delay, token).ContinueWith(t =>
            {
                if (t.IsCanceled) return;

                lock (pendingModifiers)
                {
                    if (!pendingModifiers.ContainsKey(name)) return;
                    pendingModifiers.Remove(name);
                }

                bool stillPressed;
                lock (sync)
                {
                    stillPressed = pressedKeys.Contains(name);
                    if (!stillPressed) return;

                    bool hasNonMod = false;
                    foreach (var k in pressedKeys)
                    {
                        if (!Modifiers.Contains(k)) { hasNonMod = true; break; }
                    }
                    if (hasNonMod) return;

                    activeCombos.Add(name);
                }

                KeyPressed?.Invoke(name);
            }, TaskScheduler.Default);
        }

        private void CancelPendingModifier(string name)
        {
            CancellationTokenSource cts = null;
            lock (pendingModifiers)
            {
                if (pendingModifiers.TryGetValue(name, out cts))
                    pendingModifiers.Remove(name);
            }
            if (cts != null)
            {
                try { cts.Cancel(); } catch { }
                try { cts.Dispose(); } catch { }
            }
        }

        private void CancelAllPendingModifiers()
        {
            List<CancellationTokenSource> list;
            lock (pendingModifiers)
            {
                list = new List<CancellationTokenSource>(pendingModifiers.Values);
                pendingModifiers.Clear();
            }
            foreach (var cts in list)
            {
                try { cts.Cancel(); } catch { }
                try { cts.Dispose(); } catch { }
            }
        }

        // ==================== 鼠标 ====================
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (MouseLocationFilter != null && MouseLocationFilter(e.X, e.Y)) return;

            string name = NormalizeMouse(e.Button);
            if (name == null) return;

            lock (sync) pressedMouse.Add(name);

            if (Capturing)
            {
                string combo = BuildMouseComboForCapture();
                if (combo != null) captureLastCombo = combo;
                return;
            }

            string mouseCombo = BuildMouseCombo();
            if (mouseCombo == null) return;

            lock (sync) activeCombos.Add(mouseCombo);
            KeyPressed?.Invoke(mouseCombo);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            string name = NormalizeMouse(e.Button);
            if (name == null) return;

            lock (sync) pressedMouse.Remove(name);

            if (Capturing)
            {
                if (pressedKeys.Count == 0 && pressedMouse.Count == 0 && captureLastCombo != null)
                {
                    string combo = captureLastCombo;
                    captureLastCombo = null;
                    Capturing = false;
                    Captured?.Invoke(combo);
                }
                return;
            }

            CheckReleasedCombos();
        }

        private string BuildMouseComboForCapture()
        {
            var mods = new List<string>();
            foreach (var m in new[] { "ctrl", "shift", "alt", "win" })
                if (pressedKeys.Contains(m)) mods.Add(m);
            mods.Sort();

            string main = null;
            foreach (var b in pressedMouse) { main = b; break; }
            if (main == null) return null;

            var parts = new List<string>(mods);
            parts.Add(main);
            return string.Join("+", parts);
        }

        // ==================== 工具 ====================
        private static string NormalizeKey(KeyEventArgs e)
        {
            Keys k = e.KeyCode;
            if (k == Keys.ControlKey || k == Keys.LControlKey || k == Keys.RControlKey) return "ctrl";
            if (k == Keys.ShiftKey || k == Keys.LShiftKey || k == Keys.RShiftKey) return "shift";
            if (k == Keys.Menu || k == Keys.LMenu || k == Keys.RMenu) return "alt";
            if (k == Keys.LWin || k == Keys.RWin) return "win";
            if (k >= Keys.A && k <= Keys.Z) return k.ToString().ToLower();
            if (k >= Keys.D0 && k <= Keys.D9) return ((int)(k - Keys.D0)).ToString();
            if (k >= Keys.NumPad0 && k <= Keys.NumPad9) return "numpad" + ((int)(k - Keys.NumPad0)).ToString();
            if (k >= Keys.F1 && k <= Keys.F12) return "f" + ((int)(k - Keys.F1) + 1).ToString();

            switch (k)
            {
                case Keys.Space: return "space";
                case Keys.Enter: return "enter";
                case Keys.Tab: return "tab";
                case Keys.Escape: return "escape";
                case Keys.Back: return "backspace";
                case Keys.Delete: return "delete";
                case Keys.Insert: return "insert";
                case Keys.Home: return "home";
                case Keys.End: return "end";
                case Keys.PageUp: return "pageup";
                case Keys.PageDown: return "pagedown";
                case Keys.Up: return "up";
                case Keys.Down: return "down";
                case Keys.Left: return "left";
                case Keys.Right: return "right";
                case Keys.OemMinus: return "-";
                case Keys.Oemplus: return "=";
                case Keys.OemOpenBrackets: return "[";
                case Keys.OemCloseBrackets: return "]";
                case Keys.OemPipe: return "\\";
                case Keys.OemSemicolon: return ";";
                case Keys.OemQuotes: return "'";
                case Keys.Oemcomma: return ",";
                case Keys.OemPeriod: return ".";
                case Keys.OemQuestion: return "/";
                case Keys.Oemtilde: return "`";
            }
            return null;
        }

        private static string NormalizeMouse(MouseButtons btn)
        {
            switch (btn)
            {
                case MouseButtons.Left: return "mouse_left";
                case MouseButtons.Right: return "mouse_right";
                case MouseButtons.Middle: return "mouse_middle";
                case MouseButtons.XButton1: return "mouse_x1";
                case MouseButtons.XButton2: return "mouse_x2";
            }
            return null;
        }

        private string BuildKeyCombo()
        {
            var mods = new List<string>();
            foreach (var m in new[] { "ctrl", "shift", "alt", "win" })
                if (pressedKeys.Contains(m)) mods.Add(m);
            mods.Sort();

            string main = null;
            foreach (var k in pressedKeys)
                if (!Modifiers.Contains(k)) { main = k; break; }
            if (main == null) return null;

            var parts = new List<string>(mods) { main };
            return string.Join("+", parts);
        }

        private string BuildMouseCombo()
        {
            var mods = new List<string>();
            foreach (var m in new[] { "ctrl", "shift", "alt", "win" })
                if (pressedKeys.Contains(m)) mods.Add(m);
            mods.Sort();

            string main = null;
            foreach (var b in pressedMouse) { main = b; break; }
            if (main == null) return null;

            var parts = new List<string>(mods) { main };
            return string.Join("+", parts);
        }
    }
}