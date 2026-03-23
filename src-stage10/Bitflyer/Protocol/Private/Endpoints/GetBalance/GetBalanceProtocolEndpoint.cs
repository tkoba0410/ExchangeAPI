using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.GetBalance;

public interface IGetBalanceProtocolEndpoint
{
    Task<Call<WireCallSpec, WireResponse>> SendAsync(
        CancellationToken cancellationToken = default);
}

public sealed class GetBalanceProtocolEndpoint : IGetBalanceProtocolEndpoint
{
    private readonly IWireTransport _transport;

    public GetBalanceProtocolEndpoint(IWireTransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }

    public Task<Call<WireCallSpec, WireResponse>> SendAsync(
        CancellationToken cancellationToken = default) =>
        _transport.SendAsync(GetBalanceProtocolSpec.Build(), cancellationToken);
}

internal static class GetBalanceProtocolSpec
{
    private const string Path = ProtocolPaths.GetBalance;

    public static WireCallSpec Build() =>
        ProtocolCallSpecBuilder.Get(
            EndpointIds.GetBalance,
            Path,
            query: null);
}
