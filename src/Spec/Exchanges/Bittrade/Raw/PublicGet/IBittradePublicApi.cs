using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Wire.Types;
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

    Task<BittradeRawCall<RawMergedResponse, JsonElement>> GetMergedTickerCallAsync(
        RawSymbol symbol,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawDepthResponse, JsonElement>> GetDepthCallAsync(
        RawSymbol symbol,
        string? type = null,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawTradeResponse, JsonElement>> GetTradesCallAsync(
        RawSymbol symbol,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawSymbolsResponse, JsonElement>> GetSymbolsCallAsync(
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawCurrenciesResponse, JsonElement>> GetCurrenciesCallAsync(
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawTimestampResponse, JsonElement>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawKlinesResponse, JsonElement>> GetKlinesCallAsync(
        RawSymbol symbol,
        string period,
        int? size = null,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawTickersResponse, JsonElement>> GetTickersCallAsync(
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawTradeHistoryResponse, JsonElement>> GetTradeHistoryCallAsync(
        RawSymbol symbol,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawRetailMaintainTimeResponse, JsonElement>> GetRetailMaintainTimeCallAsync(
        CancellationToken cancellationToken = default);
}
