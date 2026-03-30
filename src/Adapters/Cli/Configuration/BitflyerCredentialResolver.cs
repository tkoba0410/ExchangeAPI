using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Credentials;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Adapters.Cli.Configuration;

public static class BitflyerCredentialResolver
{
    public const int CanonicalVersion = BitflyerAgeCredentialResolver.CanonicalVersion;
    public const string CanonicalVenue = BitflyerAgeCredentialResolver.CanonicalVenue;
    public const string AgeIdentityFileEnvName = BitflyerAgeCredentialResolver.AgeIdentityFileEnvName;
    public const string CredentialsAgeFileEnvName = BitflyerAgeCredentialResolver.CredentialsAgeFileEnvName;
    public const string AuthenticationRequirementText =
        $"{AgeIdentityFileEnvName} / {CredentialsAgeFileEnvName}";

    public static BitflyerCredentialResolution Resolve(IEnvironment environment)
    {
        return Resolve(environment, ProcessAgeCredentialDecryptor.Instance);
    }

    public static BitflyerCredentialResolution Resolve(IEnvironment environment, IAgeCredentialDecryptor decryptor)
    {
        return FromCommonResolution(
            BitflyerAgeCredentialResolver.Resolve(
                environment.GetEnvironmentVariable,
                File.Exists,
                decryptor.IsAvailable,
                decryptor.Decrypt));
    }

    public static string BuildMissingCredentialMessage()
    {
        return $"Configure {AgeIdentityFileEnvName} and {CredentialsAgeFileEnvName}.";
    }

    private static BitflyerCredentialResolution FromCommonResolution(BitflyerAgeCredentialResolution resolution)
    {
        if (resolution.Credentials is not null)
        {
            return BitflyerCredentialResolution.Success(resolution.Credentials);
        }

        return resolution.HasFailure
            ? BitflyerCredentialResolution.Failure(resolution.ErrorMessage!)
            : BitflyerCredentialResolution.None();
    }
}

public sealed class BitflyerCredentialResolution
{
    private BitflyerCredentialResolution(BitflyerApiCredentials? credentials, string? errorMessage)
    {
        Credentials = credentials;
        ErrorMessage = errorMessage;
    }

    public BitflyerApiCredentials? Credentials { get; }

    public string? ErrorMessage { get; }

    public bool HasFailure => !string.IsNullOrWhiteSpace(ErrorMessage);

    public static BitflyerCredentialResolution Success(BitflyerApiCredentials credentials)
    {
        return new BitflyerCredentialResolution(credentials, null);
    }

    public static BitflyerCredentialResolution None()
    {
        return new BitflyerCredentialResolution(null, null);
    }

    public static BitflyerCredentialResolution Failure(string message)
    {
        return new BitflyerCredentialResolution(null, message);
    }
}
