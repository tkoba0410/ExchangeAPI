using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;

public interface ICancelChildOrderProtocolEndpoint
{
    Task<Call<WireCallSpec, WireResponse>> SendAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);
}

public sealed class CancelChildOrderProtocolEndpoint : ICancelChildOrderProtocolEndpoint
{
    private readonly IWireTransport _transport;

    public CancelChildOrderProtocolEndpoint(IWireTransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }

    public Task<Call<WireCallSpec, WireResponse>> SendAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(bodyJson);

        return _transport.SendAsync(CancelChildOrderProtocolSpec.Build(bodyJson), cancellationToken);
    }
}

internal static class CancelChildOrderProtocolSpec
{
    private const string Path = ProtocolPaths.CancelChildOrder;

    public static WireCallSpec Build(string bodyJson) =>
        ProtocolCallSpecBuilder.Post(
            EndpointIds.CancelChildOrder,
            Path,
            bodyJson);
}
