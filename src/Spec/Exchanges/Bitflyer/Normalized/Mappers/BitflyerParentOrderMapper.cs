using System;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerParentOrderMapper
{
    public static BitflyerOrderMethod ParseOrderMethod(string orderMethod) =>
        (orderMethod ?? string.Empty).ToUpperInvariant() switch
        {
            "SIMPLE" => BitflyerOrderMethod.Simple,
            "IFD" => BitflyerOrderMethod.Ifd,
            "OCO" => BitflyerOrderMethod.Oco,
            "IFDOCO" => BitflyerOrderMethod.IfdOco,
            _ => throw new ArgumentOutOfRangeException(nameof(orderMethod), orderMethod, "Unknown bitFlyer order_method"),
        };

    public static string ToApiOrderMethod(BitflyerOrderMethod orderMethod) =>
        orderMethod switch
        {
            BitflyerOrderMethod.Simple => "SIMPLE",
            BitflyerOrderMethod.Ifd => "IFD",
            BitflyerOrderMethod.Oco => "OCO",
            BitflyerOrderMethod.IfdOco => "IFDOCO",
            _ => throw new ArgumentOutOfRangeException(nameof(orderMethod), orderMethod, "Unknown bitFlyer order_method"),
        };

    public static BitflyerConditionType ParseConditionType(string conditionType) =>
        (conditionType ?? string.Empty).ToUpperInvariant() switch
        {
            "LIMIT" => BitflyerConditionType.Limit,
            "MARKET" => BitflyerConditionType.Market,
            "STOP" => BitflyerConditionType.Stop,
            "STOP_LIMIT" => BitflyerConditionType.StopLimit,
            "TRAIL" => BitflyerConditionType.Trail,
            _ => throw new ArgumentOutOfRangeException(nameof(conditionType), conditionType, "Unknown bitFlyer condition_type"),
        };

    public static string ToApiConditionType(BitflyerConditionType conditionType) =>
        conditionType switch
        {
            BitflyerConditionType.Limit => "LIMIT",
            BitflyerConditionType.Market => "MARKET",
            BitflyerConditionType.Stop => "STOP",
            BitflyerConditionType.StopLimit => "STOP_LIMIT",
            BitflyerConditionType.Trail => "TRAIL",
            _ => throw new ArgumentOutOfRangeException(nameof(conditionType), conditionType, "Unknown bitFlyer condition_type"),
        };

    public static BitflyerParentOrderType ParseParentOrderType(string parentOrderType) =>
        (parentOrderType ?? string.Empty).ToUpperInvariant() switch
        {
            "SIMPLE" => BitflyerParentOrderType.Simple,
            "IFD" => BitflyerParentOrderType.Ifd,
            "OCO" => BitflyerParentOrderType.Oco,
            "IFDOCO" => BitflyerParentOrderType.IfdOco,
            _ => throw new ArgumentOutOfRangeException(nameof(parentOrderType), parentOrderType, "Unknown bitFlyer parent_order_type"),
        };

    public static string ToApiParentOrderType(BitflyerParentOrderType parentOrderType) =>
        parentOrderType switch
        {
            BitflyerParentOrderType.Simple => "SIMPLE",
            BitflyerParentOrderType.Ifd => "IFD",
            BitflyerParentOrderType.Oco => "OCO",
            BitflyerParentOrderType.IfdOco => "IFDOCO",
            _ => throw new ArgumentOutOfRangeException(nameof(parentOrderType), parentOrderType, "Unknown bitFlyer parent_order_type"),
        };

    public static BitflyerParentOrderState ParseParentOrderState(string parentOrderState) =>
        (parentOrderState ?? string.Empty).ToUpperInvariant() switch
        {
            "ACTIVE" => BitflyerParentOrderState.Active,
            "CANCELED" => BitflyerParentOrderState.Canceled,
            "COMPLETED" => BitflyerParentOrderState.Completed,
            "EXPIRED" => BitflyerParentOrderState.Expired,
            _ => throw new ArgumentOutOfRangeException(nameof(parentOrderState), parentOrderState, "Unknown bitFlyer parent_order_state"),
        };

    public static string ToApiParentOrderState(BitflyerParentOrderState parentOrderState) =>
        parentOrderState switch
        {
            BitflyerParentOrderState.Active => "ACTIVE",
            BitflyerParentOrderState.Canceled => "CANCELED",
            BitflyerParentOrderState.Completed => "COMPLETED",
            BitflyerParentOrderState.Expired => "EXPIRED",
            _ => throw new ArgumentOutOfRangeException(nameof(parentOrderState), parentOrderState, "Unknown bitFlyer parent_order_state"),
        };
}
