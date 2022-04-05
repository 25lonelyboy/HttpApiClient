using System;

namespace HttpApiClient.Attributes
{
    /// <summary>
    /// 路径映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MappingAttribute :
        Attribute
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Route { get; set; }
    }
}
