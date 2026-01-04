using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using ContractSide = ExchangeApi.Common.Enums.Side;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerCommonMapper
{
    public static ContractSide MapSide(string side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static string MapSideToExchange(ContractSide side) =>
        BitflyerSideMapper.ToApi(side);

    public static string ToApiProductCode(string productCode) =>
        string.IsNullOrWhiteSpace(productCode)
            ? throw new SymbolNotSupportedException(productCode ?? string.Empty)
            : productCode;

    public static string ParseProductCode(string productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new SymbolNotSupportedException(productCode ?? string.Empty);
        }

        return productCode;
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
