namespace ExchangeApi.Contracts.Errors;

/// <summary>
/// 取引所固有コードを分類するカテゴリ。
/// </summary>
public enum ExchangeErrorCategory
{
    Unknown,
    Request,
    Auth,
    Balance,
    RateLimit,
    Network,
    Server,
}
