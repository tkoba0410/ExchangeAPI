using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record PlaceLimitOrderRequest(
    Symbol Symbol,
    Side Side,
    Size Size,
    Price Price);
