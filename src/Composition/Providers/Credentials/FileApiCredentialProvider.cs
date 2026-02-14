using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ExchangeApi.Composition.Abstractions;
using ExchangeApi.Composition.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Composition.Providers.Credentials;

/// <summary>
/// ファイル(JSON)からAPIキー/シークレットを取得するプロバイダー。クロスプラットフォームで動作。
/// 形式:
/// {
///   "bitflyer/default": { "ApiKey": "...", "ApiSecret": "..." },
///   "exchange/account": { "ApiKey": "...", "ApiSecret": "..." }
/// }
/// セキュリティはファイル権限に依存するため、配置先のアクセス制御に注意。
/// </summary>
public sealed class FileApiCredentialProvider : IApiCredentialProvider
{
    private readonly string _filePath;
    private readonly IReadOnlyDictionary<string, ApiCredentials> _store;
    private readonly string _exchangeId;

    public FileApiCredentialProvider(string filePath, string exchangeId)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("filePath is required.", nameof(filePath));
        }

        if (string.IsNullOrWhiteSpace(exchangeId))
        {
            throw new ArgumentException("ExchangeId is required.", nameof(exchangeId));
        }

        _filePath = filePath;
        _exchangeId = exchangeId;

        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException("Credential file not found.", _filePath);
        }

        var json = File.ReadAllText(_filePath);
        var dict = JsonSerializer.Deserialize<Dictionary<string, ApiCredentials>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        _store = dict ?? throw new InvalidOperationException("Credential file is empty or invalid.");
    }

    public ApiCredentials Get(AccountId accountId)
    {
        if (accountId.IsEmpty)
        {
            throw new ArgumentException("AccountId is required.", nameof(accountId));
        }

        var key = $"{_exchangeId}/{accountId.Value}";
        if (_store.TryGetValue(key, out var creds) && IsValid(creds))
        {
            return creds;
        }

        throw new InvalidOperationException($"Credentials for '{key}' not found or invalid in '{_filePath}'.");
    }

    private static bool IsValid(ApiCredentials creds)
    {
        return !string.IsNullOrWhiteSpace(creds.ApiKey) && !string.IsNullOrWhiteSpace(creds.ApiSecret);
    }
}
