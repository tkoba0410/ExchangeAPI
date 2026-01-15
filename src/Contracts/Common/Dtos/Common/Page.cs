using System.Collections.Generic;

namespace ExchangeApi.Contracts.Common.Dtos.Common;

public sealed record Page<TItem>(
    IReadOnlyList<TItem> Items,
    bool HasMore,
    string? NextCursor,
    PageMeta Meta);
