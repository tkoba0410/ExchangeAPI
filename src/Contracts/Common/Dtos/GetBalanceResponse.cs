using System.Collections.Generic;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record GetBalanceResponse(IReadOnlyList<Balance> Value);