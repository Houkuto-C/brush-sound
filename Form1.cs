using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BrushSound
{
    public partial class Form1 : Form
    {
        private class KeyItem
        {
            public string Key;
            public string Display;
            public override string ToString() { return Display; }
        }

        private class PresetItem
        {
            public bool IsCustom;
            public int Index;
            public string Name;
            public override string ToString()
            {
                return IsCustom ? ("⭐ " + Name) : Name;
            }
        }

        private HookManager hook;
        private AudioManager audio;
        private TrayIcon tray;
        private RoundButton btnPause;
        private ToolTip tooltip;
        private string soundFolder;
        private string configPath;
        private string presetsFolder;
        private bool loadingUI = false;
        private bool quitting = false;
        private volatile bool paused = false;
        private System.Windows.Forms.Timer statusTimer;

        private volatile int cachedLeft, cachedTop, cachedRight, cachedBottom;
        private volatile bool cachedVisible;
        private volatile bool cachedHasFocus;

        private readonly ConcurrentQueue<string> keyDisplayQueue = new ConcurrentQueue<string>();
        private System.Windows.Forms.Timer keyDisplayTimer;

        // 记录每个按键最后播放过的文件名（文件夹模式下显示）
        private readonly Dictionary<string, string> lastPlayedFile
            = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();
            // 从 exe 提取图标，设置给窗口和托盘
            try
            {
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }

            soundFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");
            configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            presetsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Presets");
            ThemeManager.PresetsFolder = presetsFolder;

            ConfigManager.Load(configPath);
            EnsureFolders();
            CleanupInvalidMappings();

            audio = new AudioManager(soundFolder) { Volume = ConfigManager.Current.Volume };
            audio.TrackChanged += (combo, fileName) =>
            {
                this.BeginInvoke(new Action(() => OnTrackChanged(combo, fileName)));
            };

            InitializeControls();
            CreatePauseButton();

            UpdateWindowBoundsCache();
            this.LocationChanged += (s, e) => UpdateWindowBoundsCache();
            this.SizeChanged += (s, e) => UpdateWindowBoundsCache();
            this.VisibleChanged += (s, e) => UpdateWindowBoundsCache();
            this.Activated += (s, e) => cachedHasFocus = true;
            this.Deactivate += (s, e) => cachedHasFocus = false;
            cachedHasFocus = this.ContainsFocus;

            hook = new HookManager();
            hook.ModifierDelayMs = ConfigManager.Current.ModifierDelayMs;
            hook.MouseLocationFilter = (x, y) =>
            {
                if (!cachedVisible) return false;
                return x >= cachedLeft && x <= cachedRight && y >= cachedTop && y <= cachedBottom;
            };
            hook.KeyPressed += Hook_KeyPressed;
            hook.KeyReleased += Hook_KeyReleased;
            hook.Captured += Hook_Captured;
            hook.Start();

            lblSoundDir.Text = "音效目录：" + soundFolder;
            RefreshFolderList();
            RefreshKeyList();

            SelectKeyInCombo(ConfigManager.Current.LastEditedKey);

            ThemeManager.Current = ResolveThemeByName(ConfigManager.Current.Theme);
            ApplyTheme();

            statusTimer = new System.Windows.Forms.Timer { Interval = 2500 };
            statusTimer.Tick += (s, e) =>
            {
                statusTimer.Stop();
                if (!paused) lblCurrentKeyDisplay.Text = "当前按键：—";
            };

            keyDisplayTimer = new System.Windows.Forms.Timer { Interval = 150 };
            keyDisplayTimer.Tick += (s, e) => ProcessKeyDisplayQueue();
            keyDisplayTimer.Start();

            tray = new TrayIcon(this);
            tray.ExitRequested += () => DoQuit();
            UpdateTrayTooltip();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!quitting && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                if (tray != null) tray.HideToTray();
                return;
            }
            Cleanup();
            base.OnFormClosing(e);
        }

        private void DoQuit()
        {
            quitting = true;
            Close();
        }

        private void Cleanup()
        {
            try { audio?.Dispose(); } catch { }
            try { hook?.Dispose(); } catch { }
            try { tray?.Dispose(); } catch { }
        }

        // ==================== 暂停按钮 ====================
        private void CreatePauseButton()
        {
            lblCurrentKeyDisplay.Size = new Size(400, 28);

            btnPause = new RoundButton
            {
                Text = "⏸ 暂停触发",
                Location = new Point(440, 63),
                Size = new Size(150, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                CornerRadius = 5,
                BackColor = Color.FromArgb(176, 42, 42),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPause.FlatAppearance.BorderSize = 0;
            btnPause.Click += BtnPause_Click;
            this.Controls.Add(btnPause);

            if (tooltip == null) tooltip = new ToolTip();
            tooltip.SetToolTip(btnPause, "暂停后所有按键音效将不再触发，再次点击恢复");
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            paused = !paused;
            if (paused) { try { audio?.StopAll(); } catch { } }
            UpdatePauseUI();
        }

        private void UpdatePauseUI()
        {
            if (btnPause == null) return;
            var c = ThemeManager.Current.Colors;
            if (paused)
            {
                btnPause.Text = "▶ 恢复触发";
                btnPause.BackColor = c.C_SuccessButton;
                lblCurrentKeyDisplay.Text = "⏸ 已暂停触发";
            }
            else
            {
                btnPause.Text = "⏸ 暂停触发";
                btnPause.BackColor = c.C_DangerButton;
                lblCurrentKeyDisplay.Text = "当前按键：—";
            }
            UpdateTrayTooltip();
        }

        private void UpdateTrayTooltip()
        {
            if (tray == null) return;
            string title = (ConfigManager.Current.CustomTitle ?? "").Trim();
            if (string.IsNullOrEmpty(title)) title = "画笔咻咻";
            if (paused) title += "（已暂停）";
            tray.UpdateTooltip(title);
        }

        // ==================== 初始化 ====================
        private void InitializeControls()
        {
            loadingUI = true;
            try
            {
                trkVolume.Value = Math.Max(trkVolume.Minimum,
                    Math.Min(trkVolume.Maximum, ConfigManager.Current.Volume));

                numModDelay.Value = Math.Max(numModDelay.Minimum,
                    Math.Min(numModDelay.Maximum, ConfigManager.Current.ModifierDelayMs));

                numClip.Value = 0;
                cmbClipUnit.Items.Clear();
                cmbClipUnit.Items.Add("毫秒");
                cmbClipUnit.Items.Add("秒");
                cmbClipUnit.SelectedIndex = 0;

                RefreshThemeCombo();

                string initTitle = ConfigManager.Current.CustomTitle ?? "";
                if (string.IsNullOrEmpty(initTitle)) initTitle = "画笔咻咻";
                txtAppTitle.Text = initTitle;
                txtAppTitle.ReadOnly = true;
                UpdateWindowTitle();

                UpdateSourceVisibility();
                RefreshPresetCombo();
            }
            finally { loadingUI = false; }

            trkVolume.ValueChanged += TrkVolume_ValueChanged;
            numModDelay.ValueChanged += NumModDelay_ValueChanged;
            cmbTheme.SelectedIndexChanged += CmbTheme_SelectedIndexChanged;

            txtAppTitle.TextChanged += TxtAppTitle_TextChanged;
            txtAppTitle.Leave += TxtAppTitle_Leave;

            cmbFolder.SelectedIndexChanged += CmbFolder_SelectedIndexChanged;
            cmbFolder.DropDown += (s, e) => RefreshFolderList();
            cmbFile.SelectedIndexChanged += SettingControl_Changed;

            rdoFile.CheckedChanged += SourceRadio_Changed;
            rdoFolder.CheckedChanged += SourceRadio_Changed;
            rdoSeq.CheckedChanged += SettingControl_Changed;
            rdoRand.CheckedChanged += SettingControl_Changed;
            chkLoopCurrent.CheckedChanged += SettingControl_Changed;
            chkLoopList.CheckedChanged += SettingControl_Changed;
            chkResume.CheckedChanged += SettingControl_Changed;

            numClip.ValueChanged += ClipControl_Changed;
            cmbClipUnit.SelectedIndexChanged += ClipControl_Changed;

            cmbEditKey.SelectedIndexChanged += CmbEditKey_SelectedIndexChanged;
            txtDisplayName.Leave += TxtDisplayName_Leave;
            txtDisplayName.KeyDown += TxtDisplayName_KeyDown;

            btnOpenFolder.Click += BtnOpenFolder_Click;
            btnAddKey.Click += BtnAddKey_Click;
            btnDeleteKey.Click += BtnDeleteKey_Click;
            btnAppearance.Click += BtnAppearance_Click;
            btnHelp.Click += (s, e) => { using (var dlg = new HelpForm()) dlg.ShowDialog(this); };
            btnQuit.Click += (s, e) => DoQuit();

            btnApplyPreset.Click += BtnApplyPreset_Click;
            btnSavePreset.Click += BtnSavePreset_Click;
            btnDeletePreset.Click += BtnDeletePreset_Click;

            // 标题图标可点击
            lblTitle.Cursor = Cursors.Hand;
            lblTitle.Click += (s, e) =>
            {
                txtAppTitle.ReadOnly = false;
                txtAppTitle.Focus();
                txtAppTitle.SelectAll();
            };

            // ToolTip
            tooltip = new ToolTip();
            tooltip.SetToolTip(lblTitle, "点击图标可更改标题");
            tooltip.SetToolTip(txtAppTitle, "点击 🖌 图标可编辑标题");
            tooltip.SetToolTip(lblMappingCount, "当前已配置的按键总数");
            tooltip.SetToolTip(lblCurrentKeyDisplay, "显示最近按下的按键（软件内操作不显示），2.5 秒后自动消失。");
            tooltip.SetToolTip(cmbEditKey, "选择要编辑的按键映射");
            tooltip.SetToolTip(btnAddKey, "点击后按下任意键/组合键，松开所有键即完成捕获");
            tooltip.SetToolTip(btnDeleteKey, "删除当前选中的按键映射，mouse_left 不能删除");
            tooltip.SetToolTip(txtDisplayName, "给当前按键起个中文名，会显示在下拉框里");
            tooltip.SetToolTip(numModDelay, "单按 Ctrl/Shift/Alt 后等待这段时间，才会触发它自己的音效。数值越小响应越快，但组合键越容易误判");
            tooltip.SetToolTip(btnOpenFolder, "用资源管理器打开 Sounds 目录");
            tooltip.SetToolTip(cmbFolder, "选择音效文件夹（Sounds 下的子文件夹）");
            tooltip.SetToolTip(cmbFile, "单文件模式下可下拉选择；文件夹模式下显示正在播放的文件");
            tooltip.SetToolTip(rdoFile, "播放选定的单个音频文件");
            tooltip.SetToolTip(rdoFolder, "顺序或随机播放文件夹里的音频");
            tooltip.SetToolTip(rdoSeq, "每次按下按文件名顺序依次播放，播完最后一个回到第一个");
            tooltip.SetToolTip(rdoRand, "每次按下从文件夹里随机选一个音频");
            tooltip.SetToolTip(chkLoopCurrent, "按住时无限循环当前音效，松开停止");
            tooltip.SetToolTip(chkLoopList, "按住时无限循环文件夹内的音频列表");
            tooltip.SetToolTip(chkResume, "松开暂停音频，再次按下从暂停处继续");
            tooltip.SetToolTip(numClip, "只播开头 N 毫秒后停止。0 表示不截取，播放完整音频。循环模式下每 N 毫秒从头重播");
            tooltip.SetToolTip(lblSoundDir, "音效文件存放的位置");
            tooltip.SetToolTip(trkVolume, "程序内部音量，不影响电脑系统音量");
            tooltip.SetToolTip(cmbPreset, "选择要应用的键位预设");
            tooltip.SetToolTip(btnApplyPreset, "只添加新键位，不覆盖你已有的配置");
            tooltip.SetToolTip(btnSavePreset, "把当前所有按键保存为自定义预设");
            tooltip.SetToolTip(btnDeletePreset, "删除选中的自定义预设，内置预设不能删除");
            tooltip.SetToolTip(cmbTheme, "选择深色/浅色/跟随系统，或自定义的外观预设");
            tooltip.SetToolTip(btnAppearance, "打开外观设置窗口，编辑颜色、保存/删除外观预设");
            tooltip.SetToolTip(btnHelp, "打开使用说明窗口");
            tooltip.SetToolTip(btnQuit, "彻底退出程序，任务栏和托盘都不再有图标");
        }

        /// <summary>
        /// 清理失效的音效源：文件夹不存在或文件不存在的映射，自动清空
        /// </summary>
        private void CleanupInvalidMappings()
        {
            bool changed = false;
            foreach (var kv in ConfigManager.Current.Mappings)
            {
                SoundMapping m = kv.Value;
                if (string.IsNullOrEmpty(m.Folder)) continue;

                string folderPath = Path.Combine(soundFolder, m.Folder);
                if (!Directory.Exists(folderPath))
                {
                    m.Folder = "";
                    m.File = "";
                    changed = true;
                    continue;
                }

                if (!m.IsFolderMode && !string.IsNullOrEmpty(m.File))
                {
                    string filePath = Path.Combine(folderPath, m.File);
                    if (!File.Exists(filePath))
                    {
                        m.File = "";
                        changed = true;
                    }
                }
            }

            if (changed) SaveConfig();
        }
        private void EnsureFolders()
        {
            try
            {
                if (!Directory.Exists(soundFolder)) Directory.CreateDirectory(soundFolder);
                if (!Directory.Exists(presetsFolder)) Directory.CreateDirectory(presetsFolder);
            }
            catch { }
        }

        private void SaveConfig() { ConfigManager.SaveAsync(configPath); }

        private void UpdateWindowBoundsCache()
        {
            try
            {
                var b = this.Bounds;
                cachedLeft = b.Left;
                cachedTop = b.Top;
                cachedRight = b.Right;
                cachedBottom = b.Bottom;
                cachedVisible = this.Visible;
            }
            catch { }
        }

        // ==================== 主题 ====================
        private void RefreshThemeCombo()
        {
            string current = ConfigManager.Current.Theme;
            if (string.IsNullOrEmpty(current)) current = "默认深色";

            cmbTheme.Items.Clear();
            cmbTheme.Items.Add("默认深色");
            cmbTheme.Items.Add("默认浅色");
            cmbTheme.Items.Add("跟随系统");

            foreach (var name in ThemeManager.AllNames())
            {
                if (name != "默认深色" && name != "默认浅色" && !cmbTheme.Items.Contains(name))
                    cmbTheme.Items.Add(name);
            }

            if (cmbTheme.Items.Contains(current))
                cmbTheme.SelectedItem = current;
            else
                cmbTheme.SelectedIndex = 0;
        }

        private ThemePreset ResolveThemeByName(string name)
        {
            if (name == "跟随系统")
            {
                string actual = IsSystemLightTheme()
                    ? ConfigManager.Current.FollowSystemLightPreset
                    : ConfigManager.Current.FollowSystemDarkPreset;
                if (string.IsNullOrEmpty(actual)) actual = "默认深色";
                var p = ThemeManager.FindByName(actual);
                if (p != null) return p;
                return BuiltinThemes.Dark;
            }
            var preset = ThemeManager.FindByName(name);
            if (preset != null) return preset;
            return BuiltinThemes.Dark;
        }

        private bool IsSystemLightTheme()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object val = key.GetValue("AppsUseLightTheme");
                        if (val != null) return Convert.ToInt32(val) == 1;
                    }
                }
            }
            catch { }
            return false;
        }

        private void CmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loadingUI) return;
            string name = cmbTheme.SelectedItem as string;
            if (string.IsNullOrEmpty(name)) return;

            ConfigManager.Current.Theme = name;
            ThemeManager.Current = ResolveThemeByName(name);
            ApplyTheme();
            SaveConfig();
        }

        private void ApplyTheme()
        {
            var c = ThemeManager.Current.Colors;

            this.BackColor = c.C_Background;
            this.ForeColor = c.C_Foreground;

            if (txtAppTitle != null)
            {
                txtAppTitle.BackColor = c.C_Background;
                txtAppTitle.ForeColor = c.C_Foreground;
            }

            foreach (Control ctrl in this.Controls)
                ApplyThemeToControl(ctrl, c);

            lblMappingCount.ForeColor = c.C_Highlight;
            lblHint.ForeColor = c.C_SecondaryText;
            lblSoundDir.ForeColor = c.C_SecondaryText;
            lblClipHint.ForeColor = c.C_SecondaryText;
            if (!paused) lblCurrentKeyDisplay.ForeColor = c.C_Highlight;

            btnAddKey.BackColor = c.C_AccentButton;
            btnAppearance.BackColor = c.C_AccentButton;
            btnSavePreset.BackColor = c.C_AccentButton;
            btnHelp.BackColor = c.C_AccentButton;
            btnQuit.BackColor = c.C_DangerButton;
            btnDeleteKey.BackColor = c.C_DangerButton;
            btnDeletePreset.BackColor = c.C_DangerButton;
            btnApplyPreset.BackColor = c.C_SuccessButton;
            btnOpenFolder.BackColor = c.C_NeutralButton;

            if (btnPause != null)
                btnPause.BackColor = paused ? c.C_SuccessButton : c.C_DangerButton;

            Color btnFg = c.C_Foreground;
            btnAddKey.ForeColor = btnFg;
            btnAppearance.ForeColor = btnFg;
            btnSavePreset.ForeColor = btnFg;
            btnHelp.ForeColor = btnFg;
            btnQuit.ForeColor = btnFg;
            btnDeleteKey.ForeColor = btnFg;
            btnDeletePreset.ForeColor = btnFg;
            btnApplyPreset.ForeColor = btnFg;
            btnOpenFolder.ForeColor = btnFg;
            if (btnPause != null) btnPause.ForeColor = btnFg;

            this.Invalidate(true);
        }

        private void ApplyThemeToControl(Control ctrl, ThemeColors c)
        {
            if (ctrl is RoundButton rb) { rb.ForeColor = c.C_Foreground; }
            else if (ctrl is Label)
            {
                ctrl.ForeColor = c.C_Foreground;
                try { ctrl.BackColor = Color.Transparent; } catch { }
            }
            else if (ctrl is GroupBox) { ctrl.ForeColor = c.C_Foreground; ctrl.BackColor = c.C_Background; }
            else if (ctrl is CheckBox || ctrl is RadioButton) { ctrl.ForeColor = c.C_Foreground; ctrl.BackColor = c.C_Background; }
            else if (ctrl is ComboBox) { ctrl.ForeColor = c.C_Foreground; try { ctrl.BackColor = c.C_InputBackground; } catch { } }
            else if (ctrl is TextBox)
            {
                if (ctrl == txtAppTitle) { }
                else { ctrl.ForeColor = c.C_Foreground; ctrl.BackColor = c.C_InputBackground; }
            }
            else if (ctrl is NumericUpDown) { ctrl.ForeColor = c.C_Foreground; ctrl.BackColor = c.C_InputBackground; }
            else if (ctrl is TrackBar) { ctrl.BackColor = c.C_Background; }
            else if (ctrl is Panel) { ctrl.BackColor = c.C_Background; }

            if (ctrl.HasChildren)
                foreach (Control child in ctrl.Controls) ApplyThemeToControl(child, c);
        }

        // ==================== 标题 ====================
        private void TxtAppTitle_TextChanged(object sender, EventArgs e)
        {
            if (loadingUI) return;
            if (txtAppTitle.ReadOnly) return;
            UpdateWindowTitle();
        }

        private void TxtAppTitle_Leave(object sender, EventArgs e)
        {
            if (loadingUI) return;

            string t = (txtAppTitle.Text ?? "").Trim();
            if (string.IsNullOrEmpty(t)) t = "画笔咻咻";

            txtAppTitle.Text = t;
            txtAppTitle.ReadOnly = true;

            ConfigManager.Current.CustomTitle = (t == "画笔咻咻") ? "" : t;
            SaveConfig();
            UpdateWindowTitle();
        }

        private void UpdateWindowTitle()
        {
            string t = (txtAppTitle.Text ?? "").Trim();
            string eff = string.IsNullOrEmpty(t) ? "画笔咻咻" : t;
            this.Text = eff;
            UpdateTrayTooltip();
        }

        // ==================== 音量 / 延迟 ====================
        private void TrkVolume_ValueChanged(object sender, EventArgs e)
        {
            if (loadingUI) return;
            int v = trkVolume.Value;
            ConfigManager.Current.Volume = v;
            if (audio != null) audio.Volume = v;
            SaveConfig();
        }

        private void NumModDelay_ValueChanged(object sender, EventArgs e)
        {
            if (loadingUI) return;
            int v = (int)numModDelay.Value;
            ConfigManager.Current.ModifierDelayMs = v;
            if (hook != null) hook.ModifierDelayMs = v;
            SaveConfig();
        }

        // ==================== 音效文件夹 ====================
        private void RefreshFolderList()
        {
            string current = cmbFolder.SelectedItem as string;
            cmbFolder.Items.Clear();

            if (Directory.Exists(soundFolder))
            {
                var dirs = Directory.GetDirectories(soundFolder);
                Array.Sort(dirs, StringComparer.OrdinalIgnoreCase);
                foreach (var d in dirs)
                {
                    // 只显示非空子文件夹
                    var files = Directory.GetFiles(d);
                    bool hasAudio = false;
                    foreach (var f in files)
                    {
                        string ext = Path.GetExtension(f).ToLower();
                        if (Array.IndexOf(AudioManager.SupportedExtensions, ext) >= 0)
                        {
                            hasAudio = true;
                            break;
                        }
                    }
                    if (hasAudio)
                        cmbFolder.Items.Add(Path.GetFileName(d));
                }
            }

            if (current != null && cmbFolder.Items.Contains(current))
                cmbFolder.SelectedItem = current;
            else if (cmbFolder.Items.Count > 0)
                cmbFolder.SelectedIndex = 0;
        }

        private void RefreshFileList(string folderName)
        {
            string current = cmbFile.SelectedItem as string;
            cmbFile.Items.Clear();

            if (string.IsNullOrEmpty(folderName)) return;
            string folderPath = Path.Combine(soundFolder, folderName);
            if (!Directory.Exists(folderPath)) return;

            var files = Directory.GetFiles(folderPath);
            Array.Sort(files, StringComparer.OrdinalIgnoreCase);
            foreach (var f in files)
            {
                string ext = Path.GetExtension(f).ToLower();
                if (Array.IndexOf(AudioManager.SupportedExtensions, ext) >= 0)
                    cmbFile.Items.Add(Path.GetFileName(f));
            }

            if (current != null && cmbFile.Items.Contains(current))
                cmbFile.SelectedItem = current;
            else if (cmbFile.Items.Count > 0)
                cmbFile.SelectedIndex = 0;
        }

        private void CmbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loadingUI) return;
            string folderName = cmbFolder.SelectedItem as string;

            if (rdoFile.Checked)
            {
                RefreshFileList(folderName);
            }
            ApplyMappingFromUI();
        }

        // ==================== 音效源切换 ====================
        private void UpdateSourceVisibility()
        {
            bool isFolder = rdoFolder.Checked;
            panelFolderMode.Visible = isFolder;
            chkLoopCurrent.Visible = !isFolder;
            chkLoopList.Visible = isFolder;
        }

        private void SourceRadio_Changed(object sender, EventArgs e)
        {
            UpdateSourceVisibility();
            if (loadingUI) return;
            UpdateFileComboMode();
            ApplyMappingFromUI();
        }

        private void SettingControl_Changed(object sender, EventArgs e)
        {
            if (loadingUI) return;
            ApplyMappingFromUI();
        }

        private void ClipControl_Changed(object sender, EventArgs e)
        {
            if (loadingUI) return;
            ApplyMappingFromUI();
        }

        // 根据模式切换 cmbFile 的行为
        private void UpdateFileComboMode()
        {
            string key = GetSelectedKey();

            if (rdoFolder.Checked)
            {
                // 文件夹模式：只读显示"正在播放"
                cmbFile.Items.Clear();
                string fileName = "—";
                if (key != null && lastPlayedFile.ContainsKey(key))
                    fileName = lastPlayedFile[key];
                cmbFile.Items.Add(fileName);
                cmbFile.SelectedIndex = 0;
            }
            else
            {
                // 单文件模式：加载文件夹里的所有文件
                string folderName = cmbFolder.SelectedItem as string;
                RefreshFileList(folderName);
            }
        }

        // ==================== 编辑按键 ====================
        private string GetSelectedKey()
        {
            if (cmbEditKey.SelectedItem is KeyItem ki) return ki.Key;
            return null;
        }

        private void CmbEditKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loadingUI) return;
            string key = GetSelectedKey();
            if (key == null) return;
            ConfigManager.Current.LastEditedKey = key;
            SaveConfig();
            LoadMappingToUI(key);
        }

        private void LoadMappingToUI(string key)
        {
            if (key == null) return;
            if (!ConfigManager.Current.Mappings.TryGetValue(key, out SoundMapping m))
            {
                m = new SoundMapping();
                ConfigManager.Current.Mappings[key] = m;
            }

            loadingUI = true;
            try
            {
                if (m.IsFolderMode) rdoFolder.Checked = true;
                else rdoFile.Checked = true;

                RefreshFolderList();
                if (!string.IsNullOrEmpty(m.Folder) && cmbFolder.Items.Contains(m.Folder))
                    cmbFolder.SelectedItem = m.Folder;
                else if (cmbFolder.Items.Count > 0)
                    cmbFolder.SelectedIndex = 0;

                // 强制同步 Folder
                if (string.IsNullOrEmpty(m.Folder) && cmbFolder.SelectedItem is string fs)
                    m.Folder = fs;

                if (m.IsFolderMode)
                {
                    cmbFile.Items.Clear();
                    string fileName = lastPlayedFile.ContainsKey(key) ? lastPlayedFile[key] : "—";
                    cmbFile.Items.Add(fileName);
                    cmbFile.SelectedIndex = 0;
                }
                else
                {
                    string folderName = cmbFolder.SelectedItem as string;
                    RefreshFileList(folderName);
                    if (!string.IsNullOrEmpty(m.File) && cmbFile.Items.Contains(m.File))
                        cmbFile.SelectedItem = m.File;
                    else if (cmbFile.Items.Count > 0)
                        cmbFile.SelectedIndex = 0;

                    // 强制同步 File
                    if (string.IsNullOrEmpty(m.File) && cmbFile.Items.Count > 0)
                    {
                        string firstFile = cmbFile.Items[0] as string;
                        if (!string.IsNullOrEmpty(firstFile))
                            m.File = firstFile;
                    }
                }

                rdoSeq.Checked = !m.Random;
                rdoRand.Checked = m.Random;
                chkLoopCurrent.Checked = m.Loop;
                chkLoopList.Checked = m.Loop;
                chkResume.Checked = m.Resume;

                if (m.ClipMs <= 0)
                {
                    numClip.Value = 0;
                    cmbClipUnit.SelectedItem = "毫秒";
                }
                else if (m.ClipMs % 1000 == 0)
                {
                    numClip.Value = Math.Min(numClip.Maximum, m.ClipMs / 1000);
                    cmbClipUnit.SelectedItem = "秒";
                }
                else
                {
                    numClip.Value = Math.Min(numClip.Maximum, m.ClipMs);
                    cmbClipUnit.SelectedItem = "毫秒";
                }

                txtDisplayName.Text = m.DisplayName ?? "";
                UpdateSourceVisibility();

                SaveConfig();
            }
            finally { loadingUI = false; }
        }

        private void ApplyMappingFromUI()
        {
            string key = GetSelectedKey();
            if (key == null) return;
            if (!ConfigManager.Current.Mappings.TryGetValue(key, out SoundMapping m))
            {
                m = new SoundMapping();
                ConfigManager.Current.Mappings[key] = m;
            }

            bool isFolderMode = rdoFolder.Checked;
            m.IsFolderMode = isFolderMode;

            if (cmbFolder.SelectedItem is string folderName) m.Folder = folderName;

            if (isFolderMode)
            {
                m.Loop = chkLoopList.Checked;
                m.Random = rdoRand.Checked;
            }
            else
            {
                if (cmbFile.SelectedItem is string fname) m.File = fname;
                m.Loop = chkLoopCurrent.Checked;
                m.Random = false;
            }
            m.Resume = chkResume.Checked;

            int clipVal = (int)numClip.Value;
            string unit = cmbClipUnit.SelectedItem as string;
            m.ClipMs = (unit == "秒") ? clipVal * 1000 : clipVal;

            SaveConfig();
            try { audio.Stop(key); } catch { }
        }

        // ==================== 打开文件夹 ====================
        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(soundFolder)) Directory.CreateDirectory(soundFolder);
                Process.Start("explorer.exe", "\"" + soundFolder + "\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show("无法打开文件夹：\n" + ex.Message, "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== 备注 ====================
        private void TxtDisplayName_Leave(object sender, EventArgs e) { SaveDisplayName(); }

        private void TxtDisplayName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SaveDisplayName();
                this.ActiveControl = null;
            }
        }

        private void SaveDisplayName()
        {
            string key = GetSelectedKey();
            if (key == null) return;
            if (!ConfigManager.Current.Mappings.TryGetValue(key, out SoundMapping m)) return;

            string newName = (txtDisplayName.Text ?? "").Trim();
            if ((m.DisplayName ?? "") == newName) return;
            m.DisplayName = newName;
            SaveConfig();

            string currentKey = key;
            RefreshKeyList();
            SelectKeyInCombo(currentKey);
        }

        // ==================== 按键列表 ====================
        private void RefreshKeyList()
        {
            string current = GetSelectedKey();
            cmbEditKey.Items.Clear();

            var keys = new List<string>(ConfigManager.Current.Mappings.Keys);
            var sorted = new List<string>();
            if (keys.Contains("mouse_left")) sorted.Add("mouse_left");
            keys.Sort(StringComparer.OrdinalIgnoreCase);
            foreach (var k in keys)
                if (k.StartsWith("mouse_") && k != "mouse_left") sorted.Add(k);
            foreach (var k in keys)
                if (!k.StartsWith("mouse_")) sorted.Add(k);

            foreach (var k in sorted)
            {
                string dn = ConfigManager.Current.Mappings[k].DisplayName ?? "";
                string display = string.IsNullOrEmpty(dn) ? k : (k + "  |  " + dn);
                cmbEditKey.Items.Add(new KeyItem { Key = k, Display = display });
            }

            lblMappingCount.Text = "共 " + sorted.Count + " 个按键";
            if (current != null) SelectKeyInCombo(current);
        }

        private void SelectKeyInCombo(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            for (int i = 0; i < cmbEditKey.Items.Count; i++)
            {
                if (cmbEditKey.Items[i] is KeyItem ki && ki.Key == key)
                {
                    cmbEditKey.SelectedIndex = i;
                    return;
                }
            }
            if (cmbEditKey.Items.Count > 0) cmbEditKey.SelectedIndex = 0;
        }

        // ==================== 正在播放 ====================
        private void OnTrackChanged(string combo, string fileName)
        {
            lastPlayedFile[combo] = fileName;

            string currentKey = GetSelectedKey();
            if (currentKey == combo && rdoFolder.Checked)
            {
                loadingUI = true;
                try
                {
                    cmbFile.Items.Clear();
                    cmbFile.Items.Add(fileName);
                    cmbFile.SelectedIndex = 0;
                }
                finally { loadingUI = false; }
            }
        }

        // ==================== 添加 / 删除按键 ====================
        private void BtnAddKey_Click(object sender, EventArgs e)
        {
            if (hook.Capturing) return;
            hook.StartCapture();
            lblSoundDir.Text = "请按下要添加的按键/组合键...（松开所有键后完成捕获）";
        }

        private void Hook_Captured(string combo)
        {
            this.BeginInvoke(new Action(() =>
            {
                lblSoundDir.Text = "已捕获并切换到：" + combo;
                if (!ConfigManager.Current.Mappings.ContainsKey(combo))
                {
                    ConfigManager.Current.Mappings[combo] = new SoundMapping();
                    SaveConfig();
                }
                RefreshKeyList();
                SelectKeyInCombo(combo);
            }));
        }

        private void BtnDeleteKey_Click(object sender, EventArgs e)
        {
            string key = GetSelectedKey();
            if (key == null) return;

            if (key == "mouse_left")
            {
                MessageBox.Show("鼠标左键是最核心的映射，不能删除。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var r = MessageBox.Show("确定要删除按键“" + key + "”的映射吗？",
                "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r != DialogResult.Yes) return;

            try { audio.Stop(key); } catch { }
            ConfigManager.Current.Mappings.Remove(key);
            lastPlayedFile.Remove(key);
            SaveConfig();
            RefreshKeyList();
            if (cmbEditKey.Items.Count > 0) cmbEditKey.SelectedIndex = 0;
        }

        // ==================== 外观 ====================
        private void BtnAppearance_Click(object sender, EventArgs e)
        {
            using (var dlg = new AppearanceForm(ConfigManager.Current.Theme))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    string applied = dlg.ResultThemeName;
                    ConfigManager.Current.Theme = applied;
                    ThemeManager.Current = ResolveThemeByName(applied);

                    RefreshThemeCombo();
                    loadingUI = true;
                    try
                    {
                        if (cmbTheme.Items.Contains(applied))
                            cmbTheme.SelectedItem = applied;
                    }
                    finally { loadingUI = false; }

                    ApplyTheme();
                    UpdatePauseUI();
                    SaveConfig();
                }
                else
                {
                    SaveConfig();
                    RefreshThemeCombo();
                }
            }
        }

        // ==================== 预设 ====================
        private void RefreshPresetCombo()
        {
            cmbPreset.Items.Clear();
            for (int i = 0; i < KeyPresets.All.Count; i++)
            {
                cmbPreset.Items.Add(new PresetItem
                {
                    IsCustom = false,
                    Index = i,
                    Name = KeyPresets.All[i].Name
                });
            }
            for (int i = 0; i < ConfigManager.Current.CustomKeyPresets.Count; i++)
            {
                cmbPreset.Items.Add(new PresetItem
                {
                    IsCustom = true,
                    Index = i,
                    Name = ConfigManager.Current.CustomKeyPresets[i].Name
                });
            }
            if (cmbPreset.Items.Count > 0) cmbPreset.SelectedIndex = 0;
        }

        private PresetItem GetSelectedPreset()
        {
            return cmbPreset.SelectedItem as PresetItem;
        }

        private void BtnApplyPreset_Click(object sender, EventArgs e)
        {
            var item = GetSelectedPreset();
            if (item == null) return;

            Dictionary<string, string> keys;
            string presetName;

            if (item.IsCustom)
            {
                var cp = ConfigManager.Current.CustomKeyPresets[item.Index];
                keys = cp.Keys;
                presetName = cp.Name;
            }
            else
            {
                var p = KeyPresets.All[item.Index];
                keys = p.Keys;
                presetName = p.Name;
            }

            int added = 0, skipped = 0;
            foreach (var kv in keys)
            {
                string combo = kv.Key;
                string name = kv.Value;
                if (ConfigManager.Current.Mappings.ContainsKey(combo)) { skipped++; continue; }

                ConfigManager.Current.Mappings[combo] = new SoundMapping
                {
                    Folder = "",
                    File = "",
                    IsFolderMode = false,
                    Loop = false,
                    Random = false,
                    Resume = false,
                    DisplayName = name,
                    ClipMs = 0,
                };
                added++;
            }

            SaveConfig();
            RefreshKeyList();

            MessageBox.Show(
                "预设“" + presetName + "”已应用。\n\n" +
                "新添加：" + added + " 个\n" +
                "跳过（已存在）：" + skipped + " 个\n\n" +
                "新添加的键位还没有音效，请逐个选择。",
                "预设完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSavePreset_Click(object sender, EventArgs e)
        {
            if (ConfigManager.Current.Mappings.Count == 0)
            {
                MessageBox.Show("当前没有任何映射，无法保存为预设。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string name = PromptForName("保存为预设", "请输入预设名称：", "");
            if (string.IsNullOrEmpty(name)) return;

            int existIdx = -1;
            for (int i = 0; i < ConfigManager.Current.CustomKeyPresets.Count; i++)
            {
                if (ConfigManager.Current.CustomKeyPresets[i].Name == name) { existIdx = i; break; }
            }

            if (existIdx >= 0)
            {
                var r = MessageBox.Show("已存在同名预设“" + name + "”，是否覆盖？",
                    "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r != DialogResult.Yes) return;
            }

            var preset = new KeyPreset { Name = name };
            foreach (var kv in ConfigManager.Current.Mappings)
            {
                string dn = kv.Value.DisplayName ?? "";
                preset.Keys[kv.Key] = dn;
            }

            if (existIdx >= 0) ConfigManager.Current.CustomKeyPresets[existIdx] = preset;
            else ConfigManager.Current.CustomKeyPresets.Add(preset);

            SaveConfig();
            RefreshPresetCombo();

            for (int i = 0; i < cmbPreset.Items.Count; i++)
            {
                if (cmbPreset.Items[i] is PresetItem pi && pi.IsCustom && pi.Name == name)
                {
                    cmbPreset.SelectedIndex = i;
                    break;
                }
            }

            MessageBox.Show("预设“" + name + "”已保存，共 " + preset.Keys.Count + " 个键位。",
                "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDeletePreset_Click(object sender, EventArgs e)
        {
            var item = GetSelectedPreset();
            if (item == null) return;

            if (!item.IsCustom)
            {
                MessageBox.Show("内置预设不能删除。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var r = MessageBox.Show("确定要删除自定义预设“" + item.Name + "”吗？",
                "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r != DialogResult.Yes) return;

            ConfigManager.Current.CustomKeyPresets.RemoveAt(item.Index);
            SaveConfig();
            RefreshPresetCombo();
        }

        private static string PromptForName(string title, string prompt, string defaultValue)
        {
            using (var dlg = new Form())
            {
                dlg.Text = title;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MinimizeBox = false;
                dlg.MaximizeBox = false;
                dlg.ClientSize = new Size(380, 145);
                dlg.BackColor = Color.FromArgb(43, 43, 43);
                dlg.ForeColor = Color.White;
                dlg.Font = new Font("微软雅黑", 10F);

                var lbl = new Label
                {
                    Text = prompt,
                    Location = new Point(15, 15),
                    Size = new Size(350, 25),
                    ForeColor = Color.White
                };
                var tb = new TextBox
                {
                    Text = defaultValue,
                    Location = new Point(15, 45),
                    Size = new Size(350, 28),
                    BackColor = Color.FromArgb(30, 30, 30),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                var ok = new Button
                {
                    Text = "确定",
                    Location = new Point(200, 95),
                    Size = new Size(80, 32),
                    DialogResult = DialogResult.OK,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(74, 140, 74),
                    ForeColor = Color.White
                };
                ok.FlatAppearance.BorderSize = 0;
                var cancel = new Button
                {
                    Text = "取消",
                    Location = new Point(290, 95),
                    Size = new Size(80, 32),
                    DialogResult = DialogResult.Cancel,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(85, 85, 85),
                    ForeColor = Color.White
                };
                cancel.FlatAppearance.BorderSize = 0;

                dlg.Controls.Add(lbl);
                dlg.Controls.Add(tb);
                dlg.Controls.Add(ok);
                dlg.Controls.Add(cancel);
                dlg.AcceptButton = ok;
                dlg.CancelButton = cancel;

                if (dlg.ShowDialog() == DialogResult.OK)
                    return tb.Text.Trim();
                return null;
            }
        }

        // ==================== 钩子事件 ====================
        private void Hook_KeyPressed(string combo)
        {
            if (paused) return;
            if (!combo.StartsWith("mouse_") && cachedHasFocus) return;

            keyDisplayQueue.Enqueue(combo);

            if (!ConfigManager.Current.Mappings.TryGetValue(combo, out SoundMapping m))
                return;

            audio.Trigger(combo, m);
        }

        private void Hook_KeyReleased(string combo)
        {
            if (paused) return;
            if (!ConfigManager.Current.Mappings.TryGetValue(combo, out SoundMapping m))
                return;
            audio.Release(combo, m.Resume);
        }

        private void ProcessKeyDisplayQueue()
        {
            string last = null;
            string item;
            while (keyDisplayQueue.TryDequeue(out item))
                last = item;

            if (last != null && !paused)
                ShowCurrentKey(last);
        }

        private void ShowCurrentKey(string combo)
        {
            if (statusTimer == null) return;
            if (paused) return;

            string dn = "";
            bool hasMapping = false;
            SoundMapping m;
            if (ConfigManager.Current.Mappings.TryGetValue(combo, out m))
            {
                dn = m.DisplayName ?? "";
                hasMapping = true;
            }

            string text;
            if (!hasMapping) text = "当前按键：" + combo + "  （无映射）";
            else if (string.IsNullOrEmpty(dn)) text = "当前按键：" + combo;
            else text = "当前按键：" + combo + "  |  " + dn;

            lblCurrentKeyDisplay.Text = text;
            statusTimer.Stop();
            statusTimer.Start();
        }
    }
}