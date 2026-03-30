using ExchangeApi.Exchanges.Bitflyer.Composition.Credentials;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

internal static class BitflyerCredentialResolver
{
    private const string CredentialsAgeFileEnvName = BitflyerAgeCredentialResolver.CredentialsAgeFileEnvName;
    private const string AgeIdentityFileEnvName = BitflyerAgeCredentialResolver.AgeIdentityFileEnvName;

    public static bool HasConfiguredCredentialsSource()
    {
        return BitflyerAgeCredentialResolver.HasConfiguredCredentialsSource(
            Environment.GetEnvironmentVariable,
            File.Exists,
            AgeProcessCredentialHelper.IsAvailable);
    }

    public static BitflyerApiCredentials? Load()
    {
        if (!TryResolveAgeExecutable(out var ageExecutablePath))
        {
            throw new InvalidOperationException(
                $"The 'age' executable was not found on PATH. Set {CredentialsAgeFileEnvName}/{AgeIdentityFileEnvName}, or install age.");
        }

        var resolution = BitflyerAgeCredentialResolver.Resolve(
            Environment.GetEnvironmentVariable,
            File.Exists,
            () => true,
            (identityFilePath, credentialsFilePath) => AgeProcessCredentialHelper.Decrypt(ageExecutablePath, identityFilePath, credentialsFilePath));

        if (resolution.Credentials is not null)
        {
            return resolution.Credentials;
        }

        if (resolution.HasFailure)
        {
            throw new InvalidOperationException(resolution.ErrorMessage);
        }

        return null;
    }

    private static bool TryResolveAgeExecutable(out string executablePath)
    {
        return AgeProcessCredentialHelper.TryResolveExecutablePath(out executablePath);
    }
}
