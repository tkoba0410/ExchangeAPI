using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record GetOrderBookRequest(Symbol Symbol);
