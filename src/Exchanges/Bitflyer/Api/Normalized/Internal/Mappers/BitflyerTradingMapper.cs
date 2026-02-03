using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerTradingMapper
{
    public static bool TryMapOrderType(OrderType orderType, Price? price, out BitflyerChildOrderType mapped, out CallError? error)
    {
        switch (orderType)
        {
            case OrderType.Market:
                mapped = BitflyerChildOrderType.Market;
                error = null;
                return true;
            case OrderType.Limit:
                mapped = BitflyerChildOrderType.Limit;
                error = null;
                return true;
            default:
                mapped = default;
                error = new CallError(CallErrorKind.Mapping, $"Unsupported child_order_type: {orderType}.");
                return false;
        }
    }

    public static bool TryToApiChildOrderType(BitflyerChildOrderType childOrderType, out string apiType, out CallError? error)
    {
        apiType = childOrderType switch
        {
            BitflyerChildOrderType.Market => "MARKET",
            BitflyerChildOrderType.Limit => "LIMIT",
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

    public static bool TryParseChildOrderType(string childOrderType, out BitflyerChildOrderType parsed, out CallError? error)
    {
        switch ((childOrderType ?? string.Empty).ToUpperInvariant())
        {
            case "LIMIT":
                parsed = BitflyerChildOrderType.Limit;
                error = null;
                return true;
            case "MARKET":
                parsed = BitflyerChildOrderType.Market;
                error = null;
                return true;
            default:
                parsed = default;
                error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer child_order_type: {childOrderType ?? "<null>"}.");
                return false;
        }
    }

    public static bool TryToOrderType(BitflyerChildOrderType childOrderType, out OrderType orderType, out CallError? error)
    {
        switch (childOrderType)
        {
            case BitflyerChildOrderType.Limit:
                orderType = OrderType.Limit;
                error = null;
                return true;
            case BitflyerChildOrderType.Market:
                orderType = OrderType.Market;
                error = null;
                return true;
            default:
                orderType = default;
                error = new CallError(CallErrorKind.Mapping, $"Unknown bitFlyer child_order_type: {childOrderType}.");
                return false;
        }
    }

    public static bool TryValidateOrderRequest(BitflyerOrderRequest request, out CallError? error)
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
}
