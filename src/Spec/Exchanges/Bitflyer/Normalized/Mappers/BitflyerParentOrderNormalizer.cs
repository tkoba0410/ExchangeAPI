using System;
using System.Linq;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerParentOrderNormalizer
{
    public static BitflyerParentOrderNormalized Normalize(ParentOrderResponse raw) =>
        new(
            Id: raw.Id,
            ParentOrderId: raw.ParentOrderId,
            ProductCode: raw.ProductCode,
            Side: BitflyerSideMapper.ToExchangeSide(raw.Side),
            ParentOrderType: BitflyerParentOrderMapper.ParseParentOrderType(raw.ParentOrderType),
            Price: raw.Price,
            AveragePrice: raw.AveragePrice,
            Size: raw.Size,
            ParentOrderState: BitflyerParentOrderMapper.ParseParentOrderState(raw.ParentOrderState),
            ExpireDate: raw.ExpireDate,
            ParentOrderDate: raw.ParentOrderDate,
            ParentOrderAcceptanceId: raw.ParentOrderAcceptanceId,
            OutstandingSize: raw.OutstandingSize,
            CancelSize: raw.CancelSize,
            ExecutedSize: raw.ExecutedSize,
            TotalCommission: raw.TotalCommission);

    public static BitflyerParentOrderDetailNormalized NormalizeDetail(ParentOrderDetailResponse raw) =>
        new(
            Id: raw.Id,
            ParentOrderId: raw.ParentOrderId,
            OrderMethod: BitflyerParentOrderMapper.ParseOrderMethod(raw.OrderMethod),
            ExpireDate: raw.ExpireDate,
            TimeInForce: BitflyerTradingMapper.ParseTimeInForce(raw.TimeInForce),
            Parameters: raw.Parameters.Select(NormalizeParameter).ToArray(),
            ParentOrderAcceptanceId: raw.ParentOrderAcceptanceId);

    private static BitflyerParentOrderParameterNormalized NormalizeParameter(ParentOrderDetailParameter raw) =>
        new(
            ProductCode: raw.ProductCode,
            ConditionType: BitflyerParentOrderMapper.ParseConditionType(raw.ConditionType),
            Side: BitflyerSideMapper.ToExchangeSide(raw.Side),
            Size: raw.Size,
            Price: raw.Price,
            TriggerPrice: raw.TriggerPrice,
            Offset: raw.Offset);
}
