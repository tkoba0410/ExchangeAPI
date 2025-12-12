using System.Net;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Adapter.Bitflyer.Adapters;

internal sealed class BitflyerErrorClassifier : IExchangeErrorClassifier
{
    public static readonly BitflyerErrorClassifier Instance = new();

    private BitflyerErrorClassifier() { }

    public Contracts.Errors.ExchangeErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode)
    {
        return BitflyerErrorMapper.MapErrorCategory(statusCode, exchangeErrorCode);
    }
}
