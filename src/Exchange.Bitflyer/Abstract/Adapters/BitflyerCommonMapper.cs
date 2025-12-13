using System;
using Exchange.Bitflyer.Raw;
using Common.Contract.Dtos;
using Common.Contract.Errors;

namespace Exchange.Bitflyer.Abstract;

internal static class BitflyerCommonMapper
{
    public static OrderSide MapSide(Side side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static OrderSide MapSide(string side) =>
        BitflyerSideMapper.ToOrderSide(side);

    public static Side MapSideToExchange(OrderSide side) =>
        BitflyerSideMapper.ToRawSide(side);

    public static ProductCode MapSymbolToProductCode(string symbol)
    {
        if (string.Equals(symbol, "BTC/JPY", StringComparison.Ordinal) ||
            string.Equals(symbol, "BTC_JPY", StringComparison.Ordinal))
        {
            return ProductCode.BtcJpy;
        }

        if (string.Equals(symbol, "ETH/JPY", StringComparison.Ordinal) ||
            string.Equals(symbol, "ETH_JPY", StringComparison.Ordinal))
        {
            return ProductCode.EthJpy;
        }

        if (string.Equals(symbol, "FX_BTC_JPY", StringComparison.Ordinal) ||
            string.Equals(symbol, "FX_BTC/JPY", StringComparison.Ordinal))
        {
            return ProductCode.FxBtcJpy;
        }

        throw new SymbolNotSupportedException(symbol);
    }

    public static string ToApiProductCode(ProductCode productCode) =>
        productCode switch
        {
            ProductCode.BtcJpy => "BTC_JPY",
            ProductCode.EthJpy => "ETH_JPY",
            ProductCode.FxBtcJpy => "FX_BTC_JPY",
            _ => "BTC_JPY",
        };

    public static OrderStatusType MapOrderStatusType(string childOrderState) =>
        (childOrderState ?? string.Empty).ToUpperInvariant() switch
        {
            "ACTIVE" => OrderStatusType.Active,
            "COMPLETED" => OrderStatusType.Completed,
            "CANCELED" => OrderStatusType.Canceled,
            "EXPIRED" => OrderStatusType.Expired,
            _ => OrderStatusType.Unknown,
        };

}
