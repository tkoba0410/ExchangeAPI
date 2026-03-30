using System.Diagnostics;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

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
            && File.Exists(credentialsFilePath)
            && AgeProcess.IsAvailable();
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

        if (!AgeProcess.IsAvailable())
        {
            return BitflyerCredentialResolution.Failure("The 'age' executable was not found on PATH.");
        }

        try
        {
            var decryptedJson = AgeProcess.Decrypt(identityFilePath!, credentialsFilePath!);
            return BitflyerCredentialResolution.Success(ParseCredentials(decryptedJson));
        }
        catch (Exception ex)
        {
            return BitflyerCredentialResolution.Failure(ex.Message);
        }
    }

    private static BitflyerApiCredentials ParseCredentials(string decryptedJson)
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

    private static class AgeProcess
    {
        public static bool IsAvailable()
        {
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
                    return true;
                }
            }

            return false;
        }

        public static string Decrypt(string identityFilePath, string credentialsFilePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "age",
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
    }
}

public sealed class BitflyerCredentialResolution
{
    private BitflyerCredentialResolution(BitflyerApiCredentials? credentials, string? errorMessage)
    {
        Credentials = credentials;
        ErrorMessage = errorMessage;
    }

    public BitflyerApiCredentials? Credentials { get; }

    public string? ErrorMessage { get; }

    public bool HasFailure => !string.IsNullOrWhiteSpace(ErrorMessage);

    public static BitflyerCredentialResolution Success(BitflyerApiCredentials credentials)
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
