using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Types;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

internal static class BitflyerCommonMapper
{
    public static ContractSide MapSide(string side) =>
        BitflyerSideMapper.ToContractSide(BitflyerSideMapper.ToExchangeSide(side));

    public static ContractSide MapSide(BitflyerSide side) =>
        BitflyerSideMapper.ToContractSide(side);

    public static string MapSideToExchange(ContractSide side) =>
        BitflyerSideMapper.ToApi(BitflyerSideMapper.FromContractSide(side));

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
