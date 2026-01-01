using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

internal sealed class BitflyerRawTradingApi : IBitflyerRawTradingApi
{
    private readonly IWireTransport _wire;

    public BitflyerRawTradingApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<RawSendChildOrderResponse> SendChildOrderAsync(
        RawSendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var call = await SendAsync(BitflyerEndpoints.CreateChildOrder(request), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<RawSendChildOrderResponse>(
                response.Json,
                "Bitflyer.CreateChildOrder");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.CreateChildOrder",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawCancelChildOrderResponse> CancelChildOrderAsync(
        RawCancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var call = await SendAsync(BitflyerEndpoints.CancelChildOrder(request), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<RawCancelChildOrderResponse>(
                response.Json,
                "Bitflyer.CancelChildOrder");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.CancelChildOrder",
            response.StatusCode,
            response.Json);
    }

    public async Task<IReadOnlyList<RawGetChildOrdersResponse>> GetChildOrdersAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        var call = await SendAsync(
                BitflyerEndpoints.GetChildOrders(
                    productCode,
                    childOrderStatusState,
                    childOrderAcceptanceId,
                    childOrderId,
                    parentOrderId,
                    count,
                    before,
                    after),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawGetChildOrdersResponse>>(
                response.Json,
                "Bitflyer.GetChildOrders");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.GetChildOrders",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawGetChildOrdersResponse?> GetChildOrderAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        var list = await GetChildOrdersAsync(
                productCode,
                childOrderAcceptanceId: childOrderAcceptanceId,
                childOrderId: childOrderId,
                count: 1,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return list.FirstOrDefault();
    }

    public async Task<BitflyerRawCall<RawSendChildOrderResponse, JsonElement>> SendChildOrderCallAsync(
        RawSendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var wireCall = await SendAsync(BitflyerEndpoints.CreateChildOrder(request), cancellationToken)
            .ConfigureAwait(false);
        var rawRequest = CreateRequest("Bitflyer.CreateChildOrder", new Dictionary<string, string?>
        {
            ["productCode"] = request.ProductCode.Value,
            ["childOrderType"] = request.ChildOrderType.ToString(),
            ["side"] = request.Side.ToString(),
            ["size"] = request.Size.ToString(),
            ["price"] = request.Price?.ToString(),
            ["timeInForce"] = request.TimeInForce?.ToString(),
        });
        return CreateCall<RawSendChildOrderResponse>(rawRequest, wireCall, "Bitflyer.CreateChildOrder");
    }

    public async Task<BitflyerRawCall<RawCancelChildOrderResponse, JsonElement>> CancelChildOrderCallAsync(
        RawCancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var wireCall = await SendAsync(BitflyerEndpoints.CancelChildOrder(request), cancellationToken)
            .ConfigureAwait(false);
        var rawRequest = CreateRequest("Bitflyer.CancelChildOrder", new Dictionary<string, string?>
        {
            ["productCode"] = request.ProductCode.Value,
            ["childOrderId"] = request.ChildOrderId,
            ["childOrderAcceptanceId"] = request.ChildOrderAcceptanceId,
        });
        return CreateCall<RawCancelChildOrderResponse>(rawRequest, wireCall, "Bitflyer.CancelChildOrder");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<RawGetChildOrdersResponse>, JsonElement>> GetChildOrdersCallAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        var wireCall = await SendAsync(
                BitflyerEndpoints.GetChildOrders(
                    productCode,
                    childOrderStatusState,
                    childOrderAcceptanceId,
                    childOrderId,
                    parentOrderId,
                    count,
                    before,
                    after),
                cancellationToken)
            .ConfigureAwait(false);
        var rawRequest = CreateRequest("Bitflyer.GetChildOrders", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
            ["childOrderStatusState"] = childOrderStatusState,
            ["childOrderAcceptanceId"] = childOrderAcceptanceId,
            ["childOrderId"] = childOrderId,
            ["parentOrderId"] = parentOrderId,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        });
        return CreateCall<IReadOnlyList<RawGetChildOrdersResponse>>(rawRequest, wireCall, "Bitflyer.GetChildOrders");
    }

    private static BitflyerRawRequest CreateRequest(
        string operation,
        IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BitflyerRawCall<TOk, JsonElement> CreateCall<TOk>(
        BitflyerRawRequest request,
        WireCall call,
        string context)
    {
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            var ok = BitflyerRawJson.DeserializeOrThrow<TOk>(response.Json, context);
            return new BitflyerRawCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(ok, response.StatusCode),
                call.Meta);
        }

        if (BitflyerRawJson.TryDeserialize<JsonElement>(response.Json, out var error, out _))
        {
            return new BitflyerRawCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(error!, response.StatusCode),
                call.Meta);
        }

        return new BitflyerRawCall<TOk, JsonElement>(
            request,
            new Err<TOk, JsonElement>(default, response.StatusCode),
            call.Meta);
    }

    private Task<WireCall> SendAsync(WireRequest request, CancellationToken ct) =>
        _wire.SendAsync(ExchangeCode.Bitflyer, request, ct);
}
