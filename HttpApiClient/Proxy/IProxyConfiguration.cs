using System;
using System.Net.Http;

namespace HttpApiClient.Proxy
{
    /// <summary>
    /// 代理依赖配置接口
    /// </summary>
    public interface IProxyConfiguration
    {
        /// <summary>
        /// 容器
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// HttpClient工厂
        /// </summary>
        public IHttpClientFactory HttpClientFactory { get; }
    }
}
