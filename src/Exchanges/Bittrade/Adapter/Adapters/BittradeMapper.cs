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

    public static IReadOnlyList<Balance> MapBalances(BalanceData data)
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

    public static OpenOrder MapOrder(OrderDetail detail)
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
            Status: ToWireValue(detail.State),
            ExchangeOrderId: detail.Id.ToString(CultureInfo.InvariantCulture));
    }

    public static OpenOrder MapOrderSummary(OrderSummary summary)
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
            Status: ToWireValue(summary.State),
            ExchangeOrderId: summary.Id.ToString(CultureInfo.InvariantCulture));
    }

    public static OrderState ParseStatus(BittradeOrderState state)
    {
        return state switch
        {
            BittradeOrderState.Submitted => OrderState.Active,
            BittradeOrderState.PartialFilled => OrderState.Active,
            BittradeOrderState.Filled => OrderState.Completed,
            BittradeOrderState.PartialCanceled => OrderState.Canceled,
            BittradeOrderState.Canceled => OrderState.Canceled,
            _ => OrderState.Unknown
        };
    }

    public static (Side Side, OrderType OrderType) ParseOrderType(BittradeOrderType type)
    {
        return type switch
        {
            BittradeOrderType.BuyMarket => (Side.Buy, OrderType.Market),
            BittradeOrderType.SellMarket => (Side.Sell, OrderType.Market),
            BittradeOrderType.BuyLimit => (Side.Buy, OrderType.Limit),
            BittradeOrderType.SellLimit => (Side.Sell, OrderType.Limit),
            BittradeOrderType.BuyLimitMaker => (Side.Buy, OrderType.Limit),
            BittradeOrderType.SellLimitMaker => (Side.Sell, OrderType.Limit),
            BittradeOrderType.BuyIoc => (Side.Buy, OrderType.Limit),
            BittradeOrderType.SellIoc => (Side.Sell, OrderType.Limit),
            _ => throw new ExchangeApiException($"Unsupported order type: {type}")
        };
    }

    private static decimal ParseDecimal(string s) =>
        decimal.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);

    public static string ToProductCode(Symbol symbol) =>
        BittradeSymbolMapper.ToProductCode(symbol);

    private static string ToWireValue(BittradeOrderState state)
    {
        return state switch
        {
            BittradeOrderState.Submitted => "submitted",
            BittradeOrderState.PartialFilled => "partial-filled",
            BittradeOrderState.PartialCanceled => "partial-canceled",
            BittradeOrderState.Filled => "filled",
            BittradeOrderState.Canceled => "canceled",
            _ => state.ToString()
        };
    }
}
