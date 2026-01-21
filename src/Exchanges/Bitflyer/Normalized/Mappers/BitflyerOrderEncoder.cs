using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using RawPrivateModels = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

internal static class BitflyerOrderEncoder
{
    public static string BuildChildOrderBodyJson(RawPrivateModels.CreateChildOrderRequest request)
    {
        var shape = BitflyerRawMappers.MapSendChildOrderRequest(request);
        return BitflyerRawJson.SerializeOrThrow(shape, "Bitflyer.CreateChildOrder");
    }

    public static string BuildParentOrderBodyJson(RawPrivateModels.CreateParentOrderRequest request)
    {
        var shape = BitflyerRawMappers.MapSendParentOrderRequest(request);
        return BitflyerRawJson.SerializeOrThrow(shape, "Bitflyer.CreateParentOrder");
    }
}
