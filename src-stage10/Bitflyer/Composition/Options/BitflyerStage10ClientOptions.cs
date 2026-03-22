using ExchangeApi.Transport.Http;

namespace ExchangeApi.Stage10.Bitflyer.Composition.Options;

public sealed class BitflyerStage10ClientOptions
{
    public Uri? BaseUri { get; init; }

    public TransportConfig TransportConfig { get; init; } = new TransportConfig.ManagedHttp();

    public BitflyerApiCredentials? Credentials { get; init; }

    public bool UseTickerAliasPath { get; init; }
}
