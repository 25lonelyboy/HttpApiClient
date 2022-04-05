using HttpApiClient.Nacos.NacosProxy;
using HttpApiClient.Proxy;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nacos.AspNetCore.V2;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NacosProxyServiceCollectionExtensions
    {
        /// <summary>
        /// 添加 Nacos Http 请求代理必要依赖
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNacosHttpProxyFactory(this IServiceCollection services)
        {
            var configuration = services.GetConfiguration();
            services.AddNacosAspNet(configuration, "nacos");
            services.AddHttpClientProxyFactory();
            services.Replace(new ServiceDescriptor(typeof(IFeignProxyFactory), typeof(NacosFeignProxyFactory), ServiceLifetime.Singleton));
            services.Replace(new ServiceDescriptor(typeof(IApiResultProcessor), typeof(MicroServiceApiResultProcessor), ServiceLifetime.Singleton));
            return services;
        }
    }
}
