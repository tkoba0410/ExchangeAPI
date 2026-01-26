using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Trading;

public sealed record BitflyerOrderStatus(
    string ProductCode,
    OrderKey Key,
    OrderState Status,
    Size ExecutedSize,
    Size OutstandingSize,
    Price? Price,
    Price? AveragePrice);
