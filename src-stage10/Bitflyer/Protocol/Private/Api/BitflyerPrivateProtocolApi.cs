using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api;

public sealed partial class BitflyerPrivateProtocolApi : IBitflyerPrivateProtocolApi
{
    private readonly IWireTransport _transport;

    public BitflyerPrivateProtocolApi(IWireTransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }
}
