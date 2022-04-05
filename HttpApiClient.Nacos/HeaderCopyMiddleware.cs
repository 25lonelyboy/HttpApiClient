using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace HttpApiClient.Nacos
{
    /// <summary>
    /// 用于服务间调用时token的复制传递
    /// </summary>
    public class HeaderCopyMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RequestDelegate _next;
        public HeaderCopyMiddleware(IServiceProvider serviceProvider, RequestDelegate next)
        {
            _serviceProvider = serviceProvider;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string contentType = context.Request.ContentType;
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                HeaderAsyncLocal.TokenHolder.Value = context.Request.Headers["Authorization"];
            }
            if (context.Request.Headers.ContainsKey("SysCode"))
            {
                HeaderAsyncLocal.SysCodeHolder.Value = context.Request.Headers["SysCode"];
            }
            await _next.Invoke(context);
        }
    }
}
