using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OculusGameManager.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace OculusGameManager.Oculus
{
	public class ManifestFile : IAppData
	{
		public OculusApp App { get; private set; }
		public string FilePath { get; private set; }
		public bool Exists { get { return File.Exists(FilePath); } }

		public string ApplicationId { get; private set; }
		public string CanonicalName { get; private set; }
		public string DisplayName { get; private set; }
		public string SmallLandscapeImage { get; private set; }
		public string Version { get; private set; }
		public int VersionCode { get; private set; }
		public string LaunchFile { get; private set; }
		public long PackageSize { get; private set; }
		public string[] Redistributables { get; private set; }

		public ManifestFile(OculusApp app, string path)
		{
			App = app;
			FilePath = path;

			if (Exists)
			{
				var text = File.ReadAllText(FilePath);
				var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);

				ApplicationId = data["appId"].ToString();
				CanonicalName = data["canonicalName"].ToString();
				LaunchFile = data["launchFile"].ToString();
				// TODO: launchParameters
				Version = (data["version"] ?? "0").ToString();
				VersionCode = Int32.Parse((data["versionCode"] ?? "0").ToString());
				if (data.ContainsKey("redistributables"))
				{
					Redistributables = ((JArray)data["redistributables"]).Cast<JValue>().Select(v => v.Value.ToString()).ToArray();
				}
			}

			// Estimate banner image
			var assetPath = Path.Combine(App.SoftwarePath, "..", "StoreAssets", App.CanonicalName + "_assets");
			if (Directory.Exists(assetPath))
			{
				var imgs = Directory.GetFiles(assetPath, "small_landscape_image.*");
				SmallLandscapeImage = imgs.Where(i => i.EndsWith(".jpg") || i.EndsWith(".png") || i.EndsWith(".gif")).FirstOrDefault();
			}
		}

		public void GenerateFile(Action<ProgressEventArgs> progressCallback = null)
		{
			if (!App.IsInstalled)
			{
				return;
			}

			progressCallback(new ProgressEventArgs { Total = 1 });

			var allFiles = Directory.GetFiles(App.GetRealInstallPath(), "*.*", SearchOption.AllDirectories);
			long fileCount = allFiles.Length;

			if (progressCallback != null)
			{
				progressCallback(new ProgressEventArgs { Total = fileCount });
			}

			if (App.LaunchFile == null)
			{
				LaunchFile = allFiles.Where(f => f.EndsWith(".exe"))
					.OrderBy(f => f)
					.Take(1)
					.Select(f => f.StripPath(App.GetRealInstallPath()))
					.FirstOrDefault(); // Complete guess
			}

			// Build file structure
			var data = new Dictionary<string, object>();
			data["appId"] = App.ApplicationId;
			data["canonicalName"] = App.CanonicalName;
			data["isCore"] = false;
			data["packageType"] = "APP";
			data["launchFile"] = App.LaunchFile;
			data["launchParameters"] = ""; // TODO
			data["launchFile2D"] = null; // TODO
			data["launchParameters2D"] = ""; // TODO
			data["version"] = App.Version;
			data["versionCode"] = App.VersionCode;
			data["redistributables"] = App.Redistributables;
			data["firewallExceptionsRequired"] = false;

			// Calculate checksums
			var sha256 = new SHA256Managed();
			var checksums = new Dictionary<string, string>();
			long progress = 0;
			foreach (var file in allFiles)
			{
				using (var fi = new BufferedStream(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
				{
					var fn = file.StripPath(App.GetRealInstallPath()).Replace("\\", "/");
					var chksum = sha256.ComputeHash(fi);
					checksums[fn] = BitConverter.ToString(chksum).Replace("-", "").ToLower();
				}

				if (progressCallback != null)
				{
					progressCallback(new ProgressEventArgs { Total = fileCount, Progress = ++progress });
				}
			}
			data["files"] = checksums;

			// Generate file
			var text = JsonConvert.SerializeObject(data);
			File.WriteAllText(FilePath, text);

			if (progressCallback != null)
			{
				progressCallback(new ProgressEventArgs { Total = 1, Progress = 1, IsComplete = true });
			}
		}
	}
}
