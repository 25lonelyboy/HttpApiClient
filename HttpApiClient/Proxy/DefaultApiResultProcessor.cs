using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpApiClient.Proxy
{
    public class DefaultApiResultProcessor : IApiResultProcessor
    {
        private readonly ILogger<DefaultApiResultProcessor> _logger;
        public DefaultApiResultProcessor(ILogger<DefaultApiResultProcessor> logger)
        {
            _logger = logger;
        }

        public async Task<TResult> Process<TResult>(HttpResponseMessage message)
        {
            var result = await Process(message, typeof(TResult));
            return (TResult)result;
        }

        private async Task<object> Process(HttpResponseMessage message, Type type)
        {
            string jsonText = string.Empty;
            if (message.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    jsonText = await message.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject(jsonText, type);
                    return res;
                }
                catch (Exception e)
                {
                    _logger.LogError("处理信息失败", e);
                    throw new Exception("获取接口结果失败，或者结果转换失败：" + jsonText);
                }
            }
            else
            {
                try
                {
                    jsonText = await message.Content.ReadAsStringAsync();
                }
                catch { }
                _logger.LogError("处理信息失败：" + message.StatusCode.ToString() + jsonText);
                throw new Exception("接口调用失败，返回状态码：" + message.StatusCode + ", 返回信息如下：" + jsonText);
            }
        }
    }
}
