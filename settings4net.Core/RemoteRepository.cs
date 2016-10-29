using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Model;

namespace settings4net.Core
{

    public class RemoteRepository : ISettingsRepository
    {
        public List<Setting> GetSettings(string currentEnvironment)
        {
            throw new NotImplementedException();
        }

        public void OverrideState(string currentEnvironment, List<Setting> values)
        {
            throw new NotImplementedException();
        }

        public void UpdateSetting(string currentEnvironment, Setting value)
        {
            throw new NotImplementedException();
        }

        public void UpdateSettings(string currentEnvironment, List<Setting> values)
        {
            throw new NotImplementedException();
        }
    }
}
