using System;
using Common.Contract.Enums;

namespace Common.Contract.Dtos;

/// <summary>
/// 取引所・口座情報付きのバランス。
/// </summary>
public sealed record Balance(
    ExchangeCode Exchange,
    string Currency,
    decimal Amount,
    decimal Available,
    CurrencyCode CurrencyCode = CurrencyCode.Unknown,
    string? NormalizedCurrency = null,
    string? AccountId = null,
    DateTimeOffset? Timestamp = null,
    decimal? Reserved = null,
    decimal? Withdrawing = null)
{
    /// <summary>
    /// 取引所メタ付きのバランスを生成するファクトリ。
    /// </summary>
    public static Balance Create(
        ExchangeCode exchange,
        string currency,
        decimal amount,
        decimal available,
        string? accountId = null,
        DateTimeOffset? timestamp = null,
        Func<string, string?>? normalizer = null,
        Func<string, CurrencyCode>? codeResolver = null,
        decimal? reserved = null,
        decimal? withdrawing = null)
    {
        if (currency is null) throw new ArgumentNullException(nameof(currency));

        var normalized = normalizer?.Invoke(currency) ?? currency;
        var code = codeResolver?.Invoke(currency) ?? CurrencyCodeConverter.FromString(currency);

        return new Balance(
            Exchange: exchange,
            Currency: currency,
            Amount: amount,
            Available: available,
            CurrencyCode: code,
            NormalizedCurrency: normalized,
            AccountId: accountId,
            Timestamp: timestamp,
            Reserved: reserved,
            Withdrawing: withdrawing);
    }
}
