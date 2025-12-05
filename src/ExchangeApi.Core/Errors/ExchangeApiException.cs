using System;
using System.Net;

namespace ExchangeApi.Core.Errors;

/// <summary>
/// 取引所 API 呼び出し時の例外を表す共通例外クラス。
/// HTTP エラー・通信エラー・JSON パースエラーなど、
/// 下位レイヤの技術的な失敗をドメイン側に伝えるために使用する。
/// </summary>
public class ExchangeApiException : Exception
{
    /// <summary>
    /// 取引所を識別する ID。（例: "bitFlyer"）
    /// </summary>
    public string? ExchangeId { get; }

    /// <summary>
    /// 抽象クライアント上の操作名。（例: "GetTicker", "GetBalances"）
    /// </summary>
    public string? Operation { get; }

    /// <summary>
    /// HTTP ステータスコード。HTTP レイヤで失敗した場合のみ設定される。
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// 取引所が返す固有コード（存在する場合）。
    /// </summary>
    public string? ExchangeErrorCode { get; }

    /// <summary>
    /// 固有コードのカテゴリ。
    /// </summary>
    public ExchangeErrorCategory? ErrorCategory { get; }

    /// <summary>
    /// 共通のコンストラクタ。
    /// </summary>
    public ExchangeApiException(
        string message,
        string? exchangeId = null,
        string? operation = null,
        HttpStatusCode? statusCode = null,
        string? exchangeErrorCode = null,
        ExchangeErrorCategory? errorCategory = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ExchangeId = exchangeId;
        Operation = operation;
        StatusCode = statusCode;
        ExchangeErrorCode = exchangeErrorCode;
        ErrorCategory = errorCategory;
    }

    /// <summary>
    /// 従来互換用のコンストラクタ（メッセージのみ）。
    /// </summary>
    public ExchangeApiException(string message)
        : this(message, null, null, null, null, null, null)
    {
    }

    /// <summary>
    /// 従来互換用のコンストラクタ（メッセージ + 内部例外）。
    /// </summary>
    public ExchangeApiException(string message, Exception innerException)
        : this(message, null, null, null, null, null, innerException)
    {
    }
}
