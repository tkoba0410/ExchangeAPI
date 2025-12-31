using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal sealed class BitflyerWireTradingApiNotSupported : IBitflyerWireTradingApi
{
    private static NotSupportedException NotSupported() =>
        new("Bitflyer wire trading is not supported.");

    public Task<WireResponse> CreateChildOrderAsync(
        RawSendChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse> CancelChildOrderAsync(
        RawCancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();
}
