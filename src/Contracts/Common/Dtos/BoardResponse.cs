using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record BoardResponse(
    IReadOnlyList<BoardLevel> Bids,
    IReadOnlyList<BoardLevel> Asks);

public sealed record BoardLevel(Price Price, Size Size);
