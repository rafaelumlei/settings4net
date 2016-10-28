using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Model
{

    public class Setting
    {

        /// <summary>
        /// Namespace + "." + Name
        /// </summary>
        public string Key
        {
            get
            {
                return this.Namespace + "." + this.Name;
            }
        }

        /// <summary>
        /// Env + ":" + Key
        /// </summary>
        public string FullKey
        {
            get
            {
                return this.Environment + ":" + this.Key;
            }
        }

        /// <summary>
        /// Gets or sets Namespace/Assembly name of the setting
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets ir sets the setting name
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Gets or sets the setting documentation: possible values 
        /// and impacts in the system
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        /// Gets or sets the environment to which the setting value applies
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Setting JSON value
        /// </summary>
        public String JSONValue { get; set; }

    }
}
