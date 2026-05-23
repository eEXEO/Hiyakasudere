using System;
using System.Net.Http;

namespace Hiyakasudere.Data.Internal.Functionality
{
    /// <summary>
    /// Shared HttpClient with proper timeout and connection pooling.
    /// Prevents socket exhaustion and repeated error spam.
    /// </summary>
    public static class HttpClientProvider
    {
        private static readonly Lazy<HttpClient> _instance = new(() =>
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                ConnectTimeout = TimeSpan.FromSeconds(10),
                MaxConnectionsPerServer = 10
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(15)
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd("Hiyakasudere/2.0 (Desktop App)");
            return client;
        });

        public static HttpClient Client => _instance.Value;
    }
}
