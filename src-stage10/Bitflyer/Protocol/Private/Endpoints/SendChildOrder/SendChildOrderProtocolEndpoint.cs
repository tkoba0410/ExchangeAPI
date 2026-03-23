using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;

public interface ISendChildOrderProtocolEndpoint
{
    Task<Call<WireCallSpec, WireResponse>> SendAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);
}

public sealed class SendChildOrderProtocolEndpoint : ISendChildOrderProtocolEndpoint
{
    private readonly IWireTransport _transport;

    public SendChildOrderProtocolEndpoint(IWireTransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }

    public Task<Call<WireCallSpec, WireResponse>> SendAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(bodyJson);

        return _transport.SendAsync(SendChildOrderProtocolSpec.Build(bodyJson), cancellationToken);
    }
}

internal static class SendChildOrderProtocolSpec
{
    private const string Path = ProtocolPaths.SendChildOrder;

    public static WireCallSpec Build(string bodyJson) =>
        ProtocolCallSpecBuilder.Post(
            EndpointIds.SendChildOrder,
            Path,
            bodyJson);
}
