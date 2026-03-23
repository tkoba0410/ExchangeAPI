using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;

public interface IProtocolTransport
{
    Task<ProtocolTransportResult> SendAsync(
        ProtocolRequest request,
        ProtocolTransportAuthMode authMode,
        CancellationToken cancellationToken = default);
}
