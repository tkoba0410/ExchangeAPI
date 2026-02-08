using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record GetOrdersByOrderIdResponse(
    ProductCode ProductCode,
    OrderKey Key,
    OrderState Status,
    Size ExecutedSize,
    Size OutstandingSize,
    Price? Price,
    Price? AveragePrice);
