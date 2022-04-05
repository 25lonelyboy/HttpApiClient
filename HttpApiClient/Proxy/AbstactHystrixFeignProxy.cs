using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace HttpApiClient.Proxy
{
    /// <summary>
    /// 接口动态代理, 带降级处理功能
    /// </summary>
    /// <typeparam name="T">接口类型</typeparam>
    public abstract class AbstactHystrixFeignProxy<T> : AbstactFeignProxy<T>
    {
        /// <summary>
        ///  Whenever any method on the generated proxy type is called, this method is invoked  to dispatch control.
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var method = GetOrCacheMethod(targetMethod);

            // 降级
            if (method.Fallback != null)
            {
                return HystrixInvoke(targetMethod, args, method);
            }

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

        protected abstract object HystrixInvoke(MethodInfo targetMethod, object[] args, FeignMethodInfo methodInfo);
    }
}
