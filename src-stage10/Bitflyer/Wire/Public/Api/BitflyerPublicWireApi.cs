using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Wire.Public.Api;

public sealed partial class BitflyerPublicWireApi : IBitflyerPublicWireApi
{
    private readonly IWireTransport _transport;

    public BitflyerPublicWireApi(IWireTransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }
}
