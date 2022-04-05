using Newtonsoft.Json;

namespace HttpApiClient
{
    /// <summary>
    /// Web Api接口返回结果包装类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RemoteServiceResponse<T>
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }

        /// <summary>
        /// 接口返回值
        /// </summary>
        [JsonProperty("result")]
        public T Result { get; set; }

        /// <summary>
        /// 响应状态码
        /// </summary>
        [JsonProperty("state")]
        public int State { get; set; }

        /// <summary>
        /// 调用是否成功
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
