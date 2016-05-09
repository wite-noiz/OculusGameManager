using OculusGameManager.UI;
using OculusGameManager.Utils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;

namespace OculusGameManager
{
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(params string[] args)
		{
			LoggingExtensions.Logging.Log.InitializeWith<LoggingExtensions.NLog.NLogLog>();
			"Program".Log().Debug("Starting");

			// If not admin, restart
			var hasAdminRights = WindowsIdentity.GetCurrent().Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid);
			if (!hasAdminRights && args.Contains("admin"))
			{
				"Program".Log().Info("Failed to restart with admin rights");
				var result = MessageBox.Show(Strings.M_NOT_ADMIN, "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation);
				if (result == DialogResult.Abort)
				{
					return;
				}
				if (result == DialogResult.Retry)
				{
					hasAdminRights = false;
				}
			}
			if (!hasAdminRights)
			{
				"Program".Log().Info("Restarting with admin rights");
				RestartWithAdmin("admin");
				return;
			}

			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += (s, a) =>
			{
				"Program".Log().Fatal(() => "Unhandled exception: {0}".FormatWith(a.Exception.Message), a.Exception);
			};

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FormOculusApps());
			"Program".Log().Debug("Finished");
		}

		// TODO: Proper IoC
		public static T Resolve<T>()
		{
			if (typeof(T) == typeof(IBackupHandler))
			{
				return (T)(object)(new ZipBackupHandler());
			}

			throw new ArgumentOutOfRangeException();
		}

		public static void RestartWithAdmin(string args = "")
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = Application.ExecutablePath,
					Arguments = args,
					Verb = "runas"
				}
			};
			process.Start();
			System.Threading.Thread.Sleep(1000);
			Application.Exit();
		}

	}
}
