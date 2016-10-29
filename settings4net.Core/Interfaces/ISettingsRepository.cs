using settings4net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Core.Interfaces
{
    public interface ISettingsRepository
    {
        List<Setting> GetSettings(string currentEnvironment);

        /// <summary>
        /// Updates settings' values (if they don't exist they are not added)
        /// </summary>
        /// <param name="currentEnvironment">The environment in use (DEV, QA, ...)</param>
        /// <param name="values">List of setting's values</param>
        void UpdateSettings(string currentEnvironment, List<Setting> values);

        /// <summary>
        /// Updates setting's value (if it doesn't exist it is not added)
        /// </summary>
        /// <param name="currentEnvironment">The environment in use (DEV, QA, ...)</param>
        /// <param name="values">List of setting's values</param>
        void UpdateSetting(string currentEnvironment, Setting value);

        /// <summary>
        /// Overrides the current repository with the new repository's values/state
        /// * the ones that don't exist in this list are removed from the repository;
        /// * the ones that exist are updated with the new values;
        /// </summary>
        /// <param name="currentEnvironment">The environment in use (DEV, QA, ...)</param>
        /// <param name="values">List of setting's values</param>
        void OverrideState(string currentEnvironment, List<Setting> values);
    }
}
