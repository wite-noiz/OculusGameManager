using OculusGameManager.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OculusGameManager.Oculus
{
    public class OculusApp
	{
		private readonly OculusManager _mgr;

		public string Id { get; private set; }
		public string CanonicalName { get; private set; }
		public string Title { get; private set; }
		public string SoftwarePath { get; private set; }
		public ManifestFile ManifestFile { get; private set; }
		public bool HasManifestFile { get { return ManifestFile.Exists; } }
		public string RealInstallPath { get; private set; }
		public string ImagePath { get; private set; }
		public long RequiredSpace { get; private set; }

		public bool IsInstalled { get { return RealInstallPath != null; } }
		public bool IsRelocated { get { return RealInstallPath != null && RealInstallPath != SoftwarePath; } }

		public OculusApp(OculusManager mgr, string appName, IDictionary<string, string> data)
			: this(mgr, appName)
		{
			Id = data["id"];
			Title = data["title"];
			ImagePath = data["smallLandscapeImageUrl"];

			long rs;
			if (Int64.TryParse(data["requiredSpace"], out rs))
			{
				RequiredSpace = rs;
			}
		}
		public OculusApp(OculusManager mgr, string appName)
		{
			_mgr = mgr;

			CanonicalName = appName;
			Title = appName; // Don't know it
			SoftwarePath = Path.Combine(_mgr.SoftwarePath, appName);
			if (Directory.Exists(SoftwarePath))
			{
				RealInstallPath = new JunctionHandler().ResolveTarget(SoftwarePath) ?? SoftwarePath;
			}
			RequiredSpace = -1;
			ManifestFile = new ManifestFile(this, Path.Combine(_mgr.ManifestPath, appName + ".json"));

			var assetPath = Path.Combine(_mgr.SoftwarePath, "StoreAssets", appName + "_assets");
			if (Directory.Exists(assetPath))
			{
				var imgs = Directory.GetFiles(assetPath, "small_landscape_image.*");
				ImagePath = imgs.Where(i => i.EndsWith(".jpg") || i.EndsWith(".png") || i.EndsWith(".gif")).FirstOrDefault();
			}
		}

		public void CreateBackup(bool deleteOriginal = false, Action<ProgressEventArgs> progressCallback = null)
		{
			if (!IsInstalled)
			{
				return;
			}

			// TODO: Check has enough space

			bool isComplete = false; // Control own complete state
			Action<ProgressEventArgs> myPCb = (p) =>
			{
				if (progressCallback != null)
				{
					p.IsComplete = isComplete;
					progressCallback(p);
				}
			};

			myPCb(new ProgressEventArgs { Total = 1 }); // Starting

			// Not using real path as want the "fake" addresses
			var files = Directory.GetFiles(SoftwarePath, "*.*", SearchOption.AllDirectories);
			if (ManifestFile != null)
			{
				files.Union(new string[] { ManifestFile.Path });
			}

			var bh = Program.Resolve<IBackupHandler>();
			bh.BackupPath = _mgr.BackupPath;
			bh.ArchiveName = CanonicalName;

			// TODO: Ask before deleting existing
			bh.DeleteExisting();

			bh.BackupFiles(files, _mgr.InstallationPath, progressCallback);

			myPCb(new ProgressEventArgs { IsComplete = true }); // Finished
		}

		public void RestoreBackup(string archivePath, string target, Action<ProgressEventArgs> progressCallback = null)
		{
			// TODO: Check has enough space at target

			var bh = Program.Resolve<IBackupHandler>();
			bh.BackupPath = _mgr.BackupPath;
			bh.ArchiveName = CanonicalName;
			bh.RestoreFiles(target, progressCallback);
		}

		public void Identify(string target, Action<ProgressEventArgs> progressCallback = null)
		{
			if (IsInstalled)
			{
				return;
			}

			if (progressCallback != null)
			{
				progressCallback(new ProgressEventArgs { Total = 1 });
			}

			var jp = new JunctionHandler();
			jp.Create(SoftwarePath, target);
			// TODO: React to not created junction

			RealInstallPath = target;

			if (progressCallback != null)
			{
				progressCallback(new ProgressEventArgs { Total = 1, Progress = 1, IsComplete = true });
			}
		}

		public void ParseManifestFile()
		{
			if (ManifestFile.Exists)
			{
				ManifestFile.ParseFile();
			}
		}

		public void Relocate(string target, Action<ProgressEventArgs> progressCallback = null)
		{
			if (!IsInstalled)
			{
				return;
			}

			// TODO: Check has enough space at target

			// Not using real path as want the "fake" addresses
			var files = Directory.GetFiles(SoftwarePath, "*.*", SearchOption.AllDirectories);
			if (ManifestFile != null)
			{
				files.Union(new string[] { ManifestFile.Path });
			}

			// Reuse logic of FileBackupHandler
			var fh = new FileBackupHandler();
			fh.BackupPath = target;
			fh.ArchiveName = CanonicalName;

			// TODO: Ask before deleting existing
			if (Directory.Exists(fh.FullArchivePath))
			{
				fh.DeleteExisting();
			}

			fh.BackupFiles(files, SoftwarePath, false, progressCallback);

			try
			{
				fh.FullyDeleteDirectory(RealInstallPath);
			}
			catch { } // Not ideal, but we can ignore failure to remove original dir
			if (Directory.Exists(SoftwarePath))
			{
				fh.FullyDeleteDirectory(SoftwarePath);
			}

			var jp = new JunctionHandler();
			jp.Create(SoftwarePath, fh.FullArchivePath);
			// TODO: React to not created junction

			RealInstallPath = fh.FullArchivePath;
		}
	}
}
