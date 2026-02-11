using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Requests;
using ExchangeApi.Exchanges.Common.Raw.Api;
using Requests = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Requests;
using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Exchanges.Bitflyer.Wire.Internal;
using ExchangeApi.Exchanges.Bitflyer.Wire.Public.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Public.Api;

internal sealed class PublicApi
{
    private readonly IWireCallExecutor _wire;
    private readonly RawCallExecutor _executor;

    public PublicApi(IWireCallExecutor wire, RawCallExecutor executor)
    {
        _wire = wire ?? throw new System.ArgumentNullException(nameof(wire));
        _executor = executor ?? throw new System.ArgumentNullException(nameof(executor));
    }

    public Task<Call<Requests.GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        Requests.GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetTicker),
            PublicEndpoints.GetTicker(request.ProductCode.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetTickerResponse>(json, Component(EndpointIds.GetTicker)));

    public Task<Call<Requests.GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        Requests.GetBoardRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetBoard),
            PublicEndpoints.GetBoard(request.ProductCode.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetBoardResponse>(json, Component(EndpointIds.GetBoard)));

    public Task<Call<Requests.GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        Requests.GetExecutionsPublicRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetExecutionsPublic),
            PublicEndpoints.GetExecutionsPublic(
                request.ProductCode.Value,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetExecutionsPublicResponse>(
                json,
                Component(EndpointIds.GetExecutionsPublic)));

    public Task<Call<Requests.GetMarketsRequest, GetMarketsResponse>> GetMarketsCallAsync(
        Requests.GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetMarkets),
            PublicEndpoints.GetMarkets(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetMarketsResponse>(
                json,
                Component(EndpointIds.GetMarkets)));

    public Task<Call<Requests.GetChatsRequest, GetChatsResponse>> GetChatsCallAsync(
        Requests.GetChatsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetChats),
            PublicEndpoints.GetChats(request.FromDate?.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetChatsResponse>(
                json,
                Component(EndpointIds.GetChats)));

    public Task<Call<Requests.GetHealthRequest, GetHealthResponse>> GetHealthCallAsync(
        Requests.GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetHealth),
            PublicEndpoints.GetHealth(request.ProductCode.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetHealthResponse>(json, Component(EndpointIds.GetHealth)));

    public Task<Call<Requests.GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateCallAsync(
        Requests.GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetBoardState),
            PublicEndpoints.GetBoardState(request.ProductCode.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetBoardStateResponse>(
                json,
                Component(EndpointIds.GetBoardState)));

    public Task<Call<Requests.GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        Requests.GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetCorporateLeverage),
            PublicEndpoints.GetCorporateLeverage(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetCorporateLeverageResponse>(
                json,
                Component(EndpointIds.GetCorporateLeverage)));

    public Task<Call<GetFundingRateRequest, GetFundingRateResponse>> GetFundingRateCallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetFundingRate),
            PublicEndpoints.GetFundingRate(request.ProductCode.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetFundingRateResponse>(
                json,
                Component(EndpointIds.GetFundingRate)));

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
