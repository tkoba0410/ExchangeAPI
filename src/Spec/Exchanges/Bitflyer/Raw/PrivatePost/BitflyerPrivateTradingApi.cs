using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Converters;
using WireTradingApi = ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private.IBitflyerWireTradingApi;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

/// <summary>
/// bitFlyer Private Trading REST API の実装（発注・キャンセル系）。
/// </summary>
public sealed class BitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly WireTradingApi _wire;

    public BitflyerPrivateTradingApi(WireTradingApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var wireRequest = BitflyerWireTradingMapper.MapSendChildOrderRequest(request);
        var call = await _wire.CreateChildOrderAsync(wireRequest, cancellationToken).ConfigureAwait(false);
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

        var wireRequest = BitflyerWireTradingMapper.MapCancelChildOrderRequest(request);
        var call = await _wire.CancelChildOrderAsync(wireRequest, cancellationToken).ConfigureAwait(false);
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

        var call = await _wire.CancelAllChildOrdersAsync(request, cancellationToken).ConfigureAwait(false);
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
        var call = await _wire.CreateParentOrderAsync(request, cancellationToken).ConfigureAwait(false);
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
        var call = await _wire.CancelParentOrderAsync(request, cancellationToken).ConfigureAwait(false);
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
        var call = await _wire.CreateWithdrawalAsync(request, cancellationToken).ConfigureAwait(false);
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

        var wireRequest = BitflyerWireTradingMapper.MapSendChildOrderRequest(request);
        var wireCall = await _wire.CreateChildOrderAsync(wireRequest, cancellationToken).ConfigureAwait(false);
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

        var wireRequest = BitflyerWireTradingMapper.MapCancelChildOrderRequest(request);
        var wireCall = await _wire.CancelChildOrderAsync(wireRequest, cancellationToken).ConfigureAwait(false);
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

        var wireCall = await _wire.CancelAllChildOrdersAsync(request, cancellationToken).ConfigureAwait(false);
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

        var wireCall = await _wire.CreateParentOrderAsync(request, cancellationToken).ConfigureAwait(false);
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

        var wireCall = await _wire.CancelParentOrderAsync(request, cancellationToken).ConfigureAwait(false);
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

        var wireCall = await _wire.CreateWithdrawalAsync(request, cancellationToken).ConfigureAwait(false);
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
}
