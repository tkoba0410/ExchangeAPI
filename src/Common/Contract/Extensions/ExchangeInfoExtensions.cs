using System;
using System.Linq;
using Common.Contract.Dtos;
using Common.Contract.Enums;

namespace Common.Contract.Extensions;

/// <summary>
/// ExchangeInfo の検索ヘルパ。
/// </summary>
public static class ExchangeInfoExtensions
{
    /// <summary>
    /// シンボルまたは productCode でマーケット情報を探す。
    /// </summary>
    public static ExchangeMarketInfo? FindMarket(this ExchangeInfo info, string symbol, string? productCode = null)
    {
        if (info is null) throw new ArgumentNullException(nameof(info));
        if (symbol is null) throw new ArgumentNullException(nameof(symbol));

        return info.Markets.FirstOrDefault(m =>
            string.Equals(m.Symbol, symbol, StringComparison.Ordinal) ||
            (!string.IsNullOrEmpty(productCode) && string.Equals(m.ProductCode, productCode, StringComparison.Ordinal)));
    }

    /// <summary>
    /// 手数料レート/通貨/種別を取得する。見つからない場合は false を返し、出力は null。
    /// </summary>
    public static bool TryGetFeeRates(
        this ExchangeInfo info,
        string symbol,
        out decimal? makerFeeRate,
        out decimal? takerFeeRate,
        out string? feeCurrency,
        out FeeType? feeType,
        string? productCode = null)
    {
        if (info is null) throw new ArgumentNullException(nameof(info));
        if (symbol is null) throw new ArgumentNullException(nameof(symbol));

        var market = info.FindMarket(symbol, productCode);
        if (market is null)
        {
            makerFeeRate = null;
            takerFeeRate = null;
            feeCurrency = null;
            feeType = null;
            return false;
        }

        makerFeeRate = market.MakerFeeRate;
        takerFeeRate = market.TakerFeeRate;
        feeCurrency = market.FeeCurrency;
        feeType = market.FeeType;
        return true;
    }
}
