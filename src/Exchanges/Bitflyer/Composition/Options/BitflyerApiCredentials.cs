namespace ExchangeApi.Exchanges.Bitflyer.Composition.Options;

public sealed class BitflyerApiCredentials
{
    public required string ApiKey { get; init; }
    public required string ApiSecret { get; init; }
}
