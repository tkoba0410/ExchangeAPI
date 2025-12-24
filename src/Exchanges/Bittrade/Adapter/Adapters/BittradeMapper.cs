using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExchangeApi.Exchanges.Bittrade.Raw;
using RawOrderState = ExchangeApi.Exchanges.Bittrade.Raw.OrderState;
using RawOrderType = ExchangeApi.Exchanges.Bittrade.Raw.OrderType;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Core.Contracts.Errors;
using CommonOrderState = ExchangeApi.Common.Enums.OrderState;
using CommonOrderType = ExchangeApi.Common.Enums.OrderType;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;

internal static class BittradeMapper
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public static IReadOnlyList<Balance> MapBalances(BalanceData data)
    {
        var result = new List<Balance>();
        foreach (var group in data.List.GroupBy(e => e.Currency, StringComparer.OrdinalIgnoreCase))
        {
            var total = group.Sum(e => ParseDecimalOrThrow(e.Balance, "balance", "BalanceEntry"));
            var available = group
                .Where(x => string.Equals(x.Type, "trade", StringComparison.OrdinalIgnoreCase))
                .Sum(e => ParseDecimalOrThrow(e.Balance, "balance", "BalanceEntry"));
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
        var sizeValue = ParseDecimalOrThrow(detail.Amount, "amount", "OrderDetail");
        var filledValue = ParseDecimalOrThrow(detail.FilledAmount, "field-amount", "OrderDetail");
        var outstandingValue = Math.Max(0, sizeValue - filledValue);

        return new OpenOrder(
            ExchangeCode: Exchange,
            Symbol: CommonSymbol.Parse(detail.Symbol.Value),
            Key: new OrderKey(OrderIdKind.ExchangeOrderId, detail.Id.Value),
            Side: side,
            OrderType: type,
            Size: new Size(sizeValue),
            OutstandingSize: new Size(outstandingValue),
            ExecutedSize: new Size(filledValue),
            Price: detail.Price is null ? (Price?)null : new Price(ParseDecimalOrThrow(detail.Price, "price", "OrderDetail")),
            OrderedAt: detail.CreatedAt,
            UpdatedAt: detail.FinishedAt,
            StopPrice: null,
            Status: ToWireValue(detail.State),
            ExchangeOrderId: detail.Id.Value);
    }

    public static OpenOrder MapOrderSummary(OrderSummary summary)
    {
        var (side, type) = ParseOrderType(summary.Type);
        var status = ParseStatus(summary.State);
        var sizeValue = ParseDecimalOrThrow(summary.Amount, "amount", "OrderSummary");
        var filledValue = ParseDecimalOrThrow(summary.FilledAmount, "field-amount", "OrderSummary");
        var outstandingValue = Math.Max(0, sizeValue - filledValue);

        return new OpenOrder(
            ExchangeCode: Exchange,
            Symbol: CommonSymbol.Parse(summary.Symbol.Value),
            Key: new OrderKey(OrderIdKind.ExchangeOrderId, summary.Id.Value),
            Side: side,
            OrderType: type,
            Size: new Size(sizeValue),
            OutstandingSize: new Size(outstandingValue),
            ExecutedSize: new Size(filledValue),
            Price: summary.Price is null ? (Price?)null : new Price(ParseDecimalOrThrow(summary.Price, "price", "OrderSummary")),
            OrderedAt: summary.CreatedAt,
            UpdatedAt: null,
            StopPrice: null,
            Status: ToWireValue(summary.State),
            ExchangeOrderId: summary.Id.Value);
    }

    public static CommonOrderState ParseStatus(RawOrderState state)
    {
        return state switch
        {
            RawOrderState.Submitted => CommonOrderState.Active,
            RawOrderState.PartialFilled => CommonOrderState.Active,
            RawOrderState.Filled => CommonOrderState.Completed,
            RawOrderState.PartialCanceled => CommonOrderState.Canceled,
            RawOrderState.Canceled => CommonOrderState.Canceled,
            _ => CommonOrderState.Unknown
        };
    }

    public static (Side Side, CommonOrderType OrderType) ParseOrderType(RawOrderType type)
    {
        return type switch
        {
            RawOrderType.BuyMarket => (Side.Buy, CommonOrderType.Market),
            RawOrderType.SellMarket => (Side.Sell, CommonOrderType.Market),
            RawOrderType.BuyLimit => (Side.Buy, CommonOrderType.Limit),
            RawOrderType.SellLimit => (Side.Sell, CommonOrderType.Limit),
            RawOrderType.BuyLimitMaker => (Side.Buy, CommonOrderType.Limit),
            RawOrderType.SellLimitMaker => (Side.Sell, CommonOrderType.Limit),
            RawOrderType.BuyIoc => (Side.Buy, CommonOrderType.Limit),
            RawOrderType.SellIoc => (Side.Sell, CommonOrderType.Limit),
            _ => throw new ExchangeApiException($"Unsupported order type: {type}")
        };
    }

    private static decimal ParseDecimalOrThrow(string s, string field, string dto)
    {
        if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new ExchangeApiException($"Invalid decimal for {dto}.{field}: '{s}'.");
    }

    private static string ToWireValue(RawOrderState state)
    {
        return state switch
        {
            RawOrderState.Submitted => "submitted",
            RawOrderState.PartialFilled => "partial-filled",
            RawOrderState.PartialCanceled => "partial-canceled",
            RawOrderState.Filled => "filled",
            RawOrderState.Canceled => "canceled",
            _ => state.ToString()
        };
    }
}
