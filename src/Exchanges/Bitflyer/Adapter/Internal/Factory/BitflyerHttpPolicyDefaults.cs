using ExchangeApi.Transport.Policy;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;

/// <summary>
/// bitFlyer 向けの HTTP ポリシー既定値。
/// </summary>
public static class BitflyerHttpPolicyDefaults
{
    public static HttpPolicyOptions Create() => new();
}
