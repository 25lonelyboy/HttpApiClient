using HttpApiClient.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HttpApiClient.Proxy
{
    /// <summary>
    /// 接口动态代理
    /// </summary>
    /// <typeparam name="T">接口类型</typeparam>
    public abstract class AbstactFeignProxy<T> : DispatchProxy
    {
        /// <summary>
        /// 代理配置
        /// </summary>
        public IProxyConfiguration ProxyConfiguration { get; set; }

        /// <summary>
        /// 请求结果处理器
        /// </summary>
        protected IApiResultProcessor ApiResultProcessor => ProxyConfiguration.ServiceProvider.GetRequiredService<IApiResultProcessor>();
        
        /// <summary>
        /// 缓存
        /// TODO: 缓存是否有必要？
        /// </summary>
        private Dictionary<string, FeignMethodInfo> cachedMethod = new Dictionary<string, FeignMethodInfo>();

        /// <summary>
        ///  Whenever any method on the generated proxy type is called, this method is invoked  to dispatch control.
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var method = GetOrCacheMethod(targetMethod);

            //代理请求
            if (method.Method == HttpMethod.Get)
            {
                var methodName = method.IsAsync ? "GetAsync" : "Get";
                // 反射获取方法Get方法，注意这里需要指定AbstactHystrixFeignProxy的泛型类型
                var getMethod = typeof(AbstactFeignProxy<>).MakeGenericType(typeof(T))
                    .GetMethod(methodName);
                var curMethod = getMethod.MakeGenericMethod(method.ReturnType);
                var result = curMethod.Invoke(this, new object[] { method, args });
                return result;
            }
            else
            {
                var methodName = method.IsAsync ? "PostAsync" : "Post";
                var postMethod = typeof(AbstactFeignProxy<>).MakeGenericType(typeof(T))
                    .GetMethod(methodName);
                var curMethod = postMethod.MakeGenericMethod(method.ReturnType);
                if (args != null && args.Length == 1)
                {
                    var res = curMethod.Invoke(this, new object[] { method, args[0] });
                    return res;
                }
                var result = curMethod.Invoke(this, new object[] { method, null });
                return result;
            }
        }

        /// <summary>
        /// 调用Get请求
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<TResult> GetAsync<TResult>(FeignMethodInfo targetMethod, object[] args)
        {
            var urlParam = string.Format(targetMethod.UrlTemplate, args);
            if (targetMethod.Parameters.Length == 1 && !targetMethod.Parameters[0].ParameterType.IsPrimitive
                && targetMethod.Parameters[0].ParameterType != typeof(string))
            {
                urlParam = ModelToUriParam(args[0]);
            }
            var url = targetMethod.Url + urlParam;
            var httpRes = await GetRequestAsync(targetMethod.ServiceName, url, targetMethod);
            return await ApiResultProcessor.Process<TResult>(httpRes);
        }

        /// <summary>
        /// 调用post请求
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<TResult> PostAsync<TResult>(FeignMethodInfo targetMethod, object args)
        {
            var httpRes = await PostRequestAsync(targetMethod.ServiceName, targetMethod.Url, args, targetMethod);
            return await ApiResultProcessor.Process<TResult>(httpRes);
        }

        /// <summary>
        /// 调用Get请求
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TResult Get<TResult>(FeignMethodInfo targetMethod, object[] args)
        {
            var urlParam = string.Format(targetMethod.UrlTemplate, args);
            if (targetMethod.Parameters.Length == 1 && !targetMethod.Parameters[0].ParameterType.IsPrimitive
                && targetMethod.Parameters[0].ParameterType != typeof(string))
            {
                urlParam = ModelToUriParam(args[0]);
            }
            var url = targetMethod.Url + urlParam;
            var httpRes = GetRequestAsync(targetMethod.ServiceName, url, targetMethod).GetAwaiter().GetResult();
            return ApiResultProcessor.Process<TResult>(httpRes).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 调用post请求
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TResult Post<TResult>(FeignMethodInfo targetMethod, object args)
        {
            var httpRes = PostRequestAsync(targetMethod.ServiceName, targetMethod.Url, args, targetMethod).GetAwaiter().GetResult();
            return ApiResultProcessor.Process<TResult>(httpRes).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 缓存调用方法
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <returns></returns>
        protected FeignMethodInfo GetOrCacheMethod(MethodInfo targetMethod)
        {
            // 同一个接口的不同参数进行缓存
            return cachedMethod.GetOrAdd(targetMethod.Name + targetMethod.GetParameters()
                .Select(p => p.ParameterType.Name).JoinAsString("_"), 
                () =>
                {
                    var attr = targetMethod.GetCustomAttribute<MappingAttribute>(true);
                    if (attr == null)
                    {
                        throw new InvalidOperationException("请设置代理方式！");
                    }

                    var classAttr = typeof(T).GetCustomAttribute<FeignClientAttribute>();
                    var fallback = classAttr?.FallbackImpl;
                    var serviceName = classAttr?.Name;
                    var url = attr.Route;
                    var urlTemplate = "";
                    for (int i = 0; i < targetMethod.GetParameters().Length; i++)
                    {
                        urlTemplate += targetMethod.GetParameters()[i].Name + "={" + i + "}&";
                    }
                    urlTemplate = "?" + urlTemplate.Trim('&');
                    return new FeignMethodInfo()
                    {
                        Url = url,
                        ServiceName = serviceName,
                        Method = attr is PostMappingAttribute ? HttpMethod.Post : HttpMethod.Get,
                        ReturnType = targetMethod.ReturnType.BaseType == typeof(Task) ?
                        targetMethod.ReturnType.GetGenericArguments().FirstOrDefault()
                        : targetMethod.ReturnType,
                        IsAsync = targetMethod.ReturnType.BaseType == typeof(Task),
                        Fallback = fallback,
                        UrlTemplate = urlTemplate,
                        Parameters = targetMethod.GetParameters()
                    };
                });
        }

        /// <summary>
        /// 异步Get请求
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="targetMethod"></param>
        /// <returns></returns>
        protected abstract Task<HttpResponseMessage> GetRequestAsync(string serviceName, string url, FeignMethodInfo targetMethod);

        /// <summary>
        /// 异步Post请求
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="url"></param>
        /// <param name="arg"></param>
        /// <param name="targetMethod"></param>
        /// <returns></returns>
        protected abstract Task<HttpResponseMessage> PostRequestAsync(string serviceName, string url, object arg, FeignMethodInfo targetMethod);

        /// <summary>
        /// 对象转成url参数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string ModelToUriParam(object obj)
        {
            PropertyInfo[] propertis = obj.GetType().GetProperties();
            StringBuilder sb = new StringBuilder();
            sb.Append("?");
            foreach (var p in propertis)
            {
                var name = p.Name;
                JsonPropertyAttribute jsonPropertyAttribute = p.GetCustomAttribute<JsonPropertyAttribute>();
                if (jsonPropertyAttribute != null)
                {
                    name = jsonPropertyAttribute.PropertyName;
                }
                var v = p.GetValue(obj, null);
                if (v == null)
                    continue;

                if (v.GetType().IsAssignableTo(typeof(IEnumerable<string>)))
                {
                    foreach (var item in (IEnumerable<string>)v)
                    {
                        sb.Append(name);
                        sb.Append("=");
                        sb.Append(HttpUtility.UrlEncode(item.ToString()));
                        sb.Append("&");
                    }
                }
                else
                {
                    sb.Append(name);
                    sb.Append("=");
                    sb.Append(HttpUtility.UrlEncode(v.ToString()));
                    sb.Append("&");
                }
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}
