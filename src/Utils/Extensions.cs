using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;

namespace OculusGameManager.Utils
{
    public static class Extensions
	{
		public static string FormatWith(this string format, params object[] args)
		{
			return String.Format(format, args);
		}

		public static string GetImagePath(this ServiceController service)
		{
			using (var rk = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + service.ServiceName))
			{
				var path = rk.GetValue("ImagePath").ToString();

				if (path[0] == '"')
				{
					path = path.Substring(1, path.IndexOf('"', 1) - 1);
				}
				path = Environment.ExpandEnvironmentVariables(path);
				return path;
			}
		}

		public static int IndexOf<T>(this IEnumerable<T> value, T condition)
		{
			var idx = 0;
			foreach (var v in value)
			{
				if (condition.Equals(v))
				{
					return idx;
				}
				++idx;
			}
			return -1;
		}
		public static int IndexOf<T>(this IEnumerable<T> value, Predicate<T> condition)
		{
			var idx = 0;
			foreach (var v in value)
			{
				if (condition(v))
				{
					return idx;
				}
				++idx;
			}
			return -1;
		}

		public static bool StartsWithPath(this string testPath, string root)
		{
			if (root != null && root.Length > 0 && root[root.Length - 1] != Path.DirectorySeparatorChar && root[root.Length - 1] != Path.AltDirectorySeparatorChar)
			{
				root += Path.DirectorySeparatorChar;
			}
			return testPath.StartsWith(root, StringComparison.CurrentCultureIgnoreCase);
		}

		public static string StripPath(this string testPath, string root)
		{
			if (root != null && root.Length > 0 && root[root.Length - 1] != Path.DirectorySeparatorChar && root[root.Length - 1] != Path.AltDirectorySeparatorChar)
			{
				root += Path.DirectorySeparatorChar;
			}
			if (testPath.StartsWith(root, StringComparison.CurrentCultureIgnoreCase))
			{
				return testPath.Substring(root.Length);
			}
			return testPath;
		}
	}
}
