using System;
using Common.Interfaces;
using Common.Dtos;
using Common.Enums;
namespace Composition.Factory.Credentials;

/// <summary>
/// 環境変数から API キー/シークレットを取得するプロバイダー。
/// 形式: &lt;EXCHANGE&gt;_&lt;ACCOUNT&gt;_API_KEY / _API_SECRET（大文字、ハイフンはアンダースコアに変換）。
/// </summary>
public sealed class EnvironmentVariableApiCredentialProvider : IApiCredentialProvider
{
    public ApiCredentials Get(string exchangeId, string accountId)
    {
        if (string.IsNullOrWhiteSpace(exchangeId))
        {
            throw new ArgumentException("ExchangeId is required.", nameof(exchangeId));
        }

        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new ArgumentException("AccountId is required.", nameof(accountId));
        }

        var prefix = $"{Normalize(exchangeId)}_{Normalize(accountId)}";
        var apiKeyName = $"{prefix}_API_KEY";
        var apiSecretName = $"{prefix}_API_SECRET";

        var apiKey = Environment.GetEnvironmentVariable(apiKeyName);
        var apiSecret = Environment.GetEnvironmentVariable(apiSecretName);

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException($"Environment variable '{apiKeyName}' is not set.");
        }

        if (string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new InvalidOperationException($"Environment variable '{apiSecretName}' is not set.");
        }

        return new ApiCredentials(apiKey, apiSecret);
    }

    private static string Normalize(string value)
    {
        return value
            .Trim()
            .Replace("-", "_")
            .Replace(" ", "_")
            .ToUpperInvariant();
    }
}
