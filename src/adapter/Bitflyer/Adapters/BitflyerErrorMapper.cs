using ExchangeApi.Contracts.Errors;

namespace ExchangeApi.Adapter.Bitflyer.Adapters;

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
