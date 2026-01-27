using System.Linq;
using PrivateRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal;

internal static class BitflyerRawMappers
{
    public static RawSendChildOrderRequest MapSendChildOrderRequest(PrivateRequests.CreateChildOrderRequest request) => new()
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

    public static RawSendParentOrderRequest MapSendParentOrderRequest(PrivateRequests.CreateParentOrderRequest request)
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
