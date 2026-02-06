using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Requests;
using BitflyerRequests = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Requests;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Constants;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Public.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Api;

internal sealed class BitflyerPublicApi
{
    private readonly IBitflyerWireCallExecutor _wire;
    private readonly BitflyerRawCallExecutor _executor;

    public BitflyerPublicApi(IBitflyerWireCallExecutor wire, BitflyerRawCallExecutor executor)
    {
        _wire = wire ?? throw new System.ArgumentNullException(nameof(wire));
        _executor = executor ?? throw new System.ArgumentNullException(nameof(executor));
    }

    public Task<Call<BitflyerRequests.GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        BitflyerRequests.GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetTicker),
            BitflyerPublicEndpoints.GetTicker(request.ProductCode.Value),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<GetTickerResponse>(json, Component(BitflyerEndpointIds.GetTicker)));

    public Task<Call<BitflyerRequests.GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        BitflyerRequests.GetBoardRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetBoard),
            BitflyerPublicEndpoints.GetBoard(request.ProductCode.Value),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<GetBoardResponse>(json, Component(BitflyerEndpointIds.GetBoard)));

    public Task<Call<BitflyerRequests.GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        BitflyerRequests.GetExecutionsPublicRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetExecutionsPublic),
            BitflyerPublicEndpoints.GetExecutionsPublic(
                request.ProductCode.Value,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<GetExecutionsPublicResponse>(
                json,
                Component(BitflyerEndpointIds.GetExecutionsPublic)));

    public Task<Call<BitflyerRequests.GetMarketsRequest, GetMarketsResponse>> GetMarketsCallAsync(
        BitflyerRequests.GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetMarkets),
            BitflyerPublicEndpoints.GetMarkets(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<GetMarketsResponse>(
                json,
                Component(BitflyerEndpointIds.GetMarkets)));

    public Task<Call<BitflyerRequests.GetChatsRequest, GetChatsResponse>> GetChatsCallAsync(
        BitflyerRequests.GetChatsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetChats),
            BitflyerPublicEndpoints.GetChats(request.FromDate?.Value),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<GetChatsResponse>(
                json,
                Component(BitflyerEndpointIds.GetChats)));

    public Task<Call<BitflyerRequests.GetHealthRequest, GetHealthResponse>> GetHealthCallAsync(
        BitflyerRequests.GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetHealth),
            BitflyerPublicEndpoints.GetHealth(request.ProductCode.Value),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<GetHealthResponse>(json, Component(BitflyerEndpointIds.GetHealth)));

    public Task<Call<BitflyerRequests.GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateCallAsync(
        BitflyerRequests.GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetBoardState),
            BitflyerPublicEndpoints.GetBoardState(request.ProductCode.Value),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<GetBoardStateResponse>(
                json,
                Component(BitflyerEndpointIds.GetBoardState)));

    public Task<Call<BitflyerRequests.GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        BitflyerRequests.GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetCorporateLeverage),
            BitflyerPublicEndpoints.GetCorporateLeverage(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<GetCorporateLeverageResponse>(
                json,
                Component(BitflyerEndpointIds.GetCorporateLeverage)));

    public Task<Call<GetFundingRateRequest, GetFundingRateResponse>> GetFundingRateCallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetFundingRate),
            BitflyerPublicEndpoints.GetFundingRate(request.ProductCode.Value),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<GetFundingRateResponse>(
                json,
                Component(BitflyerEndpointIds.GetFundingRate)));

    private async Task<Call<TReq, TRes>> SendAndParse<TReq, TRes>(
        TReq request,
        string component,
        WireCallSpec spec,
        CancellationToken cancellationToken,
        Func<string, TRes> parse)
    {
        var wireCall = await _wire.SendAsync(spec, cancellationToken).ConfigureAwait(false);
        return _executor.Parse(request, component, wireCall, parse);
    }

    private static string Component(string endpointId) => $"Bitflyer.{endpointId}";
}
