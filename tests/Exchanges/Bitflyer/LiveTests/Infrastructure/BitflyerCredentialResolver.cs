using ExchangeApi.Optional.Credentials;
using ExchangeApi.Optional.Credentials.Profiles;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

internal static class BitflyerCredentialResolver
{
    public static readonly string DefaultCredentialProfilePath = CredentialProfileDefaults.DefaultProfilePath;

    public static bool HasConfiguredCredentialsSource()
    {
        var profilePath = CredentialProfileDefaults.ResolveDefaultProfilePath();
        if (!File.Exists(profilePath) || !TryResolveAgeExecutable(out var ageExecutablePath))
        {
            return false;
        }

        try
        {
            var profile = CredentialProfileLoader.Load(profilePath);
            _ = CredentialProfileProviderFactory.Create(
                profile,
                ExchangeVenue.Bitflyer,
                profilePath,
                new ExchangeApi.Optional.Credentials.AgeFile.AgeCliCredentialFileDecryptor(ageExecutablePath));
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException or System.Text.Json.JsonException)
        {
            return false;
        }
    }

    public static IApiCredentialProvider? Load()
    {
        if (!TryResolveAgeExecutable(out var ageExecutablePath))
        {
            throw new InvalidOperationException(
                $"The 'age' executable was not found on PATH. Configure {DefaultCredentialProfilePath}, or install age.");
        }

        var profilePath = CredentialProfileDefaults.ResolveDefaultProfilePath();
        if (!File.Exists(profilePath))
        {
            return null;
        }

        var profile = CredentialProfileLoader.Load(profilePath);
        return CredentialProfileProviderFactory.Create(
            profile,
            ExchangeVenue.Bitflyer,
            profilePath,
            new ExchangeApi.Optional.Credentials.AgeFile.AgeCliCredentialFileDecryptor(ageExecutablePath));
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
