using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

#pragma warning disable CS0618
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal sealed class BitflyerWireTradingApiNotSupported : IBitflyerWireTradingApi
{
    private static NotSupportedException NotSupported() =>
        new("Bitflyer wire trading is not supported.");

    public Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<EmptyResponse> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<EmptyResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<CreateParentOrderResponse> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<EmptyResponse> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<CreateWithdrawalResponse> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();
}
#pragma warning restore CS0618
