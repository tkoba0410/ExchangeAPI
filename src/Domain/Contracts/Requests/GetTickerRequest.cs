using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record GetTickerRequest(Symbol Symbol);
