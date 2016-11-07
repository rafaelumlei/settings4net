using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Core.RemoteRepositories.Models
{
    class SettingMongo : IStoredSetting
    {

        public SettingMongo()
        {
        }

        public ObjectId Id { get; set;}

        /// <summary>
        /// Gets or sets the setting key (eg: app + ":" + env + ":" + fullpath.
        /// It will be saved as is to mongo for search purposes.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name of the application/host that owns this setting
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the environment to which the setting value applies
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets full path to the setting (tipically Namespace + Class + Field)
        /// </summary>
        public string Fullpath { get; set; }

        /// <summary>
        /// Gets or sets the setting documentation: possible values 
        /// and impacts in the system
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        /// Setting JSON value
        /// </summary>
        public string JSONValue { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

    }
}
