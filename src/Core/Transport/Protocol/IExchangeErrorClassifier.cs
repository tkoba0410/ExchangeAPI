using System.Net;
namespace ExchangeApi.Core.Transport.Protocol;

/// <summary>
/// HTTP ステータスや取引所エラーコードからカテゴリを判定する拡張ポイント。
/// </summary>
public interface IExchangeErrorClassifier
{
    ExchangeApi.Core.Contracts.Errors.ExchangeErrorCategory? Classify(HttpStatusCode? statusCode, string? exchangeErrorCode);
}
