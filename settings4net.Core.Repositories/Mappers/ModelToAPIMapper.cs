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
    class ModelToAPIMapper
    {
        public static APIClient.Models.Setting Map(Model.Setting setting)
        {
            return new APIClient.Models.Setting()
            {
                Application = setting.Application,
                Documentation = setting.Documentation,
                Environment = setting.Environment,
                Fullpath = setting.Fullpath,
                JSONValue = JsonConvert.SerializeObject(setting.JSONValue)
            };
        }

        public static Model.Setting Map(APIClient.Models.Setting setting)
        {
            return new Model.Setting()
            {
                Application = setting.Application,
                Documentation = setting.Documentation,
                Environment = setting.Environment,
                Fullpath = setting.Fullpath,
                JSONValue = setting.JSONValue,
            };
        }

        public static IEnumerable<APIClient.Models.Setting> Map(IEnumerable<Model.Setting> settings)
        {
            List<APIClient.Models.Setting> mapped = new List<APIClient.Models.Setting>();

            if (settings != null && settings.Any())
                foreach (var setting in settings)
                    mapped.Add(Map(setting));

            return mapped;
        }

        public static IEnumerable<Model.Setting> Map(IEnumerable<APIClient.Models.Setting> settings)
        {
            List<Model.Setting> mapped = new List<Model.Setting>();

            if (settings != null && settings.Any())
                foreach (var setting in settings)
                    mapped.Add(Map(setting));

            return mapped;
        }

    }
}
