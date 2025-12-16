using System.Net;
namespace Core.Transport.Protocol;

/// <summary>
/// HTTP ステータスや取引所エラーコードからカテゴリを判定する拡張ポイント。
/// </summary>
public interface IExchangeErrorClassifier
{
    Core.Contracts.Errors.ExchangeErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode);
}
