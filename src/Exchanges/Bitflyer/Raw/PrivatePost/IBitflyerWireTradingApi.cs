using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

public interface IBitflyerRawPrivateTradingApi
{
    Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<EmptyResponse> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<EmptyResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<CreateParentOrderResponse> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<EmptyResponse> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<CreateWithdrawalResponse> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default);
}

[Obsolete("Use IBitflyerRawPrivateTradingApi instead. This interface will be removed in a future major release.")]
public interface IBitflyerWireTradingApi : IBitflyerRawPrivateTradingApi
{
}
