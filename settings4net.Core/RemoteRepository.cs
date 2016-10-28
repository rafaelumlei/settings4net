using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Model;

namespace settings4net.Core
{

    /// <summary>
    /// Class responsible for the remote settings retrieval and synchronization process
    /// </summary>
    public class RemoteRepository : IReadableSettingsRepository
    {
        private static readonly string CurrentEnvAppKey = "settings4net.currentenv";

        private static readonly string RemoteApiAppKey = "settings4net.remotesettingsapi";

        //static readonly string DetaultEnvAppKey { get; set; }

        private string DetaultEnv { get; set; }

        private Uri RemoteApi { get; set; }

        public Setting this[string index]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Constructor with the current environment  (dev, qa, prd, ...)
        /// </summary>
        /// <param name="currentEnv">Optional, if not provided appsetting ("settings4net.currentenv") is used</param>
        /// <param name="remoteSettingsAPIUri">Optional, if not provided appsetting ("settings4net.remotesettingsapi") is used</param>
        public RemoteRepository(string currentEnv = null, string remoteSettingsAPIUri = null)
        {
            if (!string.IsNullOrEmpty(currentEnv))
                this.DetaultEnv = currentEnv;
            else
                this.DetaultEnv = System.Configuration.ConfigurationManager.AppSettings[CurrentEnvAppKey];

            if (!string.IsNullOrEmpty(remoteSettingsAPIUri))
            {
                this.RemoteApi = new Uri(remoteSettingsAPIUri);
            }
            else
            {
                string remoteAPIValue = System.Configuration.ConfigurationManager.AppSettings[RemoteApiAppKey];
                this.RemoteApi = new Uri(remoteAPIValue);
            }
        }

        public Model.Setting GetSetting(string id, string env = null)
        {
            throw new NotImplementedException();
        }

        public IList<Model.Setting> GetSettingsByEnv(string env)
        {
            throw new NotImplementedException();
        }

        public IList<Model.Setting> GetSettingsByNamespace(string env)
        {
            throw new NotImplementedException();
        }

        public string GetSettingValue(string id, string env = null)
        {
            throw new NotImplementedException();
        }

        public T GetSettingValue<T>(string id, string env = null)
        {
            throw new NotImplementedException();
        }
    }
}
