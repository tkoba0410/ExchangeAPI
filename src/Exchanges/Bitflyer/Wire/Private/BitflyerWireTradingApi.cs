using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Wire.Converters;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Private;

internal sealed class BitflyerWireTradingApi : IBitflyerWireTradingApi
{
    private readonly IBitflyerRawTradingApi _raw;
    private readonly IBitflyerPrivateTradingApi _legacy;

    public BitflyerWireTradingApi(IBitflyerRawTradingApi raw, IBitflyerPrivateTradingApi legacy)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        _legacy = legacy ?? throw new ArgumentNullException(nameof(legacy));
    }

    public async Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawRequest = BitflyerWireTradingMapper.MapSendChildOrderRequest(request);
        var rawResponse = await _raw.SendChildOrderAsync(rawRequest, cancellationToken).ConfigureAwait(false);
        return BitflyerWireTradingMapper.MapSendChildOrderResponse(rawResponse);
    }

    public async Task<EmptyResponse> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawRequest = BitflyerWireTradingMapper.MapCancelChildOrderRequest(request);
        await _raw.CancelChildOrderAsync(rawRequest, cancellationToken).ConfigureAwait(false);
        return new EmptyResponse();
    }

    public Task<EmptyResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _legacy.CancelAllChildOrdersAsync(request, cancellationToken);

    public Task<CreateParentOrderResponse> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _legacy.CreateParentOrderAsync(request, cancellationToken);

    public Task<EmptyResponse> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _legacy.CancelParentOrderAsync(request, cancellationToken);

    public Task<CreateWithdrawalResponse> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        _legacy.CreateWithdrawalAsync(request, cancellationToken);
}
