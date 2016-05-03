using Newtonsoft.Json;
using OculusGameManager.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace OculusGameManager.Oculus
{
    public class ManifestFile
	{
		public OculusApp App { get; private set; }
		public string Path { get; private set; }
		public bool Exists { get { return File.Exists(Path); } }

		public string Version { get; private set; }
		public int VersionCode { get; private set; }

		public ManifestFile(OculusApp app, string path)
		{
			App = app;
			Path = path;
		}

		public void ParseFile()
		{
			var text = File.ReadAllText(Path);
			var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
			Version = data["version"].ToString();
			VersionCode = Int32.Parse(data["versionCode"].ToString());

            // TODO
		}

		public void GenerateFile(Action<ProgressEventArgs> progressCallback = null)
        {
            if (!App.IsInstalled)
            {
                return;
            }

            progressCallback(new ProgressEventArgs { Total = 1 });

            var allFiles = Directory.GetFiles(App.RealInstallPath, "*.*", SearchOption.AllDirectories);
            long fileCount = allFiles.Length;

            if (progressCallback != null)
            {
                progressCallback(new ProgressEventArgs { Total = fileCount });
            }

            // Build file structure
            var data = new Dictionary<string, object>();
            data["appId"] = App.Id;
            data["canonicalName"] = App.CanonicalName;
            data["isCore"] = false;
            data["packageType"] = "APP";
            data["launchFile"] = allFiles.Where(f => f.EndsWith(".exe")).OrderBy(f => f).Select(f => f.StripPath(App.RealInstallPath)).FirstOrDefault(); // Complete guess
            data["launchParameters"] = "";
            data["launchFile2D"] = null;
            data["launchParameters2D"] = "";
            data["version"] = Version;
            data["versionCode"] = VersionCode;
            data["redistributables"] = new string[0];
            data["firewallExceptionsRequired"] = false;

            // Calculate checksums
            var sha256 = new SHA256Managed();
            var checksums = new Dictionary<string, string>();
            long progress = 0;
            foreach (var file in allFiles)
            {
                using (var fi = new BufferedStream(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    var fn = file.StripPath(App.RealInstallPath).Replace("\\", "/");
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
            var text = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            File.WriteAllText(Path, text);

            if (progressCallback != null)
            {
                progressCallback(new ProgressEventArgs { Total = 1, Progress = 1, IsComplete = true });
            }
        }
	}
}
