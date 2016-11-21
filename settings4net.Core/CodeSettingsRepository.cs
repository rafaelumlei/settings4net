using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;
using System.Configuration;

namespace settings4net.Core
{
    public class CodeSettingsRepository : ISingleAppSettingsRepository
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CodeSettingsRepository));

        private static readonly string SETTINGS4NET_ENV_CONF_KEY = "Settings4netCurrentEnvironment";

        private object settingsAccessSync = new object();

        private Dictionary<string, SettingToCodeData> CurrentSettings { get; set; }

        private string CurrentEnvironment { get; set; }

        private string CurrentApplication { get; set; }

        public CodeSettingsRepository(string appName, string currentEnv = null)
        {
            if (string.IsNullOrEmpty(currentEnv))
            {
                currentEnv = ConfigurationManager.AppSettings[SETTINGS4NET_ENV_CONF_KEY];

                if (string.IsNullOrEmpty(currentEnv))
                    throw new ArgumentException("Settings4netCurrentEnvironment not correctly defined in ConfigurationManager.AppSettings");
            }

            this.CurrentEnvironment = currentEnv;
            this.CurrentApplication = appName;
            this.LoadCodeSettings();
        }

        private void LoadCodeSettings()
        {
            if (this.CurrentSettings == null)
            {
                lock (settingsAccessSync)
                {
                    if (this.CurrentSettings == null)
                    {
                        Type settingsMarkType = typeof(ISettingsClass);

                        List<Type> settingsContainers = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(p => p.IsClass && settingsMarkType.IsAssignableFrom(p))
                            .ToList();

                        this.CurrentSettings = new Dictionary<string, SettingToCodeData>();
                        

                        // extract to converter 
                        settingsContainers.ForEach(t =>
                        {
                            XMLDocumentationLoader documentationLoader = new XMLDocumentationLoader(t);

                            t.GetFields().Where(f => f.IsPublic).ToList().ForEach(f =>
                            {
                                string fullpath = t.FullName + "." + f.Name;
                                JToken jsonValue = JToken.FromObject(f.GetValue(null));
                                string documentation = documentationLoader.GetDocumentation(f);

                                SettingToCodeData settingToCode = new SettingToCodeData()
                                {
                                    SettingValue = new Setting(this.CurrentApplication, this.CurrentEnvironment, fullpath, jsonValue, documentation),
                                    SettingField = f
                                };

                                this.CurrentSettings.Add(settingToCode.SettingValue.Key, settingToCode);
                            });
                        });
                    }
                }
            }
        }

        public Task<List<Setting>> GetSettingsAsync()
        {
            return Task.FromResult<List<Setting>>(this.GetSettings());
        }

        public List<Setting> GetSettings()
        {
            return CurrentSettings.Values.Select(s => s.SettingValue).ToList();
        }

        public Task OverrideStateAsync(List<Setting> values)
        {
            throw new NotImplementedException();
        }

        public void OverrideState(List<Setting> values)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateSettingsAsync(List<Setting> values)
        {
            this.UpdateSettings(values);
        }

        public void UpdateSettings(List<Setting> newValues)
        {
            foreach (Setting setting in newValues)
            {
                this.UpdateSetting(setting);
            }
        }

        public async Task UpdateSettingAsync(Setting value)
        {
            this.UpdateSetting(value);
        }

        public void UpdateSetting(Setting value)
        {
            SettingToCodeData settingToUpdate;
            CurrentSettings.TryGetValue(value.Key, out settingToUpdate);
            if (settingToUpdate != null && settingToUpdate.SettingValue != value)
            {
                settingToUpdate.SettingValue.Update(value);
                object deserializedSettingValue = JsonConvert.DeserializeObject(value.JSONValue.ToString(), settingToUpdate.SettingField.FieldType);
                settingToUpdate.SettingField.SetValue(null, deserializedSettingValue);
            }
        }

        public Task AddSettingAsync(Setting setting)
        {
            throw new NotImplementedException();
        }

        public void AddSetting(Setting setting)
        {
            throw new NotImplementedException();
        }

        public void DeleteSetting(Setting setting)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSettingAsync(Setting setting)
        {
            throw new NotImplementedException();
        }
    }
}
