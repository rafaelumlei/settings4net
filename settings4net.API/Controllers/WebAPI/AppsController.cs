using log4net;
using settings4net.Core.APIClient.Models;
using settings4net.Core.Interfaces;
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
    public class AppsController : ApiController
    {

        private static ILog logger = LogManager.GetLogger(typeof(AppsController));

        private IMultiAppSettingsRepository SettingsRepository { get; set; }

        public AppsController(IMultiAppSettingsRepository settingsRepository)
        {
            this.SettingsRepository = settingsRepository;
        }

        [HttpGet]
        [Route("applications")]
        [ResponseType(typeof(IList<string>))]
        public async Task<HttpResponseMessage> GetApps()
        {
            logger.Debug(string.Format("GET applications"));
            
            try
            {
                return Request.CreateResponse(await this.SettingsRepository.GetAppsAsync());
            }
            catch (Exception exp)
            {
                logger.Warn("Error while getting the available apps", exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while getting the settings");
            }
        }

        [HttpGet]
        [Route("applications/{app}/environments")]
        [ResponseType(typeof(IList<string>))]
        public async Task<HttpResponseMessage> GetAppEnvironments(string app)
        {
            logger.Debug(string.Format("GET applications/{0}/environments", app ?? "null"));

            if (string.IsNullOrEmpty(app))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Application not specified");
            
            try
            {
                return Request.CreateResponse(await this.SettingsRepository.GetAppEnvironmentsAsync(app));
            }
            catch (Exception exp)
            {
                logger.Warn(string.Format("Error while getting the available environments for the app {0}", app), exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unknown error occured while getting the settings");
            }
        }

    }
}
