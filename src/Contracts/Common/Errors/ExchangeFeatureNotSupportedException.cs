using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Contracts.Common.Errors;

/// <summary>
/// 取引所が特定機能をサポートしていない場合の例外。
/// </summary>
public sealed class ExchangeFeatureNotSupportedException : ExchangeApiException
{
    public ExchangeCode ExchangeCode { get; }
    public string Feature { get; }
    public string? Reason { get; }

    public ExchangeFeatureNotSupportedException(ExchangeCode exchange, string feature)
        : this(exchange, feature, reason: null)
    {
    }

    public ExchangeFeatureNotSupportedException(ExchangeCode exchange, string feature, string? reason)
        : base(
            message: reason is null
                ? $"Feature '{feature}' is not supported by exchange '{exchange}'."
                : $"Feature '{feature}' is not supported by exchange '{exchange}'. Reason: {reason}",
            exchange: exchange,
            operation: feature)
    {
        ExchangeCode = exchange;
        Feature = feature;
        Reason = reason;
    }
}
