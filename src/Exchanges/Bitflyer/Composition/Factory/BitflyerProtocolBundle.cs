using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

public sealed class BitflyerProtocolBundle
    : IDisposable
{
    private IDisposable? _lifetimeLease;

    public required IBitflyerPublicProtocolApi Public { get; init; }
    public IBitflyerPrivateProtocolApi? Private { get; init; }
    internal IDisposable? LifetimeLease { init => _lifetimeLease = value; }

    public void Dispose()
    {
        Interlocked.Exchange(ref _lifetimeLease, null)?.Dispose();
    }
}
