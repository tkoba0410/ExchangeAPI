using System.Threading;
using System.Threading.Tasks;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Public REST API の Raw アクセス（認証不要）。
/// </summary>
internal interface IBittradePublicApi
{
    Task<MergedResponse> GetMergedTickerAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<DepthResponse> GetDepthAsync(Symbol symbol, string? type = null, CancellationToken cancellationToken = default);

    Task<TradeResponse> GetTradesAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<SymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default);

    Task<CurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default);

    Task<TimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default);

    Task<KlinesResponse> GetKlinesAsync(Symbol symbol, string period, int? size = null, CancellationToken cancellationToken = default);

    Task<TickersResponse> GetTickersAsync(CancellationToken cancellationToken = default);

    Task<TradeHistoryResponse> GetTradeHistoryAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<RetailMaintainTimeResponse> GetRetailMaintainTimeAsync(CancellationToken cancellationToken = default);
}
