using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OculusGameManager.Oculus.OAF;
using OculusGameManager.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OculusGameManager.Oculus
{
	public class OculusManager
	{
		private const string OCULUS_PROCESS_NAME = "OculusClient";
		private const string OCULUS_SERVICE_NAME = "Oculus VR Runtime Service";
		private const string OCULUS_SERVER_NAME = "OVRServer_x64";
		private const string OCULUS_SERVER_SUBPATH = @"Support\oculus-runtime\OVRServer_x64.exe";

		private readonly string _backupPathFile;
		private string _backupPath;

		public string InstallationPath { get; private set; }
		public string BackupPath
		{
			get { return _backupPath; }
			set
			{
				// Remember backup path
				_backupPath = value;
				File.WriteAllText(_backupPathFile, _backupPath);
			}
		}
		public string RedistributablesPath { get; private set; }
		public string SoftwarePath { get; private set; }
		public string ManifestPath { get; private set; }
		public OAFDatastore OAFDatastore { get; private set; }

		public OculusManager()
		{
			// Figure out main installation path based on where service is running from
			var svcs = ServiceController.GetServices().Where(s => s.DisplayName.Contains("Oculus")).ToArray();
			foreach (var svcPath in svcs.Select(s => s.GetImagePath()))
			{
				var path = Path.GetDirectoryName(svcPath);
				path = Path.GetFullPath(Path.Combine(path, "..", ".."));
				if (File.Exists(Path.Combine(path, "Oculus.ico")))
				{
					InstallationPath = path;
					break;
				}
			}
			this.Log().Info("Found Oculus install path: " + (InstallationPath ?? ""));

			// Restore last used backup path, or use standard temp
			_backupPathFile = Path.Combine(Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%"), Application.ProductName + ".txt");
			if (File.Exists(_backupPathFile))
			{
				_backupPath = File.ReadAllText(_backupPathFile);
			}
			if ((_backupPath ?? "").Length == 0)
			{
				_backupPath = System.IO.Path.GetTempPath();
			}

			RedistributablesPath = Path.Combine(InstallationPath, "Redistributables");
			SoftwarePath = Path.Combine(InstallationPath, "Software");
			ManifestPath = Path.Combine(InstallationPath, "Manifests");

			var oafPath = Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), @"Oculus\sessions\_oaf\data.sqlite");
			OAFDatastore = new OAFDatastore(oafPath);
		}

		// TODO: Use session db to talk to web server for full detail?

		private OculusApp[] _apps = null;
		public OculusApp[] ProcessLibraryData(bool forceRefresh = false)
		{
			// TODO: Drop; use OAF records and discovered manifests and folders instead
			if (!forceRefresh && _apps != null)
			{
				return _apps;
			}

			// Get all in OAF store
			var oafData = OAFDatastore.GetAppRecords();
			_apps = oafData.Select(d => new OculusApp(this, Path.Combine(ManifestPath, d.CanonicalName + ".json"), d)).ToArray();

			return _apps;
		}

		public void RestoreApp(string archive, Action<ProgressEventArgs> progressCallback = null)
		{
			var bh = Program.Resolve<IBackupHandler>();
			bh.BackupPath = Path.GetDirectoryName(archive);
			bh.ArchiveName = Path.GetFileNameWithoutExtension(archive);

			// TODO: warn on file overwrites
			var files = bh.PredictRestoredFiles(InstallationPath);

			bh.RestoreFiles(InstallationPath, progressCallback);
		}

		public bool IsHomeRunning()
		{
			var appProc = Process.GetProcessesByName(OCULUS_PROCESS_NAME);
			var svcProc = Process.GetProcessesByName(OCULUS_SERVER_NAME);
			return svcProc.Length > 0 || appProc.Length > 0;
		}

		public void StopOculusHome()
		{
			// Stop service
			var svc = new ServiceController(OCULUS_SERVICE_NAME);
			if (svc != null && svc.Status == ServiceControllerStatus.Running)
			{
				svc.Stop();
			}

			// Stop processes
			var appProc = Process.GetProcessesByName(OCULUS_PROCESS_NAME);
			Array.ForEach(appProc, p => p.Kill());
			var svcProc = Process.GetProcessesByName(OCULUS_SERVER_NAME);
			Array.ForEach(svcProc, p => p.Kill());
		}

		public void StartOculusHome()
		{
			// Start service or server
			var svc = new ServiceController(OCULUS_SERVICE_NAME);
			var startedSvc = false;
			if (svc != null && svc.Status == ServiceControllerStatus.Stopped)
			{
				try
				{
					svc.Stop();
					startedSvc = true;
				}
				catch { }
			}
			if (!startedSvc)
			{
				var process = new Process
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = Path.Combine(InstallationPath, OCULUS_SERVER_SUBPATH),
						CreateNoWindow = true,
						RedirectStandardOutput = true,
						UseShellExecute = false
					}
				};
				process.Start();
			}
		}
	}
}
