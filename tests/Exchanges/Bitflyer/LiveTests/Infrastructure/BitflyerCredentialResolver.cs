using ExchangeApi.Optional.Credentials.AgeFile;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

internal static class BitflyerCredentialResolver
{
    private const string CredentialsAgeFileEnvName = "EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH";
    private const string AgeIdentityFileEnvName = "EXCHANGEAPI_AGE_IDENTITY_FILE_PATH";

    public static bool HasConfiguredCredentialsSource()
    {
        var identityFilePath = Environment.GetEnvironmentVariable(AgeIdentityFileEnvName);
        var credentialsFilePath = Environment.GetEnvironmentVariable(CredentialsAgeFileEnvName);
        return !string.IsNullOrWhiteSpace(identityFilePath)
            && !string.IsNullOrWhiteSpace(credentialsFilePath)
            && File.Exists(identityFilePath)
            && File.Exists(credentialsFilePath)
            && TryResolveAgeExecutable(out _);
    }

    public static IApiCredentialProvider? Load()
    {
        if (!TryResolveAgeExecutable(out var ageExecutablePath))
        {
            throw new InvalidOperationException(
                $"The 'age' executable was not found on PATH. Set {CredentialsAgeFileEnvName}/{AgeIdentityFileEnvName}, or install age.");
        }

        var identityFilePath = Environment.GetEnvironmentVariable(AgeIdentityFileEnvName);
        var credentialsFilePath = Environment.GetEnvironmentVariable(CredentialsAgeFileEnvName);
        var hasIdentity = !string.IsNullOrWhiteSpace(identityFilePath);
        var hasCredentialsFile = !string.IsNullOrWhiteSpace(credentialsFilePath);

        if (!hasIdentity && !hasCredentialsFile)
        {
            return null;
        }

        if (!hasIdentity || !hasCredentialsFile)
        {
            throw new InvalidOperationException(
                $"{CredentialsAgeFileEnvName} and {AgeIdentityFileEnvName} must both be set.");
        }

        return new BitflyerAgeFileApiCredentialProvider(
            identityFilePath!,
            credentialsFilePath!,
            new AgeCliCredentialFileDecryptor(ageExecutablePath));
    }

    private static bool TryResolveAgeExecutable(out string executablePath)
    {
        executablePath = "age";
        var pathValue = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(pathValue))
        {
            return false;
        }

        foreach (var directory in pathValue.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var candidate = Path.Combine(directory, "age");
            if (File.Exists(candidate))
            {
                executablePath = candidate;
                return true;
            }
        }

        return false;
    }
}
