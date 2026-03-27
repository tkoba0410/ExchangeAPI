namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class RequestBindingResult
{
    public required bool IsSuccess { get; init; }
    public object? Request { get; init; }
    public string? ErrorSummary { get; init; }
    public string? ErrorDetail { get; init; }

    public static RequestBindingResult Success(object request)
    {
        return new RequestBindingResult
        {
            IsSuccess = true,
            Request = request,
        };
    }

    public static RequestBindingResult Failure(string summary, string? detail = null)
    {
        return new RequestBindingResult
        {
            IsSuccess = false,
            ErrorSummary = summary,
            ErrorDetail = detail,
        };
    }
}
