using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Primitives.Errors;

/// <summary>
/// 取引所が特定機能をサポートしていない場合の例外。
/// </summary>
public sealed class ExchangeFeatureNotSupportedException : ExchangeApiException
{
    public string Feature { get; }
    public string? Reason { get; }

    public ExchangeFeatureNotSupportedException(string feature)
        : this(feature, reason: null)
    {
    }

    public ExchangeFeatureNotSupportedException(string feature, string? reason)
        : base(
            message: reason is null
                ? $"Feature '{feature}' is not supported."
                : $"Feature '{feature}' is not supported. Reason: {reason}",
            operation: feature)
    {
        Feature = feature;
        Reason = reason;
    }
}
