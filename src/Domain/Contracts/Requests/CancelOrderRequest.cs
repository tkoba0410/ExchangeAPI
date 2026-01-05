using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record CancelOrderRequest(
    Symbol Symbol,
    OrderKey OrderKey);
