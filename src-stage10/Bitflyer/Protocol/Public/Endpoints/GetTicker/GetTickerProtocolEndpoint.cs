using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Public.Endpoints.GetTicker;

public interface IGetTickerProtocolEndpoint
{
    Task<Call<WireCallSpec, WireResponse>> SendAsync(
        string? productCode = null,
        CancellationToken cancellationToken = default);
}

public sealed class GetTickerProtocolEndpoint : IGetTickerProtocolEndpoint
{
    private readonly IWireTransport _transport;

    public GetTickerProtocolEndpoint(IWireTransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }

    public Task<Call<WireCallSpec, WireResponse>> SendAsync(
        string? productCode = null,
        CancellationToken cancellationToken = default) =>
        _transport.SendAsync(GetTickerProtocolSpec.Build(productCode), cancellationToken);
}

internal static class GetTickerProtocolSpec
{
    private const string Path = ProtocolPaths.GetTicker;
    private const string QueryProductCode = ProtocolQueryKeys.ProductCode;

    public static WireCallSpec Build(string? productCode) =>
        ProtocolCallSpecBuilder.Get(
            EndpointIds.GetTicker,
            Path,
            ProtocolCallSpecBuilder.BuildQuery((QueryProductCode, productCode)));
}
