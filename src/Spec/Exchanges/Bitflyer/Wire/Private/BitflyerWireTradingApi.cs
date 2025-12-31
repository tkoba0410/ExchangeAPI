using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Converters;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal sealed class BitflyerWireTradingApi : IBitflyerWireTradingApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bitflyer;
    private readonly IBitflyerRawTradingApi _raw;
    private readonly IBitflyerPrivateTradingApi _legacy;

    public BitflyerWireTradingApi(IBitflyerRawTradingApi raw, IBitflyerPrivateTradingApi legacy)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        _legacy = legacy ?? throw new ArgumentNullException(nameof(legacy));
    }

    public async Task<WireResponse<CreateChildOrderResponse>> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawRequest = BitflyerWireTradingMapper.MapSendChildOrderRequest(request);
        var rawResponse = await _raw.SendChildOrderAsync(rawRequest, cancellationToken).ConfigureAwait(false);
        var response = BitflyerWireTradingMapper.MapSendChildOrderResponse(rawResponse);
        return new WireResponse<CreateChildOrderResponse>(Exchange, response);
    }

    public async Task<WireResponse<EmptyResponse>> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawRequest = BitflyerWireTradingMapper.MapCancelChildOrderRequest(request);
        await _raw.CancelChildOrderAsync(rawRequest, cancellationToken).ConfigureAwait(false);
        return new WireResponse<EmptyResponse>(Exchange, new EmptyResponse());
    }

    public async Task<WireResponse<EmptyResponse>> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        await _legacy.CancelAllChildOrdersAsync(request, cancellationToken).ConfigureAwait(false);
        return new WireResponse<EmptyResponse>(Exchange, new EmptyResponse());
    }

    public async Task<WireResponse<CreateParentOrderResponse>> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _legacy.CreateParentOrderAsync(request, cancellationToken).ConfigureAwait(false);
        return new WireResponse<CreateParentOrderResponse>(Exchange, response);
    }

    public async Task<WireResponse<EmptyResponse>> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        await _legacy.CancelParentOrderAsync(request, cancellationToken).ConfigureAwait(false);
        return new WireResponse<EmptyResponse>(Exchange, new EmptyResponse());
    }

    public async Task<WireResponse<CreateWithdrawalResponse>> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _legacy.CreateWithdrawalAsync(request, cancellationToken).ConfigureAwait(false);
        return new WireResponse<CreateWithdrawalResponse>(Exchange, response);
    }
}
