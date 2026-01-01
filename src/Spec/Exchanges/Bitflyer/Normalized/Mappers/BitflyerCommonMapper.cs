using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using ContractSide = ExchangeApi.Common.Enums.Side;
using RawSide = ExchangeApi.Exchanges.Bitflyer.Raw.Side;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.Types.RawProductCode;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerCommonMapper
{
    public static ContractSide MapSide(RawSide side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static ContractSide MapSide(string side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static RawSide MapSideToExchange(ContractSide side) =>
        BitflyerSideMapper.ToRawSide(side);

    public static string ToApiProductCode(RawProductCode productCode) =>
        string.IsNullOrWhiteSpace(productCode.Value)
            ? throw new SymbolNotSupportedException(productCode.ToString())
            : productCode.Value;

    public static RawProductCode ParseProductCode(string productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new SymbolNotSupportedException(productCode ?? string.Empty);
        }

        return new RawProductCode(productCode);
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
