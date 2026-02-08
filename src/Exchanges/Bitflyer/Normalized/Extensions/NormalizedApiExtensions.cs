using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Extensions;

public static class NormalizedApiExtensions
{
    public static Task<Call<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderCallAsync(
        this INormalizedApi api,
        IReadOnlyList<ParentOrderParameterRequest> parameters,
        OrderMethod? orderMethod = null,
        int? minuteToExpire = null,
        TimeInForce? timeInForce = null,
        CancellationToken cancellationToken = default)
    {
        MinuteToExpire? normalizedMinuteToExpire = minuteToExpire.HasValue ? new MinuteToExpire(minuteToExpire.Value) : null;
        return
        api.SendParentOrderCallAsync(
            new SendParentOrderRequest(parameters, orderMethod, normalizedMinuteToExpire, timeInForce),
            cancellationToken);
    }

    public static Task<Call<CancelParentOrderRequest, CancelParentOrderResponse>> CancelParentOrderCallAsync(
        this INormalizedApi api,
        ProductCode productCode,
        ExchangeOrderId? parentOrderId = null,
        AcceptanceId? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        api.CancelParentOrderCallAsync(
            new CancelParentOrderRequest(productCode, parentOrderId, parentOrderAcceptanceId),
            cancellationToken);

    public static Task<Call<GetParentOrdersRequest, GetParentOrdersResponse>> GetParentOrdersCallAsync(
        this INormalizedApi api,
        ProductCode productCode,
        ParentOrderState? parentOrderState = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        RequestCount? requestCount = count.HasValue ? new RequestCount(count.Value) : null;
        RequestBefore? requestBefore = before.HasValue ? new RequestBefore(before.Value) : null;
        RequestAfter? requestAfter = after.HasValue ? new RequestAfter(after.Value) : null;
        return
        api.GetParentOrdersCallAsync(
            new GetParentOrdersRequest(productCode, parentOrderState, requestCount, requestBefore, requestAfter),
            cancellationToken);
    }

    public static Task<Call<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderCallAsync(
        this INormalizedApi api,
        ExchangeOrderId? parentOrderId = null,
        AcceptanceId? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        api.GetParentOrderCallAsync(
            new GetParentOrderRequest(parentOrderId, parentOrderAcceptanceId),
            cancellationToken);
}
