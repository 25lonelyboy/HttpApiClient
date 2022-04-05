using System;

namespace HttpApiClient.Attributes
{
    /// <summary>
    /// Post路径映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PostMappingAttribute:MappingAttribute
    {
    }
}
