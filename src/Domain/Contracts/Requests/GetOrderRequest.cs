using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record GetOrderRequest(
    Symbol Symbol,
    OrderKey OrderKey);
