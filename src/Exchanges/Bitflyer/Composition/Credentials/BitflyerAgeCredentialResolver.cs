using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Credentials;

public static class BitflyerAgeCredentialResolver
{
    public const int CanonicalVersion = 1;
    public const string CanonicalVenue = "bitflyer";
    public const string AgeIdentityFileEnvName = "EXCHANGEAPI_AGE_IDENTITY_FILE_PATH";
    public const string CredentialsAgeFileEnvName = "EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH";

    public static bool HasConfiguredCredentialsSource(
        Func<string, string?> getEnvironmentVariable,
        Func<string, bool> fileExists,
        Func<bool> isAgeAvailable)
    {
        var identityFilePath = getEnvironmentVariable(AgeIdentityFileEnvName);
        var credentialsFilePath = getEnvironmentVariable(CredentialsAgeFileEnvName);
        return !string.IsNullOrWhiteSpace(identityFilePath)
            && !string.IsNullOrWhiteSpace(credentialsFilePath)
            && fileExists(identityFilePath)
            && fileExists(credentialsFilePath)
            && isAgeAvailable();
    }

    public static BitflyerAgeCredentialResolution Resolve(
        Func<string, string?> getEnvironmentVariable,
        Func<string, bool> fileExists,
        Func<bool> isAgeAvailable,
        Func<string, string, string> decrypt)
    {
        var identityFilePath = getEnvironmentVariable(AgeIdentityFileEnvName);
        var credentialsFilePath = getEnvironmentVariable(CredentialsAgeFileEnvName);
        var hasIdentity = !string.IsNullOrWhiteSpace(identityFilePath);
        var hasCredentialsFile = !string.IsNullOrWhiteSpace(credentialsFilePath);

        if (!hasIdentity && !hasCredentialsFile)
        {
            return BitflyerAgeCredentialResolution.None();
        }

        if (!hasIdentity || !hasCredentialsFile)
        {
            return BitflyerAgeCredentialResolution.Failure(
                $"{AgeIdentityFileEnvName} and {CredentialsAgeFileEnvName} must both be set to use age-backed credentials.");
        }

        if (!fileExists(identityFilePath!))
        {
            return BitflyerAgeCredentialResolution.Failure(
                $"{AgeIdentityFileEnvName} does not exist: {identityFilePath}");
        }

        if (!fileExists(credentialsFilePath!))
        {
            return BitflyerAgeCredentialResolution.Failure(
                $"{CredentialsAgeFileEnvName} does not exist: {credentialsFilePath}");
        }

        if (!isAgeAvailable())
        {
            return BitflyerAgeCredentialResolution.Failure("The 'age' executable was not found on PATH.");
        }

        try
        {
            var decryptedJson = decrypt(identityFilePath!, credentialsFilePath!);
            return BitflyerAgeCredentialResolution.Success(ParseCredentials(decryptedJson));
        }
        catch (Exception ex)
        {
            return BitflyerAgeCredentialResolution.Failure(ex.Message);
        }
    }

    public static BitflyerApiCredentials ParseCredentials(string decryptedJson)
    {
        using var document = JsonDocument.Parse(decryptedJson);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Decrypted credentials JSON must be an object.");
        }

        if (!root.TryGetProperty("version", out var versionElement) ||
            versionElement.ValueKind != JsonValueKind.Number ||
            !versionElement.TryGetInt32(out var version))
        {
            throw new InvalidOperationException("Decrypted credentials JSON must contain integer version.");
        }

        if (version != CanonicalVersion)
        {
            throw new InvalidOperationException($"Unsupported credentials JSON version: {version}.");
        }

        if (!root.TryGetProperty("venue", out var venueElement) || venueElement.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException("Decrypted credentials JSON must contain string venue.");
        }

        var venue = venueElement.GetString();
        if (!string.Equals(venue, CanonicalVenue, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Decrypted credentials JSON venue must be '{CanonicalVenue}'.");
        }

        var apiKey = ReadString(root, "apiKey");
        var apiSecret = ReadString(root, "apiSecret");
        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new InvalidOperationException("Decrypted credentials JSON must contain bitFlyer apiKey/apiSecret.");
        }

        return new BitflyerApiCredentials
        {
            ApiKey = apiKey,
            ApiSecret = apiSecret,
        };
    }

    private static string? ReadString(JsonElement source, string propertyName)
    {
        if (source.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String)
        {
            return value.GetString();
        }

        return null;
    }
}
