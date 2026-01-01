using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record PlaceLimitOrderRequest(
    Symbol Symbol,
    Side Side,
    Size Size,
    Price Price);
