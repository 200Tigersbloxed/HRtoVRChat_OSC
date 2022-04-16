namespace HRtoVRChat_OSC_UI
{
    sealed partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.materialTabSelector1 = new MaterialSkin.Controls.MaterialTabSelector();
            this.materialTabControl1 = new MaterialSkin.Controls.MaterialTabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.killAllProcesses = new MaterialSkin.Controls.MaterialButton();
            this.stopButton = new MaterialSkin.Controls.MaterialButton();
            this.startButton = new MaterialSkin.Controls.MaterialButton();
            this.materialCheckbox2 = new MaterialSkin.Controls.MaterialCheckbox();
            this.materialCheckbox1 = new MaterialSkin.Controls.MaterialCheckbox();
            this.statusLabel = new MaterialSkin.Controls.MaterialLabel();
            this.sendCommand = new MaterialSkin.Controls.MaterialButton();
            this.commandInput = new MaterialSkin.Controls.MaterialTextBox();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.overallProgress = new MaterialSkin.Controls.MaterialProgressBar();
            this.processProgress = new MaterialSkin.Controls.MaterialProgressBar();
            this.materialLabel5 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel4 = new MaterialSkin.Controls.MaterialLabel();
            this.updateSoftwareButton = new MaterialSkin.Controls.MaterialButton();
            this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
            this.refreshVersions = new MaterialSkin.Controls.MaterialButton();
            this.availableVersionLabel = new MaterialSkin.Controls.MaterialLabel();
            this.currentVersionLabel = new MaterialSkin.Controls.MaterialLabel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.configType = new MaterialSkin.Controls.MaterialLabel();
            this.configValueInput = new MaterialSkin.Controls.MaterialTextBox();
            this.configDescriptionLabel = new MaterialSkin.Controls.MaterialLabel();
            this.updateConfigValueButton = new MaterialSkin.Controls.MaterialButton();
            this.materialLabel7 = new MaterialSkin.Controls.MaterialLabel();
            this.configNameLabel = new MaterialSkin.Controls.MaterialLabel();
            this.MinHRRadioButton = new MaterialSkin.Controls.MaterialRadioButton();
            this.MaxHRRadioButton = new MaterialSkin.Controls.MaterialRadioButton();
            this.textfilelocationRadioButton = new MaterialSkin.Controls.MaterialRadioButton();
            this.pulsoidkeyRadioButton = new MaterialSkin.Controls.MaterialRadioButton();
            this.pulsoidwidgetRadioButton = new MaterialSkin.Controls.MaterialRadioButton();
            this.hyperateSessionIdRadioButton = new MaterialSkin.Controls.MaterialRadioButton();
            this.fitbitURLRadioButton = new MaterialSkin.Controls.MaterialRadioButton();
            this.hrTypeRadioButton = new MaterialSkin.Controls.MaterialRadioButton();
            this.portRadioButton = new MaterialSkin.Controls.MaterialRadioButton();
            this.materialLabel6 = new MaterialSkin.Controls.MaterialLabel();
            this.ipRadioButton = new MaterialSkin.Controls.MaterialRadioButton();
            this.materialDivider1 = new MaterialSkin.Controls.MaterialDivider();
            this.materialProgressBar1 = new MaterialSkin.Controls.MaterialProgressBar();
            this.materialTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // materialTabSelector1
            // 
            this.materialTabSelector1.BaseTabControl = this.materialTabControl1;
            this.materialTabSelector1.CharacterCasing = MaterialSkin.Controls.MaterialTabSelector.CustomCharacterCasing.Normal;
            this.materialTabSelector1.Depth = 0;
            this.materialTabSelector1.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialTabSelector1.Location = new System.Drawing.Point(-2, 60);
            this.materialTabSelector1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialTabSelector1.Name = "materialTabSelector1";
            this.materialTabSelector1.Size = new System.Drawing.Size(590, 36);
            this.materialTabSelector1.TabIndex = 0;
            this.materialTabSelector1.Text = "materialTabSelector1";
            // 
            // materialTabControl1
            // 
            this.materialTabControl1.Controls.Add(this.tabPage1);
            this.materialTabControl1.Controls.Add(this.tabPage2);
            this.materialTabControl1.Controls.Add(this.tabPage3);
            this.materialTabControl1.Depth = 0;
            this.materialTabControl1.Location = new System.Drawing.Point(6, 102);
            this.materialTabControl1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialTabControl1.Multiline = true;
            this.materialTabControl1.Name = "materialTabControl1";
            this.materialTabControl1.SelectedIndex = 0;
            this.materialTabControl1.Size = new System.Drawing.Size(582, 372);
            this.materialTabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.killAllProcesses);
            this.tabPage1.Controls.Add(this.stopButton);
            this.tabPage1.Controls.Add(this.startButton);
            this.tabPage1.Controls.Add(this.materialCheckbox2);
            this.tabPage1.Controls.Add(this.materialCheckbox1);
            this.tabPage1.Controls.Add(this.statusLabel);
            this.tabPage1.Controls.Add(this.sendCommand);
            this.tabPage1.Controls.Add(this.commandInput);
            this.tabPage1.Controls.Add(this.materialLabel2);
            this.tabPage1.Controls.Add(this.materialLabel1);
            this.tabPage1.Controls.Add(this.richTextBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(574, 346);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Program";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // killAllProcesses
            // 
            this.killAllProcesses.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.killAllProcesses.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.killAllProcesses.Depth = 0;
            this.killAllProcesses.HighEmphasis = true;
            this.killAllProcesses.Icon = null;
            this.killAllProcesses.Location = new System.Drawing.Point(503, 301);
            this.killAllProcesses.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.killAllProcesses.MouseState = MaterialSkin.MouseState.HOVER;
            this.killAllProcesses.Name = "killAllProcesses";
            this.killAllProcesses.NoAccentTextColor = System.Drawing.Color.Empty;
            this.killAllProcesses.Size = new System.Drawing.Size(64, 36);
            this.killAllProcesses.TabIndex = 10;
            this.killAllProcesses.TabStop = false;
            this.killAllProcesses.Text = "KILL";
            this.killAllProcesses.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.killAllProcesses.UseAccentColor = false;
            this.killAllProcesses.UseVisualStyleBackColor = true;
            // 
            // stopButton
            // 
            this.stopButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stopButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.stopButton.Depth = 0;
            this.stopButton.HighEmphasis = true;
            this.stopButton.Icon = null;
            this.stopButton.Location = new System.Drawing.Point(432, 301);
            this.stopButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.stopButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.stopButton.Name = "stopButton";
            this.stopButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.stopButton.Size = new System.Drawing.Size(64, 36);
            this.stopButton.TabIndex = 9;
            this.stopButton.TabStop = false;
            this.stopButton.Text = "STOP";
            this.stopButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.stopButton.UseAccentColor = false;
            this.stopButton.UseVisualStyleBackColor = true;
            // 
            // startButton
            // 
            this.startButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.startButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.startButton.Depth = 0;
            this.startButton.HighEmphasis = true;
            this.startButton.Icon = null;
            this.startButton.Location = new System.Drawing.Point(357, 301);
            this.startButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.startButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.startButton.Name = "startButton";
            this.startButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.startButton.Size = new System.Drawing.Size(67, 36);
            this.startButton.TabIndex = 8;
            this.startButton.TabStop = false;
            this.startButton.Text = "START";
            this.startButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.startButton.UseAccentColor = false;
            this.startButton.UseVisualStyleBackColor = true;
            // 
            // materialCheckbox2
            // 
            this.materialCheckbox2.Depth = 0;
            this.materialCheckbox2.Location = new System.Drawing.Point(116, 284);
            this.materialCheckbox2.Margin = new System.Windows.Forms.Padding(0);
            this.materialCheckbox2.MouseLocation = new System.Drawing.Point(-1, -1);
            this.materialCheckbox2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCheckbox2.Name = "materialCheckbox2";
            this.materialCheckbox2.ReadOnly = false;
            this.materialCheckbox2.Ripple = true;
            this.materialCheckbox2.Size = new System.Drawing.Size(186, 33);
            this.materialCheckbox2.TabIndex = 7;
            this.materialCheckbox2.TabStop = false;
            this.materialCheckbox2.Text = "Skip VRChat Check";
            this.materialCheckbox2.UseVisualStyleBackColor = true;
            // 
            // materialCheckbox1
            // 
            this.materialCheckbox1.Depth = 0;
            this.materialCheckbox1.Location = new System.Drawing.Point(6, 284);
            this.materialCheckbox1.Margin = new System.Windows.Forms.Padding(0);
            this.materialCheckbox1.MouseLocation = new System.Drawing.Point(-1, -1);
            this.materialCheckbox1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialCheckbox1.Name = "materialCheckbox1";
            this.materialCheckbox1.ReadOnly = false;
            this.materialCheckbox1.Ripple = true;
            this.materialCheckbox1.Size = new System.Drawing.Size(110, 33);
            this.materialCheckbox1.TabIndex = 6;
            this.materialCheckbox1.TabStop = false;
            this.materialCheckbox1.Text = "Auto Start";
            this.materialCheckbox1.UseVisualStyleBackColor = true;
            // 
            // statusLabel
            // 
            this.statusLabel.Depth = 0;
            this.statusLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.statusLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.statusLabel.Location = new System.Drawing.Point(71, 317);
            this.statusLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(70, 26);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.Text = "RUNNING";
            // 
            // sendCommand
            // 
            this.sendCommand.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.sendCommand.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.sendCommand.Depth = 0;
            this.sendCommand.HighEmphasis = true;
            this.sendCommand.Icon = null;
            this.sendCommand.Location = new System.Drawing.Point(503, 239);
            this.sendCommand.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.sendCommand.MouseState = MaterialSkin.MouseState.HOVER;
            this.sendCommand.Name = "sendCommand";
            this.sendCommand.NoAccentTextColor = System.Drawing.Color.Empty;
            this.sendCommand.Size = new System.Drawing.Size(64, 36);
            this.sendCommand.TabIndex = 4;
            this.sendCommand.TabStop = false;
            this.sendCommand.Text = "Send";
            this.sendCommand.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.sendCommand.UseAccentColor = false;
            this.sendCommand.UseVisualStyleBackColor = true;
            // 
            // commandInput
            // 
            this.commandInput.AnimateReadOnly = false;
            this.commandInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.commandInput.Depth = 0;
            this.commandInput.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.commandInput.LeadingIcon = null;
            this.commandInput.Location = new System.Drawing.Point(6, 231);
            this.commandInput.MaxLength = 50;
            this.commandInput.MouseState = MaterialSkin.MouseState.OUT;
            this.commandInput.Multiline = false;
            this.commandInput.Name = "commandInput";
            this.commandInput.Size = new System.Drawing.Size(490, 50);
            this.commandInput.TabIndex = 3;
            this.commandInput.TabStop = false;
            this.commandInput.Text = "";
            this.commandInput.TrailingIcon = null;
            // 
            // materialLabel2
            // 
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.materialLabel2.Location = new System.Drawing.Point(6, 317);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(59, 26);
            this.materialLabel2.TabIndex = 2;
            this.materialLabel2.Text = "Status: ";
            // 
            // materialLabel1
            // 
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel1.Location = new System.Drawing.Point(6, 3);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(562, 19);
            this.materialLabel1.TabIndex = 1;
            this.materialLabel1.Text = "HRtoVRChat_OSC Output";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.richTextBox1.Location = new System.Drawing.Point(6, 26);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(562, 199);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.TabStop = false;
            this.richTextBox1.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.overallProgress);
            this.tabPage2.Controls.Add(this.processProgress);
            this.tabPage2.Controls.Add(this.materialLabel5);
            this.tabPage2.Controls.Add(this.materialLabel4);
            this.tabPage2.Controls.Add(this.updateSoftwareButton);
            this.tabPage2.Controls.Add(this.materialLabel3);
            this.tabPage2.Controls.Add(this.refreshVersions);
            this.tabPage2.Controls.Add(this.availableVersionLabel);
            this.tabPage2.Controls.Add(this.currentVersionLabel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(574, 346);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Updates";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // overallProgress
            // 
            this.overallProgress.Depth = 0;
            this.overallProgress.Location = new System.Drawing.Point(6, 202);
            this.overallProgress.MouseState = MaterialSkin.MouseState.HOVER;
            this.overallProgress.Name = "overallProgress";
            this.overallProgress.Size = new System.Drawing.Size(561, 5);
            this.overallProgress.TabIndex = 9;
            // 
            // processProgress
            // 
            this.processProgress.Depth = 0;
            this.processProgress.Location = new System.Drawing.Point(6, 169);
            this.processProgress.MouseState = MaterialSkin.MouseState.HOVER;
            this.processProgress.Name = "processProgress";
            this.processProgress.Size = new System.Drawing.Size(561, 5);
            this.processProgress.TabIndex = 8;
            // 
            // materialLabel5
            // 
            this.materialLabel5.Depth = 0;
            this.materialLabel5.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel5.Location = new System.Drawing.Point(6, 177);
            this.materialLabel5.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel5.Name = "materialLabel5";
            this.materialLabel5.Size = new System.Drawing.Size(153, 22);
            this.materialLabel5.TabIndex = 7;
            this.materialLabel5.Text = "Overall Completion:";
            // 
            // materialLabel4
            // 
            this.materialLabel4.Depth = 0;
            this.materialLabel4.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel4.Location = new System.Drawing.Point(6, 144);
            this.materialLabel4.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel4.Name = "materialLabel4";
            this.materialLabel4.Size = new System.Drawing.Size(153, 22);
            this.materialLabel4.TabIndex = 6;
            this.materialLabel4.Text = "Current Process:";
            // 
            // updateSoftwareButton
            // 
            this.updateSoftwareButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.updateSoftwareButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.updateSoftwareButton.Depth = 0;
            this.updateSoftwareButton.HighEmphasis = true;
            this.updateSoftwareButton.Icon = null;
            this.updateSoftwareButton.Location = new System.Drawing.Point(204, 107);
            this.updateSoftwareButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.updateSoftwareButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.updateSoftwareButton.Name = "updateSoftwareButton";
            this.updateSoftwareButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.updateSoftwareButton.Size = new System.Drawing.Size(157, 36);
            this.updateSoftwareButton.TabIndex = 4;
            this.updateSoftwareButton.Text = "Update Software";
            this.updateSoftwareButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.updateSoftwareButton.UseAccentColor = false;
            this.updateSoftwareButton.UseVisualStyleBackColor = true;
            // 
            // materialLabel3
            // 
            this.materialLabel3.Depth = 0;
            this.materialLabel3.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel3.Location = new System.Drawing.Point(6, 68);
            this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel3.Name = "materialLabel3";
            this.materialLabel3.Size = new System.Drawing.Size(561, 33);
            this.materialLabel3.TabIndex = 3;
            this.materialLabel3.Text = "Updating";
            // 
            // refreshVersions
            // 
            this.refreshVersions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.refreshVersions.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.refreshVersions.Depth = 0;
            this.refreshVersions.HighEmphasis = true;
            this.refreshVersions.Icon = null;
            this.refreshVersions.Location = new System.Drawing.Point(483, 12);
            this.refreshVersions.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.refreshVersions.MouseState = MaterialSkin.MouseState.HOVER;
            this.refreshVersions.Name = "refreshVersions";
            this.refreshVersions.NoAccentTextColor = System.Drawing.Color.Empty;
            this.refreshVersions.Size = new System.Drawing.Size(84, 36);
            this.refreshVersions.TabIndex = 2;
            this.refreshVersions.Text = "Refresh";
            this.refreshVersions.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.refreshVersions.UseAccentColor = false;
            this.refreshVersions.UseVisualStyleBackColor = true;
            // 
            // availableVersionLabel
            // 
            this.availableVersionLabel.Depth = 0;
            this.availableVersionLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.availableVersionLabel.Location = new System.Drawing.Point(6, 31);
            this.availableVersionLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.availableVersionLabel.Name = "availableVersionLabel";
            this.availableVersionLabel.Size = new System.Drawing.Size(470, 28);
            this.availableVersionLabel.TabIndex = 1;
            this.availableVersionLabel.Text = "materialLabel1";
            // 
            // currentVersionLabel
            // 
            this.currentVersionLabel.Depth = 0;
            this.currentVersionLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.currentVersionLabel.Location = new System.Drawing.Point(6, 3);
            this.currentVersionLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.currentVersionLabel.Name = "currentVersionLabel";
            this.currentVersionLabel.Size = new System.Drawing.Size(470, 28);
            this.currentVersionLabel.TabIndex = 0;
            this.currentVersionLabel.Text = "materialLabel1";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.configType);
            this.tabPage3.Controls.Add(this.configValueInput);
            this.tabPage3.Controls.Add(this.configDescriptionLabel);
            this.tabPage3.Controls.Add(this.updateConfigValueButton);
            this.tabPage3.Controls.Add(this.materialLabel7);
            this.tabPage3.Controls.Add(this.configNameLabel);
            this.tabPage3.Controls.Add(this.MinHRRadioButton);
            this.tabPage3.Controls.Add(this.MaxHRRadioButton);
            this.tabPage3.Controls.Add(this.textfilelocationRadioButton);
            this.tabPage3.Controls.Add(this.pulsoidkeyRadioButton);
            this.tabPage3.Controls.Add(this.pulsoidwidgetRadioButton);
            this.tabPage3.Controls.Add(this.hyperateSessionIdRadioButton);
            this.tabPage3.Controls.Add(this.fitbitURLRadioButton);
            this.tabPage3.Controls.Add(this.hrTypeRadioButton);
            this.tabPage3.Controls.Add(this.portRadioButton);
            this.tabPage3.Controls.Add(this.materialLabel6);
            this.tabPage3.Controls.Add(this.ipRadioButton);
            this.tabPage3.Controls.Add(this.materialDivider1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(574, 346);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Config";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // configType
            // 
            this.configType.Depth = 0;
            this.configType.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.configType.Location = new System.Drawing.Point(372, 33);
            this.configType.MouseState = MaterialSkin.MouseState.HOVER;
            this.configType.Name = "configType";
            this.configType.Size = new System.Drawing.Size(199, 27);
            this.configType.TabIndex = 19;
            this.configType.Text = "type";
            // 
            // configValueInput
            // 
            this.configValueInput.AnimateReadOnly = false;
            this.configValueInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.configValueInput.Depth = 0;
            this.configValueInput.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.configValueInput.LeadingIcon = null;
            this.configValueInput.Location = new System.Drawing.Point(372, 243);
            this.configValueInput.MaxLength = 50;
            this.configValueInput.MouseState = MaterialSkin.MouseState.OUT;
            this.configValueInput.Multiline = false;
            this.configValueInput.Name = "configValueInput";
            this.configValueInput.Size = new System.Drawing.Size(199, 50);
            this.configValueInput.TabIndex = 18;
            this.configValueInput.Text = "";
            this.configValueInput.TrailingIcon = null;
            // 
            // configDescriptionLabel
            // 
            this.configDescriptionLabel.Depth = 0;
            this.configDescriptionLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.configDescriptionLabel.Location = new System.Drawing.Point(372, 72);
            this.configDescriptionLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.configDescriptionLabel.Name = "configDescriptionLabel";
            this.configDescriptionLabel.Size = new System.Drawing.Size(199, 130);
            this.configDescriptionLabel.TabIndex = 17;
            this.configDescriptionLabel.Text = "Description: ";
            // 
            // updateConfigValueButton
            // 
            this.updateConfigValueButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.updateConfigValueButton.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.updateConfigValueButton.Depth = 0;
            this.updateConfigValueButton.HighEmphasis = true;
            this.updateConfigValueButton.Icon = null;
            this.updateConfigValueButton.Location = new System.Drawing.Point(380, 302);
            this.updateConfigValueButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.updateConfigValueButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.updateConfigValueButton.Name = "updateConfigValueButton";
            this.updateConfigValueButton.NoAccentTextColor = System.Drawing.Color.Empty;
            this.updateConfigValueButton.Size = new System.Drawing.Size(182, 36);
            this.updateConfigValueButton.TabIndex = 16;
            this.updateConfigValueButton.Text = "Update Config Value";
            this.updateConfigValueButton.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.updateConfigValueButton.UseAccentColor = false;
            this.updateConfigValueButton.UseVisualStyleBackColor = true;
            // 
            // materialLabel7
            // 
            this.materialLabel7.Depth = 0;
            this.materialLabel7.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel7.Location = new System.Drawing.Point(372, 213);
            this.materialLabel7.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel7.Name = "materialLabel7";
            this.materialLabel7.Size = new System.Drawing.Size(199, 27);
            this.materialLabel7.TabIndex = 15;
            this.materialLabel7.Text = "Config Value:";
            // 
            // configNameLabel
            // 
            this.configNameLabel.Depth = 0;
            this.configNameLabel.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.configNameLabel.Location = new System.Drawing.Point(372, 6);
            this.configNameLabel.MouseState = MaterialSkin.MouseState.HOVER;
            this.configNameLabel.Name = "configNameLabel";
            this.configNameLabel.Size = new System.Drawing.Size(199, 27);
            this.configNameLabel.TabIndex = 12;
            this.configNameLabel.Text = "value";
            // 
            // MinHRRadioButton
            // 
            this.MinHRRadioButton.Depth = 0;
            this.MinHRRadioButton.Location = new System.Drawing.Point(179, 155);
            this.MinHRRadioButton.Margin = new System.Windows.Forms.Padding(0);
            this.MinHRRadioButton.MouseLocation = new System.Drawing.Point(-1, -1);
            this.MinHRRadioButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.MinHRRadioButton.Name = "MinHRRadioButton";
            this.MinHRRadioButton.Ripple = true;
            this.MinHRRadioButton.Size = new System.Drawing.Size(171, 28);
            this.MinHRRadioButton.TabIndex = 11;
            this.MinHRRadioButton.TabStop = true;
            this.MinHRRadioButton.Text = "MinHR";
            this.MinHRRadioButton.UseVisualStyleBackColor = true;
            // 
            // MaxHRRadioButton
            // 
            this.MaxHRRadioButton.Depth = 0;
            this.MaxHRRadioButton.Location = new System.Drawing.Point(3, 155);
            this.MaxHRRadioButton.Margin = new System.Windows.Forms.Padding(0);
            this.MaxHRRadioButton.MouseLocation = new System.Drawing.Point(-1, -1);
            this.MaxHRRadioButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.MaxHRRadioButton.Name = "MaxHRRadioButton";
            this.MaxHRRadioButton.Ripple = true;
            this.MaxHRRadioButton.Size = new System.Drawing.Size(171, 28);
            this.MaxHRRadioButton.TabIndex = 10;
            this.MaxHRRadioButton.TabStop = true;
            this.MaxHRRadioButton.Text = "MaxHR";
            this.MaxHRRadioButton.UseVisualStyleBackColor = true;
            // 
            // textfilelocationRadioButton
            // 
            this.textfilelocationRadioButton.Depth = 0;
            this.textfilelocationRadioButton.Location = new System.Drawing.Point(179, 127);
            this.textfilelocationRadioButton.Margin = new System.Windows.Forms.Padding(0);
            this.textfilelocationRadioButton.MouseLocation = new System.Drawing.Point(-1, -1);
            this.textfilelocationRadioButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.textfilelocationRadioButton.Name = "textfilelocationRadioButton";
            this.textfilelocationRadioButton.Ripple = true;
            this.textfilelocationRadioButton.Size = new System.Drawing.Size(171, 28);
            this.textfilelocationRadioButton.TabIndex = 9;
            this.textfilelocationRadioButton.TabStop = true;
            this.textfilelocationRadioButton.Text = "textfilelocation";
            this.textfilelocationRadioButton.UseVisualStyleBackColor = true;
            // 
            // pulsoidkeyRadioButton
            // 
            this.pulsoidkeyRadioButton.Depth = 0;
            this.pulsoidkeyRadioButton.Location = new System.Drawing.Point(3, 127);
            this.pulsoidkeyRadioButton.Margin = new System.Windows.Forms.Padding(0);
            this.pulsoidkeyRadioButton.MouseLocation = new System.Drawing.Point(-1, -1);
            this.pulsoidkeyRadioButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.pulsoidkeyRadioButton.Name = "pulsoidkeyRadioButton";
            this.pulsoidkeyRadioButton.Ripple = true;
            this.pulsoidkeyRadioButton.Size = new System.Drawing.Size(171, 28);
            this.pulsoidkeyRadioButton.TabIndex = 8;
            this.pulsoidkeyRadioButton.TabStop = true;
            this.pulsoidkeyRadioButton.Text = "pulsoidkey";
            this.pulsoidkeyRadioButton.UseVisualStyleBackColor = true;
            // 
            // pulsoidwidgetRadioButton
            // 
            this.pulsoidwidgetRadioButton.Depth = 0;
            this.pulsoidwidgetRadioButton.Location = new System.Drawing.Point(179, 99);
            this.pulsoidwidgetRadioButton.Margin = new System.Windows.Forms.Padding(0);
            this.pulsoidwidgetRadioButton.MouseLocation = new System.Drawing.Point(-1, -1);
            this.pulsoidwidgetRadioButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.pulsoidwidgetRadioButton.Name = "pulsoidwidgetRadioButton";
            this.pulsoidwidgetRadioButton.Ripple = true;
            this.pulsoidwidgetRadioButton.Size = new System.Drawing.Size(171, 28);
            this.pulsoidwidgetRadioButton.TabIndex = 7;
            this.pulsoidwidgetRadioButton.TabStop = true;
            this.pulsoidwidgetRadioButton.Text = "pulsoidwidget";
            this.pulsoidwidgetRadioButton.UseVisualStyleBackColor = true;
            // 
            // hyperateSessionIdRadioButton
            // 
            this.hyperateSessionIdRadioButton.Depth = 0;
            this.hyperateSessionIdRadioButton.Location = new System.Drawing.Point(3, 99);
            this.hyperateSessionIdRadioButton.Margin = new System.Windows.Forms.Padding(0);
            this.hyperateSessionIdRadioButton.MouseLocation = new System.Drawing.Point(-1, -1);
            this.hyperateSessionIdRadioButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.hyperateSessionIdRadioButton.Name = "hyperateSessionIdRadioButton";
            this.hyperateSessionIdRadioButton.Ripple = true;
            this.hyperateSessionIdRadioButton.Size = new System.Drawing.Size(171, 28);
            this.hyperateSessionIdRadioButton.TabIndex = 6;
            this.hyperateSessionIdRadioButton.TabStop = true;
            this.hyperateSessionIdRadioButton.Text = "hyperateSessionId";
            this.hyperateSessionIdRadioButton.UseVisualStyleBackColor = true;
            // 
            // fitbitURLRadioButton
            // 
            this.fitbitURLRadioButton.Depth = 0;
            this.fitbitURLRadioButton.Location = new System.Drawing.Point(179, 71);
            this.fitbitURLRadioButton.Margin = new System.Windows.Forms.Padding(0);
            this.fitbitURLRadioButton.MouseLocation = new System.Drawing.Point(-1, -1);
            this.fitbitURLRadioButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.fitbitURLRadioButton.Name = "fitbitURLRadioButton";
            this.fitbitURLRadioButton.Ripple = true;
            this.fitbitURLRadioButton.Size = new System.Drawing.Size(171, 28);
            this.fitbitURLRadioButton.TabIndex = 5;
            this.fitbitURLRadioButton.TabStop = true;
            this.fitbitURLRadioButton.Text = "fitbitURL";
            this.fitbitURLRadioButton.UseVisualStyleBackColor = true;
            // 
            // hrTypeRadioButton
            // 
            this.hrTypeRadioButton.Depth = 0;
            this.hrTypeRadioButton.Location = new System.Drawing.Point(3, 71);
            this.hrTypeRadioButton.Margin = new System.Windows.Forms.Padding(0);
            this.hrTypeRadioButton.MouseLocation = new System.Drawing.Point(-1, -1);
            this.hrTypeRadioButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.hrTypeRadioButton.Name = "hrTypeRadioButton";
            this.hrTypeRadioButton.Ripple = true;
            this.hrTypeRadioButton.Size = new System.Drawing.Size(171, 28);
            this.hrTypeRadioButton.TabIndex = 4;
            this.hrTypeRadioButton.TabStop = true;
            this.hrTypeRadioButton.Text = "hrType";
            this.hrTypeRadioButton.UseVisualStyleBackColor = true;
            // 
            // portRadioButton
            // 
            this.portRadioButton.Depth = 0;
            this.portRadioButton.Location = new System.Drawing.Point(179, 43);
            this.portRadioButton.Margin = new System.Windows.Forms.Padding(0);
            this.portRadioButton.MouseLocation = new System.Drawing.Point(-1, -1);
            this.portRadioButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.portRadioButton.Name = "portRadioButton";
            this.portRadioButton.Ripple = true;
            this.portRadioButton.Size = new System.Drawing.Size(171, 28);
            this.portRadioButton.TabIndex = 3;
            this.portRadioButton.TabStop = true;
            this.portRadioButton.Text = "port";
            this.portRadioButton.UseVisualStyleBackColor = true;
            // 
            // materialLabel6
            // 
            this.materialLabel6.Depth = 0;
            this.materialLabel6.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialLabel6.Location = new System.Drawing.Point(3, 6);
            this.materialLabel6.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel6.Name = "materialLabel6";
            this.materialLabel6.Size = new System.Drawing.Size(347, 27);
            this.materialLabel6.TabIndex = 2;
            this.materialLabel6.Text = "Config Name";
            // 
            // ipRadioButton
            // 
            this.ipRadioButton.Depth = 0;
            this.ipRadioButton.Location = new System.Drawing.Point(3, 43);
            this.ipRadioButton.Margin = new System.Windows.Forms.Padding(0);
            this.ipRadioButton.MouseLocation = new System.Drawing.Point(-1, -1);
            this.ipRadioButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.ipRadioButton.Name = "ipRadioButton";
            this.ipRadioButton.Ripple = true;
            this.ipRadioButton.Size = new System.Drawing.Size(171, 28);
            this.ipRadioButton.TabIndex = 1;
            this.ipRadioButton.TabStop = true;
            this.ipRadioButton.Text = "ip";
            this.ipRadioButton.UseVisualStyleBackColor = true;
            // 
            // materialDivider1
            // 
            this.materialDivider1.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (30)))), ((int) (((byte) (0)))), ((int) (((byte) (0)))), ((int) (((byte) (0)))));
            this.materialDivider1.Depth = 0;
            this.materialDivider1.Location = new System.Drawing.Point(356, 3);
            this.materialDivider1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialDivider1.Name = "materialDivider1";
            this.materialDivider1.Size = new System.Drawing.Size(10, 340);
            this.materialDivider1.TabIndex = 0;
            this.materialDivider1.Text = "materialDivider1";
            // 
            // materialProgressBar1
            // 
            this.materialProgressBar1.Depth = 0;
            this.materialProgressBar1.Location = new System.Drawing.Point(0, 0);
            this.materialProgressBar1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialProgressBar1.Name = "materialProgressBar1";
            this.materialProgressBar1.Size = new System.Drawing.Size(100, 23);
            this.materialProgressBar1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(588, 480);
            this.Controls.Add(this.materialTabControl1);
            this.Controls.Add(this.materialTabSelector1);
            this.Location = new System.Drawing.Point(15, 15);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(588, 480);
            this.MinimumSize = new System.Drawing.Size(588, 480);
            this.Name = "MainForm";
            this.Text = "HRtoVRChat_OSC";
            this.materialTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
        }

        private MaterialSkin.Controls.MaterialButton killAllProcesses;

        private MaterialSkin.Controls.MaterialLabel configType;

        private MaterialSkin.Controls.MaterialTextBox configValueInput;

        private MaterialSkin.Controls.MaterialLabel configDescriptionLabel;

        private MaterialSkin.Controls.MaterialLabel materialLabel7;
        private MaterialSkin.Controls.MaterialButton updateConfigValueButton;

        private MaterialSkin.Controls.MaterialLabel configNameLabel;

        private MaterialSkin.Controls.MaterialRadioButton MinHRRadioButton;

        private MaterialSkin.Controls.MaterialRadioButton MaxHRRadioButton;

        private MaterialSkin.Controls.MaterialRadioButton textfilelocationRadioButton;

        private MaterialSkin.Controls.MaterialRadioButton pulsoidkeyRadioButton;

        private MaterialSkin.Controls.MaterialRadioButton pulsoidwidgetRadioButton;

        private MaterialSkin.Controls.MaterialRadioButton hyperateSessionIdRadioButton;

        private MaterialSkin.Controls.MaterialRadioButton fitbitURLRadioButton;

        private MaterialSkin.Controls.MaterialRadioButton hrTypeRadioButton;

        private MaterialSkin.Controls.MaterialRadioButton portRadioButton;

        private MaterialSkin.Controls.MaterialRadioButton ipRadioButton;

        private MaterialSkin.Controls.MaterialRadioButton materialRadioButton1;
        private MaterialSkin.Controls.MaterialLabel materialLabel6;

        private MaterialSkin.Controls.MaterialDivider materialDivider1;

        private MaterialSkin.Controls.MaterialProgressBar overallProgress;

        private MaterialSkin.Controls.MaterialProgressBar processProgress;

        private MaterialSkin.Controls.MaterialLabel materialLabel5;

        private MaterialSkin.Controls.MaterialProgressBar materialProgressBar1;
        private MaterialSkin.Controls.MaterialLabel materialLabel4;

        private MaterialSkin.Controls.MaterialLabel materialLabel3;

        private MaterialSkin.Controls.MaterialButton startButton;
        private MaterialSkin.Controls.MaterialButton stopButton;

        private MaterialSkin.Controls.MaterialButton updateSoftwareButton;

        private MaterialSkin.Controls.MaterialCheckbox materialCheckbox1;
        private MaterialSkin.Controls.MaterialCheckbox materialCheckbox2;

        private MaterialSkin.Controls.MaterialLabel statusLabel;

        private MaterialSkin.Controls.MaterialTextBox commandInput;
        private MaterialSkin.Controls.MaterialButton sendCommand;

        private MaterialSkin.Controls.MaterialLabel materialLabel2;

        private MaterialSkin.Controls.MaterialButton refreshVersions;

        private MaterialSkin.Controls.MaterialLabel currentVersionLabel;
        private MaterialSkin.Controls.MaterialLabel availableVersionLabel;

        private MaterialSkin.Controls.MaterialLabel materialLabel1;

        #endregion

        private MaterialSkin.Controls.MaterialTabSelector materialTabSelector1;
        private MaterialSkin.Controls.MaterialTabControl materialTabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}