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
        return BittradeRawJson.ParseOrThrow<RawPlaceOrderResponse>(response);
    }

    public async Task<RawCancelOrderResponse> CancelOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId.Value))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        var response = await _wire.CancelOrderAsync(orderId.Value, cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawCancelOrderResponse>(response);
    }

    public async Task<RawCancelOrdersResponse> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var response = await _wire.CancelOrdersAsync(request, cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawCancelOrdersResponse>(response);
    }

    public async Task<RawCancelOpenOrdersResponse> CancelOpenOrdersAsync(RawCancelOpenOrdersRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var response = await _wire.CancelOpenOrdersAsync(request, cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawCancelOpenOrdersResponse>(response);
    }

    public async Task<RawCreateWithdrawResponse> CreateWithdrawAsync(RawCreateWithdrawRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var response = await _wire.CreateWithdrawAsync(request, cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawCreateWithdrawResponse>(response);
    }

    public async Task<RawCancelWithdrawResponse> CancelWithdrawAsync(string withdrawId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(withdrawId))
        {
            throw new ArgumentException("withdrawId is required.", nameof(withdrawId));
        }

        var response = await _wire.CancelWithdrawAsync(withdrawId, cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawCancelWithdrawResponse>(response);
    }

    public async Task<RawRetailOrderResponse> CreateRetailOrderAsync(RawCreateRetailOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var response = await _wire.CreateRetailOrderAsync(request, cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawRetailOrderResponse>(response);
    }
}
