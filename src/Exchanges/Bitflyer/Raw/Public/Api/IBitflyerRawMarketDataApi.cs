using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using BitflyerRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Public.Api;

/// <summary>
/// bitFlyer Public REST API の Mirror Raw インターフェース。
/// </summary>
public interface IBitflyerRawMarketDataApi
{
    Task<Call<BitflyerRequests.GetTickerRequest, Ticker>> GetTickerCallAsync(
        BitflyerRequests.GetTickerRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetBoardRequest, Board>> GetBoardCallAsync(
        BitflyerRequests.GetBoardRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetExecutionsRequest, IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsPublicCallAsync(
        BitflyerRequests.GetExecutionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetMarketsRequest, IReadOnlyList<Market>>> GetMarketsCallAsync(
        BitflyerRequests.GetMarketsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetChatsRequest, IReadOnlyList<Chat>>> GetChatsCallAsync(
        BitflyerRequests.GetChatsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetHealthRequest, HealthResponse>> GetHealthCallAsync(
        BitflyerRequests.GetHealthRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetBoardStateRequest, BoardStateResponse>> GetBoardStateCallAsync(
        BitflyerRequests.GetBoardStateRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<BitflyerRequests.GetCorporateLeverageRequest, CorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        BitflyerRequests.GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetFundingRateRequest, FundingRateResponse>> GetFundingRateCallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default);
}
