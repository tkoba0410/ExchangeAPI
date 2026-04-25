namespace ExchangeApi.Primitives.Credentials;

public enum ApiCredentialErrorKind
{
    NotConfigured,
    SourceUnavailable,
    DecryptFailed,
    JsonParseFailed,
    MissingRequiredField,
    UnsupportedVersion,
    VenueMismatch,
    InvalidApiKey,
    InvalidApiSecret,
}
