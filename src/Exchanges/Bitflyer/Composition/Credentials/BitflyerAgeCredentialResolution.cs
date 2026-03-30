using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Credentials;

public sealed class BitflyerAgeCredentialResolution
{
    private BitflyerAgeCredentialResolution(BitflyerApiCredentials? credentials, string? errorMessage)
    {
        Credentials = credentials;
        ErrorMessage = errorMessage;
    }

    public BitflyerApiCredentials? Credentials { get; }

    public string? ErrorMessage { get; }

    public bool HasFailure => !string.IsNullOrWhiteSpace(ErrorMessage);

    public static BitflyerAgeCredentialResolution Success(BitflyerApiCredentials credentials)
    {
        return new BitflyerAgeCredentialResolution(credentials, null);
    }

    public static BitflyerAgeCredentialResolution None()
    {
        return new BitflyerAgeCredentialResolution(null, null);
    }

    public static BitflyerAgeCredentialResolution Failure(string message)
    {
        return new BitflyerAgeCredentialResolution(null, message);
    }
}
