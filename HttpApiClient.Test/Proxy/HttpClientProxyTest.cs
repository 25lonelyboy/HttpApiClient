using HttpApiClient.Attributes;
using HttpApiClient.Proxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace HttpApiClient.Test.Proxy
{
    public class HttpClientProxyTest
    {
        [Fact]
        public void Should_Get_A_HttpClient_Proxy_Instance()
        {
            var service = DispatchProxy.Create<IService, HttpClientProxy<IService>>();
            Assert.NotNull(service);

            var hystrixService = DispatchProxy.Create<IService, HttpClientHystrixProxy<IService>>();
            Assert.NotNull(hystrixService);
        }

        [Fact]
        public void Should_Get_A_HttpClient_Proxy_Instance_By_Factory()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging().AddHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var proxyConfiguration = new ProxyConfiguration(serviceProvider, httpClientFactory);
            var proxyFactory = new DefaultFeignProxyFactory(proxyConfiguration);
            var service = proxyFactory.GetProxy<IService>();
            Assert.NotNull(service);

            var hystrixProxy = proxyFactory.GetHytrixProxy<IService>();
            Assert.NotNull(hystrixProxy);
        }

        [Fact]
        public void Should_Get_A_HttpClient_Proxy_Instance_By_Container()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging().AddHttpClient();
            serviceCollection.AddHttpClientProxyFactory();
            serviceCollection.AddHttpClientProxy<IService>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<IService>();
            Assert.NotNull(service);
        }

        [Fact]
        public void Should_Throw_Exception_If_Without_GetMappingAttribute()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging().AddHttpClient();
            serviceCollection.AddHttpClientProxyFactory();
            serviceCollection.AddHttpClientProxy<IService>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<IService>();
            Assert.ThrowsAsync<Exception>(async () => { await service.Get("test"); });
        }

        [Fact]
        public void Should_Get_Response_By_ServiceProxy()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging()
                .AddHttpClient("HttpProxy")
                .AddMockHttpMessageHandler()
                .AddMock("/service1", HttpMethod.Get, new RemoteServiceResponse<string>() { Result = "test1", Success = true });
            serviceCollection.AddHttpClientProxyFactory();
            serviceCollection.AddHttpClientProxy<IService2>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<IService2>();
            var result = service.Get("test");
            Assert.Equal("test1", result.Result);
        }

        [Fact]
        public async Task Should_Get_TaskResponse_By_ServiceProxy()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging()
                .AddHttpClient("HttpProxy")
                .AddMockHttpMessageHandler()
                .AddMock("/service1", HttpMethod.Get, new RemoteServiceResponse<string>() { Result = "test1", Success = true });
            serviceCollection.AddHttpClientProxyFactory();
            serviceCollection.AddHttpClientProxy<IService3>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<IService3>();
            var result = await service.Get("test");
            Assert.Equal("test1", result.Result);
        }

        [Fact]
        public void Should_Throw_Exception_If_Response_Not_Format_ReturnType()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging()
                .AddHttpClient("HttpProxy")
                .AddMockHttpMessageHandler()
                .AddMock("/service1", HttpMethod.Get, new RemoteServiceResponse<object>() { Result = new { Content = "test1" }, Success = true });
            serviceCollection.AddHttpClientProxyFactory();
            serviceCollection.AddHttpClientProxy<IService2>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<IService2>();
            Assert.ThrowsAny<Exception>(() => { service.Get("tesst"); });
        }

        // 降级测试

        // 
    }

    [FeignClient]
    public interface IService
    {
        // [GetMapping(Route = "http://baidu.com")]
        Task<string> Get(string name);
    }

    [FeignClient(Name = "http://loalhost:5000")]
    public interface IService2
    {
        [GetMapping(Route = "/service1")]
        RemoteServiceResponse<string> Get(string name);
    }

    [FeignClient(Name = "http://loalhost:5000")]
    public interface IService3
    {
        [GetMapping(Route = "/service1")]
        Task<RemoteServiceResponse<string>> Get(string name);
    }
}
