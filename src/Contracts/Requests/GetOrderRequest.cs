using ExchangeApi.Contracts.Common.DomainCommon.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record GetOrderRequest(
    Symbol Symbol,
    OrderKey OrderKey);
