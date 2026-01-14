using System.Net;
using ExchangeApi.Shared.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;

/// <summary>
/// Bittrade エラーをカテゴリに正規化する。
/// </summary>
public sealed class BittradeErrorClassifier : IExchangeErrorClassifier
{
    public TransportErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode)
    {
        if (statusCode is null) return TransportErrorCategory.Unknown;

        // Bittrade error_code (Huobi系) 例: 2000 invalid.ip, 2001 invalid.json, 2002 auth.fail, 4000 too.many.request
        if (exchangeErrorCode is not null)
        {
            return exchangeErrorCode switch
            {
                "invalid.ip" => TransportErrorCategory.Auth,
                "invalid.json" => TransportErrorCategory.Request,
                "invalid.authType" => TransportErrorCategory.Auth,
                "auth.fail" => TransportErrorCategory.Auth,
                "too.many.request" => TransportErrorCategory.RateLimit,
                "too.many.connection" => TransportErrorCategory.RateLimit,
                _ => null
            };
        }

        var status = statusCode.Value;
        if (status == HttpStatusCode.Unauthorized || status == HttpStatusCode.Forbidden)
            return TransportErrorCategory.Auth;
        if (status == (HttpStatusCode)429)
            return TransportErrorCategory.RateLimit;
        if ((int)status >= 500)
            return TransportErrorCategory.Server;
        if ((int)status >= 400)
            return TransportErrorCategory.Request;

        return TransportErrorCategory.Unknown;
    }
}
