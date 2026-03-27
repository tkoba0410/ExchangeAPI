using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Binance.Composition.Options;

namespace ExchangeApi.Adapters.Cli.Configuration;

public static class BinanceOptionsFactory
{
    public static (BinanceClientOptions? Options, ExecutionOutcome? Failure) Create(InvocationOptions invocationOptions)
    {
        var common = CommonOptionsReader.Read(invocationOptions);
        if (common.Failure is not null)
        {
            return (null, common.Failure);
        }

        return (new BinanceClientOptions
        {
            BaseUri = common.BaseUri ?? new Uri("https://api.binance.com"),
            RequestTimeout = common.Timeout,
            EnableProtocolDebugLogging = invocationOptions.HasFlag("enable-protocol-debug-log"),
            ProtocolDebugLogDirectory = common.ProtocolDebugLogDirectory ?? Path.Combine("local", "logs", "binance", "protocol"),
        }, null);
    }
}
