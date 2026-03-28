using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class ProtocolCallEnvelope
{
    public required ProtocolRequest Request { get; init; }
    public required ProtocolResponse Response { get; init; }
    public required ProtocolCallEnvelopeMeta Meta { get; init; }
}

public sealed class ProtocolCallEnvelopeMeta
{
    public required string Layer { get; init; }
    public required string Component { get; init; }
    public required string EndpointId { get; init; }
    public required string Scope { get; init; }
    public required string Auth { get; init; }
}
