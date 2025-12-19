using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;

internal static class BittradeMapper
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public static IReadOnlyList<Balance> MapBalances(BittradeBalanceData data)
    {
        var result = new List<Balance>();
        foreach (var group in data.List.GroupBy(e => e.Currency, StringComparer.OrdinalIgnoreCase))
        {
            var total = group.Sum(e => ParseDecimal(e.Balance));
            var available = group
                .Where(x => string.Equals(x.Type, "trade", StringComparison.OrdinalIgnoreCase))
                .Sum(e => ParseDecimal(e.Balance));
            result.Add(Balance.Create(
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
            ExchangeCode: Exchange,
            Symbol: BittradeSymbolMapper.Parse(detail.Symbol),
            Key: new OrderKey(OrderIdKind.ExchangeOrderId, detail.Id.ToString(CultureInfo.InvariantCulture)),
            Side: side,
            OrderType: type,
            Size: size,
            OutstandingSize: outstanding,
            ExecutedSize: filled,
            Price: detail.Price is null ? (decimal?)null : ParseDecimal(detail.Price),
            OrderedAt: DateTimeOffset.FromUnixTimeMilliseconds(detail.CreatedAt),
            UpdatedAt: detail.FinishedAt.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(detail.FinishedAt.Value) : null,
            StopPrice: null,
            Status: detail.State,
            ExchangeOrderId: detail.Id.ToString(CultureInfo.InvariantCulture));
    }

    public static OpenOrder MapOrderSummary(BittradeOrderSummary summary)
    {
        var (side, type) = ParseOrderType(summary.Type);
        var status = ParseStatus(summary.State);
        var size = ParseDecimal(summary.Amount);
        var filled = ParseDecimal(summary.FilledAmount);
        var outstanding = Math.Max(0, size - filled);

        return new OpenOrder(
            ExchangeCode: Exchange,
            Symbol: BittradeSymbolMapper.Parse(summary.Symbol),
            Key: new OrderKey(OrderIdKind.ExchangeOrderId, summary.Id.ToString(CultureInfo.InvariantCulture)),
            Side: side,
            OrderType: type,
            Size: size,
            OutstandingSize: outstanding,
            ExecutedSize: filled,
            Price: summary.Price is null ? (decimal?)null : ParseDecimal(summary.Price),
            OrderedAt: DateTimeOffset.FromUnixTimeMilliseconds(summary.CreatedAt),
            UpdatedAt: null,
            StopPrice: null,
            Status: summary.State,
            ExchangeOrderId: summary.Id.ToString(CultureInfo.InvariantCulture));
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

    public static (Side Side, OrderType OrderType) ParseOrderType(string type)
    {
        // format: buy-market, sell-limit, buy-limit
        var parts = type.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) throw new ExchangeApiException($"Unsupported order type: {type}");

        var side = string.Equals(parts[0], "buy", StringComparison.OrdinalIgnoreCase)
            ? Side.Buy
            : Side.Sell;

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

    public static string ToProductCode(Symbol symbol) =>
        BittradeSymbolMapper.ToProductCode(symbol);
}
