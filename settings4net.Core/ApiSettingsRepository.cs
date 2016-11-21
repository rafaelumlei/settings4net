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

        private List<Setting> CurrentSettings { get; set; }

        private string CurrentApplication { get; set; }

        private string CurrentEnviroment { get; set; }

        public ApiSettingsRepository(string appName,string baseUri = null, string currentEnv = null)
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
            this.CurrentApplication = appName;
            var task = this.LoadRemoteSettings();
            this.CurrentSettings = task.Result;
        }

        public async Task AddSettingAsync(Setting setting)
        {
            try
            {
                setting.Application = this.CurrentApplication;
                setting.Environment = this.CurrentEnviroment;
                this.CurrentSettings.Add(setting);
                Settings settingsOperation = new Settings(this.Settings4netAPI);
                APIClient.Models.Setting remoteSetting = ModelToAPIMapper.Map(setting);
                await settingsOperation.AddSettingAsync(remoteSetting).ConfigureAwait(false);
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

        private async Task<List<Setting>> LoadRemoteSettings()
        {
            if (this.CurrentSettings == null)
            {
                if (this.CurrentSettings != null)
                    return this.CurrentSettings;
                else
                {
                    Settings settingsOp = new Settings(this.Settings4netAPI);
                    IList<APIClient.Models.Setting> remoteSettings = await settingsOp.GetSettingsAsync(this.CurrentApplication, this.CurrentEnviroment).ConfigureAwait(false);
                    this.CurrentSettings = ModelToAPIMapper.Map(remoteSettings).ToList();
                    return this.CurrentSettings;
                }
            }

            return this.CurrentSettings ?? new List<Setting>();
        }

        public async Task<List<Setting>> GetSettingsAsync()
        {
            return this.CurrentSettings;
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
                    var currentSettignsByKey = this.CurrentSettings.ToDictionary(s => s.Key);
                    var newSettignsByKey = settingValues.ToDictionary(s => s.Key);
                    var settingsToAdd = settingValues.Where(s => !currentSettignsByKey.ContainsKey(s.Key));
                    var settingsToDelete = this.CurrentSettings.Where(s => !newSettignsByKey.ContainsKey(s.Key));

                    // adding to SERVER/API all that are present in code and not yet in the server
                    foreach (Setting setting in settingsToAdd)
                    {
                        await this.AddSettingAsync(setting).ConfigureAwait(false);
                    }

                    // deleting from SERVER/API all that are no more present in code
                    foreach (Setting setting in settingsToDelete)
                    {
                        await this.DeleteSettingAsync(setting).ConfigureAwait(false);
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

        public void DeleteSetting(Setting setting)
        {
            this.DeleteSettingAsync(setting).Wait();
        }

        public async Task DeleteSettingAsync(Setting setting)
        {
            try
            {
                Settings settingsOperation = new Settings(this.Settings4netAPI);
                await settingsOperation.DeleteSettingAsync(setting.Id).ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                logger.Warn("Exception when deleting setting to remote settings4net API", exp);
            }
        }

    }
}
