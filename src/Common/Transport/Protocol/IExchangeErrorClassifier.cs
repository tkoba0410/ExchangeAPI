using System.Net;

namespace Common.Transport.Protocol;

/// <summary>
/// HTTP ステータスや取引所エラーコードからカテゴリを判定する拡張ポイント。
/// </summary>
public interface IExchangeErrorClassifier
{
    Common.Contract.Errors.ExchangeErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode);
}
