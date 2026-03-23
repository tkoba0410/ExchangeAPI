using ExchangeApi.Stage10.Bitflyer.Composition.Bootstrap;
using ExchangeApi.Stage10.Bitflyer.Composition.Options;

namespace ExchangeApi.Stage10.Bitflyer.Composition.Factory;

public static class BitflyerStage10ClientFactory
{
    public static BitflyerProtocolClientBundle CreateProtocolClient(BitflyerStage10ClientOptions? options = null) =>
        BitflyerRuntimeBootstrap.CreateProtocolBundle(options ?? new BitflyerStage10ClientOptions());

    public static BitflyerNativeClientBundle CreateNativeClient(BitflyerStage10ClientOptions? options = null) =>
        BitflyerRuntimeBootstrap.CreateNativeBundle(options ?? new BitflyerStage10ClientOptions());
}
