namespace DocumentArchiever.Configuration
{
    public static class AppSettings
    {
        public static string DatabaseServer => Properties.Settings.Default.dbServer;
        public static string DatabaseName => Properties.Settings.Default.Database;
        public static string DatabaseUserId => Properties.Settings.Default.DbUserId;
        public static string DatabasePassword => Properties.Settings.Default.DbUserPwd;
        public static string SavePath => Properties.Settings.Default.SavePath;
     //   public static string ExpressPrefix => Properties.Settings.Default.ExpressPrefix;
     //   public static int ExpressLength => Properties.Settings.Default.ExpressLength;
        public static long LastSelectedBranch => Properties.Settings.Default.LastSelectedBranch;

        public static void Save()
        {
            Properties.Settings.Default.Save();
        }

        public static void Reload()
        {
            Properties.Settings.Default.Reload();
        }
    }
}