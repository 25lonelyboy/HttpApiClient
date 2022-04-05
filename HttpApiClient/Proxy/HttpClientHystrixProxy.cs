using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpApiClient.Proxy
{
    public class HttpClientHystrixProxy<T> : AbstactHystrixFeignProxy<T>
    {
        private IHttpClientFactory _clientFactory => ProxyConfiguration.HttpClientFactory;

        protected async override Task<HttpResponseMessage> GetRequestAsync(string serviceName, string url, FeignMethodInfo targetMethod)
        {
            using (var client = _clientFactory.CreateClient("HttpProxy"))
            {
                var getUrl = serviceName + url;
                var result = await client.GetAsync(getUrl);
                return result;
            }
        }

        protected async override Task<HttpResponseMessage> PostRequestAsync(string serviceName, string url, object arg, FeignMethodInfo targetMethod)
        {
            using (var client = _clientFactory.CreateClient("HttpProxy"))
            {
                var postUrl = serviceName + url;
                HttpContent content = new StringContent(JsonConvert.SerializeObject(arg), Encoding.UTF8, "application/json");
                var result = await client.PostAsync(url, content);
                return result;
            }
        }

        protected override object HystrixInvoke(MethodInfo targetMethod, object[] args, FeignMethodInfo methodInfo)
        {
            throw new NotImplementedException();
        }
    }
}
