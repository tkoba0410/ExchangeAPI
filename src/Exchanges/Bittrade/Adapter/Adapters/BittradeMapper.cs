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
        var sizeValue = ParseDecimal(detail.Amount);
        var filledValue = ParseDecimal(detail.FilledAmount);
        var outstandingValue = Math.Max(0, sizeValue - filledValue);

        return new OpenOrder(
            ExchangeCode: Exchange,
            Symbol: BittradeSymbolMapper.Parse(detail.Symbol.Value),
            Key: new OrderKey(OrderIdKind.ExchangeOrderId, detail.Id.Value),
            Side: side,
            OrderType: type,
            Size: new Size(sizeValue),
            OutstandingSize: new Size(outstandingValue),
            ExecutedSize: new Size(filledValue),
            Price: detail.Price is null ? (Price?)null : new Price(ParseDecimal(detail.Price)),
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
        var sizeValue = ParseDecimal(summary.Amount);
        var filledValue = ParseDecimal(summary.FilledAmount);
        var outstandingValue = Math.Max(0, sizeValue - filledValue);

        return new OpenOrder(
            ExchangeCode: Exchange,
            Symbol: BittradeSymbolMapper.Parse(summary.Symbol.Value),
            Key: new OrderKey(OrderIdKind.ExchangeOrderId, summary.Id.Value),
            Side: side,
            OrderType: type,
            Size: new Size(sizeValue),
            OutstandingSize: new Size(outstandingValue),
            ExecutedSize: new Size(filledValue),
            Price: summary.Price is null ? (Price?)null : new Price(ParseDecimal(summary.Price)),
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

    private static decimal ParseDecimal(string s) =>
        decimal.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);

    public static string ToProductCode(CommonSymbol symbol) =>
        BittradeSymbolMapper.ToProductCode(symbol);

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
