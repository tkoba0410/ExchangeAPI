using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Primitives.CallCommon;
using RawPrivateDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class PrivateCallMapStage
{
    public static bool TryMapOrderType(OrderType orderType, Price? price, out ChildOrderType mapped, out CallError? error)
    {
        switch (orderType)
        {
            case OrderType.Market:
                mapped = ChildOrderType.Market;
                error = null;
                return true;
            case OrderType.Limit:
                mapped = ChildOrderType.Limit;
                error = null;
                return true;
            default:
                mapped = default;
                error = new CallError(CallErrorKind.Mapping, $"Unsupported child_order_type: {orderType}.");
                return false;
        }
    }

    public static bool TryToApiChildOrderType(ChildOrderType childOrderType, out string apiType, out CallError? error)
    {
        apiType = childOrderType switch
        {
            ChildOrderType.Market => "MARKET",
            ChildOrderType.Limit => "LIMIT",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(apiType))
        {
            error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer child_order_type: {childOrderType}.");
            return false;
        }

        error = null;
        return true;
    }

    public static bool TryParseChildOrderType(string childOrderType, out ChildOrderType parsed, out CallError? error)
    {
        switch ((childOrderType ?? string.Empty).ToUpperInvariant())
        {
            case "LIMIT":
                parsed = ChildOrderType.Limit;
                error = null;
                return true;
            case "MARKET":
                parsed = ChildOrderType.Market;
                error = null;
                return true;
            default:
                parsed = default;
                error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer child_order_type: {childOrderType ?? "<null>"}.");
                return false;
        }
    }

    public static bool TryToOrderType(ChildOrderType childOrderType, out OrderType orderType, out CallError? error)
    {
        switch (childOrderType)
        {
            case ChildOrderType.Limit:
                orderType = OrderType.Limit;
                error = null;
                return true;
            case ChildOrderType.Market:
                orderType = OrderType.Market;
                error = null;
                return true;
            default:
                orderType = default;
                error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer child_order_type: {childOrderType}.");
                return false;
        }
    }

    public static bool TryValidateOrderRequest(OrderRequest request, out CallError? error)
    {
        if (request is null)
        {
            error = new CallError(CallErrorKind.Semantic, "Request is required.");
            return false;
        }

        if (request.Symbol.IsEmpty)
        {
            error = new CallError(CallErrorKind.Semantic, "Symbol is required.");
            return false;
        }

        if (request.Size.Value <= 0)
        {
            error = new CallError(CallErrorKind.Semantic, "Size must be greater than zero.");
            return false;
        }

        if (request.Price is { } price && price.Value <= 0)
        {
            error = new CallError(CallErrorKind.Semantic, "Price must be greater than zero when specified.");
            return false;
        }

        switch (request.OrderType)
        {
            case OrderType.Market:
                if (request.Price is not null)
                {
                    error = new CallError(CallErrorKind.Semantic, "Market order must not specify Price.");
                    return false;
                }
                break;
            case OrderType.Limit:
                if (request.Price is null)
                {
                    error = new CallError(CallErrorKind.Semantic, "Limit order requires Price.");
                    return false;
                }
                break;
            default:
                error = new CallError(CallErrorKind.Mapping, $"Unsupported order type: {request.OrderType}.");
                return false;
        }

        error = null;
        return true;
    }

    public static bool TryMapBalances(
        IReadOnlyList<RawPrivateDtos.GetBalanceItem> rawBalances,
        out IReadOnlyList<BalanceEntryNormalized>? normalized,
        out CallError? error)
    {
        if (rawBalances is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer balances response is null.");
            return false;
        }

        normalized = rawBalances
            .Select(balance => new BalanceEntryNormalized(
                CurrencyCode: CurrencyCodeConverter.FromString(balance.CurrencyCode),
                Amount: balance.Amount,
                Available: balance.Available))
            .ToArray();
        error = null;
        return true;
    }

    public static bool TryMapAccountExecutions(
        IReadOnlyList<RawPrivateDtos.GetExecutionsPrivateItem> rawExecutions,
        out IReadOnlyList<ExecutionAccountNormalized>? normalized,
        out CallError? error)
    {
        if (rawExecutions is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer executions response is null.");
            return false;
        }

        var mapped = new List<ExecutionAccountNormalized>(rawExecutions.Count);
        foreach (var execution in rawExecutions)
        {
            if (!CommonMapper.TryMapSide(execution.Side, out var side, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(new ExecutionAccountNormalized(
                Id: execution.Id,
                ProductCode: ProductCode.ParseNormalized(execution.ProductCode),
                Side: side,
                Price: new Price(execution.Price),
                Size: new Size(execution.Size),
                ExecutedAt: execution.ExecDate,
                ChildOrderAcceptanceId: AcceptanceId.TryParse(execution.ChildOrderAcceptanceId, out var acceptanceId)
                    ? acceptanceId
                    : null));
        }

        normalized = mapped.ToArray();
        error = null;
        return true;
    }
}
