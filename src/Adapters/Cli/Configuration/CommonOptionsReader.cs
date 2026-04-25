using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Configuration;

internal static class CommonOptionsReader
{
    internal static CommonOptionsReadResult Read(InvocationOptions options)
    {
        Uri? baseUri = null;
        var baseUriText = options.GetValue("base-uri");
        if (baseUriText is not null)
        {
            if (!Uri.TryCreate(baseUriText, UriKind.Absolute, out baseUri))
            {
                return CommonOptionsReadResult.Failed("invalid option", "invalid --base-uri");
            }
        }

        TimeSpan? timeout = null;
        var timeoutText = options.GetValue("timeout-ms");
        if (timeoutText is not null)
        {
            if (!int.TryParse(timeoutText, out var timeoutMs) || timeoutMs <= 0)
            {
                return CommonOptionsReadResult.Failed("invalid option", "invalid --timeout-ms");
            }

            timeout = TimeSpan.FromMilliseconds(timeoutMs);
        }

        return new CommonOptionsReadResult
        {
            BaseUri = baseUri,
            Timeout = timeout,
            ProtocolDebugLogDirectory = options.GetValue("protocol-debug-log-dir"),
            CredentialProfilePath = options.GetValue("credential-profile"),
        };
    }

    internal sealed class CommonOptionsReadResult
    {
        public Uri? BaseUri { get; init; }
        public TimeSpan? Timeout { get; init; }
        public string? ProtocolDebugLogDirectory { get; init; }
        public string? CredentialProfilePath { get; init; }
        public ExecutionOutcome? Failure { get; init; }

        public static CommonOptionsReadResult Failed(string summary, string detail)
        {
            return new CommonOptionsReadResult
            {
                Failure = ExecutionOutcome.InputError(summary, detail),
            };
        }
    }
}
