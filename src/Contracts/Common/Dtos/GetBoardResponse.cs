using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record GetBoardResponse(
    IReadOnlyList<GetBoardLevel> Bids,
    IReadOnlyList<GetBoardLevel> Asks);

public sealed record GetBoardLevel(Price Price, Size Size);
