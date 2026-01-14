using System;

namespace ExchangeApi.Contracts.Common.CallCommon;

public sealed record CallError(
    CallErrorKind Kind,
    string Message,
    Exception? Exception = null,
    int? HttpStatus = null,
    string? BodySnippet = null);
