using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;
using Requests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

public interface IBitflyerRawPrivateTradingApi
{
    Task<Call<string, CreateChildOrderResponse>> CreateChildOrderAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<string, CreateParentOrderResponse>> CreateParentOrderAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelChildOrderRequest, EmptyResponse>> CancelChildOrderAsync(
        Requests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelParentOrderRequest, EmptyResponse>> CancelParentOrderAsync(
        Requests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelAllChildOrdersRequest, EmptyResponse>> CancelAllChildOrdersAsync(
        Requests.CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CreateWithdrawalRequest, CreateWithdrawalResponse>> CreateWithdrawalAsync(
        Requests.CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default);
}

[Obsolete("Use IBitflyerRawPrivateTradingApi instead. This interface will be removed in a future major release.")]
public interface IBitflyerWireTradingApi : IBitflyerRawPrivateTradingApi
{
}
