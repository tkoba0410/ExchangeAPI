using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Common.Enums;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;

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

    public static ExchangeApiException EnrichBittradeException(ExchangeApiException ex, ExchangeCode exchange, string operation)
    {
        if (ex.Exchange == exchange && ex.Operation == operation)
        {
            return ex;
        }

        var category = MapErrorCategory(ex.StatusCode, ex.ExchangeErrorCode);

        return new ExchangeApiException(
            message: ex.Message,
            exchange: exchange,
            operation: operation,
            statusCode: ex.StatusCode,
            exchangeErrorCode: ex.ExchangeErrorCode,
            errorCategory: category,
            innerException: ex);
    }
}
