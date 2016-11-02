using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Core.Model;
using settings4net.Core.RemoteRepositories.APIClient;
using settings4net.Core.RemoteRepositories.Mappers;
using log4net;

namespace settings4net.Core.Repositories
{
    public class ApiSettingsRepository : ISingleAppSettingsRepository
    {

        private static ILog logger = LogManager.GetLogger(typeof(ApiSettingsRepository));

        private readonly object SettingsLoadingCtrl = new object();

        private Settings4netAPI Settings4netAPI { get; set; }

        private Dictionary<string, Setting> CurrentSettings { get; set; }

        private string CurrentApplication { get; set; }

        public ApiSettingsRepository(string baseUri)
        {
            this.Settings4netAPI = new Settings4netAPI(new Uri(baseUri));
            this.CurrentApplication = AppDomain.CurrentDomain.FriendlyName;
        }

        private void LoadRemoteSettings(string currentEnvironment)
        {
            lock (SettingsLoadingCtrl)
            {
                if (CurrentSettings == null)
                {
                    try
                    {
                        Settings settingsOp = new Settings(this.Settings4netAPI);
                        IList<RemoteRepositories.APIClient.Models.Setting> remoteSettings = settingsOp.GetSettings(this.CurrentApplication, currentEnvironment);
                        List<Setting> currentSettings = ModelToAPIMapper.Map(remoteSettings).ToList();
                        this.CurrentSettings = currentSettings.ToDictionary(s => s.Key);
                    }
                    catch (Exception exp)
                    {
                        logger.Warn("Exception when loading settings from remote settings4net api", exp);
                    }
                }
            }
        }

        public void AddSetting(string currentEnvironment, Setting setting)
        {
            try
            {
                Settings settingsOperation = new Settings(this.Settings4netAPI);
                RemoteRepositories.APIClient.Models.Setting remoteSetting = ModelToAPIMapper.Map(setting);
                settingsOperation.AddSetting(this.CurrentApplication, currentEnvironment, remoteSetting);
            }
            catch (Exception exp)
            {
                logger.Warn("Exception when adding setting to remote settings4net API", exp);
            }
        }

        public List<Setting> GetSettings(string currentEnvironment)
        {
            this.LoadRemoteSettings(currentEnvironment);
            return this.CurrentSettings.Values.ToList();
        }

        public void OverrideState(string currentEnvironment, List<Setting> settingValues)
        {
            if (settingValues != null && settingValues.Any())
            {
                if (CurrentSettings == null)
                    LoadRemoteSettings(currentEnvironment);

                foreach (Setting setting in settingValues)
                    if (!CurrentSettings.ContainsKey(setting.Key))
                        this.AddSetting(currentEnvironment, setting);
            }
        }

        public void UpdateSetting(string currentEnvironment, Setting value)
        {
            // not required for now; this class is a settings master, it is not updated, 
            // only new settings are added (the developer adds the settings to code and 
            // they are sent to the setting4net server/api in the first initialize if the 
            // connection is valid
            throw new NotImplementedException();
        }

        public void UpdateSettings(string currentEnvironment, List<Setting> values)
        {
            // not required for now; this class is a settings master, it is not updated, 
            // only new settings are added (the developer adds the settings to code and 
            // they are sent to the setting4net server/api in the first initialize if the 
            // connection is valid
            throw new NotImplementedException();
        }
    }
}
