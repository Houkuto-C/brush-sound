using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BrushSound
{
    public class HelpForm : Form
    {
        private TabControl tabControl;

        public HelpForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "画笔咻咻 · 使用说明";
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(43, 43, 43);
            this.ForeColor = Color.White;
            this.Font = new Font("微软雅黑", 10F);
            this.ClientSize = new Size(720, 720);

            tabControl = new TabControl
            {
                Location = new Point(10, 10),
                Size = new Size(700, 700),
                Font = new Font("微软雅黑", 10F)
            };
            this.Controls.Add(tabControl);

            tabControl.TabPages.Add(BuildBasicTab());
            tabControl.TabPages.Add(BuildPlayModeTab());
            tabControl.TabPages.Add(BuildPresetTab());
            tabControl.TabPages.Add(BuildFaqTab());
            tabControl.TabPages.Add(BuildAboutTab());
        }

        // ==================== 通用辅助 ====================
        private TabPage CreateTab(string title)
        {
            var page = new TabPage(title)
            {
                BackColor = Color.FromArgb(43, 43, 43),
                ForeColor = Color.White,
                Padding = new Padding(15),
                AutoScroll = true
            };
            return page;
        }

        private Label MakeTitle(string text, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(15, y),
                Size = new Size(650, 30),
                Font = new Font("微软雅黑", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 204, 102)
            };
        }

        private Label MakeText(string text, int y, int height = 25)
        {
            return new Label
            {
                Text = text,
                Location = new Point(15, y),
                Size = new Size(650, height),
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 10F)
            };
        }

        private Label MakeBullet(string text, int y, int height = 25)
        {
            return new Label
            {
                Text = "• " + text,
                Location = new Point(25, y),
                Size = new Size(640, height),
                ForeColor = Color.LightGray,
                Font = new Font("微软雅黑", 10F)
            };
        }

        // ==================== 页签 1：基础 ====================
        private TabPage BuildBasicTab()
        {
            var page = CreateTab("基础");
            int y = 15;

            page.Controls.Add(MakeTitle("快速上手", y)); y += 35;
            page.Controls.Add(MakeBullet("音效放在程序目录下的 Sounds 文件夹里", y)); y += 22;
            page.Controls.Add(MakeBullet("Sounds 里可以建多个子文件夹，每个文件夹放一组音效", y)); y += 22;
            page.Controls.Add(MakeBullet("例如：Sounds\\通用音频\\、Sounds\\快捷键音频\\", y)); y += 22;
            page.Controls.Add(MakeBullet("程序只识别子文件夹，Sounds 根目录直接放的文件不会被识别", y)); y += 22;
            page.Controls.Add(MakeBullet("支持格式：.wav / .mp3 / .aiff / .aif", y)); y += 30;

            page.Controls.Add(MakeTitle("添加新按键", y)); y += 35;
            page.Controls.Add(MakeBullet("点“＋ 添加/捕获新按键”按钮", y)); y += 22;
            page.Controls.Add(MakeBullet("按下你想绑定的键（可以是单个键或组合键）", y)); y += 22;
            page.Controls.Add(MakeBullet("松开所有键后，下拉框自动切换到该按键", y)); y += 22;
            page.Controls.Add(MakeBullet("再为它选音效文件夹和播放模式即可", y)); y += 30;

            page.Controls.Add(MakeTitle("备注", y)); y += 35;
            page.Controls.Add(MakeBullet("备注框可以填中文名，比如“撤销”、“保存”", y)); y += 22;
            page.Controls.Add(MakeBullet("填完后按回车或点其他地方保存", y)); y += 22;
            page.Controls.Add(MakeBullet("下拉框会显示为：ctrl+z  |  撤销", y)); y += 30;

            page.Controls.Add(MakeTitle("删除按键", y)); y += 35;
            page.Controls.Add(MakeBullet("选中要删的按键，点“🗑 删除”按钮", y)); y += 22;
            page.Controls.Add(MakeBullet("鼠标左键（mouse_left）是核心映射，不能删除", y)); y += 30;

            page.Controls.Add(MakeTitle("管理音效文件", y)); y += 35;
            page.Controls.Add(MakeBullet("点“📂 打开音效文件夹”按钮，用资源管理器打开 Sounds 目录", y)); y += 22;
            page.Controls.Add(MakeBullet("在资源管理器里添加、删除、重命名音效文件", y)); y += 22;
            page.Controls.Add(MakeBullet("回程序后切换音效文件夹，就会刷新列表", y)); y += 30;

            page.Controls.Add(MakeTitle("暂停触发", y)); y += 35;
            page.Controls.Add(MakeBullet("点“⏸ 暂停触发”按钮，所有按键不再触发音效", y)); y += 22;
            page.Controls.Add(MakeBullet("再次点击“▶ 恢复触发”恢复", y)); y += 22;
            page.Controls.Add(MakeBullet("常用场景：切到非目标软件时临时静音", y)); y += 30;

            return page;
        }

        // ==================== 页签 2：播放模式 ====================
        private TabPage BuildPlayModeTab()
        {
            var page = CreateTab("播放模式");
            int y = 15;

            page.Controls.Add(MakeTitle("播放模式说明", y)); y += 35;
            page.Controls.Add(MakeText("每个按键可以单独设置：", y)); y += 25;
            page.Controls.Add(MakeBullet("音效文件夹：从 Sounds 的子文件夹里选一个", y)); y += 22;
            page.Controls.Add(MakeBullet("播放模式：", y)); y += 22;
            page.Controls.Add(new Label
            {
                Text = "      ○ 单个文件：从该文件夹里选一个具体文件播放",
                Location = new Point(25, y),
                Size = new Size(640, 25),
                ForeColor = Color.LightGray
            }); y += 22;
            page.Controls.Add(new Label
            {
                Text = "      ○ 文件夹：播放整个文件夹里的音频",
                Location = new Point(25, y),
                Size = new Size(640, 25),
                ForeColor = Color.LightGray
            }); y += 30;

            page.Controls.Add(MakeText("文件夹模式下还可设置：", y)); y += 25;
            page.Controls.Add(MakeBullet("顺序播放：按文件名依次播放，播完最后一个回到第一个", y)); y += 22;
            page.Controls.Add(MakeBullet("随机播放：每次从文件夹里随机选一个", y)); y += 30;

            page.Controls.Add(MakeTitle("循环与续笔", y)); y += 35;
            page.Controls.Add(MakeBullet("循环播放：按住时无限循环", y)); y += 22;
            page.Controls.Add(MakeBullet("续笔：松开暂停，再次按下从暂停处继续", y)); y += 30;

            page.Controls.Add(MakeTitle("截取时长", y)); y += 35;
            page.Controls.Add(MakeBullet("只播开头 N 毫秒后停止", y)); y += 22;
            page.Controls.Add(MakeBullet("填 0 表示不截取，播放完整音频", y)); y += 22;
            page.Controls.Add(MakeBullet("循环模式下每 N 毫秒从头重播", y)); y += 35;

            page.Controls.Add(MakeTitle("完整行为对照表", y)); y += 38;

            var header = new Label
            {
                Text = "循环     续笔     截取N毫秒    按住时行为                 松开时",
                Location = new Point(25, y),
                Size = new Size(650, 25),
                Font = new Font("Consolas", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 204, 102)
            };
            page.Controls.Add(header);
            y += 28;

            string[,] table = new string[,]
            {
                { "✗", "✗", "0",   "播放一遍", "停止" },
                { "✓", "✗", "0",   "无限循环", "停止" },
                { "✗", "✓", "0",   "播放一遍", "暂停" },
                { "✓", "✓", "0",   "无限循环", "暂停" },
                { "✗", "✗", "N",   "播放 N 毫秒后停", "停止" },
                { "✓", "✗", "N",   "每 N 毫秒从头重播", "停止" },
                { "✗", "✓", "N",   "播放 N 毫秒后停", "暂停计时" },
                { "✓", "✓", "N",   "每 N 毫秒从头重播", "暂停计时" },
            };

            for (int i = 0; i < table.GetLength(0); i++)
            {
                string line = string.Format("{0,-7}{1,-8}{2,-12}{3,-26}{4}",
                    table[i, 0], table[i, 1], table[i, 2], table[i, 3], table[i, 4]);
                page.Controls.Add(new Label
                {
                    Text = line,
                    Location = new Point(25, y),
                    Size = new Size(650, 22),
                    Font = new Font("Consolas", 9.5F),
                    ForeColor = Color.White
                });
                y += 22;
            }

            y += 15;
            page.Controls.Add(MakeTitle("小提示", y)); y += 35;
            page.Controls.Add(MakeBullet("续笔时保持同一个音频，不会切换到列表的下一个", y)); y += 22;
            page.Controls.Add(MakeBullet("文件夹模式下，“文件/正在播放”会实时显示当前文件名", y)); y += 22;

            return page;
        }

        // ==================== 页签 3：预设 ====================
        private TabPage BuildPresetTab()
        {
            var page = CreateTab("预设");
            int y = 15;

            page.Controls.Add(MakeTitle("软件按键预设", y)); y += 35;
            page.Controls.Add(MakeText("本软件内置 3 套绘画软件的常用键位：", y)); y += 25;
            page.Controls.Add(MakeBullet("Clip Studio Paint（CSP）", y)); y += 22;
            page.Controls.Add(MakeBullet("SAI 2", y)); y += 22;
            page.Controls.Add(MakeBullet("Photoshop（PS）", y)); y += 30;

            page.Controls.Add(MakeText("应用预设：", y)); y += 25;
            page.Controls.Add(MakeBullet("从下拉框选中一套，点“应用此预设”", y)); y += 22;
            page.Controls.Add(MakeBullet("只会添加新键位，不会覆盖你已有的设置", y)); y += 22;
            page.Controls.Add(MakeBullet("预设只记录键位和中文备注，不含音效文件", y)); y += 30;

            page.Controls.Add(MakeTitle("自定义预设", y)); y += 35;
            page.Controls.Add(MakeBullet("点“保存当前映射为预设”把当前所有按键存下来", y)); y += 22;
            page.Controls.Add(MakeBullet("自己的预设会在下拉里显示 ⭐ 前缀", y)); y += 22;
            page.Controls.Add(MakeBullet("选中自己的预设，点“删除选中的自定义预设”可删除", y)); y += 22;
            page.Controls.Add(MakeBullet("内置预设不能删除", y)); y += 30;

            page.Controls.Add(MakeTitle("外观预设", y)); y += 35;
            page.Controls.Add(MakeBullet("在外观设置里可以保存整套配色方案", y)); y += 22;
            page.Controls.Add(MakeBullet("外观预设存成独立 json 文件，放在 Presets 文件夹", y)); y += 22;
            page.Controls.Add(MakeBullet("可以拷贝给别人，或从别人那里拿来用", y)); y += 30;

            page.Controls.Add(MakeTitle("文件位置", y)); y += 35;
            page.Controls.Add(MakeBullet("config.json   所有按键映射、音量、延迟等设置", y)); y += 22;
            page.Controls.Add(MakeBullet("Sounds\\       音效文件", y)); y += 22;
            page.Controls.Add(MakeBullet("Presets\\      外观预设", y)); y += 30;
            page.Controls.Add(MakeText("上面 3 样和 exe 文件必须在一个文件夹内。", y)); y += 25;

            return page;
        }

        // ==================== 页签 4：常见问题 ====================
        private TabPage BuildFaqTab()
        {
            var page = CreateTab("常见问题");
            int y = 15;

            page.Controls.Add(MakeTitle("数位笔按了没反应", y)); y += 35;
            page.Controls.Add(MakeBullet("以管理员身份运行本程序（重要）", y)); y += 22;
            page.Controls.Add(MakeBullet("尝试把数位板驱动的 Windows Ink 模式关闭", y)); y += 30;

            page.Controls.Add(MakeTitle("新加的音效不显示", y)); y += 35;
            page.Controls.Add(MakeBullet("点“📂 打开音效文件夹”，确认文件已放入正确子文件夹", y)); y += 22;
            page.Controls.Add(MakeBullet("回程序切换一次音效文件夹下拉，列表会刷新", y)); y += 22;
            page.Controls.Add(MakeBullet("确认格式是 wav / mp3 / aiff / aif 之一", y)); y += 22;
            page.Controls.Add(MakeBullet("空文件夹不会出现在下拉列表里", y)); y += 30;

            page.Controls.Add(MakeTitle("托盘图标在哪", y)); y += 35;
            page.Controls.Add(MakeBullet("关闭窗口后程序会最小化到系统托盘", y)); y += 22;
            page.Controls.Add(MakeBullet("托盘图标在任务栏右下角，点小箭头可以展开", y)); y += 22;
            page.Controls.Add(MakeBullet("双击托盘图标可以恢复窗口", y)); y += 22;
            page.Controls.Add(MakeBullet("右键托盘图标可以显示窗口或退出", y)); y += 30;

            page.Controls.Add(MakeTitle("内存占用", y)); y += 35;
            page.Controls.Add(MakeBullet("正常运行约 40-70 MB", y)); y += 22;
            page.Controls.Add(MakeBullet("音效文件越多、越大，占用会相应增加", y)); y += 30;

            page.Controls.Add(MakeTitle("修改程序标题", y)); y += 35;
            page.Controls.Add(MakeBullet("点击顶部的 🖌 图标，即可编辑标题", y)); y += 22;
            page.Controls.Add(MakeBullet("清空后失焦，会恢复默认标题“画笔咻咻”", y)); y += 30;

            page.Controls.Add(MakeTitle("暂停与退出", y)); y += 35;
            page.Controls.Add(MakeBullet("“⏸ 暂停触发”：临时静音所有按键音效", y)); y += 22;
            page.Controls.Add(MakeBullet("右上角 ✕：最小化到系统托盘（程序继续运行）", y)); y += 22;
            page.Controls.Add(MakeBullet("“完全关闭程序”：彻底退出，任务栏和托盘都不再有图标", y)); y += 30;

            page.Controls.Add(MakeTitle("数位笔兼容性说明", y)); y += 35;
            page.Controls.Add(MakeBullet("SAI2 鼠标模式、CSP、PS：完美支持", y)); y += 22;
            page.Controls.Add(MakeBullet("SAI2 Windows Ink 模式：绘画时正常，但操作本程序窗口时 SAI2 可能短暂卡顿", y, 40)); y += 40;
            page.Controls.Add(MakeBullet("OneNote：暂不支持（使用 Windows Pointer API，无法捕获）", y, 40)); y += 40;

            return page;
        }

        // ==================== 页签 5：关于 ====================
        private TabPage BuildAboutTab()
        {
            var page = CreateTab("关于");
            int y = 15;

            var lblName = new Label
            {
                Text = "画笔咻咻",
                Location = new Point(15, y),
                Size = new Size(650, 40),
                Font = new Font("微软雅黑", 20F, FontStyle.Bold),
                ForeColor = Color.White
            };
            page.Controls.Add(lblName);
            y += 45;

            var lblVer = new Label
            {
                Text = "版本 1.0.0",
                Location = new Point(15, y),
                Size = new Size(650, 25),
                ForeColor = Color.Gray
            };
            page.Controls.Add(lblVer);
            y += 35;

            var sep = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(15, y),
                Size = new Size(650, 2)
            };
            page.Controls.Add(sep);
            y += 20;

            var about = ConfigManager.Current.About ?? new AboutInfo();

            AddInfoRow(page, "作者：", about.Author, ref y, false);
            AddInfoRow(page, "联系方式：", about.Contact, ref y, false);
            AddInfoRow(page, "GitHub：", about.Github, ref y, true);

            y += 15;

            var sep2 = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(15, y),
                Size = new Size(650, 2)
            };
            page.Controls.Add(sep2);
            y += 20;

            // 灵感来源标题
            page.Controls.Add(MakeTitle("灵感来源", y));
            y += 35;

            // 画吧活爹
            page.Controls.Add(new Label
            {
                Text = "画吧活爹",
                Location = new Point(25, y),
                Size = new Size(640, 24),
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 10.5F, FontStyle.Bold)
            });
            y += 26;
            AddLinkRow(page, "下载页：", "https://pan.baidu.com/s/1VgOIM1lkeoYKpS_pKrpJAw?pwd=2mrm", ref y);
            y += 6;

            // 按键反馈-无料垃圾
            page.Controls.Add(new Label
            {
                Text = "按键反馈-无料垃圾",
                Location = new Point(25, y),
                Size = new Size(640, 24),
                ForeColor = Color.White,
                Font = new Font("微软雅黑", 10.5F, FontStyle.Bold)
            });
            y += 26;
            AddLinkRow(page, "发布页：", "https://xhslink.cn/o/4uODViNa2i1", ref y);
            AddLinkRow(page, "下载页：", "https://pan.baidu.com/s/1wG1LYCGKp_iWetDaT2h1Jw?pwd=xfqg", ref y);
            y += 15;

            var sep3 = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(15, y),
                Size = new Size(650, 2)
            };
            page.Controls.Add(sep3);
            y += 20;

            page.Controls.Add(MakeTitle("基于以下开源库", y)); y += 35;
            page.Controls.Add(MakeBullet("NAudio（MIT License）— 音频播放", y)); y += 22;
            page.Controls.Add(MakeBullet("Newtonsoft.Json（MIT License）— 配置读写", y)); y += 22;
            page.Controls.Add(MakeBullet("MouseKeyHook（MIT License）— 全局键盘鼠标监听", y)); y += 30;

            page.Controls.Add(MakeText("感谢使用画笔咻咻。", y)); y += 25;

            return page;
        }

        private void AddInfoRow(TabPage page, string label, string value, ref int y, bool isLink)
        {
            var lblKey = new Label
            {
                Text = label,
                Location = new Point(15, y),
                Size = new Size(100, 25),
                ForeColor = Color.LightGray
            };
            page.Controls.Add(lblKey);

            if (string.IsNullOrEmpty(value))
            {
                var lblEmpty = new Label
                {
                    Text = "（未填写）",
                    Location = new Point(120, y),
                    Size = new Size(540, 25),
                    ForeColor = Color.Gray
                };
                page.Controls.Add(lblEmpty);
                y += 30;
                return;
            }

            if (isLink && (value.StartsWith("http://") || value.StartsWith("https://")))
            {
                var link = new LinkLabel
                {
                    Text = value,
                    Location = new Point(120, y),
                    Size = new Size(540, 25),
                    LinkColor = Color.FromArgb(100, 180, 255),
                    ActiveLinkColor = Color.FromArgb(255, 200, 100),
                    VisitedLinkColor = Color.FromArgb(180, 140, 220),
                    Font = new Font("微软雅黑", 10F, FontStyle.Underline)
                };
                link.Click += (s, e) =>
                {
                    try { Process.Start(new ProcessStartInfo(value) { UseShellExecute = true }); }
                    catch { MessageBox.Show("无法打开链接：\n" + value, "提示"); }
                };
                page.Controls.Add(link);
            }
            else
            {
                var lblVal = new Label
                {
                    Text = value,
                    Location = new Point(120, y),
                    Size = new Size(540, 25),
                    ForeColor = Color.White
                };
                page.Controls.Add(lblVal);
            }
            y += 30;
        }

        private void AddLinkRow(TabPage page, string label, string url, ref int y)
        {
            var lblKey = new Label
            {
                Text = label,
                Location = new Point(45, y),
                Size = new Size(75, 25),
                ForeColor = Color.LightGray,
                Font = new Font("微软雅黑", 9.5F)
            };
            page.Controls.Add(lblKey);

            var link = new LinkLabel
            {
                Text = url,
                Location = new Point(125, y),
                Size = new Size(535, 25),
                LinkColor = Color.FromArgb(100, 180, 255),
                ActiveLinkColor = Color.FromArgb(255, 200, 100),
                VisitedLinkColor = Color.FromArgb(180, 140, 220),
                Font = new Font("微软雅黑", 9F, FontStyle.Underline)
            };
            link.Click += (s, e) =>
            {
                try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
                catch { MessageBox.Show("无法打开链接：\n" + url, "提示"); }
            };
            page.Controls.Add(link);
            y += 26;
        }
    }
}