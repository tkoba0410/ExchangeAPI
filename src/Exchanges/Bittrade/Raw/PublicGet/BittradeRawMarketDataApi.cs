using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class BittradeRawMarketDataApi : IBittradeRawMarketDataApi
{
    private readonly IBittradePublicApi _publicApi;

    public BittradeRawMarketDataApi(IBittradePublicApi publicApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
    }

    public Task<RawMergedResponse> GetTickerAsync(RawSymbol symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetMergedTickerAsync(EnsureSymbol(symbol), cancellationToken);

    public Task<RawDepthResponse> GetOrderBookAsync(RawSymbol symbol, string? type = null, CancellationToken cancellationToken = default) =>
        _publicApi.GetDepthAsync(EnsureSymbol(symbol), type, cancellationToken);

    public Task<RawTradeResponse> GetTradesAsync(RawSymbol symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetTradesAsync(EnsureSymbol(symbol), cancellationToken);

    private static RawSymbol EnsureSymbol(RawSymbol symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol.Value))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        return symbol;
    }
}
