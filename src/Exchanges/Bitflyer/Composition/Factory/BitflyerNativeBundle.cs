using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

public sealed class BitflyerNativeBundle
    : IDisposable
{
    private IDisposable? _lifetimeLease;

    public required IBitflyerPublicNativeApi Public { get; init; }
    public IBitflyerPrivateNativeApi? Private { get; init; }
    public required BitflyerProtocolBundle Protocol { get; init; }
    internal IDisposable? LifetimeLease { init => _lifetimeLease = value; }

    public void Dispose()
    {
        Interlocked.Exchange(ref _lifetimeLease, null)?.Dispose();
    }
}
