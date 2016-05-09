using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculusGameManager.Oculus.OAF
{
	public class OAFRedistributableRecord
	{
		public string FileId { get; private set; }
		public string CanonicalName { get; private set; }
		public string DisplayName { get; private set; }
		public string FileName { get; private set; }
		public string Arguments { get; private set; }
		public string DownloadUri { get; private set; }
		public long FileSize { get; private set; }

		public OAFRedistributableRecord(BasicOAFRecord data)
		{
			FileId = data.HashKey;
			CanonicalName = data.GetValue("canonical_name");
			DisplayName = data.GetValue("name");
			FileName = data.GetValue("file_name");
			Arguments = data.GetValue("command_line_arguments");
			DownloadUri = data.GetValue("uri");

			var fileSizeStr = data.GetValue("size");
			long fileSize = 0;
			if (Int64.TryParse(fileSizeStr, out fileSize))
			{
				FileSize = fileSize;
			}
		}

		public void DownloadFile(string path)
		{
			// TODO
		}
	}
}
