using ExchangeApi.Exchanges.Binance.Native.Public.Api;

namespace ExchangeApi.Exchanges.Binance.Composition.Factory;

public sealed class BinanceNativeBundle
    : IDisposable
{
    private IDisposable? _lifetimeLease;

    public required IBinancePublicNativeApi Public { get; init; }
    public required BinanceProtocolBundle Protocol { get; init; }
    internal IDisposable? LifetimeLease { init => _lifetimeLease = value; }

    public void Dispose()
    {
        Interlocked.Exchange(ref _lifetimeLease, null)?.Dispose();
    }
}
