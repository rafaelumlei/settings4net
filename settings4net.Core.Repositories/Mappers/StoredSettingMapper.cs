using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Core.RemoteRepositories;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using settings4net.Core.RemoteRepositories.Models;

namespace settings4net.Core.RemoteRepositories.Mappers
{
    class StoredSettingMapper
    {

        public static T Map<T>(Model.Setting setting) where T : IStoredSetting, new()
        {
            T mappedSetting = new T();

            IStoredSetting iMappedSetting = mappedSetting as IStoredSetting;
            iMappedSetting.Key = setting.Key;
            iMappedSetting.Application = setting.Application;
            iMappedSetting.Documentation = setting.Documentation;
            iMappedSetting.Environment = setting.Environment;
            iMappedSetting.Fullpath = setting.Fullpath;
            iMappedSetting.JSONValue = setting.JSONValue.ToString();
            iMappedSetting.Updated = setting.Updated;
            iMappedSetting.Created = setting.Created;

            return mappedSetting;
        }

        public static Model.Setting Map(IStoredSetting setting)
        {
            return new Model.Setting()  
            {
                Application = setting.Application,
                Documentation = setting.Documentation,
                Environment = setting.Environment,
                Fullpath = setting.Fullpath,
                JSONValue = JToken.Parse(setting.JSONValue),
                Updated = setting.Updated,
                Created = setting.Created
            };
        }

        public static IEnumerable<T> Map<T>(IEnumerable<Model.Setting> settings) where T : IStoredSetting, new()
        {
            List<T> mapped = new List<T>();

            if (settings != null && settings.Any())
                foreach (var setting in settings)
                    mapped.Add(Map<T>(setting));

            return mapped;
        }

        public static IEnumerable<Model.Setting> Map(IEnumerable<IStoredSetting> settings)
        {
            List<Model.Setting> mapped = new List<Model.Setting>();

            if (settings != null && settings.Any())
                foreach (var setting in settings)
                    mapped.Add(Map(setting));

            return mapped;
        }

    }
}
