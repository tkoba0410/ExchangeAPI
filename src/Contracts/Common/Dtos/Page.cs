using System.Collections.Generic;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record Page<TItem>(
    IReadOnlyList<TItem> Items,
    bool HasMore,
    string? NextCursor,
    PageMeta Meta);
