# 画笔咻咻

绘画时按下鼠标、键盘、数位笔，实时播放自定义音效的辅助工具。

适用于 Clip Studio Paint、SAI 2、Photoshop 等绘画软件。

## 功能

- 按住鼠标左键循环播放音效，松开立即停止
- 支持键盘单键、组合键、鼠标按键映射
- 支持数位笔（Wacom 等）
- 每个按键独立设置音效文件夹和播放模式
- 循环 / 续笔（松开暂停） / 随机 / 顺序播放
- 前段截取：只播开头 N 毫秒
- 独立音量，不影响系统音量
- 内置 CSP / SAI 2 / PS 常用键位预设
- 深色 / 浅色 / 跟随系统，支持自定义配色
- 系统托盘最小化
- 全中文界面

## 系统要求

- Windows 10 / 11
- .NET Framework 4.8

## 下载与安装

1. 前往 [Releases](https://github.com/Houkuto-C/brush-sound/releases) 页面下载最新版 zip
2. 解压到任意位置
3. 双击 `画笔咻咻.exe`（首次运行会弹 UAC，点"是"）
4. 首次运行会自动生成 `config.json` 和 `Presets\` 文件夹

## 目录结构
画笔咻咻
├── 画笔咻咻.exe
├── BrushSound.exe.config
├── NAudio.dll
├── Newtonsoft.Json.dll
├── Gma.System.MouseKeyHook.dll
├── config.json ← 首次运行生成
├── Sounds\ ← 音效文件放这里
│ ├── 通用音频
│ └── 快捷键音频
└── Presets\ ← 首次运行生成

**Sounds 里只识别子文件夹**，根目录直接放的文件不会被识别。

**支持格式**：`.wav` / `.mp3` / `.aiff` / `.aif`

## 使用说明

程序内置"？使用说明"按钮，里面有详细的操作指南。

## 从源码编译

需要 Visual Studio 2019 或更高版本，安装 **.NET 桌面开发** 工作负载。

1. 用 Visual Studio 打开 `BrushSound.sln`
2. NuGet 会自动还原依赖（NAudio、Newtonsoft.Json、MouseKeyHook）
3. 生成 → 重新生成解决方案

## 灵感来源

- [画吧活爹](https://pan.baidu.com/s/1VgOIM1lkeoYKpS_pKrpJAw?pwd=2mrm)
- [按键反馈-无料垃圾](https://xhslink.cn/o/4uODViNa2i1)

## 开源协议

[MIT License](LICENSE)

## 联系

- 作者：Houkuto-C
- GitHub：[@Houkuto-C](https://github.com/Houkuto-C)
