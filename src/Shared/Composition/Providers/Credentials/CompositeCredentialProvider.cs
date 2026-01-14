using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
namespace ExchangeApi.Shared.Composition.Providers.Credentials;

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

    public ApiCredentials Get(ExchangeCode exchange, string accountId)
    {
        var errors = new List<string>();

        foreach (var provider in _providers)
        {
            try
            {
                var creds = provider.Get(exchange, accountId);
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
            $"No credential provider could supply credentials for '{ExchangeCodeFormatter.ToCanonicalId(exchange)}/{accountId}'. Details: {string.Join(" | ", errors)}");
    }

    private static bool IsValid(ApiCredentials creds)
    {
        return !string.IsNullOrWhiteSpace(creds.ApiKey)
            && !string.IsNullOrWhiteSpace(creds.ApiSecret);
    }
}
