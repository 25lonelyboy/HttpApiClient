using HttpApiClient.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace HttpApiClient.Proxy
{
    /// <summary>
    /// 动态代理工厂
    /// </summary>
    public class DefaultFeignProxyFactory : IFeignProxyFactory
    {
        private IProxyConfiguration _proxyConfiguration;
        public DefaultFeignProxyFactory(IProxyConfiguration proxyConfiguration)
        {
            _proxyConfiguration = proxyConfiguration;
        }

        /// <summary>
        /// 创建代理
        /// </summary>
        /// <returns></returns>
        public virtual T GetProxy<T>()
        {
            Type type = typeof(T);
            if (type.GetCustomAttributes(typeof(FeignClientAttribute), true).Any())
            {
                var proxy = DispatchProxy.Create<T, HttpClientProxy<T>>();
                (proxy as AbstactFeignProxy<T>).ProxyConfiguration = _proxyConfiguration;
                return proxy;
            }
            return default;
        }

        /// <summary>
        /// 创建断路器代理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetHytrixProxy<T>()
        {
            Type type = typeof(T);
            if (type.GetCustomAttributes(typeof(FeignClientAttribute), true).Any())
            {
                var proxy = DispatchProxy.Create<T, HttpClientHystrixProxy<T>>();
                (proxy as AbstactFeignProxy<T>).ProxyConfiguration = _proxyConfiguration;
                return proxy;
            }
            return default;
        }
    }
}
