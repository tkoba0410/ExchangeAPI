using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record PlaceMarketOrderRequest(
    Symbol Symbol,
    Side Side,
    Size Size);
