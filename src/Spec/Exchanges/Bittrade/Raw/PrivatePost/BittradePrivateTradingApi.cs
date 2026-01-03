using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bittrade.Wire.Endpoints;
using ExchangeApi.Spec.Wire;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

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

    public async Task<RawPlaceOrderResponse> CreateOrderAsync(RawCreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.PlaceOrder");
        var call = await SendAsync(BittradeEndpoints.PlaceOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawPlaceOrderResponse>(response.Json, "Bittrade.PlaceOrder");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.PlaceOrder", response.StatusCode, response.Json);
    }

    public async Task<RawCancelOrderResponse> CancelOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId.Value))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        var call = await SendAsync(BittradeEndpoints.CancelOrder(orderId.Value), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawCancelOrderResponse>(response.Json, "Bittrade.CancelOrder");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.CancelOrder", response.StatusCode, response.Json);
    }

    public async Task<RawCancelOrdersResponse> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.CancelOrders");
        var call = await SendAsync(BittradeEndpoints.CancelOrders(bodyJson), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawCancelOrdersResponse>(
                response.Json,
                "Bittrade.CancelOrders");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.CancelOrders",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawCancelOpenOrdersResponse> CancelOpenOrdersAsync(RawCancelOpenOrdersRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.CancelOpenOrders");
        var call = await SendAsync(BittradeEndpoints.CancelOpenOrders(bodyJson), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawCancelOpenOrdersResponse>(
                response.Json,
                "Bittrade.CancelOpenOrders");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.CancelOpenOrders",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawCreateWithdrawResponse> CreateWithdrawAsync(RawCreateWithdrawRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.CreateWithdraw");
        var call = await SendAsync(BittradeEndpoints.CreateWithdraw(bodyJson), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                response.Json,
                "Bittrade.CreateWithdraw");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.CreateWithdraw",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawCancelWithdrawResponse> CancelWithdrawAsync(string withdrawId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(withdrawId))
        {
            throw new ArgumentException("withdrawId is required.", nameof(withdrawId));
        }

        var call = await SendAsync(BittradeEndpoints.CancelWithdraw(withdrawId), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawCancelWithdrawResponse>(
                response.Json,
                "Bittrade.CancelWithdraw");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.CancelWithdraw",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawRetailOrderResponse> CreateRetailOrderAsync(RawCreateRetailOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.CreateRetailOrder");
        var call = await SendAsync(BittradeEndpoints.CreateRetailOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                response.Json,
                "Bittrade.CreateRetailOrder");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.CreateRetailOrder",
            response.StatusCode,
            response.Json);
    }

    public async Task<BittradeRawCall<RawPlaceOrderResponse, JsonElement>> CreateOrderCallAsync(
        RawCreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.PlaceOrder");
        var wireCall = await SendAsync(BittradeEndpoints.PlaceOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bittrade.PlaceOrder", new Dictionary<string, string?>
        {
            ["accountId"] = request.AccountId,
            ["symbol"] = request.RawSymbol.Value,
            ["type"] = request.Type.ToString(),
            ["amount"] = request.Amount,
            ["price"] = request.Price,
        });
        return CreateCall<RawPlaceOrderResponse>(rawRequest, wireCall, "Bittrade.PlaceOrder");
    }

    public async Task<BittradeRawCall<RawCancelOrderResponse, JsonElement>> CancelOrderCallAsync(
        RawOrderId orderId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId.Value))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        var wireCall = await SendAsync(BittradeEndpoints.CancelOrder(orderId.Value), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bittrade.CancelOrder", new Dictionary<string, string?>
        {
            ["orderId"] = orderId.Value,
        });
        return CreateCall<RawCancelOrderResponse>(rawRequest, wireCall, "Bittrade.CancelOrder");
    }

    public async Task<BittradeRawCall<RawCancelOrdersResponse, JsonElement>> CancelOrdersCallAsync(
        RawCancelOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.CancelOrders");
        var wireCall = await SendAsync(BittradeEndpoints.CancelOrders(bodyJson), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bittrade.CancelOrders", new Dictionary<string, string?>
        {
            ["orderIds"] = string.Join(",", request.OrderIds),
        });
        return CreateCall<RawCancelOrdersResponse>(rawRequest, wireCall, "Bittrade.CancelOrders");
    }

    public async Task<BittradeRawCall<RawCancelOpenOrdersResponse, JsonElement>> CancelOpenOrdersCallAsync(
        RawCancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.CancelOpenOrders");
        var wireCall = await SendAsync(BittradeEndpoints.CancelOpenOrders(bodyJson), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bittrade.CancelOpenOrders", new Dictionary<string, string?>
        {
            ["accountId"] = request.AccountId,
            ["symbol"] = request.RawSymbol?.Value,
        });
        return CreateCall<RawCancelOpenOrdersResponse>(rawRequest, wireCall, "Bittrade.CancelOpenOrders");
    }

    public async Task<BittradeRawCall<RawCreateWithdrawResponse, JsonElement>> CreateWithdrawCallAsync(
        RawCreateWithdrawRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.CreateWithdraw");
        var wireCall = await SendAsync(BittradeEndpoints.CreateWithdraw(bodyJson), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bittrade.CreateWithdraw", new Dictionary<string, string?>
        {
            ["currency"] = request.Currency,
            ["amount"] = request.Amount,
            ["address"] = request.Address,
        });
        return CreateCall<RawCreateWithdrawResponse>(rawRequest, wireCall, "Bittrade.CreateWithdraw");
    }

    public async Task<BittradeRawCall<RawCancelWithdrawResponse, JsonElement>> CancelWithdrawCallAsync(
        string withdrawId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(withdrawId))
        {
            throw new ArgumentException("withdrawId is required.", nameof(withdrawId));
        }

        var wireCall = await SendAsync(BittradeEndpoints.CancelWithdraw(withdrawId), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bittrade.CancelWithdraw", new Dictionary<string, string?>
        {
            ["withdrawId"] = withdrawId,
        });
        return CreateCall<RawCancelWithdrawResponse>(rawRequest, wireCall, "Bittrade.CancelWithdraw");
    }

    public async Task<BittradeRawCall<RawRetailOrderResponse, JsonElement>> CreateRetailOrderCallAsync(
        RawCreateRetailOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.CreateRetailOrder");
        var wireCall = await SendAsync(BittradeEndpoints.CreateRetailOrder(bodyJson), cancellationToken).ConfigureAwait(false);
        var rawRequest = CreateRequest("Bittrade.CreateRetailOrder", new Dictionary<string, string?>
        {
            ["symbol"] = request.RawSymbol.Value,
            ["type"] = request.Type.ToString(),
            ["price"] = request.Price,
            ["amount"] = request.Amount,
            ["cash_amount"] = request.CashAmount,
        });
        return CreateCall<RawRetailOrderResponse>(rawRequest, wireCall, "Bittrade.CreateRetailOrder");
    }

    private static BittradeRawRequest CreateRequest(
        string operation,
        IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BittradeRawCall<TOk, JsonElement> CreateCall<TOk>(
        BittradeRawRequest request,
        WireCall call,
        string context)
    {
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            var ok = BittradeRawJson.DeserializeOrThrow<TOk>(response.Json, context);
            return new BittradeRawCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(ok, response.StatusCode),
                call.Meta);
        }

        if (BittradeRawJson.TryDeserialize<JsonElement>(response.Json, out var error, out _))
        {
            return new BittradeRawCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(error!, response.StatusCode),
                call.Meta);
        }

        return new BittradeRawCall<TOk, JsonElement>(
            request,
            new Err<TOk, JsonElement>(default, response.StatusCode),
            call.Meta);
    }

    private Task<WireCall> SendAsync(WireRequest request, CancellationToken ct) =>
        _wire.SendAsync(ExchangeCode.Bittrade, request, ct);
}
