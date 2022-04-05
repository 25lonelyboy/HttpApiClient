using HttpApiClient.Attributes;
using System;

namespace HttpApiClient.Nacos.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class NacosFeignClientAttribute : FeignClientAttribute
    {
    }
}
