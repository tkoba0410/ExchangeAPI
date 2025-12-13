using System;
using Common.Contract.Enums;
using Common.Contract.Dtos;
using Common.Contract.Errors;
using Exchange.Bitflyer.Raw;
using RawProductCode = Exchange.Bitflyer.Raw.ProductCode;

namespace Exchange.Bitflyer.Abstract;

internal static class BitflyerCommonMapper
{
    public static OrderSide MapSide(Side side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static OrderSide MapSide(string side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static Side MapSideToExchange(OrderSide side) =>
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

    public static string ToApiProductCode(RawProductCode productCode) =>
        productCode switch
        {
            RawProductCode.BtcJpy => "BTC_JPY",
            RawProductCode.EthJpy => "ETH_JPY",
            RawProductCode.FxBtcJpy => "FX_BTC_JPY",
            _ => "BTC_JPY",
        };

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
