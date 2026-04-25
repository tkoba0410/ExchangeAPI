using ExchangeApi.Optional.Credentials.AgeFile;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Adapters.McpServer.Configuration;

public static class BitflyerCredentialResolver
{
    public const int CanonicalVersion = 1;
    public const string CanonicalVenue = "bitflyer";
    public const string AgeIdentityFileEnvName = "EXCHANGEAPI_AGE_IDENTITY_FILE_PATH";
    public const string CredentialsAgeFileEnvName = "EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH";

    public static bool HasConfiguredCredentialsSource()
    {
        var identityFilePath = Environment.GetEnvironmentVariable(AgeIdentityFileEnvName);
        var credentialsFilePath = Environment.GetEnvironmentVariable(CredentialsAgeFileEnvName);
        return !string.IsNullOrWhiteSpace(identityFilePath)
            && !string.IsNullOrWhiteSpace(credentialsFilePath)
            && File.Exists(identityFilePath)
            && File.Exists(credentialsFilePath);
    }

    public static BitflyerCredentialResolution Resolve()
    {
        var identityFilePath = Environment.GetEnvironmentVariable(AgeIdentityFileEnvName);
        var credentialsFilePath = Environment.GetEnvironmentVariable(CredentialsAgeFileEnvName);
        var hasIdentity = !string.IsNullOrWhiteSpace(identityFilePath);
        var hasCredentialsFile = !string.IsNullOrWhiteSpace(credentialsFilePath);

        if (!hasIdentity && !hasCredentialsFile)
        {
            return BitflyerCredentialResolution.None();
        }

        if (!hasIdentity || !hasCredentialsFile)
        {
            return BitflyerCredentialResolution.Failure(
                $"{AgeIdentityFileEnvName} and {CredentialsAgeFileEnvName} must both be set to use age-backed credentials.");
        }

        if (!File.Exists(identityFilePath!))
        {
            return BitflyerCredentialResolution.Failure(
                $"{AgeIdentityFileEnvName} does not exist: {identityFilePath}");
        }

        if (!File.Exists(credentialsFilePath!))
        {
            return BitflyerCredentialResolution.Failure(
                $"{CredentialsAgeFileEnvName} does not exist: {credentialsFilePath}");
        }

        return BitflyerCredentialResolution.Success(
            new BitflyerAgeFileApiCredentialProvider(
                identityFilePath!,
                credentialsFilePath!,
                new AgeCliCredentialFileDecryptor()));
    }
}

public sealed class BitflyerCredentialResolution
{
    private BitflyerCredentialResolution(IApiCredentialProvider? credentials, string? errorMessage)
    {
        Credentials = credentials;
        ErrorMessage = errorMessage;
    }

    public IApiCredentialProvider? Credentials { get; }

    public string? ErrorMessage { get; }

    public bool HasFailure => !string.IsNullOrWhiteSpace(ErrorMessage);

    public static BitflyerCredentialResolution Success(IApiCredentialProvider credentials)
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
