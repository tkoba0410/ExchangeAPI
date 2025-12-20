using System.Threading;
using System.Threading.Tasks;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Public REST API の Raw アクセス（認証不要）。
/// </summary>
internal interface IBittradePublicApi
{
    Task<MergedResponse> GetMergedTickerAsync(string symbol, CancellationToken cancellationToken = default);

    Task<DepthResponse> GetDepthAsync(string symbol, string? type = null, CancellationToken cancellationToken = default);

    Task<TradeResponse> GetTradesAsync(string symbol, CancellationToken cancellationToken = default);

    Task<SymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default);

    Task<CurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default);

    Task<TimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default);

    Task<KlinesResponse> GetKlinesAsync(string symbol, string period, int? size = null, CancellationToken cancellationToken = default);

    Task<TickersResponse> GetTickersAsync(CancellationToken cancellationToken = default);

    Task<TradeHistoryResponse> GetTradeHistoryAsync(string symbol, CancellationToken cancellationToken = default);

    Task<RetailMaintainTimeResponse> GetRetailMaintainTimeAsync(CancellationToken cancellationToken = default);
}
