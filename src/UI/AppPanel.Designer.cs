namespace OculusGameManager.UI
{
	partial class AppPanel
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtAppTitle = new System.Windows.Forms.TextBox();
			this.txtAppPath = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.picAppImage = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtAppSize = new System.Windows.Forms.TextBox();
			this.btnBackupApp = new System.Windows.Forms.Button();
			this.btnRelocateApp = new System.Windows.Forms.Button();
			this.lblManifestError = new System.Windows.Forms.Label();
			this.btnMakeManifest = new System.Windows.Forms.Button();
			this.txtAppVersion = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.picAppImage)).BeginInit();
			this.SuspendLayout();
			// 
			// txtAppTitle
			// 
			this.txtAppTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAppTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtAppTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtAppTitle.Location = new System.Drawing.Point(558, 6);
			this.txtAppTitle.Margin = new System.Windows.Forms.Padding(6);
			this.txtAppTitle.Name = "txtAppTitle";
			this.txtAppTitle.ReadOnly = true;
			this.txtAppTitle.Size = new System.Drawing.Size(536, 34);
			this.txtAppTitle.TabIndex = 5;
			this.txtAppTitle.Text = "APP TITLE";
			// 
			// txtAppPath
			// 
			this.txtAppPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAppPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtAppPath.Location = new System.Drawing.Point(740, 46);
			this.txtAppPath.Margin = new System.Windows.Forms.Padding(6);
			this.txtAppPath.Name = "txtAppPath";
			this.txtAppPath.ReadOnly = true;
			this.txtAppPath.Size = new System.Drawing.Size(354, 24);
			this.txtAppPath.TabIndex = 6;
			this.txtAppPath.Text = "INSTALLATION PATH";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(558, 46);
			this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(171, 25);
			this.label2.TabIndex = 4;
			this.label2.Text = "Installation Path:";
			// 
			// picAppImage
			// 
			this.picAppImage.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.picAppImage.Location = new System.Drawing.Point(6, 6);
			this.picAppImage.Margin = new System.Windows.Forms.Padding(6);
			this.picAppImage.Name = "picAppImage";
			this.picAppImage.Size = new System.Drawing.Size(540, 180);
			this.picAppImage.TabIndex = 3;
			this.picAppImage.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(558, 78);
			this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 25);
			this.label1.TabIndex = 4;
			this.label1.Text = "Size:";
			// 
			// txtAppSize
			// 
			this.txtAppSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAppSize.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtAppSize.Location = new System.Drawing.Point(740, 78);
			this.txtAppSize.Margin = new System.Windows.Forms.Padding(6);
			this.txtAppSize.Name = "txtAppSize";
			this.txtAppSize.ReadOnly = true;
			this.txtAppSize.Size = new System.Drawing.Size(354, 24);
			this.txtAppSize.TabIndex = 6;
			this.txtAppSize.Text = "APP SIZE";
			// 
			// btnBackupApp
			// 
			this.btnBackupApp.Location = new System.Drawing.Point(720, 140);
			this.btnBackupApp.Margin = new System.Windows.Forms.Padding(6);
			this.btnBackupApp.Name = "btnBackupApp";
			this.btnBackupApp.Size = new System.Drawing.Size(150, 46);
			this.btnBackupApp.TabIndex = 10;
			this.btnBackupApp.Text = "&Backup";
			this.btnBackupApp.UseVisualStyleBackColor = true;
			this.btnBackupApp.Click += new System.EventHandler(this.btnBackupApp_Click);
			// 
			// btnRelocateApp
			// 
			this.btnRelocateApp.Location = new System.Drawing.Point(558, 140);
			this.btnRelocateApp.Margin = new System.Windows.Forms.Padding(6);
			this.btnRelocateApp.Name = "btnRelocateApp";
			this.btnRelocateApp.Size = new System.Drawing.Size(150, 46);
			this.btnRelocateApp.TabIndex = 9;
			this.btnRelocateApp.Text = "&Move";
			this.btnRelocateApp.UseVisualStyleBackColor = true;
			this.btnRelocateApp.Click += new System.EventHandler(this.btnRelocateApp_Click);
			// 
			// lblManifestError
			// 
			this.lblManifestError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblManifestError.ForeColor = System.Drawing.Color.Red;
			this.lblManifestError.Location = new System.Drawing.Point(558, 110);
			this.lblManifestError.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.lblManifestError.Name = "lblManifestError";
			this.lblManifestError.Size = new System.Drawing.Size(536, 26);
			this.lblManifestError.TabIndex = 11;
			this.lblManifestError.Text = "Missing Manifest File!";
			// 
			// btnMakeManifest
			// 
			this.btnMakeManifest.Location = new System.Drawing.Point(882, 140);
			this.btnMakeManifest.Margin = new System.Windows.Forms.Padding(6);
			this.btnMakeManifest.Name = "btnMakeManifest";
			this.btnMakeManifest.Size = new System.Drawing.Size(206, 46);
			this.btnMakeManifest.TabIndex = 12;
			this.btnMakeManifest.Text = "&Generate Manifest";
			this.btnMakeManifest.UseVisualStyleBackColor = true;
			this.btnMakeManifest.Visible = false;
			this.btnMakeManifest.Click += new System.EventHandler(this.btnMakeManifest_Click);
			// 
			// txtAppVersion
			// 
			this.txtAppVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAppVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtAppVersion.Location = new System.Drawing.Point(740, 110);
			this.txtAppVersion.Margin = new System.Windows.Forms.Padding(6);
			this.txtAppVersion.Name = "txtAppVersion";
			this.txtAppVersion.ReadOnly = true;
			this.txtAppVersion.Size = new System.Drawing.Size(354, 24);
			this.txtAppVersion.TabIndex = 14;
			this.txtAppVersion.Text = "APP VERSION";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(558, 110);
			this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 25);
			this.label3.TabIndex = 13;
			this.label3.Text = "Version:";
			// 
			// AppPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.lblManifestError);
			this.Controls.Add(this.txtAppVersion);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnMakeManifest);
			this.Controls.Add(this.btnBackupApp);
			this.Controls.Add(this.btnRelocateApp);
			this.Controls.Add(this.txtAppTitle);
			this.Controls.Add(this.txtAppSize);
			this.Controls.Add(this.txtAppPath);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.picAppImage);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.MaximumSize = new System.Drawing.Size(199998, 192);
			this.MinimumSize = new System.Drawing.Size(1100, 192);
			this.Name = "AppPanel";
			this.Size = new System.Drawing.Size(1100, 192);
			((System.ComponentModel.ISupportInitialize)(this.picAppImage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtAppTitle;
		private System.Windows.Forms.TextBox txtAppPath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox picAppImage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtAppSize;
		private System.Windows.Forms.Button btnBackupApp;
		private System.Windows.Forms.Button btnRelocateApp;
		private System.Windows.Forms.Label lblManifestError;
		private System.Windows.Forms.Button btnMakeManifest;
		private System.Windows.Forms.TextBox txtAppVersion;
		private System.Windows.Forms.Label label3;
	}
}
