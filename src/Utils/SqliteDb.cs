using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculusGameManager.Utils
{
	/// <summary>
	/// Quick and dirty Sqlite wrapper
	/// </summary>
	public class SqliteDb
	{
		public string DatabaseFile { get; private set; }

		public SqliteDb(string dbfile)
		{
			DatabaseFile = dbfile;
		}

		public List<Dictionary<string, object>> Execute(string query)
		{
			var res = new List<Dictionary<string, object>>();
			using (var conn = new SQLiteConnection("Data Source=" + DatabaseFile))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = query;

					using (var rdr = cmd.ExecuteReader())
					{
						while (rdr.Read())
						{
							var dic = new Dictionary<string, object>();
							for (var i = 0; i < rdr.FieldCount; ++i)
							{
								dic[rdr.GetName(i)] = rdr.GetValue(i);
							}
							res.Add(dic);
						}
					}
				}
			}
			return res;
		}
	}
}
