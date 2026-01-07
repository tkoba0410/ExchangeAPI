using System;

namespace ExchangeApi.Spec.CallCommon;

public sealed record CallError(
    CallErrorKind Kind,
    string Message,
    Exception? Exception = null,
    int? HttpStatus = null,
    string? BodySnippet = null);
