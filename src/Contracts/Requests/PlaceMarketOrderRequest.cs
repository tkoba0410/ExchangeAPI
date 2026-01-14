using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record PlaceMarketOrderRequest(
    Symbol Symbol,
    Side Side,
    Size Size);
