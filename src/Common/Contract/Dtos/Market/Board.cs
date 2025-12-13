using System.Collections.Generic;

namespace Common.Contract.Dtos;

/// <summary>
/// 板情報。
/// </summary>
public sealed record Board(
    decimal MidPrice,
    IReadOnlyList<BoardEntry> Bids,
    IReadOnlyList<BoardEntry> Asks);
