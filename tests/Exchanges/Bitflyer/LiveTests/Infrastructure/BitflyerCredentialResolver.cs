using System.Diagnostics;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

internal static class BitflyerCredentialResolver
{
    private const int CanonicalVersion = 1;
    private const string CanonicalVenue = "bitflyer";
    private const string CredentialsAgeFileEnvName = "EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH";
    private const string AgeIdentityFileEnvName = "EXCHANGEAPI_AGE_IDENTITY_FILE_PATH";

    public static bool HasConfiguredCredentialsSource()
    {
        return TryResolveAgeInputs(out _, out _) && TryResolveAgeExecutable(out _);
    }

    public static BitflyerApiCredentials? Load()
    {
        if (!TryResolveAgeInputs(out var identityFilePath, out var credentialsFilePath))
        {
            return null;
        }

        if (!TryResolveAgeExecutable(out var ageExecutablePath))
        {
            throw new InvalidOperationException(
                $"The 'age' executable was not found on PATH. Set {CredentialsAgeFileEnvName}/{AgeIdentityFileEnvName}, or install age.");
        }

        var decryptedJson = DecryptCredentials(ageExecutablePath, identityFilePath, credentialsFilePath);
        return ParseCredentials(decryptedJson);
    }

    private static bool TryResolveAgeInputs(out string identityFilePath, out string credentialsFilePath)
    {
        identityFilePath = ResolveAgeIdentityFilePath();
        credentialsFilePath = ResolveCredentialsAgeFilePath();

        if (string.IsNullOrWhiteSpace(identityFilePath) || string.IsNullOrWhiteSpace(credentialsFilePath))
        {
            return false;
        }

        return File.Exists(identityFilePath) && File.Exists(credentialsFilePath);
    }

    private static string ResolveAgeIdentityFilePath()
    {
        var configuredPath = Environment.GetEnvironmentVariable(AgeIdentityFileEnvName);
        return string.IsNullOrWhiteSpace(configuredPath) ? string.Empty : configuredPath;
    }

    private static string ResolveCredentialsAgeFilePath()
    {
        var configuredPath = Environment.GetEnvironmentVariable(CredentialsAgeFileEnvName);
        return string.IsNullOrWhiteSpace(configuredPath) ? string.Empty : configuredPath;
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

    private static string DecryptCredentials(string ageExecutablePath, string identityFilePath, string credentialsFilePath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = ageExecutablePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add("-d");
        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add(identityFilePath);
        startInfo.ArgumentList.Add(credentialsFilePath);

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start the age process.");

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var message = string.IsNullOrWhiteSpace(stderr)
                ? "age decryption failed."
                : $"age decryption failed: {stderr.Trim()}";

            throw new InvalidOperationException(message);
        }

        return stdout;
    }

    private static BitflyerApiCredentials ParseCredentials(string decryptedJson)
    {
        using var document = JsonDocument.Parse(decryptedJson);
        var root = document.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Decrypted credentials JSON must be an object.");
        }

        return ParseCanonicalCredentials(root);
    }

    private static BitflyerApiCredentials ParseCanonicalCredentials(JsonElement root)
    {
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
