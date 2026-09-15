using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BrushSound
{
    public class AppearanceForm : Form
    {
        private class ColorRow
        {
            public string Key;
            public string LabelText;
            public Panel Swatch;
            public Label HexLabel;
            public RoundButton PickButton;
            public Color CurrentColor;
        }

        private readonly Dictionary<string, ColorRow> rows = new Dictionary<string, ColorRow>();
        private readonly string[] rowOrder = new string[]
        {
            "Background", "InputBackground", "Foreground", "SecondaryText", "Highlight",
            "AccentButton", "DangerButton", "SuccessButton", "NeutralButton",
            "Border", "Selection"
        };
        private readonly Dictionary<string, string> rowLabels = new Dictionary<string, string>
        {
            { "Background", "背景色" },
            { "InputBackground", "输入框背景色" },
            { "Foreground", "主文字色" },
            { "SecondaryText", "次要文字色" },
            { "Highlight", "强调文字色" },
            { "AccentButton", "主按钮色" },
            { "DangerButton", "危险按钮色" },
            { "SuccessButton", "成功按钮色" },
            { "NeutralButton", "中性按钮色" },
            { "Border", "边框色" },
            { "Selection", "选中高亮色" },
        };

        private ComboBox cmbPreset;
        private RoundButton btnSaveAs;
        private RoundButton btnDelete;

        private CheckBox chkFollowSystem;
        private Label lblFollowHint;
        private Label lblFollowLight;
        private ComboBox cmbFollowLight;
        private Label lblFollowDark;
        private ComboBox cmbFollowDark;

        private Panel previewPanel;
        private Label lblPreviewTitle;
        private Label lblPreviewCount;
        private Label lblPreviewHint;
        private RoundButton previewAccent;
        private RoundButton previewDanger;
        private RoundButton previewSuccess;
        private RoundButton previewNeutral;

        private ThemePreset editingPreset;

        /// <summary>确定时返回给主界面的主题名（预设名或"跟随系统"）</summary>
        public string ResultThemeName { get; private set; }

        public AppearanceForm(string currentTheme)
        {
            bool isFollowSystem = (currentTheme == "跟随系统");

            string presetName = isFollowSystem ? "默认深色" : currentTheme;
            var preset = ThemeManager.FindByName(presetName);
            if (preset == null) preset = BuiltinThemes.Dark;

            editingPreset = new ThemePreset
            {
                Name = presetName,
                Colors = preset.Colors.Clone()
            };

            BuildUI();
            RefreshPresetCombo();
            LoadPresetIntoUI();
            chkFollowSystem.Checked = isFollowSystem;
            UpdateFollowSystemEnabled();
            UpdatePreview();
        }

        // ==================== UI 构建 ====================
        private void BuildUI()
        {
            this.Text = "画笔咻咻 · 外观设置";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(43, 43, 43);
            this.ForeColor = Color.White;
            this.Font = new Font("微软雅黑", 10F);

            int y = 15;

            // ===== 顶部：当前预设 + 两个按钮 =====
            var lblPreset = new Label
            {
                Text = "当前预设：",
                Location = new Point(15, y + 4),
                AutoSize = true,
                ForeColor = Color.LightGray
            };
            this.Controls.Add(lblPreset);

            cmbPreset = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(105, y),
                Size = new Size(180, 28)
            };
            cmbPreset.SelectedIndexChanged += CmbPreset_SelectedIndexChanged;
            this.Controls.Add(cmbPreset);

            btnSaveAs = new RoundButton
            {
                Text = "另存为新预设",
                Location = new Point(300, y - 2),
                Size = new Size(130, 30),
                BackColor = Color.FromArgb(74, 106, 140),
                ForeColor = Color.White,
                CornerRadius = 5
            };
            btnSaveAs.Click += BtnSaveAs_Click;
            this.Controls.Add(btnSaveAs);

            btnDelete = new RoundButton
            {
                Text = "删除当前预设",
                Location = new Point(440, y - 2),
                Size = new Size(140, 30),
                BackColor = Color.FromArgb(140, 74, 74),
                ForeColor = Color.White,
                CornerRadius = 5
            };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            y += 42;

            // ===== 颜色区 =====
            var lblColors = new Label
            {
                Text = "颜色：",
                Location = new Point(15, y),
                AutoSize = true,
                Font = new Font("微软雅黑", 11F, FontStyle.Bold),
                ForeColor = Color.White
            };
            this.Controls.Add(lblColors);
            y += 28;

            foreach (var key in rowOrder)
            {
                CreateColorRow(key, y);
                y += 32;
            }

            y += 12;

            // ===== 跟随系统区 =====
            chkFollowSystem = new CheckBox
            {
                Text = "跟随系统",
                Location = new Point(20, y),
                AutoSize = true,
                Font = new Font("微软雅黑", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            chkFollowSystem.CheckedChanged += ChkFollowSystem_CheckedChanged;
            this.Controls.Add(chkFollowSystem);
            y += 30;

            lblFollowHint = new Label
            {
                Text = "勾选后，浅色/深色模式分别使用以下预设：",
                Location = new Point(15, y),
                Size = new Size(590, 20),
                ForeColor = Color.FromArgb(136, 136, 136),
                Font = new Font("微软雅黑", 9F)
            };
            this.Controls.Add(lblFollowHint);
            y += 25;

            lblFollowLight = new Label
            {
                Text = "系统为浅色时：",
                Location = new Point(15, y + 4),
                AutoSize = true,
                ForeColor = Color.LightGray
            };
            this.Controls.Add(lblFollowLight);

            cmbFollowLight = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(130, y),
                Size = new Size(180, 28)
            };
            this.Controls.Add(cmbFollowLight);

            lblFollowDark = new Label
            {
                Text = "系统为深色时：",
                Location = new Point(325, y + 4),
                AutoSize = true,
                ForeColor = Color.LightGray
            };
            this.Controls.Add(lblFollowDark);

            cmbFollowDark = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(440, y),
                Size = new Size(165, 28)
            };
            this.Controls.Add(cmbFollowDark);

            y += 45;

            // ===== 预览区 =====
            var lblPrevTitle = new Label
            {
                Text = "预览：",
                Location = new Point(15, y),
                AutoSize = true,
                Font = new Font("微软雅黑", 11F, FontStyle.Bold),
                ForeColor = Color.White
            };
            this.Controls.Add(lblPrevTitle);
            y += 28;

            previewPanel = new Panel
            {
                Location = new Point(15, y),
                Size = new Size(590, 130),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(previewPanel);

            lblPreviewTitle = new Label
            {
                Text = "画笔咻咻",
                Location = new Point(15, 10),
                AutoSize = true,
                Font = new Font("微软雅黑", 12F, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            previewPanel.Controls.Add(lblPreviewTitle);

            lblPreviewCount = new Label
            {
                Text = "共 5 个按键",
                Location = new Point(470, 14),
                Size = new Size(110, 25),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };
            previewPanel.Controls.Add(lblPreviewCount);

            previewAccent = new RoundButton { Text = "主按钮", Location = new Point(15, 48), Size = new Size(110, 32), CornerRadius = 5 };
            previewDanger = new RoundButton { Text = "危险按钮", Location = new Point(135, 48), Size = new Size(110, 32), CornerRadius = 5 };
            previewSuccess = new RoundButton { Text = "成功按钮", Location = new Point(255, 48), Size = new Size(110, 32), CornerRadius = 5 };
            previewNeutral = new RoundButton { Text = "中性按钮", Location = new Point(375, 48), Size = new Size(110, 32), CornerRadius = 5 };
            previewPanel.Controls.Add(previewAccent);
            previewPanel.Controls.Add(previewDanger);
            previewPanel.Controls.Add(previewSuccess);
            previewPanel.Controls.Add(previewNeutral);

            lblPreviewHint = new Label
            {
                Text = "这是示例文字，强调文字会这样显示。",
                Location = new Point(15, 92),
                Size = new Size(550, 25),
                BackColor = Color.Transparent
            };
            previewPanel.Controls.Add(lblPreviewHint);

            y += 145;

            // ===== 底部按钮 =====
            var btnReset = new RoundButton
            {
                Text = "重置为默认",
                Location = new Point(15, y),
                Size = new Size(140, 38),
                BackColor = Color.FromArgb(85, 85, 85),
                ForeColor = Color.White,
                CornerRadius = 5
            };
            btnReset.Click += BtnReset_Click;
            this.Controls.Add(btnReset);

            var btnCancel = new RoundButton
            {
                Text = "取消",
                Location = new Point(340, y),
                Size = new Size(120, 38),
                BackColor = Color.FromArgb(85, 85, 85),
                ForeColor = Color.White,
                CornerRadius = 5
            };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancel);

            var btnOK = new RoundButton
            {
                Text = "确定",
                Location = new Point(470, y),
                Size = new Size(135, 38),
                BackColor = Color.FromArgb(74, 140, 74),
                ForeColor = Color.White,
                CornerRadius = 5
            };
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            this.ClientSize = new Size(620, y + 60);
            this.MinimumSize = this.ClientSize;
        }

        private void CreateColorRow(string key, int y)
        {
            var lbl = new Label
            {
                Text = rowLabels[key],
                Location = new Point(25, y + 4),
                Size = new Size(130, 24),
                ForeColor = Color.LightGray
            };
            this.Controls.Add(lbl);

            var swatch = new Panel
            {
                Location = new Point(160, y),
                Size = new Size(70, 28),
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand
            };
            swatch.Click += (s, e) => ChooseColor(key);
            this.Controls.Add(swatch);

            var hexLbl = new Label
            {
                Text = "#000000",
                Location = new Point(245, y + 4),
                Size = new Size(110, 24),
                ForeColor = Color.LightGray,
                Font = new Font("Consolas", 10F)
            };
            this.Controls.Add(hexLbl);

            var btnPick = new RoundButton
            {
                Text = "选择",
                Location = new Point(360, y),
                Size = new Size(90, 28),
                BackColor = Color.FromArgb(85, 85, 85),
                ForeColor = Color.White,
                CornerRadius = 5
            };
            btnPick.Click += (s, e) => ChooseColor(key);
            this.Controls.Add(btnPick);

            rows[key] = new ColorRow
            {
                Key = key,
                LabelText = rowLabels[key],
                Swatch = swatch,
                HexLabel = hexLbl,
                PickButton = btnPick,
                CurrentColor = Color.Black
            };
        }

        // ==================== 预设下拉 ====================
        private void RefreshPresetCombo()
        {
            var names = ThemeManager.AllNames();
            cmbPreset.Items.Clear();
            foreach (var n in names) cmbPreset.Items.Add(n);

            int idx = cmbPreset.Items.IndexOf(editingPreset.Name);
            if (idx >= 0) cmbPreset.SelectedIndex = idx;
            else if (cmbPreset.Items.Count > 0) cmbPreset.SelectedIndex = 0;

            cmbFollowLight.Items.Clear();
            cmbFollowDark.Items.Clear();
            foreach (var n in names)
            {
                cmbFollowLight.Items.Add(n);
                cmbFollowDark.Items.Add(n);
            }
            string fl = ConfigManager.Current.FollowSystemLightPreset;
            string fd = ConfigManager.Current.FollowSystemDarkPreset;
            if (cmbFollowLight.Items.Contains(fl)) cmbFollowLight.SelectedItem = fl;
            else if (cmbFollowLight.Items.Count > 0) cmbFollowLight.SelectedIndex = 0;
            if (cmbFollowDark.Items.Contains(fd)) cmbFollowDark.SelectedItem = fd;
            else if (cmbFollowDark.Items.Count > 0) cmbFollowDark.SelectedIndex = 0;
        }

        private void LoadPresetIntoUI()
        {
            foreach (var key in rowOrder)
            {
                Color c = GetColorFromPreset(editingPreset.Colors, key);
                rows[key].CurrentColor = c;
                rows[key].Swatch.BackColor = c;
                rows[key].HexLabel.Text = ThemeColors.ColorToHex(c);
            }

            bool isBuiltin = ThemeManager.IsBuiltin(editingPreset.Name);
            btnDelete.Enabled = !isBuiltin;
            btnDelete.BackColor = isBuiltin
                ? Color.FromArgb(80, 60, 60)
                : Color.FromArgb(140, 74, 74);
        }

        // ==================== 跟随系统 ====================
        private void ChkFollowSystem_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFollowSystemEnabled();
            UpdatePreview();
        }

        private void UpdateFollowSystemEnabled()
        {
            bool follow = chkFollowSystem.Checked;

            // 颜色编辑区
            foreach (var key in rowOrder)
            {
                var row = rows[key];
                row.Swatch.Enabled = !follow;
                row.PickButton.Enabled = !follow;
                row.Swatch.Cursor = follow ? Cursors.Default : Cursors.Hand;
                row.Swatch.BackColor = row.CurrentColor;
            }

            // 顶部预设相关
            cmbPreset.Enabled = !follow;
            btnSaveAs.Enabled = !follow;
            btnDelete.Enabled = !follow && !ThemeManager.IsBuiltin(editingPreset.Name);

            // 跟随系统区
            cmbFollowLight.Enabled = follow;
            cmbFollowDark.Enabled = follow;

            // 说明文字保持固定
            lblFollowHint.Text = "勾选后，浅色/深色模式分别使用以下预设：";

            // 视觉强调
            var c = ThemeManager.Current.Colors;
            lblFollowHint.ForeColor = follow
                ? c.C_Highlight
                : Color.FromArgb(136, 136, 136);
        }

        // ==================== 预览 ====================
        private void UpdatePreview()
        {
            ThemeColors previewColors;

            if (chkFollowSystem.Checked)
            {
                // 跟随系统时预览：显示"系统当前模式下"的效果
                bool light = IsSystemLightTheme();
                string presetName = light
                    ? (cmbFollowLight.SelectedItem as string ?? "默认浅色")
                    : (cmbFollowDark.SelectedItem as string ?? "默认深色");
                var preset = ThemeManager.FindByName(presetName);
                previewColors = preset != null ? preset.Colors : BuiltinThemes.Dark.Colors;
            }
            else
            {
                previewColors = editingPreset.Colors;
            }

            previewPanel.BackColor = previewColors.C_Background;
            lblPreviewTitle.ForeColor = previewColors.C_Foreground;
            lblPreviewCount.ForeColor = previewColors.C_Highlight;
            lblPreviewHint.ForeColor = previewColors.C_SecondaryText;

            previewAccent.BackColor = previewColors.C_AccentButton;
            previewDanger.BackColor = previewColors.C_DangerButton;
            previewSuccess.BackColor = previewColors.C_SuccessButton;
            previewNeutral.BackColor = previewColors.C_NeutralButton;

            Color fg = previewColors.C_Foreground;
            previewAccent.ForeColor = fg;
            previewDanger.ForeColor = fg;
            previewSuccess.ForeColor = fg;
            previewNeutral.ForeColor = fg;
        }

        private bool IsSystemLightTheme()
        {
            try
            {
                using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
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

        private void ChooseColor(string key)
        {
            if (chkFollowSystem.Checked) return;

            var row = rows[key];
            using (var dlg = new ColorDialog())
            {
                dlg.Color = row.CurrentColor;
                dlg.FullOpen = true;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    row.CurrentColor = dlg.Color;
                    row.Swatch.BackColor = dlg.Color;
                    row.HexLabel.Text = ThemeColors.ColorToHex(dlg.Color);

                    // 关键修复：把颜色写回 editingPreset.Colors
                    SetColorToPreset(editingPreset.Colors, key, dlg.Color);

                    UpdatePreview();
                }
            }
        }

        // ==================== 预设切换 ====================
        private void CmbPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = cmbPreset.SelectedItem as string;
            if (string.IsNullOrEmpty(name)) return;
            if (name == editingPreset.Name) return;

            var preset = ThemeManager.FindByName(name);
            if (preset == null) return;

            editingPreset = new ThemePreset
            {
                Name = name,
                Colors = preset.Colors.Clone()
            };
            LoadPresetIntoUI();
            UpdatePreview();
        }

        private void BtnSaveAs_Click(object sender, EventArgs e)
        {
            string name = PromptForName("另存为新预设", "请输入新预设的名称：", editingPreset.Name);
            if (string.IsNullOrEmpty(name)) return;

            if (ThemeManager.IsBuiltin(name))
            {
                MessageBox.Show("“" + name + "”是内置预设名，不能使用。请换一个名字。",
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool exists = false;
            foreach (var n in ThemeManager.AllNames())
                if (n == name) { exists = true; break; }

            if (exists)
            {
                var r = MessageBox.Show("已存在同名预设“" + name + "”，是否覆盖？",
                    "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r != DialogResult.Yes) return;
            }

            ApplyUIToPreset();
            editingPreset.Name = name;

            if (!ThemeManager.SaveUserPreset(editingPreset))
            {
                MessageBox.Show("保存失败，请检查 Presets 文件夹是否可写。",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RefreshPresetCombo();
            int idx = cmbPreset.Items.IndexOf(name);
            if (idx >= 0) cmbPreset.SelectedIndex = idx;

            MessageBox.Show("已保存为新预设“" + name + "”。",
                "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (ThemeManager.IsBuiltin(editingPreset.Name))
            {
                MessageBox.Show("内置预设不能删除。",
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var r = MessageBox.Show("确定要删除预设“" + editingPreset.Name + "”吗？",
                "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r != DialogResult.Yes) return;

            ThemeManager.DeleteUserPreset(editingPreset.Name);

            editingPreset = new ThemePreset
            {
                Name = "默认深色",
                Colors = BuiltinThemes.Dark.Colors.Clone()
            };
            RefreshPresetCombo();
            LoadPresetIntoUI();
            UpdatePreview();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (chkFollowSystem.Checked)
            {
                // 跟随系统模式下重置：恢复浅色/深色绑定到默认预设
                string fl = "默认浅色";
                string fd = "默认深色";
                if (cmbFollowLight.Items.Contains(fl)) cmbFollowLight.SelectedItem = fl;
                if (cmbFollowDark.Items.Contains(fd)) cmbFollowDark.SelectedItem = fd;
                UpdatePreview();
                return;
            }

            var builtin = BuiltinThemes.Find(editingPreset.Name);
            ThemeColors source;
            if (builtin != null) source = builtin.Colors;
            else source = BuiltinThemes.Dark.Colors;

            editingPreset.Colors = source.Clone();
            LoadPresetIntoUI();
            UpdatePreview();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (chkFollowSystem.Checked)
            {
                // 保存跟随系统绑定
                if (cmbFollowLight.SelectedItem is string fl)
                    ConfigManager.Current.FollowSystemLightPreset = fl;
                if (cmbFollowDark.SelectedItem is string fd)
                    ConfigManager.Current.FollowSystemDarkPreset = fd;

                ResultThemeName = "跟随系统";
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            ApplyUIToPreset();

            if (ThemeManager.IsBuiltin(editingPreset.Name))
            {
                var orig = BuiltinThemes.Find(editingPreset.Name);
                if (orig != null && ColorsChanged(orig.Colors, editingPreset.Colors))
                {
                    var r = MessageBox.Show(
                        "内置预设不能直接修改。是否另存为新预设？",
                        "另存为", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (r == DialogResult.Yes)
                    {
                        BtnSaveAs_Click(null, EventArgs.Empty);
                        return;
                    }
                    else
                    {
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                        return;
                    }
                }
                else
                {
                    ResultThemeName = editingPreset.Name;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }
            }

            ThemeManager.SaveUserPreset(editingPreset);
            ResultThemeName = editingPreset.Name;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // ==================== 工具方法 ====================
        private void ApplyUIToPreset()
        {
            foreach (var key in rowOrder)
            {
                SetColorToPreset(editingPreset.Colors, key, rows[key].CurrentColor);
            }
        }

        private bool ColorsChanged(ThemeColors a, ThemeColors b)
        {
            foreach (var key in rowOrder)
            {
                if (GetColorFromPreset(a, key) != GetColorFromPreset(b, key))
                    return true;
            }
            return false;
        }

        private static Color GetColorFromPreset(ThemeColors c, string key)
        {
            switch (key)
            {
                case "Background": return c.C_Background;
                case "InputBackground": return c.C_InputBackground;
                case "Foreground": return c.C_Foreground;
                case "SecondaryText": return c.C_SecondaryText;
                case "Highlight": return c.C_Highlight;
                case "AccentButton": return c.C_AccentButton;
                case "DangerButton": return c.C_DangerButton;
                case "SuccessButton": return c.C_SuccessButton;
                case "NeutralButton": return c.C_NeutralButton;
                case "Border": return c.C_Border;
                case "Selection": return c.C_Selection;
            }
            return Color.Black;
        }

        private static void SetColorToPreset(ThemeColors c, string key, Color color)
        {
            string hex = ThemeColors.ColorToHex(color);
            switch (key)
            {
                case "Background": c.Background = hex; break;
                case "InputBackground": c.InputBackground = hex; break;
                case "Foreground": c.Foreground = hex; break;
                case "SecondaryText": c.SecondaryText = hex; break;
                case "Highlight": c.Highlight = hex; break;
                case "AccentButton": c.AccentButton = hex; break;
                case "DangerButton": c.DangerButton = hex; break;
                case "SuccessButton": c.SuccessButton = hex; break;
                case "NeutralButton": c.NeutralButton = hex; break;
                case "Border": c.Border = hex; break;
                case "Selection": c.Selection = hex; break;
            }
        }

        private string PromptForName(string title, string prompt, string defaultValue)
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

                if (dlg.ShowDialog(this) == DialogResult.OK)
                    return tb.Text.Trim();
                return null;
            }
        }
    }
}