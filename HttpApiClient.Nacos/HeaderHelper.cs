using System.Net.Http;

namespace HttpApiClient.Nacos
{
    internal class HeaderHelper
    {
        /// <summary>
        /// 复制认证头
        /// </summary>
        /// <param name="client"></param>
        internal static void CopyHeader(HttpClient client)
        {
            if (!string.IsNullOrEmpty(HeaderAsyncLocal.TokenHolder.Value))
            {
                client.DefaultRequestHeaders.Add("Authorization", HeaderAsyncLocal.TokenHolder.Value);
                HeaderAsyncLocal.TokenHolder.Value = null;
            }
            if (!string.IsNullOrEmpty(HeaderAsyncLocal.SysCodeHolder.Value))
            {
                client.DefaultRequestHeaders.Add("SysCode", HeaderAsyncLocal.SysCodeHolder.Value);
                HeaderAsyncLocal.SysCodeHolder.Value = null;
            }
        }
    }
}
