namespace ExchangeApi.Contracts.Common.Errors;

/// <summary>
/// 取引所固有コードを分類するカテゴリ。
/// アダプターは個別エラーをこのカテゴリに正規化し、上位でポリシー判断に使う。
/// </summary>
/// <remarks>
/// 例: 400/バリデーションエラーを Request、401/署名エラーを Auth、429 を RateLimit、残高不足を Balance に正規化する。
/// </remarks>
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
