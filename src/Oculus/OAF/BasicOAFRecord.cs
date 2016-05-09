using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OculusGameManager.Utils;

namespace OculusGameManager.Oculus.OAF
{
	public class BasicOAFRecord
	{
		public string HashKey { get; private set; }
		public string TypeName { get; private set; }
		public string[] Value { get; private set; }

		public BasicOAFRecord(IDictionary<string, object> data)
		{
			HashKey = data["hashkey"].ToString();
			TypeName = data["typename"].ToString();
			Value = Encoding.Default.GetString((byte[])data["value"]).Split('\0');

			// TODO: understand Value and create dictionary
		}

		public bool HasKey(string key)
		{
			var i = Value.IndexOf(v => v.StartsWith(key + "\x1"));
			return i >= 0;
		}

		public string GetValue(string key)
		{
			var i = Value.IndexOf(v => v.StartsWith(key + "\x1"));
			if (i >= 0)
			{
				var vals = Value.Skip(i + 1).SkipWhile(v => v.Length == 0).ToArray();
				if (vals.Length > 0)
				{
					var val = vals.FirstOrDefault();
					if (vals.Length > 2)
					{
						val = val.Substring(0, val.Length - 2);
					}
					return val;
				}
			}
			return null;
		}

		public bool HasKey(params string[] keys)
		{
			var vals = Value.ToArray();
			for (var j = 0; j < keys.Length; ++j)
			{
				var i = vals.IndexOf(v => v.StartsWith(keys[j] + "\x1"));
				if (i < 0)
				{
					return false;
				}
				while (vals[++i].Length == 0)
					;
				vals = vals.Skip(i).ToArray();
			}
			return true;
		}

		public string GetValue(params string[] keys)
		{
			var vals = Value.ToArray();
			for (var j = 0; j < keys.Length; ++j)
			{
				var i = vals.IndexOf(v => v.StartsWith(keys[j] + "\x1"));
				if (i < 0)
				{
					return null;
				}
				while (vals[++i].Length == 0)
					;
				vals = vals.Skip(i).ToArray();
			}
			vals = vals.SkipWhile(v => v.Length == 0).ToArray();
			var val = vals.FirstOrDefault();
			if (vals.Length > 2)
			{
				val = val.Substring(0, val.Length - 2);
			}
			return val;
		}

		public string[] GetArray(string startKey, string valueKey, string endKey)
		{
			var result = new List<string>();
			var vals = Value.SkipWhile(v => !v.StartsWith(startKey + "\x1"))
				.Skip(1)
				.TakeWhile(v => !v.StartsWith(endKey + "\x1")).ToArray();
			if (vals.Length == 0)
			{
				return null;
			}
			for (var i = 0; i < vals.Length; ++i)
			{
				if (vals[i].StartsWith(valueKey + "\x1"))
				{
					while (vals[++i].Length == 0)
						;
					result.Add(vals[i]);
				}
			}
			return result.ToArray();
		}
	}
}
