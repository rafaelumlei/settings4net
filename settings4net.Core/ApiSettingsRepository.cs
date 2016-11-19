using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Core.Model;
using settings4net.Core.APIClient;
using settings4net.Core.RemoteRepositories.Mappers;
using log4net;
using System.Configuration;
using System.Threading;
using System.Collections.Concurrent;

namespace settings4net.Core.Repositories
{
    public class ApiSettingsRepository : ISingleAppSettingsRepository
    {

        private static ILog logger = LogManager.GetLogger(typeof(ApiSettingsRepository));

        private static readonly string SETTINGS4NET_REMOTE_URI_CONF_KEY = "Settings4netRemoteURI";

        private static readonly string SETTINGS4NET_ENV_CONF_KEY = "Settings4netCurrentEnvironment";

        private Settings4netAPI Settings4netAPI { get; set; }

        private ConcurrentDictionary<string, Setting> CurrentSettings { get; set; }

        private string CurrentApplication { get; set; }

        private string CurrentEnviroment { get; set; }

        public ApiSettingsRepository(string baseUri = null, string currentEnv = null)
        {
            if (string.IsNullOrEmpty(baseUri))
            {
                baseUri = ConfigurationManager.AppSettings[SETTINGS4NET_REMOTE_URI_CONF_KEY];

                if (string.IsNullOrEmpty(baseUri))
                    throw new ArgumentException("Settings4netRemoteURI not correctly defined in ConfigurationManager.AppSettings");
            }

            if (string.IsNullOrEmpty(currentEnv))
            {
                currentEnv = ConfigurationManager.AppSettings[SETTINGS4NET_ENV_CONF_KEY];

                if (string.IsNullOrEmpty(currentEnv))
                    throw new ArgumentException("Settings4netCurrentEnvironment not correctly defined in ConfigurationManager.AppSettings");
            }

            this.Settings4netAPI = new Settings4netAPI(new Uri(baseUri));
            this.CurrentEnviroment = currentEnv;
            this.CurrentApplication = AppDomain.CurrentDomain.FriendlyName;
            var task = this.LoadRemoteSettings();
            this.CurrentSettings = task.Result;
        }

        public async Task AddSettingAsync(Setting setting)
        {
            try
            {
                this.CurrentSettings.TryAdd(setting.Key, setting);
                Settings settingsOperation = new Settings(this.Settings4netAPI);
                APIClient.Models.Setting remoteSetting = ModelToAPIMapper.Map(setting);
                await settingsOperation.AddSettingAsync(this.CurrentApplication, this.CurrentEnviroment, remoteSetting).ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                logger.Warn("Exception when adding setting to remote settings4net API", exp);
            }
        }

        public void AddSetting(Setting setting)
        {
            this.AddSettingAsync(setting).Wait();
        }

        private async Task<ConcurrentDictionary<string, Setting>> LoadRemoteSettings()
        {
            if (this.CurrentSettings == null)
            {
                if (this.CurrentSettings != null)
                    return this.CurrentSettings;
                else
                {
                    Settings settingsOp = new Settings(this.Settings4netAPI);
                    IList<APIClient.Models.Setting> remoteSettings = await settingsOp.GetSettingsAsync(this.CurrentApplication, this.CurrentEnviroment).ConfigureAwait(false);
                    List<Setting> mappedCurrentSettings = ModelToAPIMapper.Map(remoteSettings).ToList();
                    this.CurrentSettings = new ConcurrentDictionary<string, Setting>(mappedCurrentSettings.ToDictionary(s => s.Key));
                    return this.CurrentSettings;
                }
            }

            return this.CurrentSettings ?? new ConcurrentDictionary<string, Setting>();
        }

        public async Task<List<Setting>> GetSettingsAsync()
        {
            return this.CurrentSettings.Values.ToList();
        }

        public List<Setting> GetSettings()
        {
            var task = this.GetSettingsAsync();
            return task.Result;
        }

        public async Task OverrideStateAsync(List<Setting> settingValues)
        {
            if (settingValues != null && settingValues.Any())
            {
                try
                {
                    var settingValuesDict = settingValues.ToDictionary(s => s.Key);
                    var settingsToAdd = settingValues.Where(s => !this.CurrentSettings.ContainsKey(s.Key));
                    var settingsToDelete = this.CurrentSettings.Where(s => !settingValuesDict.ContainsKey(s.Key)).Select(s => s.Value);

                    // adding to SERVER/API all that are present in code and not yet in the server
                    foreach (Setting setting in settingsToAdd)
                    {
                        await this.AddSettingAsync(setting).ConfigureAwait(false);
                    }

                    // deleting from SERVER/API all that are no more present in code
                    foreach (Setting setting in settingsToDelete)
                    {
                        await this.DeleteSettingAsync(setting.Key).ConfigureAwait(false);
                    }
                }
                catch (Exception exp)
                {
                    logger.Warn("Exception when overriding settings' state in remote settings4net api", exp);
                }
            }
        }

        public void OverrideState(List<Setting> settingValues)
        {
            this.OverrideStateAsync(settingValues).Wait();
        }

        public Task UpdateSettingAsync(Setting value)
        {
            // not required for now; this class is a settings master, it is not updated, 
            // only new settings are added (the developer adds the settings to code and 
            // they are sent to the setting4net server/api in the first initialize if the 
            // connection is valid
            throw new NotImplementedException();
        }

        public void UpdateSetting(Setting value)
        {
            // not required for now; this class is a settings master, it is not updated, 
            // only new settings are added (the developer adds the settings to code and 
            // they are sent to the setting4net server/api in the first initialize if the 
            // connection is valid
            throw new NotImplementedException();
        }

        public Task UpdateSettingsAsync(List<Setting> values)
        {
            // not required for now; this class is a settings master, it is not updated, 
            // only new settings are added (the developer adds the settings to code and 
            // they are sent to the setting4net server/api in the first initialize if the 
            // connection is valid
            throw new NotImplementedException();
        }

        public void UpdateSettings(List<Setting> values)
        {
            // not required for now; this class is a settings master, it is not updated, 
            // only new settings are added (the developer adds the settings to code and 
            // they are sent to the setting4net server/api in the first initialize if the 
            // connection is valid
            throw new NotImplementedException();
        }

        public void DeleteSetting(string fullpath)
        {
            this.DeleteSettingAsync(fullpath).Wait();
        }

        public async Task DeleteSettingAsync(string fullpath)
        {
            try
            {
                Setting settingForKey = new Setting() { Application = this.CurrentApplication, Environment = this.CurrentEnviroment, Fullpath = fullpath };
                this.CurrentSettings.TryRemove(settingForKey.Key, out settingForKey);
                Settings settingsOperation = new Settings(this.Settings4netAPI);
                await settingsOperation.DeleteSettingAsync(settingForKey.Id).ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                logger.Warn("Exception when deleting setting to remote settings4net API", exp);
            }
        }

    }
}
