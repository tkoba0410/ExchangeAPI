using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Mappers;

internal static class BittradeErrorMapper
{
    private static ExchangeErrorCategory? MapErrorCategory(System.Net.HttpStatusCode? statusCode, string? exchangeCode)
    {
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

    public static ExchangeApiException EnrichBittradeException(ExchangeApiException ex, string operation)
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
