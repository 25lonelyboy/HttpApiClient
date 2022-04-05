namespace HttpApiClient.Proxy
{
    public interface IFeignProxyFactory
    {
        T GetProxy<T>();

        T GetHytrixProxy<T>();
    }
}
