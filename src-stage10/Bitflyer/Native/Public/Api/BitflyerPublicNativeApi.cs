using ExchangeApi.Stage10.Bitflyer.Protocol.Public.Api;

namespace ExchangeApi.Stage10.Bitflyer.Native.Public.Api;

public sealed partial class BitflyerPublicNativeApi : IBitflyerPublicNativeApi
{
    private readonly IBitflyerPublicProtocolApi _protocol;

    public BitflyerPublicNativeApi(IBitflyerPublicProtocolApi protocol)
    {
        _protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
    }
}
