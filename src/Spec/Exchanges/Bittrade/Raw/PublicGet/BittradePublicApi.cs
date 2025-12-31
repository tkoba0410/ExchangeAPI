using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Public REST API の Raw 実装。
/// </summary>
internal sealed class BittradePublicApi : IBittradePublicApi
{
    private readonly IRestClient _restClient;

    public BittradePublicApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<RawMergedResponse> GetMergedTickerAsync(RawSymbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        return _restClient.GetAsync<RawMergedResponse>(
            $"market/detail/merged?symbol={ToApiSymbol(symbol)}",
            cancellationToken: cancellationToken);
    }

    public Task<RawDepthResponse> GetDepthAsync(RawSymbol symbol, string? type = null, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var depthType = string.IsNullOrWhiteSpace(type) ? "step0" : type;
        return _restClient.GetAsync<RawDepthResponse>(
            $"market/depth?symbol={ToApiSymbol(symbol)}&type={depthType}",
            cancellationToken: cancellationToken);
    }

    public Task<RawTradeResponse> GetTradesAsync(RawSymbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        return _restClient.GetAsync<RawTradeResponse>(
            $"market/trade?symbol={ToApiSymbol(symbol)}",
            cancellationToken: cancellationToken);
    }

    public Task<RawSymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<RawSymbolsResponse>("v1/common/symbols", cancellationToken: cancellationToken);

    public Task<RawCurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<RawCurrenciesResponse>("v1/common/currencys", cancellationToken: cancellationToken);

    public Task<RawTimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<RawTimestampResponse>("v1/common/timestamp", cancellationToken: cancellationToken);

    public Task<RawKlinesResponse> GetKlinesAsync(RawSymbol symbol, string period, int? size = null, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        if (string.IsNullOrWhiteSpace(period)) throw new ArgumentException("period is required.", nameof(period));
        var sizeParam = size.HasValue ? $"&size={size.Value}" : string.Empty;
        return _restClient.GetAsync<RawKlinesResponse>(
            $"market/history/kline?period={period}&symbol={ToApiSymbol(symbol)}{sizeParam}",
            cancellationToken: cancellationToken);
    }

    public Task<RawTickersResponse> GetTickersAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<RawTickersResponse>("market/tickers", cancellationToken: cancellationToken);

    public Task<RawTradeHistoryResponse> GetTradeHistoryAsync(RawSymbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        return _restClient.GetAsync<RawTradeHistoryResponse>(
            $"market/history/trade?symbol={ToApiSymbol(symbol)}",
            cancellationToken: cancellationToken);
    }

    public Task<RawRetailMaintainTimeResponse> GetRetailMaintainTimeAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<RawRetailMaintainTimeResponse>("v1/retail/maintain/time", cancellationToken: cancellationToken);

    private static string ToApiSymbol(RawSymbol symbol) =>
        symbol.Value.Replace("/", string.Empty, StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

    private static void EnsureSymbol(RawSymbol symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol.Value))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }
    }
}
