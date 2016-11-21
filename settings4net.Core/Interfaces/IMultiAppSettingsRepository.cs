using settings4net.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Core.Interfaces
{

    /// <summary>
    /// Interface to use when the application is not available 
    /// in the current context (eg. CurrentAppDomain) and the 
    /// application name must be passed has a parameter. 
    /// When a remote server deals with settings requests for 
    /// several applications it needs to specify the app in every 
    /// operation within the repository.
    /// </summary>
    public interface IMultiAppSettingsRepository
    {
        /// <summary>
        /// Adds a new setting to the settings' repository 
        /// </summary>
        /// <param name="setting">The setting to add</param>
        void AddSetting(Setting setting);

        Task AddSettingAsync(Setting setting);

        /// <summary>
        /// Gets a setting by id
        /// </summary>
        /// <param name="id">The setting identifier</param>
        Setting GetSetting(string id = null);

        Task<Setting> GetSettingAsync(string id);

        /// <summary>
        /// Gets all the settings available in the repository for the app and env specified 
        /// </summary>
        /// <param name="application">The application owner of the settings</param>
        /// <param name="currentEnvironment">The environment in use (DEV, QA, ...)</param>
        /// <returns>The settings for the specified application/env</returns>
        List<Setting> GetSettings(string application = null, string currentEnvironment = null);

        Task<List<Setting>> GetSettingsAsync(string application = null, string currentEnvironment = null);

        /// <summary>
        /// Updates setting's value (if it doesn't exist it is not added)
        /// </summary>
        /// <param name="id">The setting identifier</param>
        /// <param name="value">List of setting's values</param>
        void UpdateSetting(string id, Setting value);

        Task UpdateSettingAsync(string id, Setting value);

        /// <summary>
        /// Delete setting from the repository
        /// </summary>
        /// <param name="id">The setting identifier</param>
        void DeleteSetting(string id);

        Task DeleteSettingAsync(string id);

        /// <summary>
        /// Gets all available apps for which there are settings in the repository
        /// </summary>
        /// <returns>List of all available apps</returns>
        List<string> GetApps();

        Task<List<string>> GetAppsAsync();

        /// <summary>
        /// Gets all available environments for a specific app
        /// </summary>
        /// <returns>List of all available environments for the specified app</returns>
        List<string> GetAppEnvironments(string app);

        Task<List<string>> GetAppEnvironmentsAsync(string app);

    }
}
