using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record PlaceMarketOrderRequest(
    Symbol Symbol,
    Side Side,
    Size Size);
