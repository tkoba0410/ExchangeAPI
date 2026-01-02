using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

/// <summary>
/// bitFlyer Private Trading REST API の実装（発注・キャンセル系）。
/// </summary>
public sealed class BitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly IWireTransport _wire;

    public BitflyerPrivateTradingApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var raw = BitflyerRawMappers.MapSendChildOrderRequest(request);
        var bodyJson = BitflyerRawJson.SerializeOrThrow(raw, "Bitflyer.CreateChildOrder");
        var call = await SendAsync(BitflyerEndpoints.SendChildOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<CreateChildOrderResponse>(
                response.Json,
                "Bitflyer.CreateChildOrder");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.CreateChildOrder",
            response.StatusCode,
            response.Json);
    }

    public async Task<EmptyResponse> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CancelChildOrder");
        var call = await SendAsync(BitflyerEndpoints.CancelChildOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<EmptyResponse>(
                response.Json,
                "Bitflyer.CancelChildOrder");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.CancelChildOrder",
            response.StatusCode,
            response.Json);
    }

    public async Task<EmptyResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CancelAllChildOrders");
        var call = await SendAsync(BitflyerEndpoints.CancelAllChildOrders(bodyJson), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<EmptyResponse>(
                response.Json,
                "Bitflyer.CancelAllChildOrders");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.CancelAllChildOrders",
            response.StatusCode,
            response.Json);
    }

    public async Task<CreateParentOrderResponse> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CreateParentOrder");
        var call = await SendAsync(BitflyerEndpoints.SendParentOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<CreateParentOrderResponse>(
                response.Json,
                "Bitflyer.CreateParentOrder");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.CreateParentOrder",
            response.StatusCode,
            response.Json);
    }

    public async Task<EmptyResponse> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CancelParentOrder");
        var call = await SendAsync(BitflyerEndpoints.CancelParentOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<EmptyResponse>(
                response.Json,
                "Bitflyer.CancelParentOrder");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.CancelParentOrder",
            response.StatusCode,
            response.Json);
    }

    public async Task<CreateWithdrawalResponse> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CreateWithdrawal");
        var call = await SendAsync(BitflyerEndpoints.Withdraw(bodyJson), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<CreateWithdrawalResponse>(
                response.Json,
                "Bitflyer.CreateWithdrawal");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.CreateWithdrawal",
            response.StatusCode,
            response.Json);
    }

    public async Task<BitflyerRawCall<CreateChildOrderResponse, JsonElement>> CreateChildOrderCallAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var raw = BitflyerRawMappers.MapSendChildOrderRequest(request);
        var bodyJson = BitflyerRawJson.SerializeOrThrow(raw, "Bitflyer.CreateChildOrder");
        var wireCall = await SendAsync(BitflyerEndpoints.SendChildOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bitflyer.CreateChildOrder", new Dictionary<string, string?>
        {
            ["productCode"] = request.ProductCode.Value,
            ["childOrderType"] = request.ChildOrderType.ToString(),
            ["side"] = request.Side.ToString(),
            ["size"] = request.Size.ToString(),
            ["price"] = request.Price?.ToString(),
            ["timeInForce"] = request.TimeInForce?.ToString(),
        });
        return CreateCall<CreateChildOrderResponse>(rawRequest, wireCall, "Bitflyer.CreateChildOrder");
    }

    public async Task<BitflyerRawCall<EmptyResponse, JsonElement>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CancelChildOrder");
        var wireCall = await SendAsync(BitflyerEndpoints.CancelChildOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bitflyer.CancelChildOrder", new Dictionary<string, string?>
        {
            ["productCode"] = request.ProductCode.Value,
            ["childOrderId"] = request.ChildOrderId,
            ["childOrderAcceptanceId"] = request.ChildOrderAcceptanceId,
        });
        return CreateCall<EmptyResponse>(rawRequest, wireCall, "Bitflyer.CancelChildOrder");
    }

    public async Task<BitflyerRawCall<EmptyResponse, JsonElement>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CancelAllChildOrders");
        var wireCall = await SendAsync(BitflyerEndpoints.CancelAllChildOrders(bodyJson), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bitflyer.CancelAllChildOrders", new Dictionary<string, string?>
        {
            ["productCode"] = request.ProductCode.Value,
        });
        return CreateCall<EmptyResponse>(rawRequest, wireCall, "Bitflyer.CancelAllChildOrders");
    }

    public async Task<BitflyerRawCall<CreateParentOrderResponse, JsonElement>> CreateParentOrderCallAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CreateParentOrder");
        var wireCall = await SendAsync(BitflyerEndpoints.SendParentOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bitflyer.CreateParentOrder", new Dictionary<string, string?>
        {
            ["orderMethod"] = request.OrderMethod.ToString(),
            ["timeInForce"] = request.TimeInForce?.ToString(),
        });
        return CreateCall<CreateParentOrderResponse>(rawRequest, wireCall, "Bitflyer.CreateParentOrder");
    }

    public async Task<BitflyerRawCall<EmptyResponse, JsonElement>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CancelParentOrder");
        var wireCall = await SendAsync(BitflyerEndpoints.CancelParentOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bitflyer.CancelParentOrder", new Dictionary<string, string?>
        {
            ["parentOrderId"] = request.ParentOrderId,
            ["parentOrderAcceptanceId"] = request.ParentOrderAcceptanceId,
        });
        return CreateCall<EmptyResponse>(rawRequest, wireCall, "Bitflyer.CancelParentOrder");
    }

    public async Task<BitflyerRawCall<CreateWithdrawalResponse, JsonElement>> CreateWithdrawalCallAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CreateWithdrawal");
        var wireCall = await SendAsync(BitflyerEndpoints.Withdraw(bodyJson), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bitflyer.CreateWithdrawal", new Dictionary<string, string?>
        {
            ["currencyCode"] = request.CurrencyCode,
            ["amount"] = request.Amount.ToString(),
        });
        return CreateCall<CreateWithdrawalResponse>(rawRequest, wireCall, "Bitflyer.CreateWithdrawal");
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
