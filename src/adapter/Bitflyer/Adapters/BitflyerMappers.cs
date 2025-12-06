using System;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;

namespace ExchangeApi.Adapter.Bitflyer.Adapters;

internal static class BitflyerMappers
{
    public static string MapOrderType(OrderType orderType, decimal? price)
    {
        return orderType switch
        {
            OrderType.Market => "MARKET",
            OrderType.Limit => "LIMIT",
            OrderType.Stop => price is null ? "STOP" : "STOP_LIMIT",
            _ => "MARKET",
        };
    }

    public static OrderType MapOrderTypeFromExchange(string childOrderType)
    {
        return childOrderType.ToUpperInvariant() switch
        {
            "LIMIT" => OrderType.Limit,
            "MARKET" => OrderType.Market,
            "STOP" or "STOP_LIMIT" => OrderType.Stop,
            _ => OrderType.Market,
        };
    }

    public static string? MapTimeInForce(TimeInForce? tif)
    {
        return tif switch
        {
            TimeInForce.Gtc => "GTC",
            TimeInForce.Ioc => "IOC",
            TimeInForce.Fok => "FOK",
            _ => null,
        };
    }

    public static OrderSide MapSide(string side)
    {
        return string.Equals(side, "BUY", StringComparison.OrdinalIgnoreCase)
            ? OrderSide.Buy
            : OrderSide.Sell;
    }

    public static OrderStatusType MapOrderStatusType(string childOrderState)
    {
        return childOrderState.ToUpperInvariant() switch
        {
            "ACTIVE" => OrderStatusType.Active,
            "COMPLETED" => OrderStatusType.Completed,
            "CANCELED" => OrderStatusType.Canceled,
            "EXPIRED" => OrderStatusType.Expired,
            _ => OrderStatusType.Unknown,
        };
    }

    public static string MapSymbolToProductCode(string symbol)
    {
        if (string.Equals(symbol, Symbols.BtcJpy, StringComparison.Ordinal))
        {
            return "BTC_JPY";
        }

        throw new SymbolNotSupportedException(symbol);
    }

    public static void ValidateOrderRequest(OrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode))
        {
            throw new ArgumentException("ProductCode is required.", nameof(request));
        }

        if (request.Size <= 0)
        {
            throw new ArgumentException("Size must be greater than zero.", nameof(request));
        }

        if (request.MinuteToExpire is { } mte && mte <= 0)
        {
            throw new ArgumentException("MinuteToExpire must be positive when specified.", nameof(request));
        }

        if (request.Price is { } price && price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero when specified.", nameof(request));
        }

        if (request.TriggerPrice is { } tp && tp <= 0)
        {
            throw new ArgumentException("TriggerPrice must be greater than zero when specified.", nameof(request));
        }

        switch (request.OrderType)
        {
            case OrderType.Market:
                if (request.Price is not null || request.TriggerPrice is not null)
                {
                    throw new ArgumentException("Market order must not specify Price or TriggerPrice.", nameof(request));
                }
                break;
            case OrderType.Limit:
                if (request.Price is null)
                {
                    throw new ArgumentException("Limit order requires Price.", nameof(request));
                }
                if (request.TriggerPrice is not null)
                {
                    throw new ArgumentException("Limit order must not specify TriggerPrice.", nameof(request));
                }
                break;
            case OrderType.Stop:
                if (request.TriggerPrice is null)
                {
                    throw new ArgumentException("Stop order requires TriggerPrice.", nameof(request));
                }
                if (request.Price is not null && request.Price <= 0)
                {
                    throw new ArgumentException("Stop order Price must be greater than zero when specified.", nameof(request));
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(request.OrderType), request.OrderType, "Unsupported order type.");
        }
    }

    public static ExchangeErrorCategory? MapErrorCategory(System.Net.HttpStatusCode? statusCode, string? exchangeCode)
    {
        var normalizedCode = string.IsNullOrWhiteSpace(exchangeCode)
            ? null
            : exchangeCode.Trim().ToUpperInvariant();

        if (normalizedCode is not null)
        {
            return normalizedCode switch
            {
                "INSUFFICIENT_FUNDS" => ExchangeErrorCategory.Balance,
                "NO_POSITION" => ExchangeErrorCategory.Balance,
                "INVALID_ORDER" or "INVALID_PRODUCT" or "PRODUCT_NOT_FOUND" => ExchangeErrorCategory.Request,
                "LIMIT_OVER" or "ORDER_NOT_ACCEPTABLE" => ExchangeErrorCategory.Request,
                "INVALID_REQUEST" => ExchangeErrorCategory.Request,
                "PARAM_ERROR" => ExchangeErrorCategory.Request,
                "AUTHENTICATION_ERROR" or "PERMISSION_DENIED" => ExchangeErrorCategory.Auth,
                "TOO_MANY_REQUESTS" => ExchangeErrorCategory.RateLimit,
                "SERVICE_UNAVAILABLE" or "INTERNAL_ERROR" => ExchangeErrorCategory.Server,
                "TIMEOUT" => ExchangeErrorCategory.Network,
                _ => ExchangeErrorCategory.Unknown,
            };
        }

        if (statusCode is null) return null;

        return statusCode.Value switch
        {
            System.Net.HttpStatusCode.BadRequest => ExchangeErrorCategory.Request,
            System.Net.HttpStatusCode.Unauthorized or System.Net.HttpStatusCode.Forbidden => ExchangeErrorCategory.Auth,
            (System.Net.HttpStatusCode)429 => ExchangeErrorCategory.RateLimit,
            System.Net.HttpStatusCode.InternalServerError or System.Net.HttpStatusCode.ServiceUnavailable => ExchangeErrorCategory.Server,
            _ => ExchangeErrorCategory.Unknown,
        };
    }

    public static ExchangeApiException EnrichBitflyerException(ExchangeApiException ex, string exchangeId, string operation)
    {
        if (ex.ExchangeId == exchangeId && ex.Operation == operation)
        {
            return ex;
        }

        var category = MapErrorCategory(ex.StatusCode, ex.ExchangeErrorCode);

        return new ExchangeApiException(
            message: ex.Message,
            exchangeId: exchangeId,
            operation: operation,
            statusCode: ex.StatusCode,
            exchangeErrorCode: ex.ExchangeErrorCode,
            errorCategory: category,
            innerException: ex);
    }
}
