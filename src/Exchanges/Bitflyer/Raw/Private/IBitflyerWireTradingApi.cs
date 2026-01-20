using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;
using Requests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

public interface IBitflyerRawPrivateTradingApi
{
    Task<Call<string, CreateChildOrderResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<string, CreateParentOrderResponse>> SendParentOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelChildOrderRequest, EmptyResponse>> CancelChildOrderCallAsync(
        Requests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelParentOrderRequest, EmptyResponse>> CancelParentOrderCallAsync(
        Requests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelAllChildOrdersRequest, EmptyResponse>> CancelAllChildOrdersCallAsync(
        Requests.CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CreateWithdrawalRequest, CreateWithdrawalResponse>> WithdrawCallAsync(
        Requests.CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default);
}

[Obsolete("Use IBitflyerRawPrivateTradingApi instead. This interface will be removed in a future major release.")]
public interface IBitflyerWireTradingApi : IBitflyerRawPrivateTradingApi
{
}
