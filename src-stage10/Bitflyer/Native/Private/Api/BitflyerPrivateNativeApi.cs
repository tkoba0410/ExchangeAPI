using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api;

namespace ExchangeApi.Stage10.Bitflyer.Native.Private.Api;

public sealed partial class BitflyerPrivateNativeApi : IBitflyerPrivateNativeApi
{
    private readonly IBitflyerPrivateProtocolApi _protocol;

    public BitflyerPrivateNativeApi(IBitflyerPrivateProtocolApi protocol)
    {
        _protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
    }
}
