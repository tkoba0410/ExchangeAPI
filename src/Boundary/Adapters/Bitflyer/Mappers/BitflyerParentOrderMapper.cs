using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using NormalizedParentOrderMapper = ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers.BitflyerParentOrderMapper;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;

internal static class BitflyerParentOrderMapper
{
    public static ParentOrder Map(Symbol symbol, BitflyerParentOrderNormalized normalized) =>
        new(
            Exchange: ExchangeCode.Bitflyer,
            Symbol: symbol,
            ParentOrderId: normalized.ParentOrderId,
            ParentOrderAcceptanceId: normalized.ParentOrderAcceptanceId,
            Side: BitflyerCommonMapper.MapSide(normalized.Side),
            ParentOrderType: NormalizedParentOrderMapper.ToApiParentOrderType(normalized.ParentOrderType),
            ParentOrderState: NormalizedParentOrderMapper.ToApiParentOrderState(normalized.ParentOrderState),
            Price: normalized.Price == 0 ? null : new Price(normalized.Price),
            AveragePrice: normalized.AveragePrice == 0 ? null : new Price(normalized.AveragePrice),
            Size: new Size(normalized.Size),
            OutstandingSize: new Size(normalized.OutstandingSize),
            CancelSize: new Size(normalized.CancelSize),
            ExecutedSize: new Size(normalized.ExecutedSize),
            TotalCommission: normalized.TotalCommission,
            ParentOrderDate: normalized.ParentOrderDate,
            ExpireDate: normalized.ExpireDate);

    public static ParentOrderDetail MapDetail(BitflyerParentOrderDetailNormalized normalized) =>
        new(
            Exchange: ExchangeCode.Bitflyer,
            ParentOrderId: normalized.ParentOrderId,
            ParentOrderAcceptanceId: normalized.ParentOrderAcceptanceId,
            OrderMethod: NormalizedParentOrderMapper.ToApiOrderMethod(normalized.OrderMethod),
            TimeInForce: BitflyerTradingMapper.ToApiTimeInForce(normalized.TimeInForce) ?? string.Empty,
            Parameters: MapParameters(normalized.Parameters));

    private static IReadOnlyList<ParentOrderParameter> MapParameters(
        IReadOnlyList<BitflyerParentOrderParameterNormalized> parameters)
    {
        return parameters
            .Select(p => new ParentOrderParameter(
                ProductCode: p.ProductCode,
                ConditionType: NormalizedParentOrderMapper.ToApiConditionType(p.ConditionType),
                Side: BitflyerCommonMapper.MapSide(p.Side),
                Size: new Size(p.Size),
                Price: p.Price == 0 ? null : new Price(p.Price),
                TriggerPrice: p.TriggerPrice == 0 ? null : new Price(p.TriggerPrice),
                Offset: p.Offset))
            .ToArray();
    }
}
