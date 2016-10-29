using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace settings4net.Core
{
    public class CodeSettingsRepository : ISettingsRepository
    {
        private static readonly string DEFAULT_ENVIRONMENT = "dev";

        private object settingsAccessSync = new object();

        private Dictionary<string, SettingToCodeData> currentSettings = null;

        private Dictionary<string, SettingToCodeData> CurrentSettings
        {
            get
            {
                this.LoadCodeSettings();
                return currentSettings;
            }
        }

        private void LoadCodeSettings()
        {
            lock (settingsAccessSync)
            {
                if (currentSettings == null)
                {
                    Type settingsMarkType = typeof(ISettingsClass);

                    List<Type> settingsContainers = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.IsClass && settingsMarkType.IsAssignableFrom(p))
                        .ToList();

                    currentSettings = new Dictionary<string, SettingToCodeData>();
                    string applicationName = AppDomain.CurrentDomain.FriendlyName;

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
                                SettingValue = new Setting(applicationName, DEFAULT_ENVIRONMENT, fullpath, jsonValue, documentation),
                                SettingField = f
                            };

                            currentSettings.Add(settingToCode.SettingValue.Key, settingToCode);
                        });
                    });
                }
            }
        }

        public List<Setting> GetSettings(string currentEnvironment)
        {
            return CurrentSettings.Values.Select(s => s.SettingValue).ToList();
        }

        public void OverrideState(string currentEnvironment, List<Setting> values)
        {
            throw new NotImplementedException();
        }

        public void UpdateSettings(string currentEnvironment, List<Setting> newValues)
        {
            newValues.ForEach(v => 
            {
                try
                {
                    this.UpdateSetting(currentEnvironment, v);
                }
                catch { }
            });
        }

        public void UpdateSetting(string currentEnvironment, Setting value)
        {
            SettingToCodeData settingToUpdate = null;
            CurrentSettings.TryGetValue(value.Key, out settingToUpdate);
            if (settingToUpdate != null && settingToUpdate.SettingValue != value)
            {
                settingToUpdate.SettingValue.Update(value);
                object deserializedSettingValue = value.JSONValue.ToObject(settingToUpdate.SettingField.FieldType);
                settingToUpdate.SettingField.SetValue(null, deserializedSettingValue);
            }
        }
    }
}
