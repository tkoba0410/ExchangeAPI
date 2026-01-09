using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

public interface IBitflyerRawTradingApi
{
    Task<Call<SendChildOrderRequest, RawSendChildOrderResponse>> SendChildOrderAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<SendParentOrderRequest, RawSendParentOrderResponse>> SendParentOrderAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelChildOrderRawRequest, RawCancelChildOrderResponse>> CancelChildOrderAsync(
        CancelChildOrderRawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelParentOrderRawRequest, RawCancelParentOrderResponse>> CancelParentOrderAsync(
        CancelParentOrderRawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetChildOrdersRequest, IReadOnlyList<RawGetChildOrdersResponse>>> GetChildOrdersAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrdersRequest, IReadOnlyList<RawGetParentOrdersResponse>>> GetParentOrdersAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrderRequest, RawGetParentOrderResponse>> GetParentOrderAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default);
}
