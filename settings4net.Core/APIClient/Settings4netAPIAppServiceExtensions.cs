using System;
using System.Net.Http;
using Microsoft.Azure.AppService;

namespace settings4net.Core.APIClient
{
    public static class Settings4netAPIAppServiceExtensions
    {
        public static Settings4netAPI CreateSettings4netAPI(this IAppServiceClient client)
        {
            return new Settings4netAPI(client.CreateHandler());
        }

        public static Settings4netAPI CreateSettings4netAPI(this IAppServiceClient client, params DelegatingHandler[] handlers)
        {
            return new Settings4netAPI(client.CreateHandler(handlers));
        }

        public static Settings4netAPI CreateSettings4netAPI(this IAppServiceClient client, Uri uri, params DelegatingHandler[] handlers)
        {
            return new Settings4netAPI(uri, client.CreateHandler(handlers));
        }

        public static Settings4netAPI CreateSettings4netAPI(this IAppServiceClient client, HttpClientHandler rootHandler, params DelegatingHandler[] handlers)
        {
            return new Settings4netAPI(rootHandler, client.CreateHandler(handlers));
        }
    }
}
