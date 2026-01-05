using PrivateModels = ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

internal static class BitflyerRawMappers
{
    public static RawSendChildOrderRequest MapSendChildOrderRequest(PrivateModels.CreateChildOrderRequest request) => new()
    {
        ProductCode = request.ProductCode,
        ChildOrderType = request.ChildOrderType,
        Side = request.Side,
        Size = request.Size,
        Price = request.Price,
        MinuteToExpire = request.MinuteToExpire,
        TimeInForce = request.TimeInForce,
        TriggerPrice = request.TriggerPrice,
    };
}
