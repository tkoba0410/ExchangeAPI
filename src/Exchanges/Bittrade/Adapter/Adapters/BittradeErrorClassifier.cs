using System.Net;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;

/// <summary>
/// Bittrade エラーをカテゴリに正規化する。
/// </summary>
public sealed class BittradeErrorClassifier : IExchangeErrorClassifier
{
    public ExchangeErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode)
    {
        if (statusCode is null) return ExchangeErrorCategory.Unknown;

        // Bittrade error_code (Huobi系) 例: 2000 invalid.ip, 2001 invalid.json, 2002 auth.fail, 4000 too.many.request
        if (exchangeErrorCode is not null)
        {
            return exchangeErrorCode switch
            {
                "invalid.ip" => ExchangeErrorCategory.Auth,
                "invalid.json" => ExchangeErrorCategory.Request,
                "invalid.authType" => ExchangeErrorCategory.Auth,
                "auth.fail" => ExchangeErrorCategory.Auth,
                "too.many.request" => ExchangeErrorCategory.RateLimit,
                "too.many.connection" => ExchangeErrorCategory.RateLimit,
                _ => null
            };
        }

        var status = statusCode.Value;
        if (status == HttpStatusCode.Unauthorized || status == HttpStatusCode.Forbidden)
            return ExchangeErrorCategory.Auth;
        if (status == (HttpStatusCode)429)
            return ExchangeErrorCategory.RateLimit;
        if ((int)status >= 500)
            return ExchangeErrorCategory.Server;
        if ((int)status >= 400)
            return ExchangeErrorCategory.Request;

        return ExchangeErrorCategory.Unknown;
    }
}
