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

    public Task<MergedResponse> GetMergedTickerAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        return _restClient.GetAsync<MergedResponse>(
            $"market/detail/merged?symbol={ToApiSymbol(symbol)}",
            cancellationToken: cancellationToken);
    }

    public Task<DepthResponse> GetDepthAsync(Symbol symbol, string? type = null, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var depthType = string.IsNullOrWhiteSpace(type) ? "step0" : type;
        return _restClient.GetAsync<DepthResponse>(
            $"market/depth?symbol={ToApiSymbol(symbol)}&type={depthType}",
            cancellationToken: cancellationToken);
    }

    public Task<TradeResponse> GetTradesAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        return _restClient.GetAsync<TradeResponse>(
            $"market/trade?symbol={ToApiSymbol(symbol)}",
            cancellationToken: cancellationToken);
    }

    public Task<SymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<SymbolsResponse>("v1/common/symbols", cancellationToken: cancellationToken);

    public Task<CurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<CurrenciesResponse>("v1/common/currencys", cancellationToken: cancellationToken);

    public Task<TimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<TimestampResponse>("v1/common/timestamp", cancellationToken: cancellationToken);

    public Task<KlinesResponse> GetKlinesAsync(Symbol symbol, string period, int? size = null, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        if (string.IsNullOrWhiteSpace(period)) throw new ArgumentException("period is required.", nameof(period));
        var sizeParam = size.HasValue ? $"&size={size.Value}" : string.Empty;
        return _restClient.GetAsync<KlinesResponse>(
            $"market/history/kline?period={period}&symbol={ToApiSymbol(symbol)}{sizeParam}",
            cancellationToken: cancellationToken);
    }

    public Task<TickersResponse> GetTickersAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<TickersResponse>("market/tickers", cancellationToken: cancellationToken);

    public Task<TradeHistoryResponse> GetTradeHistoryAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        return _restClient.GetAsync<TradeHistoryResponse>(
            $"market/history/trade?symbol={ToApiSymbol(symbol)}",
            cancellationToken: cancellationToken);
    }

    public Task<RetailMaintainTimeResponse> GetRetailMaintainTimeAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<RetailMaintainTimeResponse>("v1/retail/maintain/time", cancellationToken: cancellationToken);

    private static string ToApiSymbol(Symbol symbol) =>
        symbol.Value.Replace("/", string.Empty, StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

    private static void EnsureSymbol(Symbol symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol.Value))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }
    }
}
