using HttpApiClient.Attributes;
using HttpApiClient.Proxy;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpProxyServiceCollectionExtensions
    {
        /// <summary>
        /// 添加 HttpClientProxy 必要依赖
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientProxyFactory(this IServiceCollection services)
        {
            services.AddSingleton<IProxyConfiguration, ProxyConfiguration>();
            services.AddSingleton<IFeignProxyFactory, DefaultFeignProxyFactory>();
            services.AddSingleton<IApiResultProcessor, DefaultApiResultProcessor>();
            return services;
        }

        /// <summary>
        /// 注册 HttpClient 代理
        /// </summary>
        /// <typeparam name="T">代理接口类型</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientProxy<T>(this IServiceCollection services) where T : class
        {
            //注册fallback
            var attr = typeof(T).GetCustomAttributes(typeof(FeignClientAttribute), true).FirstOrDefault();
            if (attr != null && (attr as FeignClientAttribute).FallbackImpl != null)
            {
                services.AddSingleton((attr as FeignClientAttribute).FallbackImpl);
            }
            //注册代理
            services.AddSingleton(serviceProvider =>
            {
                var proxyFactory = serviceProvider.GetService<IFeignProxyFactory>();
                var proxy = proxyFactory.GetHytrixProxy<T>();
                return proxy;
            });
            return services;
        }

        /// <summary>
        /// 注册 HttpClient 代理
        /// </summary>
        /// <typeparam name="T">代理接口类型</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientProxy<T>(this IServiceCollection services, Type type) where T : class
        {
            //注册fallback
            var attr = typeof(T).GetCustomAttributes(typeof(FeignClientAttribute), true).FirstOrDefault();
            if (attr != null && (attr as FeignClientAttribute).FallbackImpl != null)
            {
                services.AddSingleton((attr as FeignClientAttribute).FallbackImpl);
            }
            //注册代理
            services.AddSingleton(serviceProvider =>
            {
                var proxyFactory = serviceProvider.GetService<IFeignProxyFactory>();
                var proxy = proxyFactory.GetHytrixProxy<T>();
                return proxy;
            });
            return services;
        }
    }
}
