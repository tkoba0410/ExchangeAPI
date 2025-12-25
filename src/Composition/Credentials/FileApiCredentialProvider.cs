using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
namespace ExchangeApi.Composition.Credentials;

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

    public FileApiCredentialProvider(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("filePath is required.", nameof(filePath));
        }

        _filePath = filePath;

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

    public ApiCredentials Get(ExchangeCode exchange, string accountId)
    {
        if (exchange is ExchangeCode.None or ExchangeCode.Unknown)
        {
            throw new ArgumentException("ExchangeCode is required.", nameof(exchange));
        }

        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new ArgumentException("AccountId is required.", nameof(accountId));
        }

        var exchangeId = ExchangeCodeFormatter.ToCanonicalId(exchange);
        var key = $"{exchangeId}/{accountId}";
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
