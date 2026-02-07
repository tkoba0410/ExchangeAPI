using System.Net;
using ExchangeApi.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Mappers;

internal sealed class BitflyerErrorClassifier : IExchangeErrorClassifier
{
    public static readonly BitflyerErrorClassifier Instance = new();

    private BitflyerErrorClassifier() { }

    public TransportErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode)
    {
        return BitflyerErrorMapper.MapTransportErrorCategory(statusCode, exchangeErrorCode);
    }
}
