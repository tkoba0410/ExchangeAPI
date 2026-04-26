using ExchangeApi.Optional.Credentials.AgeFile;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Optional.Credentials.Profiles;

public static class CredentialProfileProviderFactory
{
    public static IApiCredentialProvider CreateFromFile(
        string profileFilePath,
        ExchangeVenue venue,
        IAgeCredentialFileDecryptor? ageDecryptor = null)
    {
        var profile = CredentialProfileLoader.Load(profileFilePath);
        return Create(profile, venue, profileFilePath, ageDecryptor);
    }

    public static IApiCredentialProvider Create(
        CredentialProfile profile,
        ExchangeVenue venue,
        string profileFilePath,
        IAgeCredentialFileDecryptor? ageDecryptor = null)
    {
        var venueId = CredentialProfileDefaults.GetVenueId(venue);
        if (!profile.Credentials.TryGetValue(venueId, out var entry))
        {
            throw new InvalidOperationException($"Credential profile does not contain a '{venueId}' entry.");
        }

        var provider = string.IsNullOrWhiteSpace(entry.Provider)
            ? CredentialProfileProviderNames.AgeFile
            : entry.Provider;

        if (!string.Equals(provider, CredentialProfileProviderNames.AgeFile, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(provider, "AgeFile", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unsupported credential profile provider for '{venueId}': {provider}");
        }

        var baseDirectory = Path.GetDirectoryName(Path.GetFullPath(profileFilePath)) ?? Directory.GetCurrentDirectory();
        var identityFilePath = ResolvePath(
            baseDirectory,
            entry.IdentityFilePath,
            Path.Combine("current", CredentialProfileDefaults.DefaultAgeIdentityFileName));
        var credentialsFilePath = ResolvePath(
            baseDirectory,
            entry.CredentialsFilePath,
            Path.Combine("current", CredentialProfileDefaults.GetDefaultCredentialsFileName(venue)));

        if (!File.Exists(identityFilePath))
        {
            throw new InvalidOperationException($"Credential profile identity file does not exist: {identityFilePath}");
        }

        if (!File.Exists(credentialsFilePath))
        {
            throw new InvalidOperationException($"Credential profile credentials file does not exist: {credentialsFilePath}");
        }

        return AgeFileApiCredentialProviderFactory.Create(
            venue,
            identityFilePath,
            credentialsFilePath,
            ageDecryptor ?? new AgeCliCredentialFileDecryptor());
    }

    private static string ResolvePath(string baseDirectory, string? configuredPath, string defaultPath)
    {
        var path = string.IsNullOrWhiteSpace(configuredPath)
            ? defaultPath
            : configuredPath;

        return Path.GetFullPath(Path.IsPathRooted(path)
            ? path
            : Path.Combine(baseDirectory, path));
    }
}
