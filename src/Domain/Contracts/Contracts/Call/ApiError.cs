namespace ExchangeApi.Contracts.Call;

public sealed record ApiError(
    ApiErrorKind Kind,
    string Message,
    int StatusCode,
    string? RequestId = null);
