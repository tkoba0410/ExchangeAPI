using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Requests = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Api;

public interface IBitflyerRawTradingApi
{
    Task<Call<string, RawSendChildOrderResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<string, RawSendParentOrderResponse>> SendParentOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelChildOrderRequest, RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
        Requests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.CancelParentOrderRequest, RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
        Requests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.GetChildOrdersRequest, IReadOnlyList<RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        Requests.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.GetParentOrdersRequest, IReadOnlyList<RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        Requests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<Requests.GetParentOrderRequest, RawGetParentOrderResponse>> GetParentOrderCallAsync(
        Requests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default);
}
