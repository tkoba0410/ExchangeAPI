using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerOrderEncoder
{
    public static string BuildChildOrderBodyJson(CreateChildOrderRequest request)
    {
        var shape = BitflyerRawMappers.MapSendChildOrderRequest(request);
        return BitflyerRawJson.SerializeOrThrow(shape, "Bitflyer.CreateChildOrder");
    }

    public static string BuildParentOrderBodyJson(CreateParentOrderRequest request)
    {
        var shape = BitflyerRawMappers.MapSendParentOrderRequest(request);
        return BitflyerRawJson.SerializeOrThrow(shape, "Bitflyer.CreateParentOrder");
    }
}
