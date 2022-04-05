using System;
using System.Net.Http;

namespace HttpApiClient.Proxy
{
    /// <summary>
    /// 代理依赖配置类
    /// </summary>
    public class ProxyConfiguration : IProxyConfiguration
    {
        public ProxyConfiguration(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
        {
            ServiceProvider = serviceProvider;
            HttpClientFactory = httpClientFactory;
        }

        public IServiceProvider ServiceProvider { get; }

        public IHttpClientFactory HttpClientFactory { get; }
    }
}
