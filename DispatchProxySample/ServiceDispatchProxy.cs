using System.Reflection;
using System.Threading.Tasks;

namespace DispatchProxySample
{
    public class ServiceDispatchProxy : DispatchProxy
    {
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return Task.FromResult($"{targetMethod.Name}: {args[0]}");
        }
    }
}
