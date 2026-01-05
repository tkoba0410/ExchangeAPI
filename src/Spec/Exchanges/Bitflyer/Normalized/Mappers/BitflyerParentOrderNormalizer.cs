using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Spec.ValueCommon.ClosedSet;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerParentOrderNormalizer
{
    public static BitflyerParentOrderNormalized Normalize(ParentOrderResponse raw, string? rawJson)
    {
        var snapshot = CreateSnapshot(rawJson);
        return new(
            Id: raw.Id,
            ParentOrderId: raw.ParentOrderId,
            ProductCode: raw.ProductCode,
            Side: ParseSide(raw.Side),
            ParentOrderType: ParseParentOrderType(raw.ParentOrderType),
            Price: raw.Price,
            AveragePrice: raw.AveragePrice,
            Size: raw.Size,
            ParentOrderState: ParseParentOrderState(raw.ParentOrderState),
            ExpireDate: raw.ExpireDate,
            ParentOrderDate: raw.ParentOrderDate,
            ParentOrderAcceptanceId: raw.ParentOrderAcceptanceId,
            OutstandingSize: raw.OutstandingSize,
            CancelSize: raw.CancelSize,
            ExecutedSize: raw.ExecutedSize,
            TotalCommission: raw.TotalCommission,
            RawSnapshot: snapshot,
            Extras: EmptyExtras());
    }

    public static BitflyerParentOrderDetailNormalized NormalizeDetail(ParentOrderDetailResponse raw, string? rawJson)
    {
        var snapshot = CreateSnapshot(rawJson);
        return new(
            Id: raw.Id,
            ParentOrderId: raw.ParentOrderId,
            OrderMethod: ParseOrderMethod(raw.OrderMethod),
            ExpireDate: raw.ExpireDate,
            TimeInForce: ParseTimeInForce(raw.TimeInForce),
            Parameters: raw.Parameters.Select(NormalizeParameter).ToArray(),
            ParentOrderAcceptanceId: raw.ParentOrderAcceptanceId,
            RawSnapshot: snapshot,
            Extras: EmptyExtras());
    }

    private static BitflyerParentOrderParameterNormalized NormalizeParameter(ParentOrderDetailParameter raw) =>
        new(
            ProductCode: raw.ProductCode,
            ConditionType: ParseConditionType(raw.ConditionType),
            Side: ParseSide(raw.Side),
            Size: raw.Size,
            Price: raw.Price,
            TriggerPrice: raw.TriggerPrice,
            Offset: raw.Offset);

    private static Closed<BitflyerSide> ParseSide(string side)
    {
        var value = (side ?? string.Empty).ToUpperInvariant();
        return value switch
        {
            "BUY" => Closed<BitflyerSide>.KnownValue(BitflyerSide.Buy),
            "SELL" => Closed<BitflyerSide>.KnownValue(BitflyerSide.Sell),
            _ => Closed<BitflyerSide>.UnknownValue(side ?? string.Empty)
        };
    }

    private static Closed<BitflyerParentOrderType> ParseParentOrderType(string parentOrderType)
    {
        var value = (parentOrderType ?? string.Empty).ToUpperInvariant();
        return value switch
        {
            "SIMPLE" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.Simple),
            "IFD" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.Ifd),
            "OCO" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.Oco),
            "IFDOCO" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.IfdOco),
            _ => Closed<BitflyerParentOrderType>.UnknownValue(parentOrderType ?? string.Empty)
        };
    }

    private static Closed<BitflyerParentOrderState> ParseParentOrderState(string parentOrderState)
    {
        var value = (parentOrderState ?? string.Empty).ToUpperInvariant();
        return value switch
        {
            "ACTIVE" => Closed<BitflyerParentOrderState>.KnownValue(BitflyerParentOrderState.Active),
            "CANCELED" => Closed<BitflyerParentOrderState>.KnownValue(BitflyerParentOrderState.Canceled),
            "COMPLETED" => Closed<BitflyerParentOrderState>.KnownValue(BitflyerParentOrderState.Completed),
            "EXPIRED" => Closed<BitflyerParentOrderState>.KnownValue(BitflyerParentOrderState.Expired),
            _ => Closed<BitflyerParentOrderState>.UnknownValue(parentOrderState ?? string.Empty)
        };
    }

    private static Closed<BitflyerOrderMethod> ParseOrderMethod(string orderMethod)
    {
        var value = (orderMethod ?? string.Empty).ToUpperInvariant();
        return value switch
        {
            "SIMPLE" => Closed<BitflyerOrderMethod>.KnownValue(BitflyerOrderMethod.Simple),
            "IFD" => Closed<BitflyerOrderMethod>.KnownValue(BitflyerOrderMethod.Ifd),
            "OCO" => Closed<BitflyerOrderMethod>.KnownValue(BitflyerOrderMethod.Oco),
            "IFDOCO" => Closed<BitflyerOrderMethod>.KnownValue(BitflyerOrderMethod.IfdOco),
            _ => Closed<BitflyerOrderMethod>.UnknownValue(orderMethod ?? string.Empty)
        };
    }

    private static Closed<BitflyerConditionType> ParseConditionType(string conditionType)
    {
        var value = (conditionType ?? string.Empty).ToUpperInvariant();
        return value switch
        {
            "LIMIT" => Closed<BitflyerConditionType>.KnownValue(BitflyerConditionType.Limit),
            "MARKET" => Closed<BitflyerConditionType>.KnownValue(BitflyerConditionType.Market),
            "STOP" => Closed<BitflyerConditionType>.KnownValue(BitflyerConditionType.Stop),
            "STOP_LIMIT" => Closed<BitflyerConditionType>.KnownValue(BitflyerConditionType.StopLimit),
            "TRAIL" => Closed<BitflyerConditionType>.KnownValue(BitflyerConditionType.Trail),
            _ => Closed<BitflyerConditionType>.UnknownValue(conditionType ?? string.Empty)
        };
    }

    private static Closed<BitflyerTimeInForce> ParseTimeInForce(string tif)
    {
        var value = (tif ?? string.Empty).ToUpperInvariant();
        return value switch
        {
            "GTC" => Closed<BitflyerTimeInForce>.KnownValue(BitflyerTimeInForce.Gtc),
            "IOC" => Closed<BitflyerTimeInForce>.KnownValue(BitflyerTimeInForce.Ioc),
            "FOK" => Closed<BitflyerTimeInForce>.KnownValue(BitflyerTimeInForce.Fok),
            _ => Closed<BitflyerTimeInForce>.UnknownValue(tif ?? string.Empty)
        };
    }

    private static JsonElement CreateSnapshot(string? rawJson)
    {
        var json = string.IsNullOrWhiteSpace(rawJson) ? "{}" : rawJson;
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    private static IReadOnlyDictionary<string, JsonElement> EmptyExtras() =>
        new Dictionary<string, JsonElement>();
}
