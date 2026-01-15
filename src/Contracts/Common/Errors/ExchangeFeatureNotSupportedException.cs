using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Contracts.Common.Errors;

/// <summary>
/// 取引所が特定機能をサポートしていない場合の例外。
/// </summary>
public sealed class ExchangeFeatureNotSupportedException : ExchangeApiException
{
    public ExchangeCode ExchangeCode { get; }
    public string Feature { get; }

    public ExchangeFeatureNotSupportedException(ExchangeCode exchange, string feature)
        : base($"Feature '{feature}' is not supported by exchange '{exchange}'.", exchange: exchange, operation: feature)
    {
        ExchangeCode = exchange;
        Feature = feature;
    }
}
