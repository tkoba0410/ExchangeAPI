using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ContractSide = ExchangeApi.Common.Enums.Side;
using RawSide = ExchangeApi.Exchanges.Bitflyer.Raw.Side;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.ProductCode;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;

internal static class BitflyerCommonMapper
{
    public static ContractSide MapSide(RawSide side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static ContractSide MapSide(string side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static RawSide MapSideToExchange(ContractSide side) =>
        BitflyerSideMapper.ToRawSide(side);

    public static RawProductCode MapSymbolToProductCode(string symbol)
    {
        if (string.Equals(symbol, "BTC/JPY", StringComparison.Ordinal) ||
            string.Equals(symbol, "BTC_JPY", StringComparison.Ordinal))
        {
            return RawProductCode.BtcJpy;
        }

        if (string.Equals(symbol, "ETH/JPY", StringComparison.Ordinal) ||
            string.Equals(symbol, "ETH_JPY", StringComparison.Ordinal))
        {
            return RawProductCode.EthJpy;
        }

        if (string.Equals(symbol, "FX_BTC_JPY", StringComparison.Ordinal) ||
            string.Equals(symbol, "FX_BTC/JPY", StringComparison.Ordinal))
        {
            return RawProductCode.FxBtcJpy;
        }

        throw new SymbolNotSupportedException(symbol);
    }

    public static RawProductCode MapSymbolToProductCode(Symbol symbol) =>
        symbol switch
        {
            Symbol.BtcJpy => RawProductCode.BtcJpy,
            Symbol.EthJpy => RawProductCode.EthJpy,
            Symbol.FxBtcJpy => RawProductCode.FxBtcJpy,
            _ => throw new SymbolNotSupportedException(symbol.ToString())
        };

    public static string ToApiProductCode(RawProductCode productCode) =>
        productCode switch
        {
            RawProductCode.BtcJpy => "BTC_JPY",
            RawProductCode.EthJpy => "ETH_JPY",
            RawProductCode.FxBtcJpy => "FX_BTC_JPY",
            _ => "BTC_JPY",
        };

    public static Symbol ToSymbol(string symbol)
    {
        var productCode = MapSymbolToProductCode(symbol);
        return productCode switch
        {
            RawProductCode.BtcJpy => Symbol.BtcJpy,
            RawProductCode.EthJpy => Symbol.EthJpy,
            RawProductCode.FxBtcJpy => Symbol.FxBtcJpy,
            _ => Symbol.Unknown
        };
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
