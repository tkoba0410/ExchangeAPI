using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using BitflyerRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Public;

/// <summary>
/// bitFlyer Public REST API の Mirror Raw インターフェース。
/// </summary>
public interface IBitflyerRawMarketDataApi
{
    Task<Call<BitflyerRequests.GetTickerRequest, Ticker>> GetTickerAsync(
        BitflyerRequests.GetTickerRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetBoardRequest, Board>> GetBoardAsync(
        BitflyerRequests.GetBoardRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetExecutionsRequest, IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsAsync(
        BitflyerRequests.GetExecutionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetMarketsRequest, IReadOnlyList<Market>>> GetMarketsAsync(
        BitflyerRequests.GetMarketsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetChatsRequest, IReadOnlyList<Chat>>> GetChatsAsync(
        BitflyerRequests.GetChatsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetHealthRequest, HealthResponse>> GetHealthAsync(
        BitflyerRequests.GetHealthRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetBoardStateRequest, BoardStateResponse>> GetBoardStateAsync(
        BitflyerRequests.GetBoardStateRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetCorporateLeverageRequest, CorporateLeverageResponse>> GetCorporateLeverageAsync(
        BitflyerRequests.GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetFundingRateRequest, FundingRateResponse>> GetFundingRateAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default);
}
