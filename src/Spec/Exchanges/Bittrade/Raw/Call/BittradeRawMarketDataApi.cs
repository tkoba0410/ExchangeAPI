using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Call;

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
        _publicApi.GetDepthAsync(EnsureSymbol(symbol), ApplyDefaultDepthType(type), cancellationToken);

    public Task<RawTradeResponse> GetTradesAsync(RawSymbol symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetTradesAsync(EnsureSymbol(symbol), cancellationToken);

    public Task<BittradeRawCall<RawMergedResponse, JsonElement>> GetTickerCallAsync(
        RawSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetMergedTickerCallAsync(EnsureSymbol(symbol), cancellationToken);

    public Task<BittradeRawCall<RawDepthResponse, JsonElement>> GetOrderBookCallAsync(
        RawSymbol symbol,
        string? type = null,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetDepthCallAsync(EnsureSymbol(symbol), ApplyDefaultDepthType(type), cancellationToken);

    private static string ApplyDefaultDepthType(string? type) =>
        string.IsNullOrWhiteSpace(type) ? "step0" : type;

    public Task<BittradeRawCall<RawTradeResponse, JsonElement>> GetTradesCallAsync(
        RawSymbol symbol,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTradesCallAsync(EnsureSymbol(symbol), cancellationToken);

    private static RawSymbol EnsureSymbol(RawSymbol symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol.Value))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        return symbol;
    }
}
