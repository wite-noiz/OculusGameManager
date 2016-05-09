using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OculusGameManager.Utils;

namespace OculusGameManager.Oculus.OAF
{
	public class OAFAppRecord : IAppData
	{
		public string ApplicationId { get; private set; }
		public string CanonicalName { get; private set; }
		public string DisplayName { get; private set; }
		public string SmallLandscapeImage { get; private set; }
		public string Version { get; private set; }
		public int VersionCode { get; private set; }
		public string LaunchFile { get; private set; }
		public long PackageSize { get; private set; }
		public string[] Redistributables { get; private set; }

		public OAFAppRecord(BasicOAFRecord data)
		{
			if (data.TypeName != "Application")
			{
				throw new ArgumentOutOfRangeException("data", "Must be 'Application' OAF record");
			}
			ApplicationId = data.HashKey;
			VersionCode = -1;

			if (data.HasKey("canonical_name"))
			{
				CanonicalName = data.GetValue("canonical_name");
			}
			if (data.HasKey("display_name"))
			{
				DisplayName = data.GetValue("display_name");
			}
			if (data.HasKey("small_landscape_image", "uri"))
			{
				SmallLandscapeImage = data.GetValue("small_landscape_image", "uri");
			}
		}

		public void AugmentData(BasicOAFRecord data)
		{
			if (data.TypeName != "PCBinary")
			{
				throw new ArgumentOutOfRangeException("data", "Must be 'PCBinary' OAF record");
			}
			if (!data.HashKey.StartsWith(ApplicationId + "_"))
			{
				throw new ArgumentOutOfRangeException("data", "Must be 'PCBinary' OAF record for same application: " + ApplicationId);
			}

			var myVersionCode = 0;
			if (data.HashKey.IndexOf('_') > 0)
			{
				var vcStr = data.HashKey.Substring(data.HashKey.IndexOf('_') + 1);
				myVersionCode = Int32.Parse(vcStr);
			}

			// Take relevant data
			if (data.HasKey("version") && (myVersionCode > VersionCode || (Version ?? "").Length == 0))
			{
				Version = data.GetValue("version");
			}
			if (data.HasKey("launch_file") && (myVersionCode > VersionCode || (LaunchFile ?? "").Length == 0))
			{
				LaunchFile = data.GetValue("launch_file");
			}
			if (data.HasKey("size") && (myVersionCode > VersionCode || PackageSize == 0))
			{
				var sizeStr = data.GetValue("size");
				long size;
				if (Int64.TryParse(sizeStr, out size))
				{
					PackageSize = size;
				}
			}

			// Redistributables array
			Redistributables = data.GetArray("redistributables", "PCRedistributable", "required_space");

			if (myVersionCode > VersionCode)
			{
				VersionCode = myVersionCode;
			}
		}
	}
}
