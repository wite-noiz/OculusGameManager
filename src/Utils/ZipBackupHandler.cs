using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace OculusGameManager.Utils
{
	/// <summary>
	/// Crashes on large files
	/// </summary>
	public class ZipBackupHandler : IBackupHandler
	{
		public bool IsFileBased { get { return true; } }
		public string BackupPath { get; set; }
		public string ArchiveName { get; set; }
		public string FullArchivePath { get { return Path.Combine(BackupPath, ArchiveName + ".zip"); } }

		public ZipBackupHandler()
		{
		}

		private ZipArchive getZipFile(string filename, bool overwriteExisting = false)
		{
			ZipArchive zipFile;
			if (!overwriteExisting && File.Exists(filename))
			{
				zipFile = ZipFile.Open(filename, ZipArchiveMode.Update, null);
			}
			else
			{
				zipFile = ZipFile.Open(filename, ZipArchiveMode.Create, null);
			}
			return zipFile;
		}

		public void DeleteExisting()
		{
			if (File.Exists(FullArchivePath))
			{
				File.Delete(FullArchivePath);
			}
		}

		public void BackupFile(string filename, string root = null)
		{
			// TODO
		}

		public void BackupFiles(string path, string filePattern, string root = null, Action<ProgressEventArgs> progressCallback = null)
		{
			// TODO
		}
		public void BackupFiles(IEnumerable<string> files, string root = null, Action<ProgressEventArgs> progressCallback = null)
		{
			var fileCount = files.Count();
			if (progressCallback != null)
			{
				progressCallback(new ProgressEventArgs { Total = fileCount });
			}

			using (var zipFile = getZipFile(FullArchivePath))
			{
				long progress = 0;
				foreach (var file in files.Where(f => f.StartsWithPath(root)))
				{
					var ze = zipFile.CreateEntryFromFile(file, file.StripPath(root));
					if (progressCallback != null)
					{
						progressCallback(new ProgressEventArgs { Total = fileCount, Progress = ++progress });
					}
				}

				if (progressCallback != null)
				{
					progressCallback(new ProgressEventArgs { Total = fileCount, Progress = fileCount, IsComplete = true });
				}
			}
		}

		public string[] PredictRestoredFiles(string location)
		{
			using (var zipFile = ZipFile.OpenRead(FullArchivePath))
			{
				var files = zipFile.Entries.Select(e => Path.Combine(location, e.FullName)).ToArray();
				return files;
			}
		}

		/// <summary>
		/// Extract all files in the archive to a location.
		/// </summary>
		public void RestoreFiles(string location, Action<ProgressEventArgs> progressCallback = null)
		{
			long fileCount = 0, progress = 0;
			using (var zipFile = ZipFile.OpenRead(FullArchivePath))
			{
				fileCount = zipFile.Entries.Count;
				if (progressCallback != null)
				{
					progressCallback(new ProgressEventArgs { Total = fileCount, Progress = 0 });
				}

				foreach (ZipArchiveEntry ze in zipFile.Entries)
				{
					var path = Path.Combine(location, ze.FullName);
					Directory.CreateDirectory(Path.GetDirectoryName(path));

					ze.ExtractToFile(path, true);

					if (progressCallback != null)
					{
						progressCallback(new ProgressEventArgs { Total = fileCount, Progress = ++progress });
					}
				}
			}

			if (progressCallback != null)
			{
				progressCallback(new ProgressEventArgs { Total = fileCount, Progress = progress, IsComplete = true });
			}
		}
	}
}
