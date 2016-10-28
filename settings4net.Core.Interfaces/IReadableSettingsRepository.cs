using settings4net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Core.Interfaces
{
    public interface IReadableSettingsRepository
    {

        Setting GetSetting(string key, string env = null);

        /// <summary>
        /// Gets a setting value deserialized to specific type
        /// </summary>
        /// <typeparam name="T">The type expected has a result of the deserialization</typeparam>
        /// <param name="id">The setting identifier</param>
        /// <param name="env">The environment of setting. Eg: dev, production, ...</param>
        /// <returns>The setting value</returns>
        T GetSettingValue<T>(string key, string env = null);

        string GetSettingValue(string key, string env = null);

        IList<Setting> GetSettingsByEnv(string env);

        IList<Setting> GetSettingsByNamespace(string env);

        Setting this[string index]
        {
            get;
        }
    }
}
