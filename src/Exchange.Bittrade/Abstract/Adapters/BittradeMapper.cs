using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExchangeApi.Adapter.Bittrade.RawApi;
using Common.Contract.Dtos;
using Common.Contract.Enums;
using Common.Contract.Errors;

namespace ExchangeApi.Adapter.Bittrade.Adapters;

internal static class BittradeMapper
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public static IReadOnlyList<ExchangeBalance> MapBalances(BittradeBalanceData data)
    {
        var result = new List<ExchangeBalance>();
        foreach (var group in data.List.GroupBy(e => e.Currency, StringComparer.OrdinalIgnoreCase))
        {
            var total = group.Sum(e => ParseDecimal(e.Balance));
            var available = group
                .Where(x => string.Equals(x.Type, "trade", StringComparison.OrdinalIgnoreCase))
                .Sum(e => ParseDecimal(e.Balance));
            result.Add(ExchangeBalance.Create(
                exchange: Exchange,
                currency: group.Key.ToUpperInvariant(),
                amount: total,
                available: available));
        }
        return result;
    }

    public static OpenOrder MapOrder(BittradeOrderDetail detail)
    {
        var (side, type) = ParseOrderType(detail.Type);
        var status = ParseStatus(detail.State);
        var size = ParseDecimal(detail.Amount);
        var filled = ParseDecimal(detail.FilledAmount);
        var outstanding = Math.Max(0, size - filled);

        return new OpenOrder(
            ProductCode: ToCanonicalSymbol(detail.Symbol),
            OrderId: detail.Id.ToString(CultureInfo.InvariantCulture),
            OrderAcceptanceId: detail.Id.ToString(CultureInfo.InvariantCulture),
            Side: side,
            OrderType: type,
            Size: size,
            OutstandingSize: outstanding,
            ExecutedSize: filled,
            Price: detail.Price is null ? (decimal?)null : ParseDecimal(detail.Price),
            ClientOrderId: detail.ClientOrderId);
    }

    public static OpenOrder MapOrderSummary(BittradeOrderSummary summary)
    {
        var (side, type) = ParseOrderType(summary.Type);
        var status = ParseStatus(summary.State);
        var size = ParseDecimal(summary.Amount);
        var filled = ParseDecimal(summary.FilledAmount);
        var outstanding = Math.Max(0, size - filled);

        return new OpenOrder(
            ProductCode: ToCanonicalSymbol(summary.Symbol),
            OrderId: summary.Id.ToString(CultureInfo.InvariantCulture),
            OrderAcceptanceId: summary.Id.ToString(CultureInfo.InvariantCulture),
            Side: side,
            OrderType: type,
            Size: size,
            OutstandingSize: outstanding,
            ExecutedSize: filled,
            Price: summary.Price is null ? (decimal?)null : ParseDecimal(summary.Price),
            ClientOrderId: summary.ClientOrderId);
    }

    public static OrderState ParseStatus(string state)
    {
        return state switch
        {
            "submitted" => OrderState.Active,
            "partial-filled" => OrderState.Active,
            "filled" => OrderState.Completed,
            "partial-canceled" => OrderState.Canceled,
            "canceled" => OrderState.Canceled,
            "expired" => OrderState.Expired,
            _ => OrderState.Unknown
        };
    }

    public static (OrderSide Side, OrderType OrderType) ParseOrderType(string type)
    {
        // format: buy-market, sell-limit, buy-limit
        var parts = type.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) throw new ExchangeApiException($"Unsupported order type: {type}");

        var side = string.Equals(parts[0], "buy", StringComparison.OrdinalIgnoreCase)
            ? OrderSide.Buy
            : OrderSide.Sell;

        var orderType = parts[1] switch
        {
            "market" => OrderType.Market,
            "limit" => OrderType.Limit,
            _ => throw new ExchangeApiException($"Unsupported order subtype: {parts[1]}")
        };

        return (side, orderType);
    }

    private static decimal ParseDecimal(string s) =>
        decimal.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);

    private static string ToCanonicalSymbol(string symbol)
    {
        if (symbol.Contains('/')) return symbol.ToUpperInvariant();
        var upper = symbol.ToUpperInvariant();
        if (upper.EndsWith("JPY", StringComparison.Ordinal))
        {
            var basePart = upper[..^3];
            return $"{basePart}/JPY";
        }
        if (upper.Length >= 6)
        {
            var mid = upper.Length / 2;
            return $"{upper[..mid]}/{upper[mid..]}";
        }
        return upper;
    }
}
