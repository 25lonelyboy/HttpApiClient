using System;

namespace HttpApiClient.Attributes
{
    /// <summary>
    /// 标识访问Eureka客户端接口
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class)]
    public class FeignClientAttribute : Attribute
    {
        public FeignClientAttribute()
        {
        }

        /// <summary>
        /// 添加http前缀的host名，与服务名一致
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 降级方法
        /// </summary>
        public Type FallbackImpl { get; set; }
    }
}
