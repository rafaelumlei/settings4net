using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using settings4net.Core.Interfaces;
using settings4net.Core.Model;
using settings4net.Core.RemoteRepositories;
using settings4net.Core.Repositories;
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

        private IMultiAppSettingsRepository SettingsRepository { get; set; }

        public SettingsController(IMultiAppSettingsRepository settingsRepository)
        {
            this.SettingsRepository = settingsRepository;
        }

        [HttpGet]
        [Route("applications/{app}/environments/{env}/settings")]
        public async Task<HttpResponseMessage> GetApplicationSettings(string app, string env, bool fullVersion = true)
        {
            logger.Debug(string.Format("GET applications/{0}/environments/{1}/settings", app ?? "null", env ?? "null"));

            if (string.IsNullOrEmpty(app))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Application not specified");

            if (string.IsNullOrEmpty(env))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Environment not specified");

            try
            {
                // TODO: optimize in the future. The unnecessary data should not be retrieved
                List<Setting> settings = await this.SettingsRepository.GetSettingsAsync(app, env);
                if (fullVersion)
                    return Request.CreateResponse(settings);
                else
                    return Request.CreateResponse(settings.Select(s => new { s.Application, s.Created, s.Updated, s.Environment, s.Fullpath }));

            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error while getting the settings for the app {0} and env {1}", app, env), exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while getting the settings");
            }
        }

        [HttpGet]
        [Route("settings")]
        public async Task<HttpResponseMessage> GetSettings(bool fullVersion = true)
        {
            logger.Debug("GET settings");
            
            try
            {
                // TODO: optimize in the future. The unnecessary data should not be retrieved
                List<Setting> settings = await this.SettingsRepository.GetSettingsAsync();
                if (fullVersion)
                    return Request.CreateResponse(settings);
                else
                    return Request.CreateResponse(settings.Select(s => new { s.Application, s.Created, s.Updated, s.Environment, s.Fullpath }));

            }
            catch (Exception exp)
            {
                logger.Warn("Error while getting all the settings", exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while getting the settings");
            }
        }

        [HttpPost]
        [Route("applications/{app}/environments/{env}/settings")]
        public async Task<HttpResponseMessage> AddSetting(string app, string env, [FromBody] Setting setting)
        {
            string settingImput = JsonConvert.SerializeObject(setting);
            logger.Debug(string.Format("POST applications/{0}/environments/{1}/settings BODY: {2}", app ?? "null", env ?? "null", settingImput ?? "null"));

            if (setting == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Setting to add not provided");

            if (string.IsNullOrEmpty(app))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Application not specified");

            if (string.IsNullOrEmpty(env))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Environment not specified");

            setting.Application = app;
            setting.Environment = env;

            try
            {
                await this.SettingsRepository.AddSettingAsync(app, env, setting);
                return Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error adding setting {0}", settingImput), exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while adding the setting");
            }
        }

        [HttpPut]
        [Route("applications/{app}/environments/{env}/settings/{fullpath}")]
        public async Task<HttpResponseMessage> UpdateSetting(string app, string env, string fullpath, [FromBody] Setting setting)
        {
            string settingImput = JsonConvert.SerializeObject(setting);
            logger.Debug(string.Format("POST applications/{0}/environments/{1}/settings BODY: {2}", app ?? "null", env ?? "null", settingImput ?? "null"));

            if (setting == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Setting to update not provided");

            if (string.IsNullOrEmpty(app))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Application not specified");

            if (string.IsNullOrEmpty(env))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Environment not specified");

            if (string.IsNullOrEmpty(fullpath))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Fullpath not specified");

            setting.Application = app;
            setting.Environment = env;
            setting.Fullpath = fullpath;

            try
            {
                await this.SettingsRepository.UpdateSettingAsync(app, env, setting);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error updating setting {0}", settingImput), exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while updating the setting");
            }
        }

        [HttpDelete]
        [Route("applications/{app}/environments/{env}/settings/{fullpath}")]
        public async Task<HttpResponseMessage> DeleteSetting(string app, string env, string fullpath)
        {
            logger.Debug(string.Format("DELETE applications/{0}/environments/{1}/setting/{2}", app ?? "null", env ?? "null", fullpath ?? "null"));

            if (string.IsNullOrEmpty(app))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Application not specified");

            if (string.IsNullOrEmpty(env))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Environment not specified");

            if (string.IsNullOrEmpty(fullpath))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Fullpath not specified");

            try
            {
                await this.SettingsRepository.DeleteSettingAsync(app, env, fullpath);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error deleting the setting {0}:{1}:{2}", app, env, fullpath), exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while deleting the setting");
            }
        }

    }
}
