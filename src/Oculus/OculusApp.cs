using OculusGameManager.Oculus.OAF;
using OculusGameManager.Utils;
using System;
using System.IO;
using System.Linq;

namespace OculusGameManager.Oculus
{
	public class OculusApp : IAppData
	{
		private readonly OculusManager _mgr;

		public string ApplicationId { get; private set; }
		public string CanonicalName
		{
			get { return typeof(String).Coalesce(new IAppData[] { OAFRecord, ManifestFile }, (d) => d.CanonicalName); }
		}
		public string DisplayName
		{
			get { return typeof(String).Coalesce(new IAppData[] { OAFRecord, ManifestFile }, (d) => d.DisplayName); }
		}
		public string SoftwarePath { get { return Path.Combine(_mgr.SoftwarePath, CanonicalName); } }
		public long PackageSize
		{
			get { return typeof(Int64).Coalesce(new IAppData[] { OAFRecord, ManifestFile }, (d) => d.PackageSize); }
		}
		public string SmallLandscapeImage
		{
			get
			{
				return typeof(String).Coalesce(new IAppData[] { ManifestFile, OAFRecord }, (d) => d.SmallLandscapeImage,
					(v) => File.Exists(v));
			}
		}
		public string Version
		{
			get { return typeof(String).Coalesce(new IAppData[] { OAFRecord, ManifestFile }, (d) => d.Version); }
		}
		public int VersionCode
		{
			get { return typeof(Int32).Coalesce(new IAppData[] { OAFRecord, ManifestFile }, (d) => d.VersionCode); }
		}
		public string LaunchFile
		{
			get { return typeof(String).Coalesce(new IAppData[] { OAFRecord, ManifestFile }, (d) => d.LaunchFile); }
		}
		public string[] Redistributables
		{
			get { return typeof(String).Coalesce(new IAppData[] { OAFRecord, ManifestFile }, (d) => d.Redistributables); }
		}

		public ManifestFile ManifestFile { get; private set; }
		public bool HasManifestFile { get { return ManifestFile.Exists; } }
		public OAFAppRecord OAFRecord { get; private set; }

		public bool ManifestVersionOutofdate
		{
			get { return OAFRecord != null && HasManifestFile && ManifestFile.VersionCode == 0 && OAFRecord.VersionCode > 0; }
			//get { return (OAFRecord != null && OAFRecord.VersionCode > 0 && OAFRecord.VersionCode > ManifestFile.VersionCode); }
		}

		public bool IsInstalled { get { return Directory.Exists(SoftwarePath); } }
		public bool IsRelocated { get { return GetRealInstallPath(true) != null; } }

		public OculusApp(OculusManager mgr, string manifestFilePath, OAFAppRecord oafRecord)
		{
			_mgr = mgr;

			OAFRecord = oafRecord;
			ManifestFile = new ManifestFile(this, manifestFilePath);
			ApplicationId = typeof(String).Coalesce(new IAppData[] { ManifestFile, oafRecord }, (d) => d.ApplicationId);
		}

		public string GetRealInstallPath(bool onlyIfNotDefault = false)
		{
			if (_realInstallPath == null)
			{
				var swarePath = SoftwarePath;
				if ((swarePath ?? "").Length > 0 && Directory.Exists(swarePath))
				{
					_realInstallPath = new JunctionHandler().ResolveTarget(swarePath);
				}
			}
			if (_realInstallPath != null || onlyIfNotDefault)
			{
				return _realInstallPath;
			}
			return SoftwarePath;
		}
		private string _realInstallPath = null;

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

			// Assuming they exist (i.e., not downloading them)
			var redistFiles = _mgr.OAFDatastore.GetRedistributablesRecords()
				.Where(r => Redistributables.Contains(r.FileId))
				.Select(r => Path.Combine(_mgr.RedistributablesPath, r.CanonicalName + "-" + r.FileName))
				.Where(r => File.Exists(r)).ToArray();

			// Not using real path as want the "fake" addresses
			var files = Directory.GetFiles(SoftwarePath, "*.*", SearchOption.AllDirectories)
				.Union(redistFiles);
			if (HasManifestFile)
			{
				files = files.Union(new string[] { ManifestFile.FilePath });
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

			_realInstallPath = null;

			if (progressCallback != null)
			{
				progressCallback(new ProgressEventArgs { Total = 1, Progress = 1, IsComplete = true });
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
				files.Union(new string[] { ManifestFile.FilePath });
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
				fh.FullyDeleteDirectory(GetRealInstallPath());
			}
			catch { } // Not ideal, but we can ignore failure to remove original dir
			if (Directory.Exists(SoftwarePath))
			{
				fh.FullyDeleteDirectory(SoftwarePath);
			}

			var jp = new JunctionHandler();
			jp.Create(SoftwarePath, fh.FullArchivePath);
			// TODO: React to not created junction

			_realInstallPath = null;
		}
	}
}
