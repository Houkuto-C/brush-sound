namespace BrushSound
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtAppTitle = new System.Windows.Forms.TextBox();
            this.lblMappingCount = new System.Windows.Forms.Label();
            this.lblCurrentKeyDisplay = new System.Windows.Forms.Label();
            this.lblEditKey = new System.Windows.Forms.Label();
            this.cmbEditKey = new System.Windows.Forms.ComboBox();
            this.btnDeleteKey = new BrushSound.RoundButton();
            this.lblDisplayName = new System.Windows.Forms.Label();
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.btnAddKey = new BrushSound.RoundButton();
            this.grpCurrent = new System.Windows.Forms.GroupBox();
            this.lblModDelay = new System.Windows.Forms.Label();
            this.numModDelay = new System.Windows.Forms.NumericUpDown();
            this.lblMs = new System.Windows.Forms.Label();
            this.btnOpenFolder = new BrushSound.RoundButton();
            this.lblFolder = new System.Windows.Forms.Label();
            this.cmbFolder = new System.Windows.Forms.ComboBox();
            this.lblFileLabel = new System.Windows.Forms.Label();
            this.cmbFile = new System.Windows.Forms.ComboBox();
            this.lblPlayMode = new System.Windows.Forms.Label();
            this.rdoFile = new System.Windows.Forms.RadioButton();
            this.rdoFolder = new System.Windows.Forms.RadioButton();
            this.panelFolderMode = new System.Windows.Forms.Panel();
            this.rdoSeq = new System.Windows.Forms.RadioButton();
            this.rdoRand = new System.Windows.Forms.RadioButton();
            this.chkLoopCurrent = new System.Windows.Forms.CheckBox();
            this.chkLoopList = new System.Windows.Forms.CheckBox();
            this.chkResume = new System.Windows.Forms.CheckBox();
            this.lblClip = new System.Windows.Forms.Label();
            this.numClip = new System.Windows.Forms.NumericUpDown();
            this.cmbClipUnit = new System.Windows.Forms.ComboBox();
            this.lblClipHint = new System.Windows.Forms.Label();
            this.lblSoundDir = new System.Windows.Forms.Label();
            this.lblVolume = new System.Windows.Forms.Label();
            this.trkVolume = new System.Windows.Forms.TrackBar();
            this.lblPreset = new System.Windows.Forms.Label();
            this.cmbPreset = new System.Windows.Forms.ComboBox();
            this.btnApplyPreset = new BrushSound.RoundButton();
            this.btnSavePreset = new BrushSound.RoundButton();
            this.btnDeletePreset = new BrushSound.RoundButton();
            this.lblTheme = new System.Windows.Forms.Label();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.btnAppearance = new BrushSound.RoundButton();
            this.lblHint = new System.Windows.Forms.Label();
            this.btnHelp = new BrushSound.RoundButton();
            this.btnQuit = new BrushSound.RoundButton();
            this.grpCurrent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClip)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkVolume)).BeginInit();
            this.panelFolderMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle（图标）
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Emoji", 16F);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(40, 36);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🖌";
            // 
            // txtAppTitle
            // 
            this.txtAppTitle.BackColor = System.Drawing.Color.FromArgb(43, 43, 43);
            this.txtAppTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAppTitle.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold);
            this.txtAppTitle.ForeColor = System.Drawing.Color.White;
            this.txtAppTitle.Location = new System.Drawing.Point(62, 24);
            this.txtAppTitle.MaxLength = 20;
            this.txtAppTitle.Name = "txtAppTitle";
            this.txtAppTitle.Size = new System.Drawing.Size(270, 31);
            this.txtAppTitle.TabIndex = 1;
            // 
            // lblMappingCount
            // 
            this.lblMappingCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMappingCount.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lblMappingCount.ForeColor = System.Drawing.Color.FromArgb(255, 204, 102);
            this.lblMappingCount.Location = new System.Drawing.Point(360, 24);
            this.lblMappingCount.Name = "lblMappingCount";
            this.lblMappingCount.Size = new System.Drawing.Size(230, 28);
            this.lblMappingCount.TabIndex = 2;
            this.lblMappingCount.Text = "共 1 个按键";
            this.lblMappingCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCurrentKeyDisplay
            // 
            this.lblCurrentKeyDisplay.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lblCurrentKeyDisplay.ForeColor = System.Drawing.Color.FromArgb(120, 200, 255);
            this.lblCurrentKeyDisplay.Location = new System.Drawing.Point(20, 65);
            this.lblCurrentKeyDisplay.Name = "lblCurrentKeyDisplay";
            this.lblCurrentKeyDisplay.Size = new System.Drawing.Size(400, 28);
            this.lblCurrentKeyDisplay.TabIndex = 3;
            this.lblCurrentKeyDisplay.Text = "当前按键：—";
            this.lblCurrentKeyDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEditKey
            // 
            this.lblEditKey.AutoSize = true;
            this.lblEditKey.ForeColor = System.Drawing.Color.LightGray;
            this.lblEditKey.Location = new System.Drawing.Point(20, 108);
            this.lblEditKey.Name = "lblEditKey";
            this.lblEditKey.Size = new System.Drawing.Size(85, 20);
            this.lblEditKey.TabIndex = 4;
            this.lblEditKey.Text = "编辑按键：";
            // 
            // cmbEditKey
            // 
            this.cmbEditKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEditKey.FormattingEnabled = true;
            this.cmbEditKey.Location = new System.Drawing.Point(110, 105);
            this.cmbEditKey.Name = "cmbEditKey";
            this.cmbEditKey.Size = new System.Drawing.Size(350, 28);
            this.cmbEditKey.TabIndex = 5;
            // 
            // btnDeleteKey
            // 
            this.btnDeleteKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteKey.BackColor = System.Drawing.Color.FromArgb(176, 42, 42);
            this.btnDeleteKey.CornerRadius = 5;
            this.btnDeleteKey.FlatAppearance.BorderSize = 0;
            this.btnDeleteKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteKey.ForeColor = System.Drawing.Color.White;
            this.btnDeleteKey.Location = new System.Drawing.Point(470, 103);
            this.btnDeleteKey.Name = "btnDeleteKey";
            this.btnDeleteKey.Size = new System.Drawing.Size(120, 32);
            this.btnDeleteKey.TabIndex = 6;
            this.btnDeleteKey.Text = "🗑 删除";
            this.btnDeleteKey.UseVisualStyleBackColor = false;
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.AutoSize = true;
            this.lblDisplayName.ForeColor = System.Drawing.Color.LightGray;
            this.lblDisplayName.Location = new System.Drawing.Point(20, 153);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(50, 20);
            this.lblDisplayName.TabIndex = 7;
            this.lblDisplayName.Text = "备注：";
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDisplayName.Location = new System.Drawing.Point(75, 150);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(280, 28);
            this.txtDisplayName.TabIndex = 8;
            // 
            // btnAddKey
            // 
            this.btnAddKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddKey.BackColor = System.Drawing.Color.FromArgb(74, 106, 140);
            this.btnAddKey.CornerRadius = 5;
            this.btnAddKey.FlatAppearance.BorderSize = 0;
            this.btnAddKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddKey.ForeColor = System.Drawing.Color.White;
            this.btnAddKey.Location = new System.Drawing.Point(365, 148);
            this.btnAddKey.Name = "btnAddKey";
            this.btnAddKey.Size = new System.Drawing.Size(225, 32);
            this.btnAddKey.TabIndex = 9;
            this.btnAddKey.Text = "＋ 添加/捕获新按键";
            this.btnAddKey.UseVisualStyleBackColor = false;
            // 
            // grpCurrent
            // 
            this.grpCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCurrent.Controls.Add(this.lblModDelay);
            this.grpCurrent.Controls.Add(this.numModDelay);
            this.grpCurrent.Controls.Add(this.lblMs);
            this.grpCurrent.Controls.Add(this.btnOpenFolder);
            this.grpCurrent.Controls.Add(this.lblFolder);
            this.grpCurrent.Controls.Add(this.cmbFolder);
            this.grpCurrent.Controls.Add(this.lblFileLabel);
            this.grpCurrent.Controls.Add(this.cmbFile);
            this.grpCurrent.Controls.Add(this.lblPlayMode);
            this.grpCurrent.Controls.Add(this.rdoFile);
            this.grpCurrent.Controls.Add(this.rdoFolder);
            this.grpCurrent.Controls.Add(this.panelFolderMode);
            this.grpCurrent.Controls.Add(this.chkLoopCurrent);
            this.grpCurrent.Controls.Add(this.chkLoopList);
            this.grpCurrent.Controls.Add(this.chkResume);
            this.grpCurrent.Controls.Add(this.lblClip);
            this.grpCurrent.Controls.Add(this.numClip);
            this.grpCurrent.Controls.Add(this.cmbClipUnit);
            this.grpCurrent.Controls.Add(this.lblClipHint);
            this.grpCurrent.ForeColor = System.Drawing.Color.White;
            this.grpCurrent.Location = new System.Drawing.Point(20, 190);
            this.grpCurrent.Name = "grpCurrent";
            this.grpCurrent.Size = new System.Drawing.Size(570, 385);
            this.grpCurrent.TabIndex = 10;
            this.grpCurrent.TabStop = false;
            this.grpCurrent.Text = " 当前按键设置 ";
            // 
            // lblModDelay
            // 
            this.lblModDelay.AutoSize = true;
            this.lblModDelay.Location = new System.Drawing.Point(15, 32);
            this.lblModDelay.Name = "lblModDelay";
            this.lblModDelay.Size = new System.Drawing.Size(115, 20);
            this.lblModDelay.TabIndex = 0;
            this.lblModDelay.Text = "修饰键延迟：";
            // 
            // numModDelay
            // 
            this.numModDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numModDelay.Location = new System.Drawing.Point(130, 29);
            this.numModDelay.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            this.numModDelay.Minimum = new decimal(new int[] { 30, 0, 0, 0 });
            this.numModDelay.Name = "numModDelay";
            this.numModDelay.Size = new System.Drawing.Size(70, 28);
            this.numModDelay.TabIndex = 1;
            this.numModDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numModDelay.Value = new decimal(new int[] { 120, 0, 0, 0 });
            // 
            // lblMs
            // 
            this.lblMs.AutoSize = true;
            this.lblMs.Location = new System.Drawing.Point(208, 32);
            this.lblMs.Name = "lblMs";
            this.lblMs.Size = new System.Drawing.Size(35, 20);
            this.lblMs.TabIndex = 2;
            this.lblMs.Text = "毫秒";
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.BackColor = System.Drawing.Color.FromArgb(85, 85, 85);
            this.btnOpenFolder.CornerRadius = 5;
            this.btnOpenFolder.FlatAppearance.BorderSize = 0;
            this.btnOpenFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenFolder.ForeColor = System.Drawing.Color.White;
            this.btnOpenFolder.Location = new System.Drawing.Point(15, 72);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(200, 32);
            this.btnOpenFolder.TabIndex = 3;
            this.btnOpenFolder.Text = "📂 打开音效文件夹";
            this.btnOpenFolder.UseVisualStyleBackColor = false;
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(15, 122);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(95, 20);
            this.lblFolder.TabIndex = 4;
            this.lblFolder.Text = "音效文件夹：";
            // 
            // cmbFolder
            // 
            this.cmbFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbFolder.FormattingEnabled = true;
            this.cmbFolder.Location = new System.Drawing.Point(115, 119);
            this.cmbFolder.Name = "cmbFolder";
            this.cmbFolder.Size = new System.Drawing.Size(170, 28);
            this.cmbFolder.TabIndex = 5;
            // 
            // lblFileLabel
            // 
            this.lblFileLabel.AutoSize = true;
            this.lblFileLabel.Location = new System.Drawing.Point(300, 122);
            this.lblFileLabel.Name = "lblFileLabel";
            this.lblFileLabel.Size = new System.Drawing.Size(125, 20);
            this.lblFileLabel.TabIndex = 6;
            this.lblFileLabel.Text = "文件/正在播放：";
            // 
            // cmbFile
            // 
            this.cmbFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbFile.FormattingEnabled = true;
            this.cmbFile.Location = new System.Drawing.Point(430, 119);
            this.cmbFile.Name = "cmbFile";
            this.cmbFile.Size = new System.Drawing.Size(125, 28);
            this.cmbFile.TabIndex = 7;
            // 
            // lblPlayMode
            // 
            this.lblPlayMode.AutoSize = true;
            this.lblPlayMode.Location = new System.Drawing.Point(15, 168);
            this.lblPlayMode.Name = "lblPlayMode";
            this.lblPlayMode.Size = new System.Drawing.Size(70, 20);
            this.lblPlayMode.TabIndex = 8;
            this.lblPlayMode.Text = "播放模式：";
            // 
            // rdoFile
            // 
            this.rdoFile.AutoSize = true;
            this.rdoFile.Checked = true;
            this.rdoFile.Location = new System.Drawing.Point(105, 166);
            this.rdoFile.Name = "rdoFile";
            this.rdoFile.Size = new System.Drawing.Size(88, 24);
            this.rdoFile.TabIndex = 9;
            this.rdoFile.TabStop = true;
            this.rdoFile.Text = "单个文件";
            this.rdoFile.UseVisualStyleBackColor = true;
            // 
            // rdoFolder
            // 
            this.rdoFolder.AutoSize = true;
            this.rdoFolder.Location = new System.Drawing.Point(210, 166);
            this.rdoFolder.Name = "rdoFolder";
            this.rdoFolder.Size = new System.Drawing.Size(73, 24);
            this.rdoFolder.TabIndex = 10;
            this.rdoFolder.Text = "文件夹";
            this.rdoFolder.UseVisualStyleBackColor = true;
            // 
            // panelFolderMode（顺序/随机）
            // 
            this.panelFolderMode.Controls.Add(this.rdoSeq);
            this.panelFolderMode.Controls.Add(this.rdoRand);
            this.panelFolderMode.Location = new System.Drawing.Point(12, 205);
            this.panelFolderMode.Name = "panelFolderMode";
            this.panelFolderMode.Size = new System.Drawing.Size(300, 32);
            this.panelFolderMode.TabIndex = 11;
            this.panelFolderMode.Visible = false;
            // 
            // rdoSeq
            // 
            this.rdoSeq.AutoSize = true;
            this.rdoSeq.Checked = true;
            this.rdoSeq.Location = new System.Drawing.Point(3, 4);
            this.rdoSeq.Name = "rdoSeq";
            this.rdoSeq.Size = new System.Drawing.Size(103, 24);
            this.rdoSeq.TabIndex = 0;
            this.rdoSeq.TabStop = true;
            this.rdoSeq.Text = "顺序播放";
            this.rdoSeq.UseVisualStyleBackColor = true;
            // 
            // rdoRand
            // 
            this.rdoRand.AutoSize = true;
            this.rdoRand.Location = new System.Drawing.Point(118, 4);
            this.rdoRand.Name = "rdoRand";
            this.rdoRand.Size = new System.Drawing.Size(103, 24);
            this.rdoRand.TabIndex = 1;
            this.rdoRand.Text = "随机播放";
            this.rdoRand.UseVisualStyleBackColor = true;
            // 
            // chkLoopCurrent（单文件模式）
            // 
            this.chkLoopCurrent.AutoSize = true;
            this.chkLoopCurrent.Location = new System.Drawing.Point(15, 210);
            this.chkLoopCurrent.Name = "chkLoopCurrent";
            this.chkLoopCurrent.Size = new System.Drawing.Size(168, 24);
            this.chkLoopCurrent.TabIndex = 12;
            this.chkLoopCurrent.Text = "循环播放当前音效";
            this.chkLoopCurrent.UseVisualStyleBackColor = true;
            // 
            // chkLoopList（文件夹模式）
            // 
            this.chkLoopList.AutoSize = true;
            this.chkLoopList.Location = new System.Drawing.Point(15, 245);
            this.chkLoopList.Name = "chkLoopList";
            this.chkLoopList.Size = new System.Drawing.Size(148, 24);
            this.chkLoopList.TabIndex = 13;
            this.chkLoopList.Text = "循环播放列表";
            this.chkLoopList.UseVisualStyleBackColor = true;
            this.chkLoopList.Visible = false;
            // 
            // chkResume
            // 
            this.chkResume.AutoSize = true;
            this.chkResume.Location = new System.Drawing.Point(15, 285);
            this.chkResume.Name = "chkResume";
            this.chkResume.Size = new System.Drawing.Size(178, 24);
            this.chkResume.TabIndex = 14;
            this.chkResume.Text = "续笔（松开暂停）";
            this.chkResume.UseVisualStyleBackColor = true;
            // 
            // lblClip
            // 
            this.lblClip.AutoSize = true;
            this.lblClip.Location = new System.Drawing.Point(15, 330);
            this.lblClip.Name = "lblClip";
            this.lblClip.Size = new System.Drawing.Size(85, 20);
            this.lblClip.TabIndex = 15;
            this.lblClip.Text = "截取时长：";
            // 
            // numClip
            // 
            this.numClip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numClip.Location = new System.Drawing.Point(110, 327);
            this.numClip.Maximum = new decimal(new int[] { 999999, 0, 0, 0 });
            this.numClip.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numClip.Name = "numClip";
            this.numClip.Size = new System.Drawing.Size(100, 28);
            this.numClip.TabIndex = 16;
            this.numClip.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cmbClipUnit
            // 
            this.cmbClipUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClipUnit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbClipUnit.FormattingEnabled = true;
            this.cmbClipUnit.Location = new System.Drawing.Point(220, 327);
            this.cmbClipUnit.Name = "cmbClipUnit";
            this.cmbClipUnit.Size = new System.Drawing.Size(90, 28);
            this.cmbClipUnit.TabIndex = 17;
            // 
            // lblClipHint
            // 
            this.lblClipHint.AutoSize = true;
            this.lblClipHint.ForeColor = System.Drawing.Color.FromArgb(136, 136, 136);
            this.lblClipHint.Location = new System.Drawing.Point(320, 330);
            this.lblClipHint.Name = "lblClipHint";
            this.lblClipHint.Size = new System.Drawing.Size(110, 20);
            this.lblClipHint.TabIndex = 18;
            this.lblClipHint.Text = "（0 = 不截取）";
            // 
            // lblSoundDir
            // 
            this.lblSoundDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSoundDir.ForeColor = System.Drawing.Color.FromArgb(136, 136, 136);
            this.lblSoundDir.Location = new System.Drawing.Point(20, 583);
            this.lblSoundDir.Name = "lblSoundDir";
            this.lblSoundDir.Size = new System.Drawing.Size(570, 25);
            this.lblSoundDir.TabIndex = 11;
            this.lblSoundDir.Text = "音效目录：(未设置)";
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.ForeColor = System.Drawing.Color.LightGray;
            this.lblVolume.Location = new System.Drawing.Point(20, 618);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(50, 20);
            this.lblVolume.TabIndex = 12;
            this.lblVolume.Text = "音量：";
            // 
            // trkVolume
            // 
            this.trkVolume.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trkVolume.Location = new System.Drawing.Point(85, 613);
            this.trkVolume.Maximum = 100;
            this.trkVolume.Minimum = 0;
            this.trkVolume.Name = "trkVolume";
            this.trkVolume.Size = new System.Drawing.Size(505, 45);
            this.trkVolume.TabIndex = 13;
            this.trkVolume.TickFrequency = 10;
            this.trkVolume.Value = 80;
            // 
            // lblPreset
            // 
            this.lblPreset.AutoSize = true;
            this.lblPreset.ForeColor = System.Drawing.Color.LightGray;
            this.lblPreset.Location = new System.Drawing.Point(20, 673);
            this.lblPreset.Name = "lblPreset";
            this.lblPreset.Size = new System.Drawing.Size(125, 20);
            this.lblPreset.TabIndex = 14;
            this.lblPreset.Text = "软件按键预设：";
            // 
            // cmbPreset
            // 
            this.cmbPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPreset.FormattingEnabled = true;
            this.cmbPreset.Location = new System.Drawing.Point(150, 670);
            this.cmbPreset.Name = "cmbPreset";
            this.cmbPreset.Size = new System.Drawing.Size(260, 28);
            this.cmbPreset.TabIndex = 15;
            // 
            // btnApplyPreset
            // 
            this.btnApplyPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyPreset.BackColor = System.Drawing.Color.FromArgb(74, 140, 74);
            this.btnApplyPreset.CornerRadius = 5;
            this.btnApplyPreset.FlatAppearance.BorderSize = 0;
            this.btnApplyPreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyPreset.ForeColor = System.Drawing.Color.White;
            this.btnApplyPreset.Location = new System.Drawing.Point(420, 668);
            this.btnApplyPreset.Name = "btnApplyPreset";
            this.btnApplyPreset.Size = new System.Drawing.Size(170, 32);
            this.btnApplyPreset.TabIndex = 16;
            this.btnApplyPreset.Text = "应用此预设";
            this.btnApplyPreset.UseVisualStyleBackColor = false;
            // 
            // btnSavePreset
            // 
            this.btnSavePreset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavePreset.BackColor = System.Drawing.Color.FromArgb(74, 106, 140);
            this.btnSavePreset.CornerRadius = 5;
            this.btnSavePreset.FlatAppearance.BorderSize = 0;
            this.btnSavePreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePreset.ForeColor = System.Drawing.Color.White;
            this.btnSavePreset.Location = new System.Drawing.Point(20, 708);
            this.btnSavePreset.Name = "btnSavePreset";
            this.btnSavePreset.Size = new System.Drawing.Size(280, 32);
            this.btnSavePreset.TabIndex = 17;
            this.btnSavePreset.Text = "保存当前映射为预设";
            this.btnSavePreset.UseVisualStyleBackColor = false;
            // 
            // btnDeletePreset
            // 
            this.btnDeletePreset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeletePreset.BackColor = System.Drawing.Color.FromArgb(176, 42, 42);
            this.btnDeletePreset.CornerRadius = 5;
            this.btnDeletePreset.FlatAppearance.BorderSize = 0;
            this.btnDeletePreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeletePreset.ForeColor = System.Drawing.Color.White;
            this.btnDeletePreset.Location = new System.Drawing.Point(310, 708);
            this.btnDeletePreset.Name = "btnDeletePreset";
            this.btnDeletePreset.Size = new System.Drawing.Size(280, 32);
            this.btnDeletePreset.TabIndex = 18;
            this.btnDeletePreset.Text = "删除选中的自定义预设";
            this.btnDeletePreset.UseVisualStyleBackColor = false;
            // 
            // lblTheme
            // 
            this.lblTheme.AutoSize = true;
            this.lblTheme.ForeColor = System.Drawing.Color.LightGray;
            this.lblTheme.Location = new System.Drawing.Point(20, 758);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(50, 20);
            this.lblTheme.TabIndex = 19;
            this.lblTheme.Text = "外观：";
            // 
            // cmbTheme
            // 
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Location = new System.Drawing.Point(85, 755);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(180, 28);
            this.cmbTheme.TabIndex = 20;
            // 
            // btnAppearance
            // 
            this.btnAppearance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAppearance.BackColor = System.Drawing.Color.FromArgb(74, 106, 140);
            this.btnAppearance.CornerRadius = 5;
            this.btnAppearance.FlatAppearance.BorderSize = 0;
            this.btnAppearance.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAppearance.ForeColor = System.Drawing.Color.White;
            this.btnAppearance.Location = new System.Drawing.Point(430, 753);
            this.btnAppearance.Name = "btnAppearance";
            this.btnAppearance.Size = new System.Drawing.Size(160, 32);
            this.btnAppearance.TabIndex = 21;
            this.btnAppearance.Text = "🎨 外观设置";
            this.btnAppearance.UseVisualStyleBackColor = false;
            // 
            // lblHint
            // 
            this.lblHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(136, 136, 136);
            this.lblHint.Location = new System.Drawing.Point(20, 800);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(400, 25);
            this.lblHint.TabIndex = 22;
            this.lblHint.Text = "提示：鼠标悬停控件可查看说明，点 ✕ 会缩小到托盘。";
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.BackColor = System.Drawing.Color.FromArgb(74, 106, 140);
            this.btnHelp.CornerRadius = 5;
            this.btnHelp.FlatAppearance.BorderSize = 0;
            this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelp.ForeColor = System.Drawing.Color.White;
            this.btnHelp.Location = new System.Drawing.Point(430, 797);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(160, 32);
            this.btnHelp.TabIndex = 23;
            this.btnHelp.Text = "？ 使用说明";
            this.btnHelp.UseVisualStyleBackColor = false;
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.BackColor = System.Drawing.Color.FromArgb(176, 42, 42);
            this.btnQuit.CornerRadius = 5;
            this.btnQuit.FlatAppearance.BorderSize = 0;
            this.btnQuit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuit.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.btnQuit.ForeColor = System.Drawing.Color.White;
            this.btnQuit.Location = new System.Drawing.Point(20, 840);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(570, 50);
            this.btnQuit.TabIndex = 24;
            this.btnQuit.Text = "完全关闭程序";
            this.btnQuit.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(43, 43, 43);
            this.ClientSize = new System.Drawing.Size(610, 910);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.btnAppearance);
            this.Controls.Add(this.cmbTheme);
            this.Controls.Add(this.lblTheme);
            this.Controls.Add(this.btnDeletePreset);
            this.Controls.Add(this.btnSavePreset);
            this.Controls.Add(this.btnApplyPreset);
            this.Controls.Add(this.cmbPreset);
            this.Controls.Add(this.lblPreset);
            this.Controls.Add(this.trkVolume);
            this.Controls.Add(this.lblVolume);
            this.Controls.Add(this.lblSoundDir);
            this.Controls.Add(this.grpCurrent);
            this.Controls.Add(this.btnAddKey);
            this.Controls.Add(this.txtDisplayName);
            this.Controls.Add(this.lblDisplayName);
            this.Controls.Add(this.btnDeleteKey);
            this.Controls.Add(this.cmbEditKey);
            this.Controls.Add(this.lblEditKey);
            this.Controls.Add(this.lblCurrentKeyDisplay);
            this.Controls.Add(this.lblMappingCount);
            this.Controls.Add(this.txtAppTitle);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(626, 949);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "画笔咻咻";
            this.grpCurrent.ResumeLayout(false);
            this.grpCurrent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClip)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkVolume)).EndInit();
            this.panelFolderMode.ResumeLayout(false);
            this.panelFolderMode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtAppTitle;
        private System.Windows.Forms.Label lblMappingCount;
        private System.Windows.Forms.Label lblCurrentKeyDisplay;
        private System.Windows.Forms.Label lblEditKey;
        private System.Windows.Forms.ComboBox cmbEditKey;
        private BrushSound.RoundButton btnDeleteKey;
        private System.Windows.Forms.Label lblDisplayName;
        private System.Windows.Forms.TextBox txtDisplayName;
        private BrushSound.RoundButton btnAddKey;
        private System.Windows.Forms.GroupBox grpCurrent;
        private System.Windows.Forms.Label lblModDelay;
        private System.Windows.Forms.NumericUpDown numModDelay;
        private System.Windows.Forms.Label lblMs;
        private BrushSound.RoundButton btnOpenFolder;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.ComboBox cmbFolder;
        private System.Windows.Forms.Label lblFileLabel;
        private System.Windows.Forms.ComboBox cmbFile;
        private System.Windows.Forms.Label lblPlayMode;
        private System.Windows.Forms.RadioButton rdoFile;
        private System.Windows.Forms.RadioButton rdoFolder;
        private System.Windows.Forms.Panel panelFolderMode;
        private System.Windows.Forms.RadioButton rdoSeq;
        private System.Windows.Forms.RadioButton rdoRand;
        private System.Windows.Forms.CheckBox chkLoopCurrent;
        private System.Windows.Forms.CheckBox chkLoopList;
        private System.Windows.Forms.CheckBox chkResume;
        private System.Windows.Forms.Label lblClip;
        private System.Windows.Forms.NumericUpDown numClip;
        private System.Windows.Forms.ComboBox cmbClipUnit;
        private System.Windows.Forms.Label lblClipHint;
        private System.Windows.Forms.Label lblSoundDir;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.TrackBar trkVolume;
        private System.Windows.Forms.Label lblPreset;
        private System.Windows.Forms.ComboBox cmbPreset;
        private BrushSound.RoundButton btnApplyPreset;
        private BrushSound.RoundButton btnSavePreset;
        private BrushSound.RoundButton btnDeletePreset;
        private System.Windows.Forms.Label lblTheme;
        private System.Windows.Forms.ComboBox cmbTheme;
        private BrushSound.RoundButton btnAppearance;
        private System.Windows.Forms.Label lblHint;
        private BrushSound.RoundButton btnHelp;
        private BrushSound.RoundButton btnQuit;
    }
}