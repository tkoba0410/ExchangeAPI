using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal sealed class BitflyerWireTradingApiNotSupported : IBitflyerWireTradingApi
{
    private static NotSupportedException NotSupported() =>
        new("Bitflyer wire trading is not supported.");

    public Task<WireResponse<CreateChildOrderResponse>> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse<EmptyResponse>> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse<EmptyResponse>> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse<CreateParentOrderResponse>> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse<EmptyResponse>> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse<CreateWithdrawalResponse>> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();
}
