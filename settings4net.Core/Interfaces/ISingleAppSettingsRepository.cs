using settings4net.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Core.Interfaces
{

    /// <summary>
    /// Interface to use when the application is available in the 
    /// current context (eg. CurrentAppDomain) and because of that the 
    /// repository will only deal with settings for a specific application.
    /// Tipically, this repository will be the most common for Code/File repository.
    /// </summary>
    public interface ISingleAppSettingsRepository
    {

        /// <summary>
        /// Adds a new setting to the settings' repository 
        /// </summary>
        /// <param name="values">List of setting's values</param>
        void AddSetting(Setting setting);

        Task AddSettingAsync(Setting setting);

        /// <summary>
        /// Gets all the settings available in the repository
        /// </summary>
        /// <param name="values">List of setting's values</param>
        List<Setting> GetSettings();

        Task<List<Setting>> GetSettingsAsync();

        /// <summary>
        /// Updates settings' values (if they don't exist they are not added)
        /// </summary>
        /// <param name="values">List of setting's values</param>
        void UpdateSettings(List<Setting> values);

        Task UpdateSettingsAsync(List<Setting> values);

        /// <summary>
        /// Updates setting's value (if it doesn't exist it is not added)
        /// </summary>
        /// <param name="values">List of setting's values</param>
        void UpdateSetting(Setting value);

        Task UpdateSettingAsync(Setting value);

        /// <summary>
        /// Deletes setting from the respository
        /// </summary>
        /// <param name="fullpath">The setting fullpath</param>
        void DeleteSetting(string fullpath);

        Task DeleteSettingAsync(string fullpath);

        /// <summary>
        /// Overrides the current repository with the new repository's values/state
        /// * the ones that don't exist in this list are removed from the repository;
        /// * the ones that exist are updated with the new values;
        /// </summary>
        /// <param name="values">List of setting's values</param>
        void OverrideState(List<Setting> values);

        Task OverrideStateAsync(List<Setting> values);
    }
}
