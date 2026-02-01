using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Requests;
using BitflyerRequests = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Requests;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Public.Endpoints;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Api;

internal sealed class BitflyerPublicApi
{
    private readonly BitflyerRawCallExecutor _executor;

    public BitflyerPublicApi(BitflyerRawCallExecutor executor)
    {
        _executor = executor;
    }

    public Task<Call<BitflyerRequests.GetTickerRequest, Ticker>> GetTickerCallAsync(
        BitflyerRequests.GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetTicker",
            BitflyerPublicEndpoints.GetTicker(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<Ticker>(json, "Bitflyer.GetTicker"));

    public Task<Call<BitflyerRequests.GetBoardRequest, Board>> GetBoardCallAsync(
        BitflyerRequests.GetBoardRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetBoard",
            BitflyerPublicEndpoints.GetBoard(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<Board>(json, "Bitflyer.GetBoard"));

    public Task<Call<BitflyerRequests.GetExecutionsRequest, IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsPublicCallAsync(
        BitflyerRequests.GetExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetExecutions",
            BitflyerPublicEndpoints.GetExecutionsPublic(
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
        _executor.SendAndParse(
            request,
            "Bitflyer.GetMarkets",
            BitflyerPublicEndpoints.GetMarkets(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<Market>>(
                json,
                "Bitflyer.GetMarkets"));

    public Task<Call<BitflyerRequests.GetChatsRequest, IReadOnlyList<Chat>>> GetChatsCallAsync(
        BitflyerRequests.GetChatsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetChats",
            BitflyerPublicEndpoints.GetChats(request.FromDate),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<Chat>>(
                json,
                "Bitflyer.GetChats"));

    public Task<Call<BitflyerRequests.GetHealthRequest, HealthResponse>> GetHealthCallAsync(
        BitflyerRequests.GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetHealth",
            BitflyerPublicEndpoints.GetHealth(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<HealthResponse>(json, "Bitflyer.GetHealth"));

    public Task<Call<BitflyerRequests.GetBoardStateRequest, BoardStateResponse>> GetBoardStateCallAsync(
        BitflyerRequests.GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetBoardState",
            BitflyerPublicEndpoints.GetBoardState(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<BoardStateResponse>(
                json,
                "Bitflyer.GetBoardState"));

    public Task<Call<BitflyerRequests.GetCorporateLeverageRequest, CorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        BitflyerRequests.GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetCorporateLeverage",
            BitflyerPublicEndpoints.GetCorporateLeverage(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<CorporateLeverageResponse>(
                json,
                "Bitflyer.GetCorporateLeverage"));

    public Task<Call<GetFundingRateRequest, FundingRateResponse>> GetFundingRateCallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetFundingRate",
            BitflyerPublicEndpoints.GetFundingRate(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<FundingRateResponse>(
                json,
                "Bitflyer.GetFundingRate"));
}
