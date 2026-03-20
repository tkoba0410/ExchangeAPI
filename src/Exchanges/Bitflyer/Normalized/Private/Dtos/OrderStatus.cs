using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record OrderStatus(
    ProductCode ProductCode,
    ExchangeOrderId? ExchangeOrderId,
    AcceptanceId? AcceptanceId,
    OrderState Status,
    Size ExecutedSize,
    Size OutstandingSize,
    Price? Price,
    Price? AveragePrice);
