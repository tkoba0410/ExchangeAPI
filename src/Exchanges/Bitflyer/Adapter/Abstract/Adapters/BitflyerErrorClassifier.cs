using System.Net;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;

internal sealed class BitflyerErrorClassifier : IExchangeErrorClassifier
{
    public static readonly BitflyerErrorClassifier Instance = new();

    private BitflyerErrorClassifier() { }

    public ExchangeErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode)
    {
        return BitflyerErrorMapper.MapErrorCategory(statusCode, exchangeErrorCode);
    }
}
