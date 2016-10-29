using Newtonsoft.Json.Linq;
using settings4net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace settings4net.API.Controllers.WebAPI
{
    [RoutePrefix("api")]
    public class SettingsController : ApiController
    {

        //public IReadableSettingsRepository SettingsRepository { get; set; }

        public SettingsController()
        {
            // this.SettingsRepository = null;
        }

        [HttpGet]
        [Route("environment/{env}/settings")]
        [ResponseType(typeof(IList<Setting>))]
        public HttpResponseMessage GetSettings(string env)
        {
            if (string.IsNullOrEmpty(env))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid environment.");

            IList<Setting> settings = new List<Setting>();
            return Request.CreateResponse(settings);
        }

        [HttpGet]
        [Route("environment/{env}/settings/{key}")]
        [ResponseType(typeof(Setting))]
        public HttpResponseMessage GetSetting(string env, string key)
        {
            //if (string.IsNullOrEmpty(env))
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid environment.");

            //if (string.IsNullOrEmpty(key))
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid setting key.");

            //Setting setting = this.SettingsRepository.GetSetting(key, env);

            //if (setting != null)
            //    return Request.CreateResponse();
            //else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Setting requested not found");
        }


        [HttpGet]
        [Route("environment/{env}/settings/{key}/value")]
        public HttpResponseMessage GetSettingValue(string env, string key)
        {
            //if (string.IsNullOrEmpty(env))
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid environment.");

            //if (string.IsNullOrEmpty(key))
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid setting key.");

            //Setting setting = this.SettingsRepository.GetSetting(key, env);

            //if (setting != null)
            //{
            //    return Request.CreateResponse(new JObject(setting.JSONValue));
            //}
            //else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Setting requested not found");
        }

    }
}
