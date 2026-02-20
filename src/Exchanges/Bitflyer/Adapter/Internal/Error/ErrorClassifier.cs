using System.Net;
using ExchangeApi.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Error;

internal sealed class ErrorClassifier : IExchangeErrorClassifier
{
    public static readonly ErrorClassifier Instance = new();

    private ErrorClassifier() { }

    public TransportErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode)
    {
        return CallErrorTranslator.MapTransportErrorCategory(statusCode, exchangeErrorCode);
    }
}
