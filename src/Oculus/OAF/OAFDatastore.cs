using OculusGameManager.Utils;
using System.Linq;

namespace OculusGameManager.Oculus.OAF
{
	public class OAFDatastore
	{
		private readonly string _oafDataPath;
		private OAFAppRecord[] _appRecords = null;

		public OAFDatastore(string oafDataPath)
		{
			_oafDataPath = oafDataPath;
		}

		private void parseRecords()
		{
			// TODO: Get for known ids and for unknown
			var db = new SqliteDb(_oafDataPath);
			var data = db.Execute("select * from Objects where hashkey like '%_%'");

			// OAF record types and interesting data:
			// * PCRedistributable - Per redistributable (hashkey = id); canonical name, download URI
			// * Application - Per application (hashkey = appid); canonical name, display name, image URIs, (redistributables?)
			// * PCBinary - Per file (hashkey = id) + per application version (hashkey = appid_versioncode); launchfile, size, version, (redistributables?)
			// * OAFOfflineData(1) - user id, auth token
			// * User(1) - name

			// Parse redistributables
			_redistRecords = data.Where(d => d["typename"].ToString() == "PCRedistributable")
				.Select(d => new OAFRedistributableRecord(new BasicOAFRecord(d))).ToArray();

			// Parse applications
			_appRecords = data.Where(d => d["typename"].ToString() == "Application")
				.Select(d => new OAFAppRecord(new BasicOAFRecord(d))).ToArray();
			foreach (var rec in data.Where(d => d["typename"].ToString() == "PCBinary").Select(d => new BasicOAFRecord(d)))
			{
				// Add application version detail
				var app = _appRecords.FirstOrDefault(a => rec.HashKey.StartsWith(a.ApplicationId + "_"));
				if (app != null)
				{
					app.AugmentData(rec);
				}
			}

			// TODO: user info
		}

		public OAFRedistributableRecord[] GetRedistributablesRecords()
		{
			if (_redistRecords == null)
			{
				parseRecords();
			}
			return _redistRecords;
		}
		private OAFRedistributableRecord[] _redistRecords = null;

		public OAFAppRecord[] GetAppRecords()
		{
			if (_appRecords == null)
			{
				parseRecords();
			}
			return _appRecords;
		}
	}
}
