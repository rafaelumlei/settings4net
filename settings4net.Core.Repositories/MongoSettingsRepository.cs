using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Core.Model;

namespace settings4net.Core.Repositories
{
    class MongoSettingsRepository : IMultiAppSettingsRepository
    {
        public void AddSetting(string application, string currentEnvironment, Setting setting)
        {
            throw new NotImplementedException();
        }

        public List<Setting> GetSettings(string application, string currentEnvironment)
        {
            throw new NotImplementedException();
        }

        public void OverrideState(string application, string currentEnvironment, List<Setting> values)
        {
            throw new NotImplementedException();
        }

        public void UpdateSetting(string application, string currentEnvironment, Setting value)
        {
            throw new NotImplementedException();
        }

        public void UpdateSettings(string application, string currentEnvironment, List<Setting> values)
        {
            throw new NotImplementedException();
        }
    }
}
