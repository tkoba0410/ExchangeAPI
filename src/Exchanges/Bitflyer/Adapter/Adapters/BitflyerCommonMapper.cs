using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ContractSide = ExchangeApi.Common.Enums.Side;
using RawSide = ExchangeApi.Exchanges.Bitflyer.Wire.Side;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Wire.ProductCode;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;

internal static class BitflyerCommonMapper
{
    public static ContractSide MapSide(RawSide side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static ContractSide MapSide(string side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static RawSide MapSideToExchange(ContractSide side) =>
        BitflyerSideMapper.ToRawSide(side);

    public static RawProductCode MapSymbolToProductCode(string symbol) =>
        BitflyerSymbolMapper.ToProductCode(symbol);

    public static RawProductCode MapSymbolToProductCode(Symbol symbol) =>
        BitflyerSymbolMapper.ToProductCode(symbol);

    public static string ToApiProductCode(RawProductCode productCode) =>
        BitflyerSymbolMapper.ToApiProductCode(productCode);

    public static Symbol ToSymbol(string symbol)
    {
        return BitflyerSymbolMapper.FromProductCode(symbol);
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
