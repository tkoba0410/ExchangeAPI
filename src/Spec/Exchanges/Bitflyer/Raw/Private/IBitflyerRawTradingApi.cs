using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using Requests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

public interface IBitflyerRawTradingApi
{
    Task<Call<Requests.CreateChildOrderRequest, RawSendChildOrderResponse>> SendChildOrderAsync(
        Requests.CreateChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CreateParentOrderRequest, RawSendParentOrderResponse>> SendParentOrderAsync(
        Requests.CreateParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelChildOrderRequest, RawCancelChildOrderResponse>> CancelChildOrderAsync(
        Requests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelParentOrderRequest, RawCancelParentOrderResponse>> CancelParentOrderAsync(
        Requests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.GetChildOrdersRequest, IReadOnlyList<RawGetChildOrdersResponse>>> GetChildOrdersAsync(
        Requests.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.GetParentOrdersRequest, IReadOnlyList<RawGetParentOrdersResponse>>> GetParentOrdersAsync(
        Requests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.GetParentOrderRequest, RawGetParentOrderResponse>> GetParentOrderAsync(
        Requests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default);
}
