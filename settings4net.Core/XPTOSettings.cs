using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace settings4net.Core
{
    class XPTOSettings : BaseSettings
    {

        /// <summary>
        /// Endpoint to the SAPO service 
        /// </summary>
        public string ServiceUrl = "http://www.sapo.pt/";

        /// <summary>
        /// Endpoint to the SAPO service 
        /// </summary>
        public string[] ContentTypes = new string[] { "a", "b", "c" };

        /// <summary>
        /// My custom object documentation
        /// </summary>
        public dynamic MyCutomObject = new { a = 1, b = 2, c = 3 };


    }
}
