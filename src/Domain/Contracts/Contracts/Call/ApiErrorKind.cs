namespace ExchangeApi.Contracts.Call;

public enum ApiErrorKind
{
    HttpError,
    RateLimit,
    Auth,
    NotFound,
    Validation,
    Timeout,
    Canceled,
    Unknown,
}
