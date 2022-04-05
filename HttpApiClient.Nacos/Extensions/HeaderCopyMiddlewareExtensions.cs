using Microsoft.AspNetCore.Builder;

namespace HttpApiClient.Nacos.Extensions
{
    public static class HeaderCopyMiddlewareExtensions
    {
        public static IApplicationBuilder UseHeaderCopy(this IApplicationBuilder app)
        {
            app.UseMiddleware<HeaderCopyMiddleware>();
            return app;
        }
    }
}
