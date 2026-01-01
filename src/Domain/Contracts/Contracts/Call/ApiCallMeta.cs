namespace ExchangeApi.Contracts.Call;

public sealed record ApiCallMeta(
    DateTimeOffset StartedAt,
    TimeSpan Elapsed,
    string? RequestId);
