using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;

/// <summary>
/// Bittrade Private REST API（取引系 POST）の Raw 実装。
/// </summary>
internal sealed class BittradePrivateTradingApi : IBittradePrivateTradingApi
{
    private readonly IWireTransport _wire;

    public BittradePrivateTradingApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Call<CreateOrderRequest, RawPlaceOrderResponse>> PostOrdersPlaceCallAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.PlaceOrder",
            BittradePrivateEndpoints.PostOrdersPlace(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.PlaceOrder")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawPlaceOrderResponse>(json, "Bittrade.PlaceOrder"));

    public Task<Call<CancelOrderRequest, RawCancelOrderResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.CancelOrder",
            BittradePrivateEndpoints.PostOrdersSubmitCancelByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOrderResponse>(json, "Bittrade.CancelOrder"));

    public Task<Call<CancelOrdersRequest, RawCancelOrdersResponse>> PostOrdersBatchCancelCallAsync(
        CancelOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.CancelOrders",
            BittradePrivateEndpoints.PostOrdersBatchCancel(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.CancelOrders")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOrdersResponse>(
                json,
                "Bittrade.CancelOrders"));

    public Task<Call<CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        CancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.CancelOpenOrders",
            BittradePrivateEndpoints.PostOrdersBatchCancelOpenOrders(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.CancelOpenOrders")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOpenOrdersResponse>(
                json,
                "Bittrade.CancelOpenOrders"));

    public Task<Call<CreateWithdrawRequest, RawCreateWithdrawResponse>> PostWithdrawApiCreateCallAsync(
        CreateWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.CreateWithdraw",
            BittradePrivateEndpoints.PostWithdrawApiCreate(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.CreateWithdraw")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                json,
                "Bittrade.CreateWithdraw"));

    public Task<Call<CreateWithdrawVirtualByAddressIdRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        CreateWithdrawVirtualByAddressIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.CreateWithdrawByAddressId",
            BittradePrivateEndpoints.PostWithdrawVirtualByAddressIdCreate(request.AddressId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                json,
                "Bittrade.CreateWithdrawByAddressId"));

    public Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.CancelWithdraw",
            BittradePrivateEndpoints.PostWithdrawVirtualByWithdrawIdCancel(request.WithdrawId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelWithdrawResponse>(
                json,
                "Bittrade.CancelWithdraw"));

    public Task<Call<PlaceWithdrawVirtualRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PlaceWithdrawVirtualRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.PlaceWithdraw",
            BittradePrivateEndpoints.PostWithdrawVirtualByWithdrawIdPlace(request.WithdrawId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                json,
                "Bittrade.PlaceWithdraw"));

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderPlaceCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.CreateRetailOrder",
            BittradePrivateEndpoints.PostRetailOrderPlace(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.CreateRetailOrder")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                "Bittrade.CreateRetailOrder"));

    public Task<Call<CancelRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        CancelRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.CancelRetailOrder",
            BittradePrivateEndpoints.PostRetailOrderCancelByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                "Bittrade.CancelRetailOrder"));

    public Task<Call<PostRetailOrderHistoryRequest, RawRetailOrdersResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetRetailOrderHistory",
            BittradePrivateEndpoints.PostRetailOrderHistory(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.GetRetailOrderHistory")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrdersResponse>(
                json,
                "Bittrade.GetRetailOrderHistory"));

    public Task<Call<PostRetailOrderDetailRequest, RawRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetRetailOrderDetail",
            BittradePrivateEndpoints.PostRetailOrderDetail(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.GetRetailOrderDetail")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderDetailResponse>(
                json,
                "Bittrade.GetRetailOrderDetail"));

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCreateCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.CreateRetailOrder",
            BittradePrivateEndpoints.PostRetailOrderCreate(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.CreateRetailOrder")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                "Bittrade.CreateRetailOrder"));

    private async Task<Call<TReq, TRes>> SendAndParse<TReq, TRes>(
        TReq request,
        string component,
        WireCallSpec spec,
        CancellationToken cancellationToken,
        Func<string, TRes> parse)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (parse is null) throw new ArgumentNullException(nameof(parse));

        var wireCall = await _wire.SendAsync(ExchangeCode.Bittrade, spec, cancellationToken).ConfigureAwait(false);
        return CreateCall(request, component, wireCall, parse);
    }

    private static Call<TReq, TRes> CreateCall<TReq, TRes>(
        TReq request,
        string component,
        Call<WireCallSpec, WireResponse> wireCall,
        Func<string, TRes> parse)
    {
        return wireCall.Result switch
        {
            CallResult<WireResponse>.Err err => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(err.Error),
                Meta: wireCall.Meta),
            CallResult<WireResponse>.Ok ok => CreateOkCall(request, component, ok.Response, wireCall, parse),
            _ => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(new CallError(CallErrorKind.Unknown, "Wire call returned unknown result.")),
                Meta: wireCall.Meta)
        };
    }

    private static Call<TReq, TRes> CreateOkCall<TReq, TRes>(
        TReq request,
        string component,
        WireResponse response,
        Call<WireCallSpec, WireResponse> wireCall,
        Func<string, TRes> parse)
    {
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
                Meta: wireCall.Meta);
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
                Meta: wireCall.Meta);
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
                Meta: wireCall.Meta);
        }
    }

    private static string? Snip(string? json)
    {
        if (string.IsNullOrEmpty(json)) return json;
        return json.Length <= 512 ? json : json[..512];
    }
}
