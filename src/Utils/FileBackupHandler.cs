using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OculusGameManager.Utils
{
	public class FileBackupHandler : IBackupHandler
	{
		public bool IsFileBased { get { return false; } }
		public string BackupPath { get; set; }
		public string ArchiveName { get; set; }
		public string FullArchivePath { get { return Path.Combine(BackupPath, ArchiveName); } }

		public FileBackupHandler()
		{
		}

		public void DeleteExisting()
		{
			FullyDeleteDirectory(FullArchivePath);
		}

		/// <summary>
		/// Attempting the safest possible full delete
		/// </summary>
		public void FullyDeleteDirectory(string path)
		{
			try
			{
				if (Directory.Exists(path))
				{
					var allFiles = new string[0];
					try
					{
						allFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
					}
					catch { } // Sometimes we can't get the list of files
					if (allFiles.Length > 0)
					{
						Array.ForEach(allFiles, f => File.Delete(f));
					}

					var allDirs = new string[0];
					try
					{
						// Get in reverse name order so as to delete from deepest to shallowest
						allDirs = Directory.GetDirectories(path, "*.*", SearchOption.AllDirectories).OrderByDescending(d => d).ToArray();
					}
					catch { } // Sometimes we can't get the list of directories
					if (allDirs.Length > 0)
					{
						Array.ForEach(allDirs, d => Directory.Delete(d, true));
					}

					Directory.Delete(path, true);
				}
			}
			catch (Exception)
			{
				throw;
			}

		}

		public void BackupFiles(IEnumerable<string> files, string root = null, Action<ProgressEventArgs> progressCallback = null)
		{
			BackupFiles(files, root, true, null);
		}
		public void BackupFiles(IEnumerable<string> files, string root = null, bool preserveOriginals = true, Action<ProgressEventArgs> progressCallback = null)
		{
			var fileCount = files.Count();
			progressCallback(new ProgressEventArgs { Total = fileCount });

			long progress = 0;
			foreach (var file in files.Where(f => f.StartsWithPath(root)))
			{
				var target = Path.Combine(BackupPath, ArchiveName, file.StripPath(root));
				Directory.CreateDirectory(Path.GetDirectoryName(target));
				Console.WriteLine(progress + ": " + file + " > " + target);
				if (!preserveOriginals)
				{
					File.Move(file, target);
				}
				else
				{
					File.Copy(file, target);
				}

				progressCallback(new ProgressEventArgs { Total = fileCount, Progress = ++progress });
			}

			progressCallback(new ProgressEventArgs { Total = fileCount, Progress = fileCount, IsComplete = true });
		}

		public string[] PredictRestoredFiles(string location)
		{
			var archivePath = FullArchivePath;
			var files = Directory.GetFiles(archivePath, "*.*", SearchOption.AllDirectories)
				.Select(f => f.StripPath(archivePath))
				.Select(f => Path.Combine(location, f))
				.ToArray();
			return files;
		}

		public void RestoreFiles(string location, Action<ProgressEventArgs> progressCallback = null)
		{
			var archivePath = FullArchivePath;
			var files = Directory.GetFiles(archivePath, "*.*", SearchOption.AllDirectories);
			var fileCount = files.Count();
			progressCallback(new ProgressEventArgs { Total = fileCount });

			long progress = 0;
			foreach (var file in files.Where(f => f.StartsWithPath(archivePath)))
			{
				var target = Path.Combine(location, file.StripPath(archivePath));
				Directory.CreateDirectory(Path.GetDirectoryName(target));
				File.Copy(file, target);

				progressCallback(new ProgressEventArgs { Total = fileCount, Progress = ++progress });
			}

			progressCallback(new ProgressEventArgs { Total = fileCount, Progress = fileCount, IsComplete = true });
		}
	}
}
