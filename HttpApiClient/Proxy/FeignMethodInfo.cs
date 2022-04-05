using System;
using System.Net.Http;
using System.Reflection;

namespace HttpApiClient.Proxy
{
    /// <summary>
    /// 代理方法信息
    /// </summary>
    public class FeignMethodInfo
    {
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// 是否异步方法
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public Type ReturnType { get; set; }

        /// <summary>
        /// fallback实现
        /// </summary>
        public Type Fallback { get; set; }

        /// <summary>
        /// Url参数模板
        /// </summary>
        public string UrlTemplate { get; set; }

        /// <summary>
        /// 请求参数定义
        /// </summary>
        public ParameterInfo[] Parameters { get; set; }
    }
}
