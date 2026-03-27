using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Adapters.Cli.Configuration;

public static class BitflyerOptionsFactory
{
    public static (BitflyerClientOptions? Options, ExecutionOutcome? Failure) Create(
        InvocationOptions invocationOptions,
        IEnvironment environment,
        bool requiresCredentials)
    {
        var common = CommonOptionsReader.Read(invocationOptions);
        if (common.Failure is not null)
        {
            return (null, common.Failure);
        }

        var apiKey = environment.GetEnvironmentVariable("BITFLYER_API_KEY");
        var apiSecret = environment.GetEnvironmentVariable("BITFLYER_API_SECRET");
        BitflyerApiCredentials? credentials = null;

        if (!string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(apiSecret))
        {
            credentials = new BitflyerApiCredentials
            {
                ApiKey = apiKey,
                ApiSecret = apiSecret,
            };
        }

        if (requiresCredentials && credentials is null)
        {
            return (null, ExecutionOutcome.InputError(
                "missing credential",
                "BITFLYER_API_KEY and BITFLYER_API_SECRET must be set"));
        }

        return (new BitflyerClientOptions
        {
            BaseUri = common.BaseUri ?? new Uri("https://api.bitflyer.com"),
            RequestTimeout = common.Timeout,
            Credentials = credentials,
            UseTickerAliasPath = invocationOptions.HasFlag("use-ticker-alias-path"),
            EnableProtocolDebugLogging = invocationOptions.HasFlag("enable-protocol-debug-log"),
            ProtocolDebugLogDirectory = common.ProtocolDebugLogDirectory ?? Path.Combine("local", "logs", "bitflyer", "protocol"),
        }, null);
    }
}
