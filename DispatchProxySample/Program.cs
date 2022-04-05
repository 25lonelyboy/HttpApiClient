using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace DispatchProxySample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var service = DispatchProxy.Create<IService, ServiceDispatchProxy>();
            var result = service.GetAsync("test").Result;
            Console.WriteLine(result);

            //var method = typeof(Test).GetMethod("Get");
            //var cur = method.MakeGenericMethod(typeof(List<string>));
            //var result = cur.Invoke(new Test(), null);

            Console.ReadKey();
        }
    }

    public class Test
    {
        public Task<T> Get<T>() where T : new()
        {
            var result = new T();
            return Task.FromResult(result);
        }
    }
}
