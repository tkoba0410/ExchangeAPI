using System.Collections.Generic;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record GetExecutionsPublicResponse(IReadOnlyList<ExecutionMarket> Value);