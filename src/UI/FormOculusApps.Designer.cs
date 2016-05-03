namespace OculusGameManager.UI
{
    partial class FormOculusApps
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOculusApps));
			this.pnlApps = new System.Windows.Forms.FlowLayoutPanel();
			this.pnlTop = new System.Windows.Forms.Panel();
			this.chkShowAll = new System.Windows.Forms.CheckBox();
			this.btnSetBackupLocation = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtBackupLocation = new System.Windows.Forms.TextBox();
			this.btnRestore = new System.Windows.Forms.Button();
			this.txtHomePath = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.pnlProgress = new System.Windows.Forms.Panel();
			this.lblProgressAction = new System.Windows.Forms.Label();
			this.prgProgress = new System.Windows.Forms.ProgressBar();
			this.dlgBrowseFolder = new System.Windows.Forms.FolderBrowserDialog();
			this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
			this.pnlTop.SuspendLayout();
			this.pnlProgress.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlApps
			// 
			this.pnlApps.AutoScroll = true;
			this.pnlApps.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlApps.Location = new System.Drawing.Point(0, 57);
			this.pnlApps.Margin = new System.Windows.Forms.Padding(0);
			this.pnlApps.Name = "pnlApps";
			this.pnlApps.Size = new System.Drawing.Size(700, 618);
			this.pnlApps.TabIndex = 1;
			// 
			// pnlTop
			// 
			this.pnlTop.Controls.Add(this.chkShowAll);
			this.pnlTop.Controls.Add(this.btnSetBackupLocation);
			this.pnlTop.Controls.Add(this.label1);
			this.pnlTop.Controls.Add(this.txtBackupLocation);
			this.pnlTop.Controls.Add(this.btnRestore);
			this.pnlTop.Controls.Add(this.txtHomePath);
			this.pnlTop.Controls.Add(this.label2);
			this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTop.Location = new System.Drawing.Point(0, 0);
			this.pnlTop.Name = "pnlTop";
			this.pnlTop.Size = new System.Drawing.Size(700, 57);
			this.pnlTop.TabIndex = 2;
			// 
			// chkShowAll
			// 
			this.chkShowAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.chkShowAll.AutoSize = true;
			this.chkShowAll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkShowAll.Location = new System.Drawing.Point(576, 8);
			this.chkShowAll.Name = "chkShowAll";
			this.chkShowAll.Size = new System.Drawing.Size(112, 17);
			this.chkShowAll.TabIndex = 13;
			this.chkShowAll.Text = "Show not installed";
			this.chkShowAll.UseVisualStyleBackColor = true;
			this.chkShowAll.CheckedChanged += new System.EventHandler(this.chkShowAll_CheckedChanged);
			// 
			// btnSetBackupLocation
			// 
			this.btnSetBackupLocation.Location = new System.Drawing.Point(116, 28);
			this.btnSetBackupLocation.Name = "btnSetBackupLocation";
			this.btnSetBackupLocation.Size = new System.Drawing.Size(25, 23);
			this.btnSetBackupLocation.TabIndex = 12;
			this.btnSetBackupLocation.Text = "...";
			this.btnSetBackupLocation.UseVisualStyleBackColor = true;
			this.btnSetBackupLocation.Click += new System.EventHandler(this.btnSetBackupLocation_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 33);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 13);
			this.label1.TabIndex = 11;
			this.label1.Text = "Backup Location:";
			// 
			// txtBackupLocation
			// 
			this.txtBackupLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtBackupLocation.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtBackupLocation.Location = new System.Drawing.Point(147, 33);
			this.txtBackupLocation.Name = "txtBackupLocation";
			this.txtBackupLocation.ReadOnly = true;
			this.txtBackupLocation.Size = new System.Drawing.Size(441, 13);
			this.txtBackupLocation.TabIndex = 10;
			this.txtBackupLocation.Text = "INSTALLATION PATH";
			// 
			// btnRestore
			// 
			this.btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRestore.Location = new System.Drawing.Point(594, 28);
			this.btnRestore.Name = "btnRestore";
			this.btnRestore.Size = new System.Drawing.Size(94, 23);
			this.btnRestore.TabIndex = 9;
			this.btnRestore.Text = "&Restore Backup";
			this.btnRestore.UseVisualStyleBackColor = true;
			this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
			// 
			// txtHomePath
			// 
			this.txtHomePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtHomePath.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtHomePath.Location = new System.Drawing.Point(116, 9);
			this.txtHomePath.Name = "txtHomePath";
			this.txtHomePath.ReadOnly = true;
			this.txtHomePath.Size = new System.Drawing.Size(472, 13);
			this.txtHomePath.TabIndex = 8;
			this.txtHomePath.Text = "INSTALLATION PATH";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(98, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Oculus Install Path:";
			// 
			// pnlProgress
			// 
			this.pnlProgress.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.pnlProgress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlProgress.Controls.Add(this.lblProgressAction);
			this.pnlProgress.Controls.Add(this.prgProgress);
			this.pnlProgress.Location = new System.Drawing.Point(15, 287);
			this.pnlProgress.Name = "pnlProgress";
			this.pnlProgress.Size = new System.Drawing.Size(654, 55);
			this.pnlProgress.TabIndex = 11;
			this.pnlProgress.Visible = false;
			// 
			// lblProgressAction
			// 
			this.lblProgressAction.AutoSize = true;
			this.lblProgressAction.Location = new System.Drawing.Point(3, 9);
			this.lblProgressAction.Name = "lblProgressAction";
			this.lblProgressAction.Size = new System.Drawing.Size(78, 13);
			this.lblProgressAction.TabIndex = 1;
			this.lblProgressAction.Text = "ACTION TEXT";
			// 
			// prgProgress
			// 
			this.prgProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.prgProgress.Location = new System.Drawing.Point(3, 25);
			this.prgProgress.Name = "prgProgress";
			this.prgProgress.Size = new System.Drawing.Size(644, 23);
			this.prgProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.prgProgress.TabIndex = 0;
			// 
			// FormOculusApps
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(700, 675);
			this.Controls.Add(this.pnlProgress);
			this.Controls.Add(this.pnlApps);
			this.Controls.Add(this.pnlTop);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOculusApps";
			this.Text = "Oculus Game Manager";
			this.ResizeEnd += new System.EventHandler(this.FormOculusApps_ResizeEnd);
			this.pnlTop.ResumeLayout(false);
			this.pnlTop.PerformLayout();
			this.pnlProgress.ResumeLayout(false);
			this.pnlProgress.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.FlowLayoutPanel pnlApps;
		private System.Windows.Forms.Panel pnlTop;
		private System.Windows.Forms.Button btnRestore;
		private System.Windows.Forms.TextBox txtHomePath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel pnlProgress;
		private System.Windows.Forms.Label lblProgressAction;
		private System.Windows.Forms.ProgressBar prgProgress;
		private System.Windows.Forms.Button btnSetBackupLocation;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtBackupLocation;
		private System.Windows.Forms.FolderBrowserDialog dlgBrowseFolder;
		private System.Windows.Forms.OpenFileDialog dlgOpenFile;
		private System.Windows.Forms.CheckBox chkShowAll;
    }
}

