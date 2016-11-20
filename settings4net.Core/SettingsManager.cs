using Newtonsoft.Json;
using settings4net.Core.Model;
using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using System.Threading.Tasks;
using settings4net.Core.Repositories;
using System.Diagnostics;
using System.Reflection;

namespace settings4net.Core
{
    public class SettingsManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(SettingsManager));


        ///// <summary>
        ///// Gets the Title of the caller assembly by inspecting a call in a specific 
        ///// and provided position in the stack.
        ///// </summary>
        /// <param name="stackPosition">Position to inspect to get the caller assembly type</param>
        /// <returns>The caller assembly's title</returns>
        private static string GetCallerAssemblyTitle(int stackPosition)
        {
            StackFrame frame = new StackFrame(stackPosition);
            MethodBase callerMethod = frame.GetMethod();
            Type declaringType = callerMethod.DeclaringType;
            Assembly hostAssembly = Assembly.GetAssembly(declaringType);
            var assemblyAttr = hostAssembly.GetCustomAttribute(typeof(AssemblyTitleAttribute));

            if (assemblyAttr == null || !(assemblyAttr is AssemblyTitleAttribute) || string.IsNullOrEmpty((assemblyAttr as AssemblyTitleAttribute).Title))
                throw new ArgumentNullException("Impossible to get caller assembly title attribute. Is [assembly: AssemblyTitle(\"...\")] in AssembleInfo?");
            else
                return (assemblyAttr as AssemblyTitleAttribute).Title;
        }

        /// <summary>
        /// Initializes all the setting classes present in the domain, local config files 
        /// are created if they do not exist. Remote repository is optional.
        /// </summary>
        /// <param name="withRemote">Specifies if the settings are to synchronize with a remote api</param>
        /// <param name="appName">The application name, if not provided the Title of the assembly will be used</param>
        public static void InitializeSettings4net(bool withRemote = true, string appName = null)
        {
            if (string.IsNullOrEmpty(appName))
                appName = GetCallerAssemblyTitle(2);

            InitializeSettings4netAsync(withRemote, appName: appName).Wait();
        }

        /// <summary>
        /// Initializes all the setting classes present in the domain, local config files 
        /// are created if they do not exist. Remote repository is optional.
        /// </summary>
        /// <param name="withRemote">Specifies if the settings are to synchronize with a remote api</param>
        /// <param name="appName">The application name, if not provided the Title of the assembly will be used</param>
        public static async Task InitializeSettings4netAsync(bool withRemote = true, string appName = null)
        {
            if (string.IsNullOrEmpty(appName))
                appName = GetCallerAssemblyTitle(2);

            if (withRemote)
                await InitializeSettings4netAsync(new CodeSettingsRepository(appName), new JSONSettingsRepository(appName), new ApiSettingsRepository(appName)).ConfigureAwait(false);
            else
                await InitializeSettings4netAsync(new CodeSettingsRepository(appName), new JSONSettingsRepository(appName)).ConfigureAwait(false);
        }

        public static async Task InitializeSettings4netAsync(params ISingleAppSettingsRepository[] settingRepositoriesChain)
        {
            List<Tuple<ISingleAppSettingsRepository, List<Setting>>> activeSettingsReporitory = new List<Tuple<ISingleAppSettingsRepository, List<Setting>>>();

            foreach (var repository in settingRepositoriesChain)
            {
                // some settings repositories may be offline, we have to check if they are accessible or not
                try
                {
                    var repoSettings = new Tuple<ISingleAppSettingsRepository, List<Setting>>(repository, await repository.GetSettingsAsync().ConfigureAwait(false));
                    activeSettingsReporitory.Add(repoSettings);
                }
                catch (Exception exp)
                {
                    logger.Error(string.Format("Error when loading settings from repository {0}", repository.ToString()), exp);
                }
            }

            // if more than one settings repository available
            int activeReposCount = activeSettingsReporitory.Count() - 1;
            if (activeReposCount >= 1)
            {
                // lowest relevant: tipically the default setttings that come from the code (dev environment) - has all settings, but the values have less priority
                Tuple<ISingleAppSettingsRepository, List<Setting>> lowestPriority = activeSettingsReporitory.ElementAt(0);

                // highest relevant: tipically the setttings that come from DB or remote repositories (dev config overrides) - has less settings, more priority
                Dictionary<string, Setting> highestPrioritySettings = activeSettingsReporitory.ElementAt(activeReposCount).Item2.ToDictionary(s => s.Key);

                // from highest priority to lowest, updating
                for (int i = activeReposCount; i != 0; i--)
                {
                    // if there are settings that were not configured in a more priority repo, this one is the master
                    activeSettingsReporitory.ElementAt(i).Item2.ForEach(s =>
                    {
                        if (!highestPrioritySettings.ContainsKey(s.Key))
                            highestPrioritySettings.Add(s.Key, s);
                    });
                }

                lowestPriority.Item1.UpdateSettings(highestPrioritySettings.Values.ToList());
                List<Setting> currentSettings = await lowestPriority.Item1.GetSettingsAsync().ConfigureAwait(false);

                // from lowest to highest priority, setting all equal to the lowest priority one
                for (int i = 1; i <= activeReposCount; i++)
                {
                    await activeSettingsReporitory.ElementAt(i).Item1.OverrideStateAsync(currentSettings).ConfigureAwait(false);
                }
            }
        }

        public static void InitializeSettings4net(params ISingleAppSettingsRepository[] settingRepositoriesChain)
        {
            InitializeSettings4netAsync(settingRepositoriesChain).Wait();
        }

    }
}
