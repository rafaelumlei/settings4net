using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using settings4net.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace settings4net.API.Controllers.WebAPI
{
    [RoutePrefix("api")]
    public class SettingsController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(SettingsController));

        public SettingsController()
        {
        }

        [HttpGet]
        [Route("applications/{app}/environments/{env}/settings")]
        [ResponseType(typeof(IList<Setting>))]
        public async Task<HttpResponseMessage> GetSettings(string app, string env)
        {
            logger.Debug(string.Format("GET applications/{0}/environments/{1}/settings", app, env));
            return Request.CreateResponse(new List<Setting>()); 
        }

        [HttpPost]
        [Route("applications/{app}/environments/{env}/settings")]
        public async Task<HttpResponseMessage> AddSetting(string app, string env, [FromBody] Setting setting)
        {
            logger.Debug(string.Format("POST applications/{0}/environments/{1}/settings BODY: {2}", app, env, JsonConvert.SerializeObject(setting)));
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        [HttpPut]
        [Route("applications/{app}/environments/{env}/settings/{key}")]
        public async Task<HttpResponseMessage> AddOrUpdateSetting(string app, string env, [FromBody] Setting setting)
        {
            logger.Debug(string.Format("POST applications/{0}/environments/{1}/settings BODY: {2}", app, env, JsonConvert.SerializeObject(setting)));
            return Request.CreateResponse(HttpStatusCode.Created);
        }

    }
}
