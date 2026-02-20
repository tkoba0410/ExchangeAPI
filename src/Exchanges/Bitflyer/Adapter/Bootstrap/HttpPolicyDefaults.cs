using ExchangeApi.Transport.Policy;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Bootstrap;

/// <summary>
/// bitFlyer 向けの HTTP ポリシー既定値。
/// </summary>
public static class HttpPolicyDefaults
{
    public static HttpPolicyOptions Create() => new();
}
