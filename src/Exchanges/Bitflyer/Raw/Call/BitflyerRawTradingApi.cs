using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Call;

internal sealed class BitflyerRawTradingApi : IBitflyerRawTradingApi
{
    private readonly IWireTransport _wire;

    public BitflyerRawTradingApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Call<string, RawSendChildOrderResponse>> SendChildOrderAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            bodyJson,
            "Bitflyer.SendChildOrder",
            BitflyerEndpoints.SendChildOrder(
                bodyJson),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawSendChildOrderResponse>(
                json,
                "Bitflyer.SendChildOrder"));

    public Task<Call<string, RawSendParentOrderResponse>> SendParentOrderAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            bodyJson,
            "Bitflyer.SendParentOrder",
            BitflyerEndpoints.SendParentOrder(
                bodyJson),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawSendParentOrderResponse>(
                json,
                "Bitflyer.SendParentOrder"));

    public Task<Call<Requests.CancelChildOrderRequest, RawCancelChildOrderResponse>> CancelChildOrderAsync(
        Requests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.CancelChildOrder",
            BitflyerEndpoints.CancelChildOrder(
                BitflyerRawJson.SerializeOrThrow(
                    request.Body,
                    "Bitflyer.CancelChildOrder")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelChildOrderResponse>(
                json,
                "Bitflyer.CancelChildOrder"));

    public Task<Call<Requests.CancelParentOrderRequest, RawCancelParentOrderResponse>> CancelParentOrderAsync(
        Requests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.CancelParentOrder",
            BitflyerEndpoints.CancelParentOrder(
                BitflyerRawJson.SerializeOrThrow(
                    request.Body,
                    "Bitflyer.CancelParentOrder")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelParentOrderResponse>(
                json,
                "Bitflyer.CancelParentOrder"));

    public Task<Call<Requests.GetChildOrdersRequest, IReadOnlyList<RawGetChildOrdersResponse>>> GetChildOrdersAsync(
        Requests.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetChildOrders",
            BitflyerEndpoints.GetChildOrders(
                request.ProductCode,
                request.ChildOrderStatusState,
                request.ChildOrderAcceptanceId,
                request.ChildOrderId,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture),
                request.ParentOrderId),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawGetChildOrdersResponse>>(
                json,
                "Bitflyer.GetChildOrders"));

    public Task<Call<Requests.GetParentOrdersRequest, IReadOnlyList<RawGetParentOrdersResponse>>> GetParentOrdersAsync(
        Requests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetParentOrders",
            BitflyerEndpoints.GetParentOrders(
                request.ProductCode,
                request.ParentOrderState,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawGetParentOrdersResponse>>(
                json,
                "Bitflyer.GetParentOrders"));

    public Task<Call<Requests.GetParentOrderRequest, RawGetParentOrderResponse>> GetParentOrderAsync(
        Requests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetParentOrder",
            BitflyerEndpoints.GetParentOrder(
                request.ParentOrderId,
                request.ParentOrderAcceptanceId),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawGetParentOrderResponse>(
                json,
                "Bitflyer.GetParentOrder"));

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
