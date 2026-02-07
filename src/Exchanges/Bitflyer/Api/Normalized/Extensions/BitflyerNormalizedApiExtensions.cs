using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Extensions;

public static class BitflyerNormalizedApiExtensions
{
    public static Task<Call<SendParentOrderRequest, BitflyerParentOrderAcceptance>> SendParentOrderCallAsync(
        this IBitflyerNormalizedApi api,
        IReadOnlyList<BitflyerParentOrderParameterRequest> parameters,
        BitflyerOrderMethod? orderMethod = null,
        int? minuteToExpire = null,
        BitflyerTimeInForce? timeInForce = null,
        CancellationToken cancellationToken = default) =>
        api.SendParentOrderCallAsync(
            new SendParentOrderRequest(parameters, orderMethod, minuteToExpire, timeInForce),
            cancellationToken);

    public static Task<Call<CancelParentOrderRequest, BitflyerParentOrderCancelResult>> CancelParentOrderCallAsync(
        this IBitflyerNormalizedApi api,
        ProductCode productCode,
        ExchangeOrderId? parentOrderId = null,
        AcceptanceId? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        api.CancelParentOrderCallAsync(
            new CancelParentOrderRequest(productCode, parentOrderId, parentOrderAcceptanceId),
            cancellationToken);

    public static Task<Call<GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        this IBitflyerNormalizedApi api,
        ProductCode productCode,
        BitflyerParentOrderState? parentOrderState = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        api.GetParentOrdersCallAsync(
            new GetParentOrdersRequest(productCode, parentOrderState, count, before, after),
            cancellationToken);

    public static Task<Call<GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        this IBitflyerNormalizedApi api,
        ExchangeOrderId? parentOrderId = null,
        AcceptanceId? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        api.GetParentOrderCallAsync(
            new GetParentOrderRequest(parentOrderId, parentOrderAcceptanceId),
            cancellationToken);
}
