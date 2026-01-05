using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record GetOpenPositionsRequest(Symbol Symbol);
