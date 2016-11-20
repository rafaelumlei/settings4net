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
            if (setting != null)
            {
                T mappedSetting = new T();
                IStoredSetting iMappedSetting = mappedSetting as IStoredSetting;
                iMappedSetting.Id = setting.Id;
                iMappedSetting.Application = setting.Application;
                iMappedSetting.Documentation = setting.Documentation;
                iMappedSetting.Environment = setting.Environment;
                iMappedSetting.Fullpath = setting.Fullpath;
                iMappedSetting.JSONValue = JsonConvert.SerializeObject(setting.JSONValue);
                iMappedSetting.Updated = setting.Updated;
                iMappedSetting.Created = setting.Created;
                return mappedSetting;
            }
            else
                return default(T);
        }

        public static Model.Setting Map(IStoredSetting setting)
        {
            if (setting != null)
            {
                Model.Setting mappedSetting = new Model.Setting();
                mappedSetting.Id = setting.Id;
                mappedSetting.Application = setting.Application;
                mappedSetting.Documentation = setting.Documentation;
                mappedSetting.Environment = setting.Environment;
                mappedSetting.Fullpath = setting.Fullpath;
                mappedSetting.JSONValue = JToken.Parse(setting.JSONValue);
                mappedSetting.Updated = setting.Updated;
                mappedSetting.Created = setting.Created;
                return mappedSetting;
            }
            else
            {
                return null;
            }
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
