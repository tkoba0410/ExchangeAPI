using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Private REST API（取引系 POST）の Raw 実装。
/// </summary>
internal sealed class BittradePrivateTradingApi : IBittradePrivateTradingApi
{
    private readonly IBittradeWireTradingApi _wire;

    public BittradePrivateTradingApi(IBittradeWireTradingApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<RawPlaceOrderResponse> CreateOrderAsync(RawCreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var response = await _wire.PlaceOrderAsync(request, cancellationToken).ConfigureAwait(false);
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

        var response = await _wire.CancelOrderAsync(orderId.Value, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawCancelOrderResponse>(response.Json, "Bittrade.CancelOrder");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.CancelOrder", response.StatusCode, response.Json);
    }

    public async Task<RawCancelOrdersResponse> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var response = await _wire.CancelOrdersAsync(request, cancellationToken).ConfigureAwait(false);
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

        var response = await _wire.CancelOpenOrdersAsync(request, cancellationToken).ConfigureAwait(false);
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

        var response = await _wire.CreateWithdrawAsync(request, cancellationToken).ConfigureAwait(false);
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

        var response = await _wire.CancelWithdrawAsync(withdrawId, cancellationToken).ConfigureAwait(false);
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

        var response = await _wire.CreateRetailOrderAsync(request, cancellationToken).ConfigureAwait(false);
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
}
