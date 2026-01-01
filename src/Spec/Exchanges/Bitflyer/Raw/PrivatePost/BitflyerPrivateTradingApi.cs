using System;
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
        var response = await _wire.CreateChildOrderAsync(wireRequest, cancellationToken).ConfigureAwait(false);
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
        var response = await _wire.CancelChildOrderAsync(wireRequest, cancellationToken).ConfigureAwait(false);
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

        var response = await _wire.CancelAllChildOrdersAsync(request, cancellationToken).ConfigureAwait(false);
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
        var response = await _wire.CreateParentOrderAsync(request, cancellationToken).ConfigureAwait(false);
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
        var response = await _wire.CancelParentOrderAsync(request, cancellationToken).ConfigureAwait(false);
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
        var response = await _wire.CreateWithdrawalAsync(request, cancellationToken).ConfigureAwait(false);
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
}
