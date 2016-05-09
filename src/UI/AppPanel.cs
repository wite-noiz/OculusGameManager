using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OculusGameManager.Oculus;
using System.IO;
using OculusGameManager.Utils;

namespace OculusGameManager.UI
{
	public partial class AppPanel : UserControl
	{
		public OculusApp App { get; private set; }

		public AppPanel()
		{
			InitializeComponent();
		}
		public AppPanel(OculusApp app)
		{
			App = app;

			InitializeComponent();
		}

		public ProgressChangedEventHandler ProgressChangedListener { get; set; }
		public RunWorkerCompletedEventHandler WorkCompleteListener { get; set; }

		public event EventHandler<OculusAppEventArgs> StartedAction;
		protected virtual void OnStartedAction(object sender, OculusAppEventArgs e)
		{
			if (StartedAction != null)
			{
				StartedAction(sender, e);
			}
		}

		private void refreshAppDetails()
		{
			txtAppTitle.Text = App.DisplayName;
			txtAppPath.Text = App.IsInstalled ? App.GetRealInstallPath() : "Not installed";
			txtAppVersion.Text = App.Version;
			picAppImage.ImageLocation = App.SmallLandscapeImage;
			btnBackupApp.Enabled = App.IsInstalled && App.HasManifestFile;

			if (!App.IsInstalled)
			{
				btnRelocateApp.Text = "&Identify";
			}

			lblManifestError.Visible = false;
			if (App.IsInstalled && !App.HasManifestFile)
			{
				lblManifestError.Visible = true;
				lblManifestError.Text = Strings.W_MISSING_MANIFEST;
			}
			else if (App.ManifestVersionOutofdate)
			{
				lblManifestError.Visible = true;
				lblManifestError.Text = Strings.W_MANIFEST_OLDVER;
			}
			btnMakeManifest.Visible = lblManifestError.Visible;

			var size = "Unknown";
			if (App.PackageSize > 0)
			{
				if (App.PackageSize < 1024)
				{
					size = App.PackageSize + " B";
				}
				else if (App.PackageSize < 1024 * 1024)
				{
					size = Math.Round(App.PackageSize / 1024.0, 2) + " KB";
				}
				else if (App.PackageSize < 1024 * 1024 * 1024)
				{
					size = Math.Round(App.PackageSize / 1024.0 / 1024.0, 2) + " MB";
				}
				else if (App.PackageSize < 1024L * 1024 * 1024 * 1024)
				{
					size = Math.Round(App.PackageSize / 1024.0 / 1024.0 / 1024.0, 2) + " GB";
				}
			}
			txtAppSize.Text = size;
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			if (App != null)
			{
				refreshAppDetails();
			}

			base.OnLayout(e);
		}

		private void btnRelocateApp_Click(object sender, EventArgs e)
		{
			string target;
			using (var dlg = new FolderBrowserDialog())
			{
                // TODO: Might be locate not relocate
				dlg.Description = "Locate folder to move application to (subfolder will be created)";
				dlg.SelectedPath = Path.GetFullPath(Path.Combine(App.GetRealInstallPath() ?? App.SoftwarePath, ".."));
				if (dlg.ShowDialog() != DialogResult.OK)
				{
					return;
				}
				target = dlg.SelectedPath;
			}

			OnStartedAction(this, new OculusAppEventArgs { EventName = "Relocating", App = App });

			runInBackground((cb) =>
			{
				Action<ProgressEventArgs> cb2 = (p) =>
				{
					cb(p.ToPercent());
				};

				if (App.IsInstalled)
				{
					App.Relocate(target, cb2);
				}
				else
				{
					App.Identify(target, cb2);
				}
			}, refreshAppDetails);
		}

		private void btnBackupApp_Click(object sender, EventArgs e)
		{
			OnStartedAction(this, new OculusAppEventArgs { EventName = "Backing up", App = App });

			runInBackground((cb) =>
			{
				App.CreateBackup(false, (p) =>
				{
					cb(p.ToPercent());
				});
			});
		}

		private void btnMakeManifest_Click(object sender, EventArgs e)
		{
			OnStartedAction(this, new OculusAppEventArgs { EventName = "Making manifest for", App = App });

			runInBackground((cb) =>
			{
				App.ManifestFile.GenerateFile((p) =>
				{
					cb(p.ToPercent());
				});
			});
		}

		private void runInBackground(Action<Action<int>> worker, Action complete = null)
		{
			var bw = new BackgroundWorker();
			bw.WorkerReportsProgress = true;
			bw.DoWork += (s, a) =>
			{
				worker(bw.ReportProgress);
			};
			if (ProgressChangedListener != null)
			{
				bw.ProgressChanged += ProgressChangedListener;
			}
			bw.RunWorkerCompleted += (s, e) =>
			{
				if (complete != null)
				{
					complete();
				}
				if (WorkCompleteListener != null)
				{
					WorkCompleteListener(s, e);
				}
			};
			bw.RunWorkerAsync();
		}
	}
}
