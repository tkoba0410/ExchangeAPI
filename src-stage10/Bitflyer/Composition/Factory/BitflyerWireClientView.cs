using ExchangeApi.Stage10.Bitflyer.Wire.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Wire.Public.Api;

namespace ExchangeApi.Stage10.Bitflyer.Composition.Factory;

public sealed class BitflyerWireClientView
{
    internal BitflyerWireClientView(
        IBitflyerPublicWireApi publicApi,
        IBitflyerPrivateWireApi? privateApi)
    {
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        Private = privateApi;
    }

    public IBitflyerPublicWireApi Public { get; }

    public IBitflyerPrivateWireApi? Private { get; }
}
