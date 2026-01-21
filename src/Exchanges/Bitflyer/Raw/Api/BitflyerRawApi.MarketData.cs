using System.Collections.Generic;
using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using BitflyerRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Api;

public sealed partial class BitflyerRawApi
{
    public Task<Call<BitflyerRequests.GetTickerRequest, Ticker>> GetTickerCallAsync(
        BitflyerRequests.GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetTicker",
            BitflyerEndpoints.GetTicker(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<Ticker>(json, "Bitflyer.GetTicker"));

    public Task<Call<BitflyerRequests.GetBoardRequest, Board>> GetBoardCallAsync(
        BitflyerRequests.GetBoardRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetBoard",
            BitflyerEndpoints.GetBoard(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<Board>(json, "Bitflyer.GetBoard"));

    public Task<Call<BitflyerRequests.GetExecutionsRequest, IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsPublicCallAsync(
        BitflyerRequests.GetExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetExecutions",
            BitflyerEndpoints.GetExecutionsPublic(
                request.ProductCode,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ExecutionPublicResponse>>(
                json,
                "Bitflyer.GetExecutions"));

    public Task<Call<BitflyerRequests.GetMarketsRequest, IReadOnlyList<Market>>> GetMarketsCallAsync(
        BitflyerRequests.GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetMarkets",
            BitflyerEndpoints.GetMarkets(request.Region),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<Market>>(
                json,
                "Bitflyer.GetMarkets"));

    public Task<Call<BitflyerRequests.GetChatsRequest, IReadOnlyList<Chat>>> GetChatsCallAsync(
        BitflyerRequests.GetChatsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetChats",
            BitflyerEndpoints.GetChats(request.FromDate, request.Region),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<Chat>>(
                json,
                "Bitflyer.GetChats"));

    public Task<Call<BitflyerRequests.GetHealthRequest, HealthResponse>> GetHealthCallAsync(
        BitflyerRequests.GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetHealth",
            BitflyerEndpoints.GetHealth(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<HealthResponse>(json, "Bitflyer.GetHealth"));

    public Task<Call<BitflyerRequests.GetBoardStateRequest, BoardStateResponse>> GetBoardStateCallAsync(
        BitflyerRequests.GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetBoardState",
            BitflyerEndpoints.GetBoardState(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<BoardStateResponse>(
                json,
                "Bitflyer.GetBoardState"));

    public Task<Call<BitflyerRequests.GetCorporateLeverageRequest, CorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        BitflyerRequests.GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetCorporateLeverage",
            BitflyerEndpoints.GetCorporateLeverage(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<CorporateLeverageResponse>(
                json,
                "Bitflyer.GetCorporateLeverage"));

    public Task<Call<GetFundingRateRequest, FundingRateResponse>> GetFundingRateCallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetFundingRate",
            BitflyerEndpoints.GetFundingRate(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<FundingRateResponse>(
                json,
                "Bitflyer.GetFundingRate"));
}
