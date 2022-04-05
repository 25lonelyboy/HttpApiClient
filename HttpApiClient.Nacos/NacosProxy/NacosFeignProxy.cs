using HttpApiClient.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nacos.V2;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpApiClient.Nacos.NacosProxy
{
    /// <summary>
    /// Nacos接口代理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NacosFeignProxy<T> : AbstactFeignProxy<T>
    {
        private IHttpClientFactory _clientFactory => ProxyConfiguration.HttpClientFactory;
        private INacosNamingService _nacosNamingService => ProxyConfiguration.ServiceProvider.GetRequiredService<INacosNamingService>();
        protected ILogger _logger => ProxyConfiguration.ServiceProvider.GetRequiredService<ILogger<NacosFeignProxy<T>>>();

        /// <summary>
        /// 通过服务发现调用服务接口
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> GetRequestAsync(string serviceName, string url, FeignMethodInfo targetMethod)
        {
            var baseUrl = await GetBaseUrl(serviceName);
            using (var client = _clientFactory.CreateClient("NacosProxyClient"))
            {
                // 设置请求超时时间
                client.Timeout = TimeSpan.FromMinutes(30);
                HeaderHelper.CopyHeader(client);
                var getUrl = baseUrl + url;
                _logger.LogInformation("请求地址: " + getUrl);
                var result = await client.GetAsync(getUrl);
                return result;
            }
        }

        /// <summary>
        /// 通过服务发现调用服务接口
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> PostRequestAsync(string serviceName, string url, object arg, FeignMethodInfo targetMethod)
        {
            var baseUrl = await GetBaseUrl(serviceName);
            using (var client = _clientFactory.CreateClient("NacosProxyClient"))
            {
                // 设置请求超时时间
                client.Timeout = TimeSpan.FromMinutes(30);
                HeaderHelper.CopyHeader(client);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(arg), Encoding.UTF8, "application/json");
                var postUrl = baseUrl + url;
                _logger.LogInformation("请求地址: " + postUrl);
                var result = await client.PostAsync(postUrl, content);
                return result;
            }
        }

        public async Task<string> GetBaseUrl(string serviceName)
        {
            var instance = await _nacosNamingService.SelectOneHealthyInstance(serviceName);
            if(instance != null)
            {
                var host = $"{instance.Ip}:{instance.Port}";

                var baseUrl = instance.Metadata.TryGetValue("secure", out _)
                    ? $"https://{host}"
                    : $"http://{host}";
                return baseUrl;
            }
            throw new Exception($"{serviceName}服务不可用！");
        }
    }
}
