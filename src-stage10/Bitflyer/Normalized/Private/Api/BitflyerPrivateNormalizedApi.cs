using ExchangeApi.Stage10.Bitflyer.Wire.Private.Api;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Private.Api;

public sealed partial class BitflyerPrivateNormalizedApi : IBitflyerPrivateNormalizedApi
{
    private readonly IBitflyerPrivateWireApi _wire;

    public BitflyerPrivateNormalizedApi(IBitflyerPrivateWireApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }
}
