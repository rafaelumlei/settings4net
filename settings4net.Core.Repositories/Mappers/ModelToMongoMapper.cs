using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Core.RemoteRepositories;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace settings4net.Core.RemoteRepositories.Mappers
{
    class ModelToMongoMapper
    {
        public static Models.SettingMongo Map(Model.Setting setting)
        {
            return new Models.SettingMongo()
            {
                Key = setting.Key,
                Application = setting.Application,
                Documentation = setting.Documentation,
                Environment = setting.Environment,
                Fullpath = setting.Fullpath,
                JSONValue = setting.JSONValue.ToString()
            };
        }

        public static Model.Setting Map(Models.SettingMongo setting)
        {
            return new Model.Setting()  
            {
                Application = setting.Application,
                Documentation = setting.Documentation,
                Environment = setting.Environment,
                Fullpath = setting.Fullpath,
                JSONValue = JToken.Parse(setting.JSONValue)
            };
        }

        public static IEnumerable<Models.SettingMongo> Map(IEnumerable<Model.Setting> settings)
        {
            List<Models.SettingMongo> mapped = new List<Models.SettingMongo>();

            if (settings != null && settings.Any())
                foreach (var setting in settings)
                    mapped.Add(Map(setting));

            return mapped;
        }

        public static IEnumerable<Model.Setting> Map(IEnumerable<Models.SettingMongo> settings)
        {
            List<Model.Setting> mapped = new List<Model.Setting>();

            if (settings != null && settings.Any())
                foreach (var setting in settings)
                    mapped.Add(Map(setting));

            return mapped;
        }

    }
}
