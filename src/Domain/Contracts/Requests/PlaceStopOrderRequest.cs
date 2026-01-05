using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record PlaceStopOrderRequest(
    Symbol Symbol,
    Side Side,
    Size Size,
    Price TriggerPrice);
