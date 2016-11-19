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
using System.Linq.Expressions;

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
            try
            {
                using (var context = new SettingsContext(this.ConnectionString))
                {
                    SettingEF settingToAdd = StoredSettingMapper.Map<SettingEF>(setting);
                    settingToAdd.Created = settingToAdd.Updated = DateTimeOffset.UtcNow;
                    context.Settings.Add(settingToAdd);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error adding setting {0} to EF", Newtonsoft.Json.JsonConvert.SerializeObject(setting)), exp);
                throw;
            }
        }

        public void DeleteSetting(string id)
        {
            this.DeleteSettingAsync(id).Wait();
        }

        public async Task DeleteSettingAsync(string id)
        {
            try
            {
                using (var context = new SettingsContext(this.ConnectionString))
                {
                    long dbId;
                    if (long.TryParse(id, out dbId))
                    {
                        SettingEF settingToDelete = await context.Settings.Where(s => s.DbId == dbId)
                                                                          .FirstOrDefaultAsync()
                                                                          .ConfigureAwait(false);

                        if (settingToDelete != null)
                        {
                            context.Settings.Remove(settingToDelete);
                            await context.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error while deleting setting with id {0} from EF", id), exp);
                throw;
            }
}

        public List<Setting> GetSettings(string application = null, string currentEnvironment = null)
        {
            return this.GetSettingsAsync(application, currentEnvironment).Result;
        }

        public async Task<List<Setting>> GetSettingsAsync(string application = null, string currentEnvironment = null)
        {
            try
            {
                Expression<Func<SettingEF, bool>> filter = s => true;

                if (!string.IsNullOrEmpty(application))
                {
                    if (string.IsNullOrEmpty(currentEnvironment))
                        filter = s => s.Application == application;
                    else 
                        filter = s => s.Application == application && s.Environment == currentEnvironment;
                }

                using (var context = new SettingsContext(this.ConnectionString))
                {
                    List<SettingEF> results = await context.Settings.Where(filter)
                                                                    .ToListAsync()
                                                                    .ConfigureAwait(false);
                    return StoredSettingMapper.Map(results).ToList();
                }
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error getting settings from EF for the app {0} and env {1}", application, currentEnvironment), exp);
                throw;
            }
        }

        public void UpdateSetting(string id, Setting value)
        {
            this.UpdateSettingAsync(id, value).Wait();
        }

        public async Task UpdateSettingAsync(string id, Setting value)
        {
            try
            {
                using (var context = new SettingsContext(this.ConnectionString))
                {
                    SettingEF settingMapped = StoredSettingMapper.Map<SettingEF>(value);
                    SettingEF settingEF = await context.Settings.Where(s => s.Id == id).FirstOrDefaultAsync().ConfigureAwait(false);

                    if (settingEF != null)
                    {
                        settingEF.JSONValue = settingMapped.JSONValue;
                        settingEF.Updated = DateTimeOffset.UtcNow;
                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error while updating setting {0} in EF", Newtonsoft.Json.JsonConvert.SerializeObject(value)), exp);
                throw;
            }
        }

        public List<string> GetApps()
        {
            return this.GetAppsAsync().Result;
        }

        public async Task<List<string>> GetAppsAsync()
        {
            try
            {
                using (var context = new SettingsContext(this.ConnectionString))
                {
                    List<string> results = await context.Settings.Select(s => s.Application)
                                                                 .Distinct()
                                                                 .ToListAsync()
                                                                 .ConfigureAwait(false);
                    return results;
                }
            }
            catch (Exception exp)
            {
                logger.Warn("Error getting available apps from EF", exp);
                throw;
            }
        }

        public List<string> GetAppEnvironments(string app)
        {
            return this.GetAppEnvironmentsAsync(app).Result;
        }

        public async Task<List<string>> GetAppEnvironmentsAsync(string app)
        {
            try
            {
                using (var context = new SettingsContext(this.ConnectionString))
                {
                    List<string> results = await context.Settings.Where(s => s.Application == app)
                                                                 .Select(s => s.Environment)
                                                                 .Distinct()
                                                                 .ToListAsync()
                                                                 .ConfigureAwait(false);
                    return results;
                }
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error getting environments available for the app {0} from EF", app), exp);
                throw;
            }
        }

        public Setting GetSetting(string id = null)
        {
            return this.GetSettingAsync(id).Result;
        }

        public async Task<Setting> GetSettingAsync(string id)
        {
            try
            {
                using (var context = new SettingsContext(this.ConnectionString))
                {
                    long lId;
                    if (long.TryParse(id, out lId))
                    { 
                        SettingEF result = await context.Settings.FirstOrDefaultAsync(s => s.DbId == lId)
                                                                 .ConfigureAwait(false);
                        return StoredSettingMapper.Map(result);
                    }

                    return null;
                }
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error getting setting with id {0} from mongo", id), exp);
                throw;
            }
        }
    }
}
