using OculusGameManager.Oculus;
using OculusGameManager.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace OculusGameManager.UI
{
	public partial class FormOculusApps : Form
	{
		private readonly OculusManager _mgr;
		private OculusApp[] _apps;
		private bool _stoppedHome = false;

		public FormOculusApps()
		{
			InitializeComponent();

			_mgr = new OculusManager();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			txtHomePath.Text = _mgr.InstallationPath ?? Strings.I_NO_OCULUS_PATH;
			txtBackupLocation.Text = _mgr.BackupPath;

			if (_mgr.InstallationPath == null)
			{
				pnlTop.Enabled = false;
				pnlApps.Enabled = false;
			}
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			// If Home running, prompt to stop
			if (_mgr.IsHomeRunning())
			{
				if (MessageBox.Show(Strings.M_HOME_RUNNING_STOP, "Stop Oculus Home", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
				{
					_mgr.StopOculusHome();
					_stoppedHome = true;
				}
            }

            rebuildApps();
        }

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			if (_stoppedHome)
			{
				if (MessageBox.Show(Strings.M_START_HOME, "Start Oculus Home", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
				{
					_mgr.StartOculusHome();
				}
			}
		}

		private void rebuildApps()
		{
			_apps = _mgr.ProcessLibraryData(true);

			// Unsubscribe from existing
			foreach (AppPanel pnl in pnlApps.Controls)
			{
				pnl.StartedAction -= pnlApp_StartedAction;
				pnl.ProgressChangedListener = null;
				pnl.WorkCompleteListener = null;
			}
			pnlApps.Controls.Clear();

			// Build app panels
			foreach (var app in _apps.OrderBy(a => a.DisplayName))
			{
				var pnl = new AppPanel(app);
				pnlApps.SetFlowBreak(pnl, true);
				pnlApps.Controls.Add(pnl);

				pnl.Visible = app.IsInstalled || chkShowAll.Checked;
				pnl.StartedAction += pnlApp_StartedAction;
				pnl.ProgressChangedListener = pnlApp_ProgressChanged;
				pnl.WorkCompleteListener = pnlApp_WorkComplete;
			}

			chkShowAll.Text = Strings.I_SHOW_ALL.FormatWith(_apps.Count(a => !a.IsInstalled), _apps.Length);

			FormOculusApps_ResizeEnd(null, null);
		}

		private void pnlApp_StartedAction(object sender, OculusAppEventArgs e)
		{
			pnlProgress.Visible = true;
			pnlApps.Enabled = false;
			pnlTop.Enabled = false;
			lblProgressAction.Text = "{0} {1}...".FormatWith(e.EventName, e.App != null ? e.App.DisplayName : "an app");
		}

		private void pnlApp_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			prgProgress.Value = e.ProgressPercentage;
		}

		private void pnlApp_WorkComplete(object sender, EventArgs e)
		{
			_mgr.ProcessLibraryData(true);
			rebuildApps();

			pnlProgress.Visible = false;
			pnlApps.Enabled = true;
			pnlTop.Enabled = true;
		}

		private void FormOculusApps_ResizeEnd(object sender, EventArgs e)
		{
			SuspendLayout();
			foreach (Control ctl in pnlApps.Controls)
			{
				ctl.Width = pnlApps.Width - 17;
			}
			ResumeLayout();
		}

		private void btnSetBackupLocation_Click(object sender, EventArgs e)
		{
			dlgBrowseFolder.Description = Strings.D_CHOOSE_BACKUP_DIR;
			dlgBrowseFolder.SelectedPath = _mgr.BackupPath;
			if (dlgBrowseFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				txtBackupLocation.Text = dlgBrowseFolder.SelectedPath;
				_mgr.BackupPath = dlgBrowseFolder.SelectedPath;
			}
		}

		private void btnRestore_Click(object sender, EventArgs e)
		{
			string archivePath = null;

			var bh = Program.Resolve<IBackupHandler>();
			if (bh.IsFileBased)
			{
				dlgOpenFile.Title = Strings.D_CHOOSE_BACKUP_FILE;
				dlgOpenFile.InitialDirectory = _mgr.BackupPath;
				dlgOpenFile.CheckFileExists = true;
				dlgOpenFile.Filter = "Archive File (*.zip)|*.zip";
				dlgOpenFile.FilterIndex = 0;
				if (dlgOpenFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				{
					return;
				}
				archivePath = dlgOpenFile.FileName;
			}
			else
			{
				dlgBrowseFolder.Description = "Choose backup folder to restore";
				dlgBrowseFolder.SelectedPath = _mgr.BackupPath;
				if (dlgBrowseFolder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				{
					return;
				}
				archivePath = dlgBrowseFolder.SelectedPath;
			}

			var appName = Path.GetFileNameWithoutExtension(archivePath);
			var apps = _mgr.ProcessLibraryData().Where(a => a.CanonicalName == appName).ToArray();
			if (apps.Length == 0)
			{
				MessageBox.Show("Could not find application name in supported library: " + appName, "Unknown Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			else if (apps[0].IsInstalled)
			{
				MessageBox.Show("Selected backup appears to already be installed: " + appName, "Application Installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			dlgBrowseFolder.Description = "Choose location to restore to (subfolder will be created)";
			dlgBrowseFolder.SelectedPath = _mgr.SoftwarePath;
			if (dlgBrowseFolder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}

			var targetPath = dlgBrowseFolder.SelectedPath;
			if (targetPath != _mgr.SoftwarePath)
			{
				var truePath = Path.Combine(_mgr.SoftwarePath, appName);
				var jh = new JunctionHandler();
				jh.Create(truePath, Path.Combine(targetPath, appName), false);
				if (!Directory.Exists(truePath))
				{
					MessageBox.Show(Strings.M_DIR_CREATE_FAILED.FormatWith(Application.ProductName), "Directory Not Created", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}

			if (archivePath != null)
			{
				pnlApp_StartedAction(this, new OculusAppEventArgs { EventName = "Restoring" });

				var bw = new BackgroundWorker();
				bw.WorkerReportsProgress = true;
				bw.DoWork += (s, ea) =>
				{
					_mgr.RestoreApp(archivePath, (p) =>
					{
						bw.ReportProgress(p.ToPercent());
					});
				};
				bw.ProgressChanged += pnlApp_ProgressChanged;
				bw.RunWorkerCompleted += pnlApp_WorkComplete;
				bw.RunWorkerAsync();
			}
		}

		private void chkShowAll_CheckedChanged(object sender, EventArgs e)
		{
			foreach (AppPanel pnl in pnlApps.Controls)
			{
				pnl.Visible = pnl.App.IsInstalled || chkShowAll.Checked;
			}
		}
	}
}
