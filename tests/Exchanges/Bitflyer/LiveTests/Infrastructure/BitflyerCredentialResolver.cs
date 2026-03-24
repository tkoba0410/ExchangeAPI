using System.Diagnostics;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

internal static class BitflyerCredentialResolver
{
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

        var source = ResolveBitflyerSection(root);
        var apiKey = ReadFirstString(source, "apiKey", "api_key", "ApiKey");
        var apiSecret = ReadFirstString(source, "apiSecret", "api_secret", "ApiSecret");

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

    private static JsonElement ResolveBitflyerSection(JsonElement root)
    {
        foreach (var propertyName in new[] { "bitflyer/default", "bitflyer", "Bitflyer" })
        {
            if (root.TryGetProperty(propertyName, out var section) && section.ValueKind == JsonValueKind.Object)
            {
                return section;
            }
        }

        return root;
    }

    private static string? ReadFirstString(JsonElement source, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            if (source.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String)
            {
                return value.GetString();
            }
        }

        return null;
    }
}
