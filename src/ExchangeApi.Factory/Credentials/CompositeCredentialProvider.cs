using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Contracts.Dtos;

namespace ExchangeApi.Factory.Credentials;

/// <summary>
/// 複数のプロバイダーをフォールバック順に束ねる。
/// </summary>
public sealed class CompositeCredentialProvider : IApiCredentialProvider
{
    private readonly IReadOnlyList<IApiCredentialProvider> _providers;

    public CompositeCredentialProvider(IEnumerable<IApiCredentialProvider> providers)
    {
        if (providers is null)
        {
            throw new ArgumentNullException(nameof(providers));
        }

        var list = providers.ToArray();
        if (list.Length == 0)
        {
            throw new ArgumentException("At least one provider is required.", nameof(providers));
        }

        _providers = list;
    }

    public ApiCredentials Get(string exchangeId, string accountId)
    {
        var errors = new List<string>();

        foreach (var provider in _providers)
        {
            try
            {
                var creds = provider.Get(exchangeId, accountId);
                if (IsValid(creds))
                {
                    return creds;
                }

                errors.Add($"{provider.GetType().Name}: returned empty credentials.");
            }
            catch (Exception ex)
            {
                errors.Add($"{provider.GetType().Name}: {ex.GetType().Name} - {ex.Message}");
            }
        }

        throw new InvalidOperationException(
            $"No credential provider could supply credentials for '{exchangeId}/{accountId}'. Details: {string.Join(" | ", errors)}");
    }

    private static bool IsValid(ApiCredentials creds)
    {
        return !string.IsNullOrWhiteSpace(creds.ApiKey)
            && !string.IsNullOrWhiteSpace(creds.ApiSecret);
    }
}
