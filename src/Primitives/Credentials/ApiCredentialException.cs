namespace ExchangeApi.Primitives.Credentials;

public sealed class ApiCredentialException : Exception
{
    public ApiCredentialException(ApiCredentialErrorKind kind, string message)
        : base(message)
    {
        Kind = kind;
    }

    public ApiCredentialException(ApiCredentialErrorKind kind, string message, Exception innerException)
        : base(message, innerException)
    {
        Kind = kind;
    }

    public ApiCredentialErrorKind Kind { get; }
}
