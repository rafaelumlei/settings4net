using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Core.Model;
using settings4net.Core.RemoteRepositories.Mappers;
using MongoDB.Driver;
using settings4net.Core.RemoteRepositories.Models;
using log4net;

namespace settings4net.Core.RemoteRepositories
{
    public class MongoSettingsRepository : IMultiAppSettingsRepository
    {
        private static ILog logger = LogManager.GetLogger(typeof(MongoSettingsRepository));

        protected IMongoDatabase Database { get; private set; }

        private IMongoCollection<SettingMongo> SettingsCollection { get; set; }

        private IMongoClient MongoClient { get; set; }

        public MongoSettingsRepository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Invalid mongo connection string");

            try
            {
                var mongoUrl = MongoUrl.Create(connectionString);
                this.MongoClient = new MongoClient(mongoUrl);
                this.Database = this.MongoClient.GetDatabase(mongoUrl.DatabaseName);
                this.SettingsCollection = this.Database.GetCollection<SettingMongo>("Settings");
                // the key has to be unique
                this.SettingsCollection.Indexes.CreateOne(Builders<SettingMongo>.IndexKeys.Ascending(s => s.Key), new CreateIndexOptions() { Unique = true });
                // filters by application and common will be very common, specially in the API and Web Management tool
                this.SettingsCollection.Indexes.CreateOne(Builders<SettingMongo>.IndexKeys.Ascending(s => s.Application).Ascending(s => s.Environment));
                // filters by Fullpath will be very common, specially with wild cards in the Web Management tool
                this.SettingsCollection.Indexes.CreateOne(Builders<SettingMongo>.IndexKeys.Text(s => s.Fullpath));
            }
            catch (Exception exp)
            {
                logger.Error("Expcetion when setting up Mongo Connection/DB/Collection for the settings4net", exp);
                throw;
            }
        }

        public async Task AddSettingAsync(string application, string currentEnvironment, Setting setting)
        {
            try
            {
                SettingMongo settingMongo = StoredSettingMapper.Map<SettingMongo>(setting);
                settingMongo.Created = settingMongo.Updated = DateTimeOffset.UtcNow;
                await this.SettingsCollection.InsertOneAsync(settingMongo).ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error adding setting {0} to mongo", Newtonsoft.Json.JsonConvert.SerializeObject(setting)), exp);
                throw;
            }
        }

        public void AddSetting(string application, string currentEnvironment, Setting setting)
        {
            this.AddSettingAsync(application, currentEnvironment, setting).RunSynchronously();
        }

        public async Task<List<Setting>> GetSettingsAsync(string application, string currentEnvironment)
        {
            try
            {
                var filter = Builders<SettingMongo>.Filter.Where(s => s.Application == application && s.Environment == currentEnvironment);
                var result = await this.SettingsCollection.FindAsync<SettingMongo>(filter).ConfigureAwait(false);
                return StoredSettingMapper.Map(await result.ToListAsync().ConfigureAwait(false)).ToList();
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error getting settings from mongo for the app {0} and env {1}", application, currentEnvironment), exp);
                throw;
            }
        }

        public List<Setting> GetSettings(string application, string currentEnvironment)
        {
            var result = this.GetSettingsAsync(application, currentEnvironment);
            return result.Result;
        }

        public async Task UpdateSettingAsync(string application, string currentEnvironment, Setting value)
        {
            try
            {
                SettingMongo settingMongo = StoredSettingMapper.Map<SettingMongo>(value);
                var filter = Builders<SettingMongo>.Filter.Eq(s => s.Key, value.Key);
                var update = Builders<SettingMongo>.Update.Set(s => s.Documentation, settingMongo.Documentation)
                                                          .Set(s => s.JSONValue, settingMongo.JSONValue)
                                                          .Set(s => s.Updated, DateTimeOffset.UtcNow);

                await this.SettingsCollection.UpdateOneAsync(filter, update).ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error while updating setting {0} into mongo", Newtonsoft.Json.JsonConvert.SerializeObject(value)), exp);
                throw;
            }
        }

        public void UpdateSetting(string application, string currentEnvironment, Setting value)
        {
            this.UpdateSettingAsync(application, currentEnvironment, value).RunSynchronously();
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

        public void UpdateSettings(string application, string currentEnvironment, List<Setting> settings)
        {
            this.UpdateSettingsAsync(application, currentEnvironment, settings).RunSynchronously();
        }

        public async Task DeleteSettingAsync(string application, string currentEnvironment, string fullpath)
        {
            try
            {
                var filter = Builders<SettingMongo>.Filter.Where(s => s.Application == application && s.Environment == currentEnvironment && s.Fullpath == fullpath);
                await this.SettingsCollection.DeleteOneAsync(filter);
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error while deleting setting {0}:{1}:{2} into mongo", application, currentEnvironment, fullpath), exp);
                throw;
            }
        }

        public void DeleteSetting(string application, string currentEnvironment, string fullpath)
        {
            this.DeleteSettingAsync(application, currentEnvironment, fullpath).RunSynchronously();
        }

    }
}
