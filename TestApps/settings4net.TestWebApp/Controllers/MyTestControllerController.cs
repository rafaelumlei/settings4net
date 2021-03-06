﻿using settings4net.TestWebApp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace settings4net.TestWebApp.Controllers
{
    [RoutePrefix("api")]
    public class MyTestController : ApiController
    {

        [HttpGet]
        [Route("menuitems")]
        public HttpResponseMessage GetMenuItems()
        {
            return Request.CreateResponse(XPTOSettings.MainMenuOptions);
        }

    }
}
