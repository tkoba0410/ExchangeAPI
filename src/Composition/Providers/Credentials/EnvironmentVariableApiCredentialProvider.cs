using System;
using ExchangeApi.Composition.Abstractions;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Composition.Providers.Credentials;

/// <summary>
/// 環境変数から API キー/シークレットを取得するプロバイダー。
/// 形式: &lt;EXCHANGE&gt;_&lt;ACCOUNT&gt;_API_KEY / _API_SECRET（大文字、ハイフンはアンダースコアに変換）。
/// </summary>
public sealed class EnvironmentVariableApiCredentialProvider : IApiCredentialProvider
{
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
