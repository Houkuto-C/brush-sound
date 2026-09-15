using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace BrushSound
{
    // ==================== 音效映射 ====================
    public class SoundMapping
    {
        /// <summary>音效文件夹（Sounds 下的子文件夹名），空表示未选</summary>
        public string Folder { get; set; } = "";

        /// <summary>单文件模式下的文件名</summary>
        public string File { get; set; } = "";

        /// <summary>true = 文件夹模式（顺序/随机播放整个文件夹），false = 单文件模式</summary>
        public bool IsFolderMode { get; set; } = false;

        /// <summary>循环播放</summary>
        public bool Loop { get; set; } = false;

        /// <summary>随机播放（仅文件夹模式有效）</summary>
        public bool Random { get; set; } = false;

        /// <summary>续笔（松开暂停，再次按下继续）</summary>
        public bool Resume { get; set; } = false;

        /// <summary>中文备注</summary>
        public string DisplayName { get; set; } = "";

        /// <summary>前段截取时长（毫秒），0 表示不截取</summary>
        public int ClipMs { get; set; } = 0;

        public SoundMapping Clone()
        {
            return new SoundMapping
            {
                Folder = this.Folder,
                File = this.File,
                IsFolderMode = this.IsFolderMode,
                Loop = this.Loop,
                Random = this.Random,
                Resume = this.Resume,
                DisplayName = this.DisplayName,
                ClipMs = this.ClipMs,
            };
        }
    }

    // ==================== 键位预设 ====================
    public class KeyPreset
    {
        public string Name { get; set; } = "";
        public Dictionary<string, string> Keys { get; set; }
            = new Dictionary<string, string>();
    }

    // ==================== 外观颜色 ====================
    public class ThemeColors
    {
        public string Background { get; set; } = "#2B2B2B";
        public string InputBackground { get; set; } = "#1E1E1E";
        public string Foreground { get; set; } = "#E0D0B6";
        public string SecondaryText { get; set; } = "#888888";
        public string Highlight { get; set; } = "#CC9E4C";
        public string AccentButton { get; set; } = "#4A6A8C";
        public string DangerButton { get; set; } = "#6B2717";
        public string SuccessButton { get; set; } = "#769365";
        public string NeutralButton { get; set; } = "#555555";
        public string Border { get; set; } = "#444444";
        public string Selection { get; set; } = "#8B9EA5";

        public ThemeColors Clone()
        {
            return new ThemeColors
            {
                Background = this.Background,
                InputBackground = this.InputBackground,
                Foreground = this.Foreground,
                SecondaryText = this.SecondaryText,
                Highlight = this.Highlight,
                AccentButton = this.AccentButton,
                DangerButton = this.DangerButton,
                SuccessButton = this.SuccessButton,
                NeutralButton = this.NeutralButton,
                Border = this.Border,
                Selection = this.Selection,
            };
        }

        public Color C_Background { get { return HexToColor(Background, Color.FromArgb(43, 43, 43)); } }
        public Color C_InputBackground { get { return HexToColor(InputBackground, Color.FromArgb(30, 30, 30)); } }
        public Color C_Foreground { get { return HexToColor(Foreground, Color.White); } }
        public Color C_SecondaryText { get { return HexToColor(SecondaryText, Color.Gray); } }
        public Color C_Highlight { get { return HexToColor(Highlight, Color.Gold); } }
        public Color C_AccentButton { get { return HexToColor(AccentButton, Color.SteelBlue); } }
        public Color C_DangerButton { get { return HexToColor(DangerButton, Color.IndianRed); } }
        public Color C_SuccessButton { get { return HexToColor(SuccessButton, Color.SeaGreen); } }
        public Color C_NeutralButton { get { return HexToColor(NeutralButton, Color.Gray); } }
        public Color C_Border { get { return HexToColor(Border, Color.DimGray); } }
        public Color C_Selection { get { return HexToColor(Selection, Color.SeaGreen); } }

        public static Color HexToColor(string hex, Color fallback)
        {
            try
            {
                if (string.IsNullOrEmpty(hex)) return fallback;
                hex = hex.TrimStart('#');
                if (hex.Length == 6)
                {
                    int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                    return Color.FromArgb(r, g, b);
                }
                if (hex.Length == 8)
                {
                    int a = Convert.ToInt32(hex.Substring(0, 2), 16);
                    int r = Convert.ToInt32(hex.Substring(2, 2), 16);
                    int g = Convert.ToInt32(hex.Substring(4, 2), 16);
                    int b = Convert.ToInt32(hex.Substring(6, 2), 16);
                    return Color.FromArgb(a, r, g, b);
                }
            }
            catch { }
            return fallback;
        }

        public static string ColorToHex(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
        }
    }

    // ==================== 外观预设 ====================
    public class ThemePreset
    {
        public string Name { get; set; } = "";
        public ThemeColors Colors { get; set; } = new ThemeColors();
    }

    // ==================== 关于页 ====================
    public class AboutInfo
    {
        public string Author { get; set; } = "";
        public string Contact { get; set; } = "";
        public string Github { get; set; } = "";
    }

    // ==================== 全局配置 ====================
    public class AppConfig
    {
        public int Volume { get; set; } = 80;
        public string Theme { get; set; } = "默认深色";
        public string FollowSystemLightPreset { get; set; } = "默认浅色";
        public string FollowSystemDarkPreset { get; set; } = "默认深色";
        public int ModifierDelayMs { get; set; } = 120;
        public string LastEditedKey { get; set; } = "mouse_left";
        public string CustomTitle { get; set; } = "";
        public Dictionary<string, SoundMapping> Mappings { get; set; }
            = new Dictionary<string, SoundMapping>();
        public List<KeyPreset> CustomKeyPresets { get; set; }
            = new List<KeyPreset>();
        public AboutInfo About { get; set; } = new AboutInfo();
    }

    public static class ConfigManager
    {
        public static AppConfig Current { get; private set; }

        public static void Load(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    Current = JsonConvert.DeserializeObject<AppConfig>(json) ?? new AppConfig();
                }
                catch { Current = new AppConfig(); }
            }
            else Current = new AppConfig();

            if (Current.Mappings == null)
                Current.Mappings = new Dictionary<string, SoundMapping>();
            if (Current.CustomKeyPresets == null)
                Current.CustomKeyPresets = new List<KeyPreset>();
            if (Current.About == null)
                Current.About = new AboutInfo();

            if (!Current.Mappings.ContainsKey("mouse_left"))
            {
                Current.Mappings["mouse_left"] = new SoundMapping
                {
                    Folder = "",
                    File = "",
                    IsFolderMode = false,
                    Loop = true,
                    Resume = true,
                    DisplayName = "画笔",
                    ClipMs = 0,
                };
            }
        }

        public static void Save(string path)
        {
            try
            {
                string json = JsonConvert.SerializeObject(Current, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch { }
        }

        private static readonly object saveLock = new object();
        private static string pendingJson = null;
        private static bool saveWorkerRunning = false;

        public static void SaveAsync(string path)
        {
            string json;
            try
            {
                json = JsonConvert.SerializeObject(Current, Formatting.Indented);
            }
            catch { return; }

            lock (saveLock)
            {
                pendingJson = json;
                if (saveWorkerRunning) return;
                saveWorkerRunning = true;
            }

            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                while (true)
                {
                    string toWrite;
                    lock (saveLock)
                    {
                        toWrite = pendingJson;
                        pendingJson = null;
                        if (toWrite == null)
                        {
                            saveWorkerRunning = false;
                            return;
                        }
                    }
                    try { File.WriteAllText(path, toWrite); } catch { }
                }
            });
        }
    }

    // ==================== 内置键位预设 ====================
    public static class KeyPresets
    {
        public class Preset
        {
            public string Name;
            public Dictionary<string, string> Keys;
        }

        public static List<Preset> All = new List<Preset>
        {
            new Preset
            {
                Name = "Clip Studio Paint (CSP)",
                Keys = new Dictionary<string, string>
                {
                    { "ctrl+z", "撤销" }, { "ctrl+shift+z", "重做" },
                    { "ctrl+s", "保存" }, { "ctrl+shift+s", "另存为" },
                    { "ctrl+n", "新建" }, { "ctrl+o", "打开" },
                    { "ctrl+c", "复制" }, { "ctrl+v", "粘贴" },
                    { "ctrl+x", "剪切" }, { "ctrl+a", "全选" },
                    { "ctrl+d", "取消选择" }, { "ctrl+t", "自由变换" },
                    { "ctrl+shift+n", "新建图层" }, { "ctrl+e", "合并图层" },
                    { "e", "橡皮擦" }, { "b", "画笔" }, { "g", "填充" },
                    { "i", "吸管" }, { "m", "选区" }, { "h", "移动" },
                    { "space", "平移" }, { "alt", "临时吸管" },
                    { "[", "缩小笔刷" }, { "]", "放大笔刷" },
                }
            },
            new Preset
            {
                Name = "SAI 2",
                Keys = new Dictionary<string, string>
                {
                    { "ctrl+z", "撤销" }, { "ctrl+y", "重做" },
                    { "ctrl+s", "保存" }, { "ctrl+n", "新建" },
                    { "ctrl+o", "打开" }, { "n", "铅笔" }, { "b", "笔刷" },
                    { "v", "水彩笔" }, { "a", "喷枪" }, { "e", "橡皮擦" },
                    { "l", "套索" }, { "w", "魔棒" }, { "space", "移动画布" },
                    { "alt", "吸管" }, { "[", "缩小笔刷" }, { "]", "放大笔刷" },
                }
            },
            new Preset
            {
                Name = "Photoshop (PS)",
                Keys = new Dictionary<string, string>
                {
                    { "ctrl+z", "撤销" }, { "ctrl+alt+z", "历史后退" },
                    { "ctrl+shift+z", "历史前进" }, { "ctrl+s", "保存" },
                    { "ctrl+shift+s", "另存为" }, { "ctrl+alt+shift+s", "存储为 Web" },
                    { "ctrl+n", "新建" }, { "ctrl+o", "打开" },
                    { "ctrl+t", "自由变换" }, { "ctrl+j", "复制图层" },
                    { "ctrl+shift+n", "新建图层" }, { "ctrl+e", "合并图层" },
                    { "ctrl+shift+e", "合并可见图层" }, { "ctrl+d", "取消选择" },
                    { "ctrl+shift+i", "反选" }, { "b", "画笔" }, { "e", "橡皮擦" },
                    { "g", "渐变/填充" }, { "i", "吸管" }, { "l", "套索" },
                    { "m", "选框" }, { "p", "钢笔" }, { "t", "文字" },
                    { "v", "移动" }, { "w", "魔棒" }, { "space", "抓手" },
                    { "alt", "临时吸管" }, { "[", "缩小笔刷" }, { "]", "放大笔刷" },
                }
            },
        };
    }

    // ==================== 内置外观预设 ====================
    public static class BuiltinThemes
    {
        public static ThemePreset Dark = new ThemePreset
        {
            Name = "默认深色",
            Colors = new ThemeColors
            {
                Background = "#2B2B2B",
                InputBackground = "#1E1E1E",
                Foreground = "#E0D0B6",
                SecondaryText = "#888888",
                Highlight = "#CC9E4C",
                AccentButton = "#4A6A8C",
                DangerButton = "#6B2717",
                SuccessButton = "#769365",
                NeutralButton = "#555555",
                Border = "#444444",
                Selection = "#8B9EA5",
            }
        };

        public static ThemePreset Light = new ThemePreset
        {
            Name = "默认浅色",
            Colors = new ThemeColors
            {
                Background = "#F5F5F5",
                InputBackground = "#FFFFFF",
                Foreground = "#222222",
                SecondaryText = "#707070",
                Highlight = "#D08000",
                AccentButton = "#3A6EBF",
                DangerButton = "#AA333C",
                SuccessButton = "#3C5227",
                NeutralButton = "#D0D0D0",
                Border = "#C0C0C0",
                Selection = "#99D2FB",
            }
        };

        public static List<ThemePreset> All()
        {
            return new List<ThemePreset> { Dark, Light };
        }

        public static ThemePreset Find(string name)
        {
            foreach (var t in All())
                if (t.Name == name) return t;
            return null;
        }
    }

    // ==================== 外观预设管理 ====================
    public static class ThemeManager
    {
        public static string PresetsFolder { get; set; }
        public static ThemePreset Current { get; set; } = BuiltinThemes.Dark;

        public static List<ThemePreset> LoadUserPresets()
        {
            var list = new List<ThemePreset>();
            try
            {
                if (string.IsNullOrEmpty(PresetsFolder) || !Directory.Exists(PresetsFolder))
                    return list;

                foreach (var f in Directory.GetFiles(PresetsFolder, "*.json"))
                {
                    try
                    {
                        string json = File.ReadAllText(f);
                        var p = JsonConvert.DeserializeObject<ThemePreset>(json);
                        if (p != null && !string.IsNullOrEmpty(p.Name) && p.Colors != null)
                            list.Add(p);
                    }
                    catch { }
                }
            }
            catch { }
            list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            return list;
        }

        public static bool SaveUserPreset(ThemePreset preset)
        {
            try
            {
                if (string.IsNullOrEmpty(PresetsFolder)) return false;
                if (!Directory.Exists(PresetsFolder))
                    Directory.CreateDirectory(PresetsFolder);

                string safeName = SanitizeFileName(preset.Name);
                if (string.IsNullOrEmpty(safeName)) return false;
                string path = Path.Combine(PresetsFolder, safeName + ".json");

                string json = JsonConvert.SerializeObject(preset, Formatting.Indented);
                File.WriteAllText(path, json);
                return true;
            }
            catch { return false; }
        }

        public static bool DeleteUserPreset(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(PresetsFolder)) return false;
                string safeName = SanitizeFileName(name);
                string path = Path.Combine(PresetsFolder, safeName + ".json");
                if (File.Exists(path)) { File.Delete(path); return true; }
            }
            catch { }
            return false;
        }

        public static ThemePreset FindByName(string name)
        {
            var b = BuiltinThemes.Find(name);
            if (b != null) return b;

            foreach (var p in LoadUserPresets())
                if (p.Name == name) return p;

            return null;
        }

        public static bool IsBuiltin(string name)
        {
            return name == "默认深色" || name == "默认浅色";
        }

        public static List<string> AllNames()
        {
            var names = new List<string> { "默认深色", "默认浅色" };
            foreach (var p in LoadUserPresets())
                names.Add(p.Name);
            return names;
        }

        private static string SanitizeFileName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Trim();
        }
    }
}