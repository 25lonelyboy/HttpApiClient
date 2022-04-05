using System;

namespace HttpApiClient.Attributes
{
    /// <summary>
    /// Get路径映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GetMappingAttribute: MappingAttribute
    {
    }
}
