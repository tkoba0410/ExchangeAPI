using System.Net;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Transport.Protocol;

namespace Exchange.Bitflyer.Abstract.Adapters;

internal sealed class BitflyerErrorClassifier : IExchangeErrorClassifier
{
    public static readonly BitflyerErrorClassifier Instance = new();

    private BitflyerErrorClassifier() { }

    public ExchangeErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode)
    {
        return BitflyerErrorMapper.MapErrorCategory(statusCode, exchangeErrorCode);
    }
}
