using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Optional.Credentials.PlainText;

public static class PlainTextApiCredentialProviderFactory
{
    public static IApiCredentialProvider Create(ExchangeVenue venue, string apiKey, string apiSecret)
    {
        return venue switch
        {
            ExchangeVenue.Bitflyer => new BitflyerPlainTextApiCredentialProvider(apiKey, apiSecret),
            ExchangeVenue.Binance => new BinancePlainTextApiCredentialProvider(apiKey, apiSecret),
            _ => throw new ArgumentOutOfRangeException(nameof(venue), venue, "Unsupported venue."),
        };
    }
}
