using System.Net;
using ExchangeApi.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Mappers;

internal sealed class ErrorClassifier : IExchangeErrorClassifier
{
    public static readonly ErrorClassifier Instance = new();

    private ErrorClassifier() { }

    public TransportErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode)
    {
        return ErrorMapper.MapTransportErrorCategory(statusCode, exchangeErrorCode);
    }
}
