using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace settings4net.API.CORS
{

    public class ConfigurableCorsPolicy : ICorsPolicyProvider
    {
        private CorsPolicy _policy;

        public ConfigurableCorsPolicy()
        {
            // Create a CORS policy.
            _policy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true
            };

            // Add allowed origins.
            Properties.Settings.Default.CORSOrigins.Cast<string>().ToList().ForEach(s =>
            {
                _policy.Origins.Add(s);
            });
        }

        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_policy);
        }
    }
}