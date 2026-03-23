using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

public sealed class BitflyerProtocolBundle
{
    public required IBitflyerPublicProtocolApi Public { get; init; }
    public IBitflyerPrivateProtocolApi? Private { get; init; }
}
