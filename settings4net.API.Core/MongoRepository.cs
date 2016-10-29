//using settings4net.Core.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using settings4net.Model;

//namespace settings4net.Core
//{
//    class MongoRepository : IReadableSettingsRepository
//    {

//        private string CurrentEnv { get; set; }

//        private Uri RemoteApi { get; set; }

//        public Setting this[string index]
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }

//        /// <summary>
//        /// Constructor with the current environment  (dev, qa, prd, ...)
//        /// </summary>
//        /// <param name="env">Optional, if not provided appsetting ("settings4net.currentenv") is used</param>
//        /// <param name="remoteSettingsAPIUri">Optional, if not provided appsetting ("settings4net.remotesettingsapi") is used</param>
//        public MongoRepository(string env = null, string remoteSettingsAPIUri = null)
//        {
//            //if (!string.IsNullOrEmpty(env))
//            //    this.DetaultEnv = env;
//            //else
//            //    this.DetaultEnv = System.Configuration.ConfigurationManager.AppSettings[DetaultEnvAppKey];

//            //if (!string.IsNullOrEmpty(remoteSettingsAPIUri))
//            //{
//            //    this.RemoteApi = new Uri(remoteSettingsAPIUri);
//            //}
//            //else
//            //{
//            //    string remoteAPIValue = System.Configuration.ConfigurationManager.AppSettings[RemoteApiAppKey];
//            //    this.RemoteApi = new Uri(remoteAPIValue);
//            //}
//        }

//        public Model.Setting GetSetting(string key, string env = null)
//        {
//            throw new NotImplementedException();
//        }

//        public IList<Model.Setting> GetSettingsByEnv(string env)
//        {
//            throw new NotImplementedException();
//        }

//        public IList<Model.Setting> GetSettingsByNamespace(string env)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetSettingValue(string key, string env = null)
//        {
//            throw new NotImplementedException();
//        }

//        public T GetSettingValue<T>(string key, string env = null)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
