using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer Public REST API の Mirror Raw インターフェース。
/// </summary>
public interface IBitflyerRawMarketDataApi
{
    Task<Call<GetTickerRequest, Ticker>> GetTickerAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardRequest, Board>> GetBoardAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsRequest, IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsAsync(
        GetExecutionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetMarketsRequest, IReadOnlyList<Market>>> GetMarketsAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetChatsRequest, IReadOnlyList<Chat>>> GetChatsAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetHealthRequest, HealthResponse>> GetHealthAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardStateRequest, BoardStateResponse>> GetBoardStateAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCorporateLeverageRequest, CorporateLeverageResponse>> GetCorporateLeverageAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetFundingRateRequest, FundingRateResponse>> GetFundingRateAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default);
}
