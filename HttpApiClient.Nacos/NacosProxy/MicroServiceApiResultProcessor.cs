using HttpApiClient.Nacos.Exceptions;
using HttpApiClient.Proxy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpApiClient.Nacos.NacosProxy
{
    public class MicroServiceApiResultProcessor : IApiResultProcessor
    {
        private readonly ILogger<MicroServiceApiResultProcessor> _logger;
        public MicroServiceApiResultProcessor(ILogger<MicroServiceApiResultProcessor> logger)
        {
            _logger = logger;
        }

        public async Task<TResult> Process<TResult>(HttpResponseMessage result)
        {
            string jsonText = string.Empty;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    // 读取http请求结果
                    jsonText = await result.Content.ReadAsStringAsync();
                    // 进行反序列化, 由于我们的结果结果统一通过RemoteServiceResponse<>进行包装，
                    // 这里要按照RemoteServiceResponse<>类型进行反序列
                    var res = JsonConvert.DeserializeObject<RemoteServiceResponse<TResult>>(jsonText);
                    if (!res.Success)
                    {
                        _logger.LogError("处理信息失败：" + jsonText);
                        throw new MicroServiceException(MicroServiceException.SERVICE_ERR, res.State.ToString(), res.Msg);
                    }
                    return res.Result;
                }
                catch (Exception e)
                {
                    _logger.LogError("处理信息失败", e);
                    throw new MicroServiceException(MicroServiceException.SER_ERR, jsonText);
                }
            }
            else
            {
                try
                {
                    jsonText = await result.Content.ReadAsStringAsync();
                }
                catch { }
                _logger.LogError("处理信息失败：" + result.StatusCode.ToString() + jsonText);
                throw new MicroServiceException(MicroServiceException.STA_ERR, result.StatusCode.ToString(), jsonText);
            }
        }
    }
}
