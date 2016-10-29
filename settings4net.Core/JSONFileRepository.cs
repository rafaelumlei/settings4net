using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Model;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Concurrent;

namespace settings4net.Core
{
    public class JSONSettingsRepository : ISettingsRepository
    {
        private static readonly string SETTINGS_FILE_NAME = "{0}_{1}_settings4net.json";

        private static readonly string APPLICATION_NAME;

        private static string SettingsFileDirectory;

        private ConcurrentDictionary<string, Setting> CurrentSettings { get; set; }

        private ReaderWriterLockSlim rwlControl = new ReaderWriterLockSlim();

        static JSONSettingsRepository()
        {
            string appname = AppDomain.CurrentDomain.FriendlyName;
            SettingsFileDirectory = AppDomain.CurrentDomain.BaseDirectory;
            APPLICATION_NAME = appname;
        }

        public List<Setting> GetSettings(string currentEnvironment)
        {
            rwlControl.EnterUpgradeableReadLock();
            try
            {
                if (CurrentSettings != null)
                    return CurrentSettings.Values.ToList();

                rwlControl.EnterWriteLock();
                try
                {
                    string settingsFilePath = SettingsFileDirectory + string.Format(SETTINGS_FILE_NAME, APPLICATION_NAME, currentEnvironment);
                    if (File.Exists(settingsFilePath))
                    {
                        try
                        {
                            string text = File.ReadAllText(settingsFilePath);
                            return JsonConvert.DeserializeObject<List<Setting>>(text);
                        }
                        catch { }
                    }
                }
                finally
                {
                    rwlControl.ExitWriteLock();
                }
            }
            finally
            {
                rwlControl.ExitUpgradeableReadLock();
            }

            return new List<Setting>();
        }

        public void OverrideState(string currentEnvironment, List<Setting> values)
        {
            if (rwlControl.IsWriteLockHeld)
                rwlControl.EnterWriteLock();

            try
            {
                try
                {
                    string settingsFilePath = SettingsFileDirectory + string.Format(SETTINGS_FILE_NAME, APPLICATION_NAME, currentEnvironment);
                    string settingsJsonText = JsonConvert.SerializeObject(values, Formatting.Indented);
                    File.WriteAllText(settingsFilePath, settingsJsonText);
                }
                catch { }
            }
            finally
            {
                rwlControl.ExitWriteLock();
            }
        }

        public void UpdateSetting(string currentEnvironment, Setting value)
        {
            rwlControl.EnterWriteLock();
            try
            {
                Setting oldValue = null;
                if (this.CurrentSettings.TryGetValue(value.Key, out oldValue))
                    if (this.CurrentSettings.TryUpdate(value.Key, value, oldValue))
                        this.OverrideState(currentEnvironment, this.CurrentSettings.Values.ToList());
            }
            catch
            {

            }
            finally
            {
                if (rwlControl.IsWriteLockHeld)
                    rwlControl.ExitWriteLock();
            }
        }

        public void UpdateSettings(string currentEnvironment, List<Setting> values)
        {
            rwlControl.EnterWriteLock();
            try
            {
                bool anyUpdated = false;
                foreach (Setting value in values)
                {
                    Setting oldValue = null;
                    if (this.CurrentSettings.TryGetValue(value.Key, out oldValue))
                        if (this.CurrentSettings.TryUpdate(value.Key, value, oldValue))
                            anyUpdated = true;
                }

                // only serializes to file if at least one setting was updated
                if (anyUpdated)
                    this.OverrideState(currentEnvironment, this.CurrentSettings.Values.ToList());
            }
            catch
            {

            }
            finally
            {
                if (rwlControl.IsWriteLockHeld)
                    rwlControl.ExitWriteLock();
            }
        }
    }
}
