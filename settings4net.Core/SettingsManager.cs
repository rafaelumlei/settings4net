using Newtonsoft.Json;
using settings4net.Core.Model;
using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace settings4net.Core
{
    public class SettingsManager
    {

        public static void InitializeSettings4net(string currentEnvironment, bool remote = true)
        {
            InitializeSettings4net(currentEnvironment, new CodeSettingsRepository(), new JSONSettingsRepository());
        }

        public static void InitializeSettings4net(string currentEnvironment, params ISingleAppSettingsRepository[] settingRepositoriesChain)
        {
            List<Tuple<ISingleAppSettingsRepository, List<Setting>>> activeSettingsReporitory = new List<Tuple<ISingleAppSettingsRepository, List<Setting>>>();

            settingRepositoriesChain.ToList().ForEach(repository =>
            {
                // some settings repositories may be offline, we have to check they are accessible or not
                try
                {
                    var repoSettings = new Tuple<ISingleAppSettingsRepository, List<Setting>>(repository, repository.GetSettings(currentEnvironment));
                    activeSettingsReporitory.Add(repoSettings);
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.ToString());
                }
            });

            // if more than one repository available
            int activeReposCount = activeSettingsReporitory.Count() - 1;
            if (activeReposCount >= 1)
            {
                // lowest relevant: tipically the default setttings that come from the code (dev environment) - has more settings, less priority
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

                lowestPriority.Item1.UpdateSettings(currentEnvironment, highestPrioritySettings.Values.ToList());
                List<Setting> currentSettings = lowestPriority.Item1.GetSettings(currentEnvironment);

                // from lowest to highest priority, setting all equal to the lowest priority one
                for (int i = 1; i <= activeReposCount; i++)
                {
                    activeSettingsReporitory.ElementAt(i).Item1.OverrideState(currentEnvironment, currentSettings);
                }
            }
        }

    }
}
