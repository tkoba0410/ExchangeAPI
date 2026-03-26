using ExchangeApi.Exchanges.Binance.Protocol.Public.Api;

namespace ExchangeApi.Exchanges.Binance.Composition.Factory;

public sealed class BinanceProtocolBundle
    : IDisposable
{
    private IDisposable? _lifetimeLease;

    public required IBinancePublicProtocolApi Public { get; init; }
    internal IDisposable? LifetimeLease { init => _lifetimeLease = value; }

    public void Dispose()
    {
        Interlocked.Exchange(ref _lifetimeLease, null)?.Dispose();
    }
}
