using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Wire.Private.Api;

public sealed partial class BitflyerPrivateWireApi : IBitflyerPrivateWireApi
{
    private readonly IWireTransport _transport;

    public BitflyerPrivateWireApi(IWireTransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }
}
