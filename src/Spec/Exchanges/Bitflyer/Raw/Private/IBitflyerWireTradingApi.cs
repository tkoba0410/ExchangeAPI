using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Spec.CallCommon;
using Requests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

public interface IBitflyerRawPrivateTradingApi
{
    Task<Call<Requests.CreateChildOrderRequest, CreateChildOrderResponse>> CreateChildOrderAsync(
        Requests.CreateChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelChildOrderRequest, EmptyResponse>> CancelChildOrderAsync(
        Requests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelAllChildOrdersRequest, EmptyResponse>> CancelAllChildOrdersAsync(
        Requests.CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CreateParentOrderRequest, CreateParentOrderResponse>> CreateParentOrderAsync(
        Requests.CreateParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelParentOrderRequest, EmptyResponse>> CancelParentOrderAsync(
        Requests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CreateWithdrawalRequest, CreateWithdrawalResponse>> CreateWithdrawalAsync(
        Requests.CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default);
}

[Obsolete("Use IBitflyerRawPrivateTradingApi instead. This interface will be removed in a future major release.")]
public interface IBitflyerWireTradingApi : IBitflyerRawPrivateTradingApi
{
}
