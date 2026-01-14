using System.Linq;
using PrivateModels = ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;

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

    public static RawSendParentOrderRequest MapSendParentOrderRequest(PrivateModels.CreateParentOrderRequest request)
    {
        var parameters = request.Parameters.Select(p => new RawSendParentOrderParameter
        {
            ProductCode = p.ProductCode,
            ConditionType = p.ConditionType,
            Side = p.Side,
            Price = p.Price,
            Size = p.Size,
            TriggerPrice = p.TriggerPrice,
            Offset = p.Offset,
        }).ToArray();

        return new RawSendParentOrderRequest
        {
            OrderMethod = request.OrderMethod,
            MinuteToExpire = request.MinuteToExpire,
            TimeInForce = request.TimeInForce,
            Parameters = parameters,
        };
    }

}
