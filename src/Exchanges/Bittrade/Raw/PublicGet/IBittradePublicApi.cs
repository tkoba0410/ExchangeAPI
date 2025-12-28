using System.Threading;
using System.Threading.Tasks;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Public REST API の Raw アクセス（認証不要）。
/// </summary>
internal interface IBittradePublicApi
{
    Task<RawMergedResponse> GetMergedTickerAsync(RawSymbol symbol, CancellationToken cancellationToken = default);

    Task<RawDepthResponse> GetDepthAsync(RawSymbol symbol, string? type = null, CancellationToken cancellationToken = default);

    Task<RawTradeResponse> GetTradesAsync(RawSymbol symbol, CancellationToken cancellationToken = default);

    Task<RawSymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default);

    Task<RawCurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default);

    Task<RawTimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default);

    Task<RawKlinesResponse> GetKlinesAsync(RawSymbol symbol, string period, int? size = null, CancellationToken cancellationToken = default);

    Task<RawTickersResponse> GetTickersAsync(CancellationToken cancellationToken = default);

    Task<RawTradeHistoryResponse> GetTradeHistoryAsync(RawSymbol symbol, CancellationToken cancellationToken = default);

    Task<RawRetailMaintainTimeResponse> GetRetailMaintainTimeAsync(CancellationToken cancellationToken = default);
}
