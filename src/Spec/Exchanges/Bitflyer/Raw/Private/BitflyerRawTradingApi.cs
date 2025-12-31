using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

internal sealed class BitflyerRawTradingApi : IBitflyerRawTradingApi
{
    private readonly IBitflyerWireApi _wire;

    public BitflyerRawTradingApi(IBitflyerWireApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<RawSendChildOrderResponse> SendChildOrderAsync(
        RawSendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var response = await _wire.Trading
            .CreateChildOrderAsync(request, cancellationToken)
            .ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<RawSendChildOrderResponse>(response);
    }

    public async Task<RawCancelChildOrderResponse> CancelChildOrderAsync(
        RawCancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var response = await _wire.Trading
            .CancelChildOrderAsync(request, cancellationToken)
            .ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<RawCancelChildOrderResponse>(response);
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

        var response = await _wire.Account
            .GetChildOrdersAsync(
                productCode,
                childOrderStatusState,
                childOrderAcceptanceId,
                childOrderId,
                parentOrderId,
                count,
                before,
                after,
                cancellationToken)
            .ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<RawGetChildOrdersResponse>>(response);
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
}
