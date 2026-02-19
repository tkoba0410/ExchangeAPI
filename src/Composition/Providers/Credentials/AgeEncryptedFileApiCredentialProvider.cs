using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using ExchangeApi.Composition.Abstractions;
using ExchangeApi.Composition.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Composition.Providers.Credentials;

/// <summary>
/// age で暗号化された JSON ファイルから API キー/シークレットを取得するプロバイダー。
/// 形式:
/// {
///   "bitflyer/default": {
///     "ApiKey": "...",
///     "ApiSecret": "...",
///     "ExpiresAt": null,
///     "Version": null,
///     "UpdatedAt": null,
///     "Comment": null
///   }
/// }
/// </summary>
public sealed class AgeEncryptedFileApiCredentialProvider : IApiCredentialProvider
{
    private readonly string _encryptedFilePath;
    private readonly string _secretKeyPath;
    private readonly string _exchangeId;
    private readonly IReadOnlyDictionary<string, ApiCredentials> _store;
    private readonly Func<string, string, string> _decryptor;

    public AgeEncryptedFileApiCredentialProvider(
        string encryptedFilePath,
        string exchangeId,
        string secretKeyPath,
        Func<string, string, string>? decryptor = null)
    {
        if (string.IsNullOrWhiteSpace(encryptedFilePath))
        {
            throw new ArgumentException("encryptedFilePath is required.", nameof(encryptedFilePath));
        }

        if (string.IsNullOrWhiteSpace(exchangeId))
        {
            throw new ArgumentException("exchangeId is required.", nameof(exchangeId));
        }

        if (string.IsNullOrWhiteSpace(secretKeyPath))
        {
            throw new ArgumentException("secretKeyPath is required.", nameof(secretKeyPath));
        }

        if (!File.Exists(encryptedFilePath))
        {
            throw new FileNotFoundException("Encrypted credential file not found.", encryptedFilePath);
        }

        if (!File.Exists(secretKeyPath))
        {
            throw new FileNotFoundException("age secret key file not found.", secretKeyPath);
        }

        _encryptedFilePath = encryptedFilePath;
        _secretKeyPath = secretKeyPath;
        _exchangeId = exchangeId;
        _decryptor = decryptor ?? DecryptWithAgeCli;

        var plaintextJson = _decryptor(_encryptedFilePath, _secretKeyPath);
        _store = ParseAndValidateStore(plaintextJson);
    }

    public ApiCredentials Get(AccountId accountId)
    {
        if (accountId.IsEmpty)
        {
            throw new ArgumentException("AccountId is required.", nameof(accountId));
        }

        var entryKey = $"{_exchangeId}/{accountId.Value}";
        if (_store.TryGetValue(entryKey, out var credentials))
        {
            return credentials;
        }

        throw new InvalidOperationException($"CRED_NOT_FOUND: Credentials for '{entryKey}' not found.");
    }

    private static string DecryptWithAgeCli(string encryptedFilePath, string secretKeyPath)
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "age",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            processStartInfo.ArgumentList.Add("-d");
            processStartInfo.ArgumentList.Add("-i");
            processStartInfo.ArgumentList.Add(secretKeyPath);
            processStartInfo.ArgumentList.Add(encryptedFilePath);

            using var process = Process.Start(processStartInfo)
                ?? throw new InvalidOperationException("CRED_DECRYPT_FAILED: Failed to start 'age' process.");

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                var details = string.IsNullOrWhiteSpace(error) ? "unknown error" : error.Trim();
                throw new InvalidOperationException($"CRED_DECRYPT_FAILED: age exited with code {process.ExitCode}. {details}");
            }

            return output;
        }
        catch (Win32Exception ex)
        {
            throw new InvalidOperationException("CRED_DECRYPT_TOOL_MISSING: 'age' command was not found.", ex);
        }
    }

    private static IReadOnlyDictionary<string, ApiCredentials> ParseAndValidateStore(string plaintextJson)
    {
        if (string.IsNullOrWhiteSpace(plaintextJson))
        {
            throw new InvalidOperationException("CRED_SCHEMA_INVALID: Decrypted credential payload is empty.");
        }

        using var document = JsonDocument.Parse(plaintextJson);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("CRED_SCHEMA_INVALID: Root must be a JSON object.");
        }

        var credentialsByEntryKey = new Dictionary<string, ApiCredentials>(StringComparer.Ordinal);
        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.Object)
            {
                throw new InvalidOperationException($"CRED_SCHEMA_INVALID: '{property.Name}' must be a JSON object.");
            }

            var credentials = ParseCredentialEntry(property.Name, property.Value);
            credentialsByEntryKey[property.Name] = credentials;
        }

        return credentialsByEntryKey;
    }

    private static ApiCredentials ParseCredentialEntry(string entryKey, JsonElement entryElement)
    {
        var apiKeyElement = RequireProperty(entryElement, "ApiKey", entryKey);
        var apiSecretElement = RequireProperty(entryElement, "ApiSecret", entryKey);
        var expiresAtElement = RequireProperty(entryElement, "ExpiresAt", entryKey);
        var versionElement = RequireProperty(entryElement, "Version", entryKey);
        var updatedAtElement = RequireProperty(entryElement, "UpdatedAt", entryKey);
        _ = RequireProperty(entryElement, "Comment", entryKey);

        var apiKey = ExtractRequiredString(apiKeyElement, "ApiKey", entryKey);
        var apiSecret = ExtractRequiredString(apiSecretElement, "ApiSecret", entryKey);
        var expiresAt = ParseDateTimeOffsetOrNull(expiresAtElement, "ExpiresAt", entryKey);
        _ = ValidateOptionalStringOrNull(versionElement, "Version", entryKey);
        _ = ParseDateTimeOffsetOrNull(updatedAtElement, "UpdatedAt", entryKey);

        return new ApiCredentials(apiKey, apiSecret, ExpiresAt: expiresAt);
    }

    private static JsonElement RequireProperty(JsonElement entryElement, string propertyName, string entryKey)
    {
        if (!entryElement.TryGetProperty(propertyName, out var property))
        {
            throw new InvalidOperationException($"CRED_SCHEMA_INVALID: '{entryKey}' is missing required property '{propertyName}'.");
        }

        return property;
    }

    private static string ExtractRequiredString(JsonElement element, string propertyName, string entryKey)
    {
        if (element.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException($"CRED_SCHEMA_INVALID: '{entryKey}.{propertyName}' must be a string.");
        }

        var value = element.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"CRED_SCHEMA_INVALID: '{entryKey}.{propertyName}' must not be empty.");
        }

        return value;
    }

    private static string? ValidateOptionalStringOrNull(JsonElement element, string propertyName, string entryKey)
    {
        if (element.ValueKind is JsonValueKind.Null)
        {
            return null;
        }

        if (element.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException($"CRED_SCHEMA_INVALID: '{entryKey}.{propertyName}' must be string or null.");
        }

        return element.GetString();
    }

    private static DateTimeOffset? ParseDateTimeOffsetOrNull(JsonElement element, string propertyName, string entryKey)
    {
        if (element.ValueKind is JsonValueKind.Null)
        {
            return null;
        }

        if (element.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException($"CRED_SCHEMA_INVALID: '{entryKey}.{propertyName}' must be string or null.");
        }

        var value = element.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!DateTimeOffset.TryParse(value, out var parsed))
        {
            throw new InvalidOperationException($"CRED_SCHEMA_INVALID: '{entryKey}.{propertyName}' must be ISO-8601.");
        }

        return parsed;
    }
}
