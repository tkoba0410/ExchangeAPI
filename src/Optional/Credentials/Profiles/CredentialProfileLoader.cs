using System.Text.Json;

namespace ExchangeApi.Optional.Credentials.Profiles;

public static class CredentialProfileLoader
{
    public static CredentialProfile Load(string profileFilePath)
    {
        if (string.IsNullOrWhiteSpace(profileFilePath))
        {
            throw new ArgumentException("Credential profile path is required.", nameof(profileFilePath));
        }

        using var stream = File.OpenRead(profileFilePath);
        using var document = JsonDocument.Parse(stream);
        return Parse(document.RootElement);
    }

    public static bool TryLoad(string profileFilePath, out CredentialProfile? profile, out string? errorMessage)
    {
        profile = null;
        errorMessage = null;

        try
        {
            profile = Load(profileFilePath);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or ArgumentException or InvalidOperationException)
        {
            errorMessage = ex.Message;
            return false;
        }
    }

    private static CredentialProfile Parse(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Credential profile must be a JSON object.");
        }

        if (TryParseCTradeBotFlatSettings(root, out var flatProfile))
        {
            return flatProfile;
        }

        var version = TryGetInt32(root, "version") ?? 1;
        if (version != 1)
        {
            throw new InvalidOperationException($"Unsupported credential profile version: {version}");
        }

        if (!root.TryGetProperty("credentials", out var credentialsElement) ||
            credentialsElement.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Credential profile requires a credentials object.");
        }

        var credentials = new Dictionary<string, CredentialProfileEntry>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in credentialsElement.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException($"Credential profile entry '{property.Name}' must be a JSON object.");
            }

            credentials[property.Name] = new CredentialProfileEntry
            {
                Provider = GetString(property.Value, "provider") ?? CredentialProfileProviderNames.AgeFile,
                IdentityFilePath = GetString(property.Value, "identityFilePath") ?? GetString(property.Value, "ageIdentityFile"),
                CredentialsFilePath = GetString(property.Value, "credentialsFilePath") ?? GetString(property.Value, "credentialsAgeFile"),
            };
        }

        return new CredentialProfile
        {
            Version = version,
            Credentials = credentials,
        };
    }

    private static bool TryParseCTradeBotFlatSettings(JsonElement root, out CredentialProfile profile)
    {
        profile = new CredentialProfile();
        var source = GetString(root, "credentialsSource");
        var credentialsAgeFile = GetString(root, "credentialsAgeFile");
        var ageIdentityFile = GetString(root, "ageIdentityFile");

        if (source is null && credentialsAgeFile is null && ageIdentityFile is null)
        {
            return false;
        }

        profile = new CredentialProfile
        {
            Version = 1,
            Credentials = new Dictionary<string, CredentialProfileEntry>(StringComparer.OrdinalIgnoreCase)
            {
                ["bitflyer"] = new CredentialProfileEntry
                {
                    Provider = NormalizeProvider(source),
                    IdentityFilePath = ageIdentityFile,
                    CredentialsFilePath = credentialsAgeFile,
                },
            },
        };

        return true;
    }

    private static string NormalizeProvider(string? provider)
    {
        if (string.IsNullOrWhiteSpace(provider) ||
            string.Equals(provider, "AgeFile", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(provider, CredentialProfileProviderNames.AgeFile, StringComparison.OrdinalIgnoreCase))
        {
            return CredentialProfileProviderNames.AgeFile;
        }

        return provider;
    }

    private static string? GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static int? TryGetInt32(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out var value)
            ? value
            : null;
    }
}
