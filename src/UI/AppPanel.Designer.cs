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
			((System.ComponentModel.ISupportInitialize)(this.picAppImage)).BeginInit();
			this.SuspendLayout();
			// 
			// txtAppTitle
			// 
			this.txtAppTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAppTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtAppTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtAppTitle.Location = new System.Drawing.Point(279, 3);
			this.txtAppTitle.Name = "txtAppTitle";
			this.txtAppTitle.ReadOnly = true;
			this.txtAppTitle.Size = new System.Drawing.Size(268, 17);
			this.txtAppTitle.TabIndex = 5;
			this.txtAppTitle.Text = "APP TITLE";
			// 
			// txtAppPath
			// 
			this.txtAppPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAppPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtAppPath.Location = new System.Drawing.Point(370, 23);
			this.txtAppPath.Name = "txtAppPath";
			this.txtAppPath.ReadOnly = true;
			this.txtAppPath.Size = new System.Drawing.Size(177, 13);
			this.txtAppPath.TabIndex = 6;
			this.txtAppPath.Text = "INSTALLATION PATH";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(279, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Installation Path:";
			// 
			// picAppImage
			// 
			this.picAppImage.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.picAppImage.Location = new System.Drawing.Point(3, 3);
			this.picAppImage.Name = "picAppImage";
			this.picAppImage.Size = new System.Drawing.Size(270, 90);
			this.picAppImage.TabIndex = 3;
			this.picAppImage.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(279, 39);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Size:";
			// 
			// txtAppSize
			// 
			this.txtAppSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAppSize.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtAppSize.Location = new System.Drawing.Point(370, 39);
			this.txtAppSize.Name = "txtAppSize";
			this.txtAppSize.ReadOnly = true;
			this.txtAppSize.Size = new System.Drawing.Size(177, 13);
			this.txtAppSize.TabIndex = 6;
			this.txtAppSize.Text = "APP SIZE";
			// 
			// btnBackupApp
			// 
			this.btnBackupApp.Location = new System.Drawing.Point(360, 70);
			this.btnBackupApp.Name = "btnBackupApp";
			this.btnBackupApp.Size = new System.Drawing.Size(75, 23);
			this.btnBackupApp.TabIndex = 10;
			this.btnBackupApp.Text = "&Backup";
			this.btnBackupApp.UseVisualStyleBackColor = true;
			this.btnBackupApp.Click += new System.EventHandler(this.btnBackupApp_Click);
			// 
			// btnRelocateApp
			// 
			this.btnRelocateApp.Location = new System.Drawing.Point(279, 70);
			this.btnRelocateApp.Name = "btnRelocateApp";
			this.btnRelocateApp.Size = new System.Drawing.Size(75, 23);
			this.btnRelocateApp.TabIndex = 9;
			this.btnRelocateApp.Text = "&Move";
			this.btnRelocateApp.UseVisualStyleBackColor = true;
			this.btnRelocateApp.Click += new System.EventHandler(this.btnRelocateApp_Click);
			// 
			// lblMissingManifest
			// 
			this.lblManifestError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblManifestError.ForeColor = System.Drawing.Color.Red;
			this.lblManifestError.Location = new System.Drawing.Point(279, 55);
			this.lblManifestError.Name = "lblMissingManifest";
			this.lblManifestError.Size = new System.Drawing.Size(268, 13);
			this.lblManifestError.TabIndex = 11;
			this.lblManifestError.Text = "Missing Manifest File!";
			// 
			// btnMakeManifest
			// 
			this.btnMakeManifest.Location = new System.Drawing.Point(441, 70);
			this.btnMakeManifest.Name = "btnMakeManifest";
			this.btnMakeManifest.Size = new System.Drawing.Size(103, 23);
			this.btnMakeManifest.TabIndex = 12;
			this.btnMakeManifest.Text = "&Generate Manifest";
			this.btnMakeManifest.UseVisualStyleBackColor = true;
			this.btnMakeManifest.Visible = false;
			this.btnMakeManifest.Click += new System.EventHandler(this.btnMakeManifest_Click);
			// 
			// AppPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.btnMakeManifest);
			this.Controls.Add(this.lblManifestError);
			this.Controls.Add(this.btnBackupApp);
			this.Controls.Add(this.btnRelocateApp);
			this.Controls.Add(this.txtAppTitle);
			this.Controls.Add(this.txtAppSize);
			this.Controls.Add(this.txtAppPath);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.picAppImage);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.MaximumSize = new System.Drawing.Size(99999, 96);
			this.MinimumSize = new System.Drawing.Size(550, 96);
			this.Name = "AppPanel";
			this.Size = new System.Drawing.Size(550, 96);
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

	}
}
