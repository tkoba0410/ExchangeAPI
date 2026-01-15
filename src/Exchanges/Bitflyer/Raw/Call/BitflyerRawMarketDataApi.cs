using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using BitflyerRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Call;

/// <summary>
/// bitFlyer Public REST API の Mirror Raw 実装。
/// </summary>
internal sealed class BitflyerRawMarketDataApi : IBitflyerRawMarketDataApi
{
    private readonly IWireTransport _wire;

    public BitflyerRawMarketDataApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Call<BitflyerRequests.GetTickerRequest, Ticker>> GetTickerAsync(
        BitflyerRequests.GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetTicker",
            BitflyerEndpoints.GetTicker(request.ProductCode, request.UseAliasPath),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<Ticker>(json, "Bitflyer.GetTicker"));

    public Task<Call<BitflyerRequests.GetBoardRequest, Board>> GetBoardAsync(
        BitflyerRequests.GetBoardRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetBoard",
            BitflyerEndpoints.GetBoard(request.ProductCode, request.UseAliasPath),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<Board>(json, "Bitflyer.GetBoard"));

    public Task<Call<BitflyerRequests.GetExecutionsRequest, IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsAsync(
        BitflyerRequests.GetExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetExecutions",
            BitflyerEndpoints.GetExecutions(
                request.ProductCode,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture),
                request.UseAliasPath),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ExecutionPublicResponse>>(
                json,
                "Bitflyer.GetExecutions"));

    public Task<Call<BitflyerRequests.GetMarketsRequest, IReadOnlyList<Market>>> GetMarketsAsync(
        BitflyerRequests.GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetMarkets",
            BitflyerEndpoints.GetMarkets(request.Region, request.UseAliasPath),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<Market>>(
                json,
                "Bitflyer.GetMarkets"));

    public Task<Call<BitflyerRequests.GetChatsRequest, IReadOnlyList<Chat>>> GetChatsAsync(
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

    public Task<Call<BitflyerRequests.GetHealthRequest, HealthResponse>> GetHealthAsync(
        BitflyerRequests.GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetHealth",
            BitflyerEndpoints.GetHealth(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<HealthResponse>(json, "Bitflyer.GetHealth"));

    public Task<Call<BitflyerRequests.GetBoardStateRequest, BoardStateResponse>> GetBoardStateAsync(
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

    public Task<Call<BitflyerRequests.GetCorporateLeverageRequest, CorporateLeverageResponse>> GetCorporateLeverageAsync(
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

    public Task<Call<GetFundingRateRequest, FundingRateResponse>> GetFundingRateAsync(
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

    private async Task<Call<TReq, TRes>> SendAndParse<TReq, TRes>(
        TReq request,
        string component,
        WireCallSpec spec,
        CancellationToken cancellationToken,
        Func<string, TRes> parse)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (parse is null) throw new ArgumentNullException(nameof(parse));

        var wireCall = await _wire.SendAsync(ExchangeCode.Bitflyer, spec, cancellationToken).ConfigureAwait(false);
        return CreateCall(request, component, wireCall, parse);
    }

    private static Call<TReq, TRes> CreateCall<TReq, TRes>(
        TReq request,
        string component,
        Call<WireCallSpec, WireResponse> wireCall,
        Func<string, TRes> parse)
    {
        var meta = new CallMeta(
            Layer: "Raw",
            Component: component,
            Tags: null,
            Children: new[] { wireCall.Id });

        return wireCall.Result switch
        {
            CallResult<WireResponse>.Err err => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(err.Error),
                Meta: meta),
            CallResult<WireResponse>.Ok ok => CreateOkCall(request, component, ok.Response, wireCall, parse, meta),
            _ => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(new CallError(CallErrorKind.Unknown, "Wire call returned unknown result.")),
                Meta: meta)
        };
    }

    private static Call<TReq, TRes> CreateOkCall<TReq, TRes>(
        TReq request,
        string component,
        WireResponse response,
        Call<WireCallSpec, WireResponse> wireCall,
        Func<string, TRes> parse,
        CallMeta meta)
    {
        var metaWithRaw = meta with { RawJson = response.Json };
        if (response.StatusCode is < 200 or >= 300)
        {
            var error = new CallError(
                CallErrorKind.Http,
                $"{component} failed with status {response.StatusCode}.",
                HttpStatus: response.StatusCode,
                BodySnippet: Snip(response.Json));
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(error),
                Meta: metaWithRaw);
        }

        try
        {
            var parsed = parse(response.Json);
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Ok(parsed),
                Meta: metaWithRaw);
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            var error = new CallError(
                CallErrorKind.Codec,
                $"{component} failed to parse response.",
                ex,
                response.StatusCode,
                Snip(response.Json));
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(error),
                Meta: metaWithRaw);
        }
    }

    private static string? Snip(string? json)
    {
        if (string.IsNullOrEmpty(json)) return json;
        return json.Length <= 512 ? json : json[..512];
    }
}
