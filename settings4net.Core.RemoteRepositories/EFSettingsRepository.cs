using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Core.Model;
using settings4net.Core.RemoteRepositories.EF;
using System.Data.Entity;
using settings4net.Core.RemoteRepositories.Models;
using settings4net.Core.RemoteRepositories.Mappers;
using log4net;

namespace settings4net.Core.RemoteRepositories
{
    public class EFSettingsRepository : IMultiAppSettingsRepository
    {
        private static ILog logger = LogManager.GetLogger(typeof(EFSettingsRepository));

        private string ConnectionString { get; set; }

        public EFSettingsRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public void AddSetting(string application, string currentEnvironment, Setting setting)
        {
            this.AddSettingAsync(application, currentEnvironment, setting).Wait();
        }

        public async Task AddSettingAsync(string application, string currentEnvironment, Setting setting)
        {
            using (var context = new SettingsContext(this.ConnectionString))
            {
                SettingEF settingToAdd = StoredSettingMapper.Map<SettingEF>(setting);
                settingToAdd.Created = settingToAdd.Updated = DateTimeOffset.UtcNow;
                context.Settings.Add(settingToAdd);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public void DeleteSetting(string application, string currentEnvironment, string fullpath)
        {
            this.DeleteSettingAsync(application, currentEnvironment, fullpath).Wait();
        }

        public async Task DeleteSettingAsync(string application, string currentEnvironment, string fullpath)
        {
            using (var context = new SettingsContext(this.ConnectionString))
            {
                SettingEF settingToDelete = await context.Settings.Where(s => s.Application == application && s.Environment == currentEnvironment && s.Fullpath == fullpath)
                                                                  .FirstOrDefaultAsync()
                                                                  .ConfigureAwait(false);

                if (settingToDelete != null)
                {
                    context.Settings.Remove(settingToDelete);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public List<Setting> GetSettings(string application, string currentEnvironment)
        {
            return this.GetSettingsAsync(application, currentEnvironment).Result;
        }

        public async Task<List<Setting>> GetSettingsAsync(string application, string currentEnvironment)
        {
            using (var context = new SettingsContext(this.ConnectionString))
            {
                List<SettingEF> results = await context.Settings.Where(s => s.Application == application && s.Environment == currentEnvironment)
                                                                .ToListAsync()
                                                                .ConfigureAwait(false);
                return StoredSettingMapper.Map(results).ToList();
            }
        }

        public void UpdateSetting(string application, string currentEnvironment, Setting value)
        {
            this.UpdateSettingAsync(application, currentEnvironment, value).Wait();
        }

        public async Task UpdateSettingAsync(string application, string currentEnvironment, Setting value)
        {
            using (var context = new SettingsContext(this.ConnectionString))
            {
                SettingEF settingMapped = StoredSettingMapper.Map<SettingEF>(value);
                SettingEF settingEF = await context.Settings.Where(s => s.Key == settingMapped.Key).FirstOrDefaultAsync().ConfigureAwait(false);

                if (settingEF != null)
                {
                    settingEF.JSONValue = settingMapped.JSONValue;
                    settingEF.Documentation = settingMapped.Documentation;
                    settingEF.Updated = DateTimeOffset.UtcNow;
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }

        public void UpdateSettings(string application, string currentEnvironment, List<Setting> values)
        {
            this.UpdateSettingsAsync(application, currentEnvironment, values).Wait();
        }

        public async Task UpdateSettingsAsync(string application, string currentEnvironment, List<Setting> settings)
        {
            List<Task> tasks = new List<Task>();

            if (settings != null && settings.Any())
            {
                foreach (var setting in settings)
                {
                    tasks.Add(this.UpdateSettingAsync(application, currentEnvironment, setting));
                }
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
