using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

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
