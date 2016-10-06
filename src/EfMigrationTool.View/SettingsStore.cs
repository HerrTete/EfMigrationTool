using System.Reflection;
using System.Configuration;
using System.Linq;

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

        public void WriteDefaultSettings()
        {
            AppendSetting(SettingKeyEnum.DiffToolPath, "C:\\Program Files (x86)\\WinMerge\\WinMergeU.exe");
            AppendSetting(SettingKeyEnum.DiffToolPattern, "{file1} {file2}");
            _config.Save(ConfigurationSaveMode.Minimal);
        }

        public void AppendSetting(SettingKeyEnum key, string value)
        {

            if (GetSettingValue(key) == null)
            {
                _config.AppSettings.Settings.Add(key.ToString(), value);
            }
        }

        public string GetSettingValue(SettingKeyEnum key)
        {
            string value = null;
            if(_config.AppSettings.Settings.AllKeys.ToList().Contains(key.ToString()))
            {
                value = _config.AppSettings.Settings[key.ToString()].Value;
            }
            return value;
        }
    }
}
