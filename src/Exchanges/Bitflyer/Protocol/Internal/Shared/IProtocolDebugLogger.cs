namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;

public interface IProtocolDebugLogger
{
    Task LogAsync(ProtocolDebugLogEntry entry, CancellationToken cancellationToken = default);
}
