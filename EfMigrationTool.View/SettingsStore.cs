using System.Reflection;
using System.Configuration;

namespace EfMigrationTool.View
{
    public class SettingsStore
    {
        private Configuration _config = null;

        public SettingsStore()
        {
            var executingAssembly = Assembly.GetExecutingAssembly().Location;
            _config = ConfigurationManager.OpenExeConfiguration(executingAssembly);
        }

        public static void WriteDefaultSettings()
        {
            var executingAssembly = Assembly.GetExecutingAssembly().Location;
            var config = ConfigurationManager.OpenExeConfiguration(executingAssembly);
            config.AppSettings.Settings.Add(SettingKeyEnum.DiffToolPath.ToString(), "C:\\Program Files (x86)\\WinMerge\\WinMergeU.exe");
            config.AppSettings.Settings.Add(SettingKeyEnum.DiffToolPattern.ToString(), "{file1} {file2}");
            config.Save(ConfigurationSaveMode.Minimal);
        }

        public string GetSettingValue(SettingKeyEnum key)
        {
            var value = _config.AppSettings.Settings[key.ToString()].Value;
            return value;
        }
    }
}
