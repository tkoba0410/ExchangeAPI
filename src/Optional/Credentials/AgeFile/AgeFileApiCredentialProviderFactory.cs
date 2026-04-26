using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Optional.Credentials.AgeFile;

public static class AgeFileApiCredentialProviderFactory
{
    public static IApiCredentialProvider Create(
        ExchangeVenue venue,
        string identityFilePath,
        string credentialsFilePath,
        IAgeCredentialFileDecryptor decryptor)
    {
        return venue switch
        {
            ExchangeVenue.Bitflyer => new BitflyerAgeFileApiCredentialProvider(identityFilePath, credentialsFilePath, decryptor),
            ExchangeVenue.Binance => new BinanceAgeFileApiCredentialProvider(identityFilePath, credentialsFilePath, decryptor),
            _ => throw new ArgumentOutOfRangeException(nameof(venue), venue, "Unsupported venue."),
        };
    }
}
