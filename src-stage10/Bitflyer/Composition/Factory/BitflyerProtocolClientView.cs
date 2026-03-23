using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Protocol.Public.Api;

namespace ExchangeApi.Stage10.Bitflyer.Composition.Factory;

public sealed class BitflyerProtocolClientView
{
    internal BitflyerProtocolClientView(
        IBitflyerPublicProtocolApi publicApi,
        IBitflyerPrivateProtocolApi? privateApi)
    {
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        Private = privateApi;
    }

    public IBitflyerPublicProtocolApi Public { get; }

    public IBitflyerPrivateProtocolApi? Private { get; }
}
