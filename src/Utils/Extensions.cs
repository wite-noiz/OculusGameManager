using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OculusGameManager.Utils
{
    public static class Extensions
    {
        public static TResult Coalesce<TType, TResult>(this Type type, TType[] objs, Func<TType, TResult> fn, bool allowDefault = false)
            where TType : class
        {
            return Coalesce(type, objs, fn, default(TResult), allowDefault);
        }
        public static TResult Coalesce<TType, TResult>(this Type type, TType[] objs, Func<TType, TResult> fn, TResult defValue, bool allowDefault = false)
            where TType : class
        {
            Func<TResult, bool> cond = null;
            if (!allowDefault)
            {
                cond = (v) => !Object.Equals(v, default(TResult));
            }
            return Coalesce(type, objs, fn, cond, defValue);
        }
        public static TResult Coalesce<TType, TResult>(this Type type, TType[] objs, Func<TType, TResult> fn, Func<TResult, bool> cond)
            where TType : class
        {
            return Coalesce(type, objs, fn, cond, default(TResult));
        }
        public static TResult Coalesce<TType, TResult>(this Type type, TType[] objs, Func<TType, TResult> fn, Func<TResult, bool> cond, TResult defValue)
            where TType : class
        {
            foreach (var obj in objs)
            {
                if (obj != null)
                {
                    var val = fn(obj);
                    if (type.IsInstanceOfType(val) || cond == null || cond(val))
                    {
                        return val;
                    }
                }
            }
            return defValue;
        }

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
