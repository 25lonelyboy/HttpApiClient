using System.Threading;

namespace HttpApiClient.Nacos
{
    /// <summary>
    /// 传递用Header信息
    /// </summary>
    public class HeaderAsyncLocal
    {
        /// <summary>
        /// Token
        /// </summary>
        public static AsyncLocal<string> TokenHolder { get; } = new AsyncLocal<string>();

        /// <summary>
        /// SysCode
        /// </summary>
        public static AsyncLocal<string> SysCodeHolder { get; } = new AsyncLocal<string>();
    }
}
