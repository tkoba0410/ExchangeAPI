using System;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Wire.ProductCode;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;

internal static class BitflyerSymbolMapper
{
    public static RawProductCode ToProductCode(Symbol symbol)
    {
        if (symbol.IsEmpty)
        {
            throw new SymbolNotSupportedException(symbol.ToString());
        }

        return ToProductCode(symbol.Value);
    }

    public static RawProductCode ToProductCode(string symbol)
    {
        if (string.Equals(symbol, "BTC/JPY", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(symbol, "BTC_JPY", StringComparison.OrdinalIgnoreCase))
        {
            return RawProductCode.BtcJpy;
        }

        if (string.Equals(symbol, "ETH/JPY", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(symbol, "ETH_JPY", StringComparison.OrdinalIgnoreCase))
        {
            return RawProductCode.EthJpy;
        }

        if (string.Equals(symbol, "FX_BTC_JPY", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(symbol, "FX_BTC/JPY", StringComparison.OrdinalIgnoreCase))
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
            _ => throw new SymbolNotSupportedException(productCode.ToString()),
        };

    public static Symbol FromProductCode(string symbol)
    {
        var productCode = ToProductCode(symbol);
        return productCode switch
        {
            RawProductCode.BtcJpy => new Symbol("BTC/JPY"),
            RawProductCode.EthJpy => new Symbol("ETH/JPY"),
            RawProductCode.FxBtcJpy => new Symbol("FX_BTC/JPY"),
            _ => throw new SymbolNotSupportedException(symbol)
        };
    }
}
