using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Spec.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Public REST API の Raw アクセス（認証不要）。
/// </summary>
internal interface IBittradePublicApi
{
    Task<Call<GetMergedTickerRequest, RawMergedResponse>> GetMergedTickerAsync(
        GetMergedTickerRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetDepthRequest, RawDepthResponse>> GetDepthAsync(
        GetDepthRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTradesRequest, RawTradeResponse>> GetTradesAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetSymbolsRequest, RawSymbolsResponse>> GetSymbolsAsync(
        GetSymbolsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCurrenciesRequest, RawCurrenciesResponse>> GetCurrenciesAsync(
        GetCurrenciesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTimestampRequest, RawTimestampResponse>> GetTimestampAsync(
        GetTimestampRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetKlinesRequest, RawKlinesResponse>> GetKlinesAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTickersRequest, RawTickersResponse>> GetTickersAsync(
        GetTickersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTradeHistoryRequest, RawTradeHistoryResponse>> GetTradeHistoryAsync(
        GetTradeHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetRetailMaintainTimeRequest, RawRetailMaintainTimeResponse>> GetRetailMaintainTimeAsync(
        GetRetailMaintainTimeRequest request,
        CancellationToken cancellationToken = default);
}
