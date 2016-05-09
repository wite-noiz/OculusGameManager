using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculusGameManager.Utils
{
	public interface IBackupHandler
	{
		bool IsFileBased { get; }
		string BackupPath { get; set; }
		string ArchiveName { get; set; }
		string FullArchivePath { get; }

		void DeleteExisting();
		void BackupFiles(IEnumerable<string> files, string root = null, Action<ProgressEventArgs> progressCallback = null);
		string[] PredictRestoredFiles(string location);
		void RestoreFiles(string location, Action<ProgressEventArgs> progressCallback = null);
	}
}
