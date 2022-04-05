using HttpApiClient.Attributes;
using HttpApiClient.Nacos.Attributes;
using HttpApiClient.Nacos.NacosProxy;
using HttpApiClient.Proxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HttpApiClient.Test.Proxy
{
    public class NacosFeignProxyTest
    {
        // 测试MicroServiceApiResultProcessor解析请求结果
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
            serviceCollection.Replace(new ServiceDescriptor(typeof(IApiResultProcessor), typeof(MicroServiceApiResultProcessor), ServiceLifetime.Singleton));
            serviceCollection.AddHttpClientProxy<IService4>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<IService4>();
            var result = service.Get("test");
            Assert.Equal("test1", result);
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
            serviceCollection.Replace(new ServiceDescriptor(typeof(IApiResultProcessor), typeof(MicroServiceApiResultProcessor), ServiceLifetime.Singleton));
            serviceCollection.AddHttpClientProxy<IService5>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<IService5>();
            var result = await service.GetAsync("test");
            Assert.Equal("test1", result);
        }

        // 测试服务基地址解析, 获取不到服务应该抛出异常
        [Fact]
        public async Task Should_Throw_Exception_IF_Not_Service_Health()
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            serviceCollection.AddSingleton<IConfiguration>(configuration);

            serviceCollection.AddNacosHttpProxyFactory();
            serviceCollection.AddHttpClientProxy<IService6>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<IService6>();
            await Assert.ThrowsAsync<Exception>(async () => await service.GetAsync("test"));
        }

        // 测试从其他服务调用接口
        // 需要先启动本地服务注册到nacos
        [Fact]
        public async Task Should_Get_ServiceName_IF_Not_Service_Health()
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            serviceCollection.AddSingleton<IConfiguration>(configuration);

            serviceCollection.AddNacosHttpProxyFactory();
            serviceCollection.AddHttpClientProxy<IService7>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<IService7>();
            var host = await service.GetAsync();
            Assert.Equal("http://172.17.240.1:5000", host);
        }
    }

    [FeignClient(Name = "http://loalhost:5000")]
    public interface IService4
    {
        [GetMapping(Route = "/service1")]
        string Get(string name);
    }

    [FeignClient(Name = "http://loalhost:5000")]
    public interface IService5
    {
        [GetMapping(Route = "/service1")]
        Task<string> GetAsync(string name);
    }

    [NacosFeignClient(Name = "test-1")]
    public interface IService6
    {
        [GetMapping(Route = "/service1")]
        Task<string> GetAsync(string name);
    }

    [NacosFeignClient(Name = "App1")]
    public interface IService7
    {
        [GetMapping(Route = "/Service")]
        Task<string> GetAsync();
    }
}
