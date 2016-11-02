using settings4net.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using settings4net.Core.Model;
using settings4net.Core.RemoteRepositories.APIClient;
using settings4net.Core.RemoteRepositories.Mappers;
using MongoDB.Driver;
using settings4net.Core.RemoteRepositories.Models;
using log4net;

namespace settings4net.Core.Repositories
{
    public class MongoSettingsRepository : IMultiAppSettingsRepository
    {
        private static ILog logger = LogManager.GetLogger(typeof(MongoSettingsRepository));

        protected IMongoDatabase Database { get; private set; }

        private IMongoCollection<SettingMongo> SettingsCollection { get; set; }

        private IMongoClient MongoClient { get; set; }

        public MongoSettingsRepository(string connnectionString)
        {
            if (string.IsNullOrEmpty(connnectionString))
                throw new ArgumentException("Invalid mongo connection string");

            try
            {
                var mongoUrl = MongoUrl.Create(connnectionString);
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

        public void AddSetting(string application, string currentEnvironment, Setting setting)
        {
            try
            {
                SettingMongo settingMongo = ModelToMongoMapper.Map(setting);
                settingMongo.Created = settingMongo.Updated = DateTimeOffset.UtcNow;
                this.SettingsCollection.InsertOne(settingMongo);
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error adding setting {0} to mongo", setting.Key), exp);
            }
        }

        public List<Setting> GetSettings(string application, string currentEnvironment)
        {
            try
            {
                IEnumerable<SettingMongo> settingsMongo = this.SettingsCollection.AsQueryable()
                    .Where(s => s.Application == application && s.Environment == currentEnvironment);

                return ModelToMongoMapper.Map(settingsMongo).ToList();
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error getting settings from mongo for the app {0} and env {1}", application, currentEnvironment), exp);
            }

            return new List<Setting>();
        }

        public void OverrideState(string application, string currentEnvironment, List<Setting> settings)
        {
            if (settings != null && settings.Any())
            {
                //IEnumerable<SettingMongo> settings = ModelToMongoMapper.Map(values);
                foreach (var setting in settings)
                {
                    if (this.SettingsCollection.AsQueryable().Any(s => s.Key == setting.Key))
                    {
                        this.UpdateSetting(application, currentEnvironment, setting);
                    }
                    else
                    {
                        this.AddSetting(application, currentEnvironment, setting);
                    }
                }

                // removing all the settings that were not provided and still exist in the collection
                try
                {
                    string[] currentKeys = settings.Select(s => s.Key).ToArray();
                    this.SettingsCollection.DeleteMany(model => !currentKeys.Contains(model.Key) && model.Environment == currentEnvironment);
                }
                catch (Exception exp)
                {
                    string log = string.Format("Error removing all obsolete settings for the app {0} and env {1}.", application, currentEnvironment);
                    logger.Warn(log, exp);
                }
            }
        }

        public void UpdateSetting(string application, string currentEnvironment, Setting value)
        {
            try
            {
                SettingMongo settingMongo = ModelToMongoMapper.Map(value);
                var filter = Builders<SettingMongo>.Filter.Eq(s => s.Key, value.Key);
                var update = Builders<SettingMongo>.Update.Set(s => s.Documentation, settingMongo.Documentation)
                                                          .Set(s => s.JSONValue, settingMongo.JSONValue)
                                                          .Set(s => s.Updated, DateTimeOffset.UtcNow);

                this.SettingsCollection.UpdateOne(filter, update);
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error while updating setting {0} into mongo", value.Key), exp);
            }
        }

        public void UpdateSettings(string application, string currentEnvironment, List<Setting> settings)
        {
            if (settings != null && settings.Any())
            {
                foreach (var setting in settings)
                {
                    this.UpdateSetting(application, currentEnvironment, setting);
                }
            }
        }
    }
}
