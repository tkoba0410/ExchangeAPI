using System.Linq;
using PrivateRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal;

internal static class RawMappers
{
    public static RawSendChildOrderRequest MapSendChildOrderRequest(PrivateRequests.SendChildOrderRequest request) => new()
    {
        ProductCode = request.ProductCode.Value,
        ChildOrderType = request.ChildOrderType.Value,
        Side = request.Side.Value,
        Size = request.Size,
        Price = request.Price,
        MinuteToExpire = request.MinuteToExpire,
        TimeInForce = request.TimeInForce?.Value,
        TriggerPrice = request.TriggerPrice,
    };

    public static RawSendParentOrderRequest MapSendParentOrderRequest(PrivateRequests.SendParentOrderRequest request)
    {
        var parameters = request.Parameters.Select(p => new RawSendParentOrderParameter
        {
            ProductCode = p.ProductCode.Value,
            ConditionType = p.ConditionType.Value,
            Side = p.Side.Value,
            Price = p.Price,
            Size = p.Size,
            TriggerPrice = p.TriggerPrice,
            Offset = p.Offset,
        }).ToArray();

        return new RawSendParentOrderRequest
        {
            OrderMethod = request.OrderMethod?.Value,
            MinuteToExpire = request.MinuteToExpire,
            TimeInForce = request.TimeInForce?.Value,
            Parameters = parameters,
        };
    }

}
