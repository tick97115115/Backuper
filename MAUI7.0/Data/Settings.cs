using BackuperCore;
namespace BackuperGUI.Data
{
    public static class Settings
    {
        private static string _dupPath = Preferences.Default.Get("DataPath", Path.Combine(FileSystem.Current.AppDataDirectory, "backup"));
        public static string  DupPath
        {
            get
            {
                if (!Directory.Exists(_dupPath))
                {
                    Directory.CreateDirectory(_dupPath);
                }
                return _dupPath;
            }
            set
            {
                _dupPath = Path.Combine(Path.Combine(value, "backup"));
                Preferences.Default.Set("DataPath", _dupPath);
            }
        }
        public static string SqlitePath => Path.Combine(FileSystem.Current.AppDataDirectory, "db.sqlite");
        private static int _taskListLimit = Preferences.Default.Get("TaskListLimit", 10);
        public static int TaskListLimit
        {
            get
            {
                return _taskListLimit;
            }
            set
            {
                Preferences.Default.Set("TaskListLimit", value);
                _taskListLimit = value;
            }
        }
    }
    public class BackuperInMaui: Backuper
    {
        public BackuperInMaui(): base(Settings.DupPath, Settings.SqlitePath)
        {

        }
    }
}