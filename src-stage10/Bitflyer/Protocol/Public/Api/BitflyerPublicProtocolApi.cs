using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Public.Api;

public sealed partial class BitflyerPublicProtocolApi : IBitflyerPublicProtocolApi
{
    private readonly IWireTransport _transport;

    public BitflyerPublicProtocolApi(IWireTransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }
}
