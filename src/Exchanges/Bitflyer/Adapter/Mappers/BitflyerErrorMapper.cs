using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;

internal static class BitflyerErrorMapper
{
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

    public static TransportErrorCategory? MapTransportErrorCategory(System.Net.HttpStatusCode? statusCode, string? exchangeCode)
    {
        return ToTransportErrorCategory(MapErrorCategory(statusCode, exchangeCode));
    }

    public static ExchangeApiException EnrichBitflyerException(ExchangeApiException ex, string operation)
    {
        var category = MapErrorCategory(ex.StatusCode, ex.ExchangeErrorCode);

        return new ExchangeApiException(
            message: ex.Message,
            operation: operation,
            statusCode: ex.StatusCode,
            exchangeErrorCode: ex.ExchangeErrorCode,
            errorCategory: category,
            innerException: ex);
    }

    public static ExchangeApiException FromTransportException(TransportException ex, string operation)
    {
        var category = ToExchangeErrorCategory(ex.ErrorCategory) ?? MapErrorCategory(ex.StatusCode, ex.ErrorCode);

        return new ExchangeApiException(
            message: ex.Message,
            operation: operation,
            statusCode: ex.StatusCode,
            exchangeErrorCode: ex.ErrorCode,
            errorCategory: category,
            innerException: ex);
    }

    private static TransportErrorCategory? ToTransportErrorCategory(ExchangeErrorCategory? category)
    {
        return category switch
        {
            null => null,
            ExchangeErrorCategory.Request => TransportErrorCategory.Request,
            ExchangeErrorCategory.Auth => TransportErrorCategory.Auth,
            ExchangeErrorCategory.Balance => TransportErrorCategory.Balance,
            ExchangeErrorCategory.RateLimit => TransportErrorCategory.RateLimit,
            ExchangeErrorCategory.Network => TransportErrorCategory.Network,
            ExchangeErrorCategory.Server => TransportErrorCategory.Server,
            _ => TransportErrorCategory.Unknown,
        };
    }

    private static ExchangeErrorCategory? ToExchangeErrorCategory(TransportErrorCategory? category)
    {
        return category switch
        {
            null => null,
            TransportErrorCategory.Request => ExchangeErrorCategory.Request,
            TransportErrorCategory.Auth => ExchangeErrorCategory.Auth,
            TransportErrorCategory.Balance => ExchangeErrorCategory.Balance,
            TransportErrorCategory.RateLimit => ExchangeErrorCategory.RateLimit,
            TransportErrorCategory.Network => ExchangeErrorCategory.Network,
            TransportErrorCategory.Server => ExchangeErrorCategory.Server,
            _ => ExchangeErrorCategory.Unknown,
        };
    }
}
