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
                    return Request.CreateResponse(settings.Select(s => new { s.Id, s.Application, s.Created, s.Updated, s.Environment, s.Fullpath }));

            }
            catch (Exception exp)
            {
                logger.Warn("Error while getting all the settings", exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while getting the settings");
            }
        }

        [HttpPost]
        [Route("settings")]
        public async Task<HttpResponseMessage> AddSetting([FromBody] Setting setting)
        {
            string settingImput = JsonConvert.SerializeObject(setting);
            logger.Debug(string.Format("POST /settings BODY: {0}", settingImput ?? "null"));

            if (setting == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Setting to add not provided");

            if (string.IsNullOrEmpty(setting.Application))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Application not specified");

            if (string.IsNullOrEmpty(setting.Environment))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Environment not specified");

            try
            {
                await this.SettingsRepository.AddSettingAsync(setting);
                return Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error adding setting {0}", settingImput), exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while adding the setting");
            }
        }

        [HttpGet]
        [Route("settings/{id}")]
        public async Task<HttpResponseMessage> GetSetting(string id)
        {
            logger.Debug(string.Format("GET settings/{0}", id));

            if (string.IsNullOrEmpty(id))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid identifier");

            try
            { 
                Setting setting = await this.SettingsRepository.GetSettingAsync(id);
                if (setting != null)
                    return Request.CreateResponse(setting);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Setting not found");

            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error while getting the setting with the id {0}", id), exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while getting the settings");
            }
        }

        [HttpPut]
        [Route("settings/{id}")]
        public async Task<HttpResponseMessage> UpdateSetting(string id, [FromBody] Setting setting)
        {
            string settingInput = JsonConvert.SerializeObject(setting);
            logger.Debug(string.Format("PUT settings/{0} BODY: {1}", id, settingInput));

            if (setting == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Setting to update not provided");

            if (string.IsNullOrEmpty(id))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Setting identifier not specified");

            try
            {
                await this.SettingsRepository.UpdateSettingAsync(id, setting);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error updating setting {0}", settingInput), exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while updating the setting");
            }
        }

        [HttpDelete]
        [Route("settings/{id}")]
        public async Task<HttpResponseMessage> DeleteSetting(string id)
        {
            logger.Debug(string.Format("DELETE settings/{0}", id ?? "null"));

            if (string.IsNullOrEmpty(id))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Identifier not specified");

            try
            {
                await this.SettingsRepository.DeleteSettingAsync(id);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error deleting the setting with id {0}", id), exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while deleting the setting");
            }
        }

        [HttpGet]
        [Route("applications/{app}/settings")]
        public async Task<HttpResponseMessage> GetApplicationSettings(string app, bool fullVersion = true)
        {
            logger.Debug(string.Format("GET applications/{0}/settings", app ?? "null"));

            if (string.IsNullOrEmpty(app))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Application not specified");
            

            try
            {
                // TODO: optimize in the future. The unnecessary data should not be retrieved
                List<Setting> settings = await this.SettingsRepository.GetSettingsAsync(app);
                if (fullVersion)
                    return Request.CreateResponse(settings);
                else
                    return Request.CreateResponse(settings.Select(s => new { s.Id, s.Application, s.Created, s.Updated, s.Environment, s.Fullpath }));

            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error while getting the settings for the app {0}", app), exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while getting the settings");
            }
        }

        [HttpGet]
        [Route("applications/{app}/environments/{env}/settings")]
        public async Task<HttpResponseMessage> GetApplicationEnvironmentSettings(string app, string env, bool fullVersion = true)
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
                    return Request.CreateResponse(settings.Select(s => new { s.Id, s.Application, s.Created, s.Updated, s.Environment, s.Fullpath }));

            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error while getting the settings for the app {0} and env {1}", app, env), exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while getting the settings");
            }
        }

    }
}
