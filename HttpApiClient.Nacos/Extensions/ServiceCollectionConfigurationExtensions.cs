using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionConfigurationExtensions
    {
        public static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            HostBuilderContext singletonInstanceOrNull = services.GetSingletonInstanceOrNull<HostBuilderContext>();
            if (singletonInstanceOrNull?.Configuration != null)
            {
                return singletonInstanceOrNull.Configuration as IConfigurationRoot;
            }

            return services.GetSingletonInstance<IConfiguration>();
        }

        public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
        {
            return (T)(services.FirstOrDefault((ServiceDescriptor d) => d.ServiceType == typeof(T))?.ImplementationInstance);
        }

        public static T GetSingletonInstance<T>(this IServiceCollection services)
        {
            return services.GetSingletonInstanceOrNull<T>() ?? throw new InvalidOperationException("Could not find singleton service: " + typeof(T).AssemblyQualifiedName);
        }
    }
}
