using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record Page<TItem>(
    IReadOnlyList<TItem> Items,
    bool HasMore,
    Cursor? NextCursor,
    PageMeta Meta);
