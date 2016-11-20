using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Concurrent;
using settings4net.Core.Model;
using log4net;
using System.Configuration;

namespace settings4net.Core
{
    public class JSONSettingsRepository : ISingleAppSettingsRepository
    {
        private static ILog logger = LogManager.GetLogger(typeof(JSONSettingsRepository));

        private static readonly string SETTINGS4NET_ENV_CONF_KEY = "Settings4netCurrentEnvironment";

        private static readonly string SETTINGS_FILE_NAME = "{0}_{1}_settings4net.json";

        private string SettingsFileDirectory;

        private ConcurrentDictionary<string, Setting> CurrentSettings;

        private SemaphoreSlim rwControl = new SemaphoreSlim(1);

        private string CurrentApplication { get; set; }

        private string CurrentEnvironment { get; set; }

        public JSONSettingsRepository(string appName, string currentEnv = null)
        {
            if (string.IsNullOrEmpty(currentEnv))
            {
                currentEnv = ConfigurationManager.AppSettings[SETTINGS4NET_ENV_CONF_KEY];

                if (string.IsNullOrEmpty(currentEnv))
                    throw new ArgumentException("Settings4netCurrentEnvironment not correctly defined in ConfigurationManager.AppSettings");
            }

            this.SettingsFileDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.CurrentApplication = appName;
            this.CurrentEnvironment = currentEnv;
            var task = this.LoadSettingsFromFile();
            this.CurrentSettings = task.Result;
        }

        private async Task<ConcurrentDictionary<string, Setting>> LoadSettingsFromFile()
        {
            if (this.CurrentSettings != null)
                return this.CurrentSettings;

            string settingsFilePath = SettingsFileDirectory + string.Format(SETTINGS_FILE_NAME, this.CurrentApplication, this.CurrentEnvironment);
            if (File.Exists(settingsFilePath))
            {
                try
                {
                    string text = string.Empty;

                    using (var reader = new StreamReader(settingsFilePath))
                        text = await reader.ReadToEndAsync().ConfigureAwait(false);

                    var result = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Setting>>(text)).ConfigureAwait(false);
                    this.CurrentSettings = new ConcurrentDictionary<string, Setting>(result.ToDictionary(s => s.Key));
                    return CurrentSettings;
                }
                catch (Exception exp)
                {
                    logger.Warn("Exception getting settings from file.", exp);
                }
            }

            return this.CurrentSettings ?? new ConcurrentDictionary<string, Setting>();
        }

        public async Task<List<Setting>> GetSettingsAsync()
        {
            return this.CurrentSettings.Values.ToList();
        }

        public List<Setting> GetSettings()
        {
            var task = this.GetSettingsAsync();
            return task.Result;
        }

        private async Task OverrideStateAsyncImpl(List<Setting> values, bool lockAlreadyHeld = false)
        {
            if (!lockAlreadyHeld)
                await this.rwControl.WaitAsync().ConfigureAwait(false);

            try
            {
                try
                {
                    string settingsFilePath = SettingsFileDirectory + string.Format(SETTINGS_FILE_NAME, this.CurrentApplication, this.CurrentEnvironment);
                    string settingsJsonText = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(values, Formatting.Indented)).ConfigureAwait(false);

                    using (var writer = new StreamWriter(settingsFilePath))
                        await writer.WriteAsync(settingsJsonText).ConfigureAwait(false);
                }
                catch (Exception exp)
                {
                    logger.Warn("Exception writing new settings state to JSON file.", exp);
                }
            }
            finally
            {
                // only releases the lock if it was acquired in this method
                if (!lockAlreadyHeld)
                    this.rwControl.Release();
            }
        }

        public async Task OverrideStateAsync(List<Setting> values)
        {
            await this.OverrideStateAsyncImpl(values, false).ConfigureAwait(false);
        }

        public void OverrideState(List<Setting> values)
        {
            this.OverrideStateAsync(values).Wait();
        }

        public async Task UpdateSettingAsync(Setting value)
        {
            await rwControl.WaitAsync().ConfigureAwait(false);
            try
            {
                if (CurrentSettings.ContainsKey(value.Key))
                {
                    CurrentSettings[value.Key] = value;
                    await this.OverrideStateAsyncImpl(CurrentSettings.Values.ToList(), true).ConfigureAwait(false);
                }
            }
            catch (Exception exp)
            {
                logger.Warn("Error when updating setting.", exp);
            }
            finally
            {
                rwControl.Release();
            }
        }

        public void UpdateSetting(Setting value)
        {
            this.UpdateSettingAsync(value).Wait();
        }

        public async Task UpdateSettingsAsync(List<Setting> values)
        {
            await this.rwControl.WaitAsync().ConfigureAwait(false);
            try
            {
                bool anyUpdated = false;
                foreach (Setting value in values)
                {
                    if (CurrentSettings.ContainsKey(value.Key))
                    {
                        CurrentSettings[value.Key] = value;
                        anyUpdated = true;
                    }
                }

                // only serializes to file if at least one setting was updated
                if (anyUpdated)
                    await this.OverrideStateAsyncImpl(CurrentSettings.Values.ToList(), true).ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                logger.Warn("Exception when updating all settings and serializing them to file", exp);
            }
            finally
            {
                this.rwControl.Release();
            }
        }

        public void UpdateSettings(List<Setting> values)
        {
            this.UpdateSettingsAsync(values).Wait();
        }

        public void AddSetting(Setting setting)
        {
            throw new NotImplementedException();
        }

        public Task AddSettingAsync(Setting setting)
        {
            throw new NotImplementedException();
        }

        public void DeleteSetting(string fullpath)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSettingAsync(string fullpath)
        {
            throw new NotImplementedException();
        }

    }
}
