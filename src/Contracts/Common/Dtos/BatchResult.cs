using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record BatchResult<TItem>
{
    public BatchResult(
        IReadOnlyList<TItem> successes,
        IReadOnlyList<BatchError> errors)
    {
        Successes = successes ?? throw new ArgumentNullException(nameof(successes));
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }

    public IReadOnlyList<TItem> Successes { get; }
    public IReadOnlyList<BatchError> Errors { get; }

    public bool HasSuccesses => Successes.Count > 0;
    public bool HasErrors => Errors.Count > 0;
    public bool IsSuccessOnly => HasSuccesses && !HasErrors;
    public bool IsFailureOnly => !HasSuccesses && HasErrors;
    public bool IsPartialSuccess => HasSuccesses && HasErrors;

    public static BatchResult<TItem> Success(params TItem[] items) =>
        new(items ?? Array.Empty<TItem>(), Array.Empty<BatchError>());

    public static BatchResult<TItem> Failure(params BatchError[] errors) =>
        new(Array.Empty<TItem>(), errors ?? Array.Empty<BatchError>());

    public static BatchResult<TItem> From(
        IEnumerable<TItem> successes,
        IEnumerable<BatchError> errors) =>
        new(
            successes?.ToArray() ?? throw new ArgumentNullException(nameof(successes)),
            errors?.ToArray() ?? throw new ArgumentNullException(nameof(errors)));
}
