namespace ExchangeApi.Optional.Logging.Redaction;

public static class RedactionRules
{
    public static IReadOnlySet<string> DefaultSensitivePropertyNames { get; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "apiKey",
            "api_key",
            "apiSecret",
            "api_secret",
            "secret",
            "signature",
            "Authorization",
            "ACCESS-KEY",
            "ACCESS-SIGN",
            "ACCESS-TIMESTAMP",
            "X-Bitflyer-Access-Key",
            "X-Bitflyer-Access-Sign",
            "X-Bitflyer-Access-Timestamp",
        };
}
