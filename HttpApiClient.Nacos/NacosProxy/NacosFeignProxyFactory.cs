using HttpApiClient.Nacos.Attributes;
using HttpApiClient.Proxy;
using System;
using System.Linq;
using System.Reflection;

namespace HttpApiClient.Nacos.NacosProxy
{
    public class NacosFeignProxyFactory : IFeignProxyFactory
    {
        private IProxyConfiguration _proxyConfiguration;
        public NacosFeignProxyFactory(IProxyConfiguration proxyConfiguration)
        {
            _proxyConfiguration = proxyConfiguration;
        }

        public T GetHytrixProxy<T>()
        {
            Type type = typeof(T);
            if (type.GetCustomAttributes(typeof(NacosFeignClientAttribute), true).Any())
            {
                var proxy = DispatchProxy.Create<T, NacosFeignHystrixProxy<T>>();
                (proxy as NacosFeignHystrixProxy<T>).ProxyConfiguration = _proxyConfiguration;
                return proxy;
            }
            return default;
        }

        public T GetProxy<T>()
        {
            Type type = typeof(T);
            if (type.GetCustomAttributes(typeof(NacosFeignClientAttribute), true).Any())
            {
                var proxy = DispatchProxy.Create<T, NacosFeignProxy<T>>();
                (proxy as NacosFeignProxy<T>).ProxyConfiguration = _proxyConfiguration;
                return proxy;
            }
            return default;
        }
    }
}
