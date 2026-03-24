namespace ExchangeApi.Exchanges.Binance.Protocol.Internal.Shared;

public sealed class NoOpProtocolDebugLogger : IProtocolDebugLogger
{
    public Task LogAsync(ProtocolDebugLogEntry entry, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
