using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Call;

internal sealed class BitflyerRawTradingApi : IBitflyerRawTradingApi
{
    private readonly IWireTransport _wire;

    public BitflyerRawTradingApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Call<SendChildOrderRequest, RawSendChildOrderResponse>> SendChildOrderAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.SendChildOrder",
            BitflyerEndpoints.SendChildOrder(
                BitflyerRawJson.SerializeOrThrow(request.Body, "Bitflyer.SendChildOrder")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawSendChildOrderResponse>(
                json,
                "Bitflyer.SendChildOrder"));

    public Task<Call<CancelChildOrderRawRequest, RawCancelChildOrderResponse>> CancelChildOrderAsync(
        CancelChildOrderRawRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.CancelChildOrder",
            BitflyerEndpoints.CancelChildOrder(
                BitflyerRawJson.SerializeOrThrow(request.Body, "Bitflyer.CancelChildOrder")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelChildOrderResponse>(
                json,
                "Bitflyer.CancelChildOrder"));

    public Task<Call<GetChildOrdersRequest, IReadOnlyList<RawGetChildOrdersResponse>>> GetChildOrdersAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetChildOrders",
            BitflyerEndpoints.GetChildOrders(
                request.ProductCode,
                request.ChildOrderStatusState,
                request.ChildOrderAcceptanceId,
                request.ChildOrderId,
                request.Count,
                request.Before,
                request.After),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawGetChildOrdersResponse>>(
                json,
                "Bitflyer.GetChildOrders"));

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
