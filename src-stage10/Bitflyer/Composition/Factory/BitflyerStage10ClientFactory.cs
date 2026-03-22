using ExchangeApi.Stage10.Bitflyer.Composition.Bootstrap;
using ExchangeApi.Stage10.Bitflyer.Composition.Options;

namespace ExchangeApi.Stage10.Bitflyer.Composition.Factory;

public static class BitflyerStage10ClientFactory
{
    public static BitflyerWireClientBundle CreateWireClient(BitflyerStage10ClientOptions? options = null) =>
        BitflyerRuntimeBootstrap.CreateWireBundle(options ?? new BitflyerStage10ClientOptions());

    public static BitflyerNormalizedClientBundle CreateNormalizedClient(BitflyerStage10ClientOptions? options = null) =>
        BitflyerRuntimeBootstrap.CreateNormalizedBundle(options ?? new BitflyerStage10ClientOptions());
}
