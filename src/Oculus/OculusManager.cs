using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OculusGameManager.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
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
		public string SoftwarePath { get; private set; }
		public string ManifestPath { get; private set; }
		public string OAFDataPath { get; private set; }

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

			SoftwarePath = Path.Combine(InstallationPath ?? "", "Software");
			ManifestPath = Path.Combine(InstallationPath ?? "", "Manifests");
			OAFDataPath = Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), @"Oculus\sessions\_oaf\data.sqlite");
		}

		public OculusApp[] GetInstalledApps()
		{
			var softwareDir = new DirectoryInfo(SoftwarePath);
            if (!softwareDir.Exists)
            {
                return new OculusApp[0];
            }

			var appDirs = softwareDir.GetDirectories();
			var apps = appDirs.Where(d => d.Name != "StoreAssets").Select(d => new OculusApp(this, d.Name)).ToArray();
			return apps;
		}

		// TODO: Use session db to talk to web server for full detail?

		private OculusApp[] _apps = null;
		public OculusApp[] ProcessLibraryData(bool forceRefresh = false)
		{
			if (!forceRefresh && _apps != null)
			{
				return _apps;
			}

			// Need to check both app datas
			var localDataPath = Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%");
			var localLowDataPath = Path.GetFullPath(Path.Combine(localDataPath, "..", "LocalLow"));

			// Find all files in name order desc (newest first)
			IEnumerable<string> files = new string[0];
			var path = Path.Combine(localDataPath, "Oculus", "Home", "logs");
			if (Directory.Exists(path))
			{
				files = files.Union(Directory.GetFiles(path, "Home_*.txt"));
			}
			path = Path.Combine(localLowDataPath, "Oculus", "Home", "logs");
			if (Directory.Exists(path))
			{
				files = files.Union(Directory.GetFiles(path, "Home_*.txt"));
			}
			files = files.OrderByDescending(f => Path.GetFileName(f)).ToArray();

			// Check file at a time until find a full library
			var libraryApps = new Dictionary<string, OculusApp>();
			foreach (var file in files)
			{
				var storeData = File.ReadLines(file) // Read each line
					.Select(l => JsonConvert.DeserializeObject<Dictionary<string, string>>(l)) // Parse line as JSON
					.Where(d => d.ContainsKey("Message") && d["Message"].StartsWith("Process Payload [LIBRARY_UPDATE]:")) // Only want LIBRARY_UPDATE
					.OrderByDescending(l => l["Time"]) // Want latest first
					.Select(d => d["Message"].Substring(33)) // Strip non-JSON
					.Select(m => JsonConvert.DeserializeObject<Dictionary<string, object>>(m)) // Parse message as JSON
					.Where(m => m.ContainsKey("isFullLibrary") && m["isFullLibrary"].ToString().ToLower() == "true") // Looking for a "full library"
					.Take(1) // Only want the latest
					.SelectMany(m => (JArray)m["entitlements"]) // Take all entitlements
					.Cast<JObject>()
					.Select(o => o.Properties().ToDictionary<JProperty, string, string>(p => p.Name, p => o[p.Name].ToString())) // Convert to a proper dictionary
					.ToArray();
				if (storeData != null && storeData.Length > 0)
				{
					foreach (var dic in storeData)
					{
						var appName = dic["packageName"];
						if (!libraryApps.ContainsKey(appName))
						{
							libraryApps[appName] = new OculusApp(this, appName, dic);
						}
					}
					break;
				}
			}

			_apps = libraryApps.Values.ToArray();
			if (_apps.Length == 0)
			{
				_apps = GetInstalledApps();
			}
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
