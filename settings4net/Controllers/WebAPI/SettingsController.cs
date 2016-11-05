﻿using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using settings4net.Core.Interfaces;
using settings4net.Core.Model;
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

        public SettingsController()
        {
            this.SettingsRepository = new MongoSettingsRepository("mongodb://localhost:27017/Settings4net");
        }

        [HttpGet]
        [Route("applications/{app}/environments/{env}/settings")]
        [ResponseType(typeof(IList<Setting>))]
        public async Task<HttpResponseMessage> GetSettings(string app, string env)
        {
            logger.Debug(string.Format("GET applications/{0}/environments/{1}/settings", app ?? "null", env ?? "null"));

            if (string.IsNullOrEmpty(app))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Application not specified");

            if (string.IsNullOrEmpty(env))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Environment not specified");

            try
            {
                return Request.CreateResponse(await this.SettingsRepository.GetSettingsAsync(app, env));
            }
            catch (Exception exp)
            {
                logger.Warn("Error while getting the settings", exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while adding the setting");
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
