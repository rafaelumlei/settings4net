using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Core.RemoteRepositories.Models
{
    interface IStoredSetting
    {
        
        /// <summary>
        /// Gets or sets the setting identifier
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the application/host that owns this setting
        /// </summary>
        string Application { get; set; }

        /// <summary>
        /// Gets or sets the environment to which the setting value applies
        /// </summary>
        string Environment { get; set; }

        /// <summary>
        /// Gets or sets full path to the setting (tipically Namespace + Class + Field)
        /// </summary>
        string Fullpath { get; set; }

        /// <summary>
        /// Gets or sets the setting documentation: possible values 
        /// and impacts in the system
        /// </summary>
        string Documentation { get; set; }

        /// <summary>
        /// Setting JSON value
        /// </summary>
        string JSONValue { get; set; }

        DateTimeOffset Created { get; set; }

        DateTimeOffset Updated { get; set; }

    }
}
