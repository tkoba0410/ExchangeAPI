using System;
using System.Linq;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Utilities.Extensions;

/// <summary>
/// ExchangeInfo の検索ヘルパ。
/// </summary>
public static class ExchangeInfoExtensions
{
    /// <summary>
    /// シンボルまたは productCode でマーケット情報を探す。
    /// </summary>
    public static ExchangeMarketInfo? FindMarket(this GetExchangeInfoResponse info, Symbol symbol, ProductCode? productCode = null)
    {
        if (info is null) throw new ArgumentNullException(nameof(info));

        return info.Markets.FirstOrDefault(m =>
            m.Symbol.Equals(symbol) ||
            (productCode is { } code && m.ProductCode.Equals(code)));
    }

    /// <summary>
    /// 手数料レート/通貨/種別を取得する。見つからない場合は false を返し、出力は null。
    /// </summary>
    public static bool TryGetFeeRates(
        this GetExchangeInfoResponse info,
        Symbol symbol,
        out decimal? makerFeeRate,
        out decimal? takerFeeRate,
        out CurrencyCode? feeCurrency,
        out FeeType? feeType,
        ProductCode? productCode = null)
    {
        if (info is null) throw new ArgumentNullException(nameof(info));

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
