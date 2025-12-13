using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bittrade.RawApi;
using Common.Transport.Protocol;

namespace ExchangeApi.Adapter.Bittrade;

/// <summary>
/// Bittrade Public REST API（Ticker/Depth/Trades/ExchangeInfo）の Raw 実装。
/// </summary>
public sealed class BittradePublicApi : IBittradePublicApi
{
    private readonly IRestClient _restClient;

    public BittradePublicApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<BittradeMergedResponse> GetTickerRawAsync(string symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        return _restClient.GetAsync<BittradeMergedResponse>(
            $"market/detail/merged?symbol={ToApiSymbol(symbol)}",
            cancellationToken: cancellationToken);
    }

    public Task<BittradeDepthResponse> GetOrderBookRawAsync(string symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        return _restClient.GetAsync<BittradeDepthResponse>(
            $"market/depth?symbol={ToApiSymbol(symbol)}&type=step0",
            cancellationToken: cancellationToken);
    }

    public Task<BittradeTradeResponse> GetTradesRawAsync(string symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        return _restClient.GetAsync<BittradeTradeResponse>(
            $"market/trade?symbol={ToApiSymbol(symbol)}",
            cancellationToken: cancellationToken);
    }

    public Task<BittradeSymbolsResponse> GetSymbolsRawAsync(CancellationToken cancellationToken = default)
    {
        return _restClient.GetAsync<BittradeSymbolsResponse>(
            "v1/common/symbols",
            cancellationToken: cancellationToken);
    }

    private static string ToApiSymbol(string symbol) =>
        symbol.Replace("/", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

    private static void EnsureSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }
    }
}
