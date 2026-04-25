using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Optional.Credentials.AgeFile;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Adapters.Cli.Configuration;

public static class BitflyerCredentialResolver
{
    public const int CanonicalVersion = 1;
    public const string CanonicalVenue = "bitflyer";
    public const string AgeIdentityFileEnvName = "EXCHANGEAPI_AGE_IDENTITY_FILE_PATH";
    public const string CredentialsAgeFileEnvName = "EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH";
    public const string AuthenticationRequirementText =
        $"{AgeIdentityFileEnvName} / {CredentialsAgeFileEnvName}";

    public static BitflyerCredentialResolution Resolve(IEnvironment environment)
    {
        return Resolve(environment, ProcessAgeCredentialDecryptor.Instance);
    }

    public static BitflyerCredentialResolution Resolve(IEnvironment environment, IAgeCredentialDecryptor decryptor)
    {
        var identityFilePath = environment.GetEnvironmentVariable(AgeIdentityFileEnvName);
        var credentialsFilePath = environment.GetEnvironmentVariable(CredentialsAgeFileEnvName);
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

        if (!decryptor.IsAvailable())
        {
            return BitflyerCredentialResolution.Failure("The 'age' executable was not found on PATH.");
        }

        return BitflyerCredentialResolution.Success(
            new BitflyerAgeFileApiCredentialProvider(
                identityFilePath!,
                credentialsFilePath!,
                new CliAgeCredentialFileDecryptor(decryptor)));
    }

    public static string BuildMissingCredentialMessage()
    {
        return $"Configure {AgeIdentityFileEnvName} and {CredentialsAgeFileEnvName}.";
    }

    private sealed class CliAgeCredentialFileDecryptor : IAgeCredentialFileDecryptor
    {
        private readonly IAgeCredentialDecryptor _decryptor;

        public CliAgeCredentialFileDecryptor(IAgeCredentialDecryptor decryptor)
        {
            _decryptor = decryptor;
        }

        public string Decrypt(string identityFilePath, string credentialsFilePath)
        {
            return _decryptor.Decrypt(identityFilePath, credentialsFilePath);
        }
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
