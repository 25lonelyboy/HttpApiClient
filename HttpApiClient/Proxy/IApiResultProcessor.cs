using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpApiClient.Proxy
{
    public interface IApiResultProcessor
    {
        Task<TResult> Process<TResult>(HttpResponseMessage result);
    }
}
