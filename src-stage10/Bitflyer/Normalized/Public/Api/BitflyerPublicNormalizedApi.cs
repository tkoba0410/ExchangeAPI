using ExchangeApi.Stage10.Bitflyer.Wire.Public.Api;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Public.Api;

public sealed partial class BitflyerPublicNormalizedApi : IBitflyerPublicNormalizedApi
{
    private readonly IBitflyerPublicWireApi _wire;

    public BitflyerPublicNormalizedApi(IBitflyerPublicWireApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }
}
