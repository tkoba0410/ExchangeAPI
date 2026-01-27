using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using RawPrivateDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class BitflyerOrderEncoder
{
    public static string BuildChildOrderBodyJson(RawPrivateRequests.CreateChildOrderRequest request)
    {
        var shape = BitflyerRawMappers.MapSendChildOrderRequest(request);
        return BitflyerRawJson.SerializeOrThrow(shape, "Bitflyer.CreateChildOrder");
    }

    public static string BuildParentOrderBodyJson(RawPrivateRequests.CreateParentOrderRequest request)
    {
        var shape = BitflyerRawMappers.MapSendParentOrderRequest(request);
        return BitflyerRawJson.SerializeOrThrow(shape, "Bitflyer.CreateParentOrder");
    }
}
