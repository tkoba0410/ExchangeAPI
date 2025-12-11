using System.Net;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Adapter.Bittrade.Adapters;

/// <summary>
/// Bittrade エラーをカテゴリに正規化する。
/// </summary>
public sealed class BittradeErrorClassifier : IExchangeErrorClassifier
{
    public ExchangeErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode)
    {
        if (statusCode is null) return ExchangeErrorCategory.Unknown;

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
