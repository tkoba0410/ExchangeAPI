using System;
using ExchangeApi.Composition.Abstractions;
using ExchangeApi.Composition.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Composition.Providers.Credentials;

/// <summary>
/// 環境変数から API キー/シークレットを取得するプロバイダー。
/// 形式: &lt;EXCHANGE&gt;_&lt;ACCOUNT&gt;_API_KEY / _API_SECRET（大文字、ハイフンはアンダースコアに変換）。
/// </summary>
public sealed class EnvironmentVariableApiCredentialProvider : IApiCredentialProvider
{
    private readonly string _exchangeId;

    public EnvironmentVariableApiCredentialProvider(string exchangeId)
    {
        if (string.IsNullOrWhiteSpace(exchangeId))
        {
            throw new ArgumentException("ExchangeId is required.", nameof(exchangeId));
        }

        _exchangeId = exchangeId;
    }

    public ApiCredentials Get(AccountId accountId)
    {
        if (accountId.IsEmpty)
        {
            throw new ArgumentException("AccountId is required.", nameof(accountId));
        }

        var prefix = $"{Normalize(_exchangeId)}_{Normalize(accountId.Value)}";
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
