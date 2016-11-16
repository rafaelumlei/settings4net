using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Cors;

namespace settings4net.API.CORS
{
    public class CorsPolicyFactory : ICorsPolicyProviderFactory
    {
        ICorsPolicyProvider _provider = new ConfigurableCorsPolicy();

        public ICorsPolicyProvider GetCorsPolicyProvider(HttpRequestMessage request)
        {
            return _provider;
        }
    }
}