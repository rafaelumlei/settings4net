﻿using settings4net.Core.Model;
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
        /// <param name="application">The application owner of the settings</param>
        /// <param name="currentEnvironment">The environment in use (DEV, QA, ...)</param>
        /// <param name="values">List of setting's values</param>
        void AddSetting(string application, string currentEnvironment, Setting setting);

        /// <summary>
        /// Gets all the settings available in the repository
        /// </summary>
        /// <param name="application">The application owner of the settings</param>
        /// <param name="currentEnvironment">The environment in use (DEV, QA, ...)</param>
        /// <param name="values">List of setting's values</param>
        List<Setting> GetSettings(string application, string currentEnvironment);

        /// <summary>
        /// Updates settings' values (if they don't exist they are not added)
        /// </summary>
        /// <param name="application">The application owner of the settings</param>
        /// <param name="currentEnvironment">The environment in use (DEV, QA, ...)</param>
        /// <param name="values">List of setting's values</param>
        void UpdateSettings(string application, string currentEnvironment, List<Setting> values);

        /// <summary>
        /// Updates setting's value (if it doesn't exist it is not added)
        /// </summary>
        /// <param name="application">The application owner of the settings</param>
        /// <param name="currentEnvironment">The environment in use (DEV, QA, ...)</param>
        /// <param name="values">List of setting's values</param>
        void UpdateSetting(string application, string currentEnvironment, Setting value);

        /// <summary>
        /// Overrides the current repository with the new repository's values/state
        /// * the ones that don't exist in this list are removed from the repository;
        /// * the ones that exist are updated with the new values;
        /// </summary>
        /// <param name="application">The application owner of the settings</param>
        /// <param name="currentEnvironment">The environment in use (DEV, QA, ...)</param>
        /// <param name="values">List of setting's values</param>
        void OverrideState(string application, string currentEnvironment, List<Setting> values);
    }
}
