using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using ContractSide = ExchangeApi.Common.Enums.Side;
using RawSide = ExchangeApi.Exchanges.Bitflyer.Raw.Side;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.ProductCode;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;

internal static class BitflyerCommonMapper
{
    private static readonly IReadOnlyDictionary<RawProductCode, string> ProductCodeMap = BuildProductCodeMap();
    private static readonly IReadOnlyDictionary<string, RawProductCode> ProductCodeLookup =
        ProductCodeMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.Ordinal);

    public static ContractSide MapSide(RawSide side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static ContractSide MapSide(string side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static RawSide MapSideToExchange(ContractSide side) =>
        BitflyerSideMapper.ToRawSide(side);

    public static string ToApiProductCode(RawProductCode productCode) =>
        ProductCodeMap.TryGetValue(productCode, out var code)
            ? code
            : throw new SymbolNotSupportedException(productCode.ToString());

    public static RawProductCode ParseProductCode(string productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new SymbolNotSupportedException(productCode ?? string.Empty);
        }

        return ProductCodeLookup.TryGetValue(productCode, out var code)
            ? code
            : throw new SymbolNotSupportedException(productCode);
    }

    private static IReadOnlyDictionary<RawProductCode, string> BuildProductCodeMap()
    {
        var map = new Dictionary<RawProductCode, string>();
        foreach (var value in Enum.GetValues<RawProductCode>())
        {
            if (value == RawProductCode.Unknown) continue;
            var member = typeof(RawProductCode).GetField(value.ToString());
            var attr = member?.GetCustomAttribute<EnumMemberAttribute>();
            var code = string.IsNullOrWhiteSpace(attr?.Value) ? value.ToString() : attr.Value!;
            map[value] = code;
        }

        return map;
    }

    public static OrderState MapOrderStatus(string childOrderStatusState) =>
        (childOrderStatusState ?? string.Empty).ToUpperInvariant() switch
        {
            "ACTIVE" => OrderState.Active,
            "COMPLETED" => OrderState.Completed,
            "CANCELED" => OrderState.Canceled,
            "EXPIRED" => OrderState.Expired,
            _ => OrderState.Unknown,
        };

}
