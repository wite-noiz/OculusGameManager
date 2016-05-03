namespace OculusGameManager
{
    public static class Strings
	{
		public const string D_CHOOSE_BACKUP_DIR = "Locate root backup folder";
		public const string D_CHOOSE_BACKUP_FILE = "Choose backup file to restore";

		public const string I_NO_OCULUS_PATH = "Unable to determine!";
		public const string I_SHOW_ALL = "Show not installed ({0} of {1})";

		public const string M_DIR_CREATE_FAILED = "Failed to create directory under Oculus\\Software.\nPlease try restarting {0} using 'Run As Administrator'.";
		public const string M_HOME_RUNNING_STOP = "Oculus Home or its services appear to be running.\nManaging apps is safer with Home stopped. Do you want Home to be stopped?\nContinuing will stop the application, services, and any running games.\n\nYou will prompted to restart it on exit.";
		public const string M_NOT_ADMIN = "Failed to restart as admin";
		public const string M_START_HOME = "Do you want the Home services to be restarted?\nThis may not restart the Home application.";

		public const string W_MISSING_MANIFEST = "Missing Manifest File!";
	}
}
