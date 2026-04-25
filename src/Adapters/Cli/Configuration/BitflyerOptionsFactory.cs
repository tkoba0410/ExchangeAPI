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

        var credentialResolution = BitflyerCredentialResolver.Resolve(environment);
        if (credentialResolution.HasFailure)
        {
            return (null, ExecutionOutcome.InputError(
                "invalid credential source",
                credentialResolution.ErrorMessage!));
        }

        var credentials = credentialResolution.Credentials;

        if (requiresCredentials && credentials is null)
        {
            return (null, ExecutionOutcome.InputError(
                "missing credential",
                BitflyerCredentialResolver.BuildMissingCredentialMessage()));
        }

        return (new BitflyerClientOptions
        {
            BaseUri = common.BaseUri ?? new Uri("https://api.bitflyer.com"),
            RequestTimeout = common.Timeout,
            ApiCredentialProvider = credentials,
            UseTickerAliasPath = invocationOptions.HasFlag("use-ticker-alias-path"),
            EnableProtocolDebugLogging = invocationOptions.HasFlag("enable-protocol-debug-log"),
            ProtocolDebugLogDirectory = common.ProtocolDebugLogDirectory ?? Path.Combine("local", "logs", "bitflyer", "protocol"),
        }, null);
    }
}
