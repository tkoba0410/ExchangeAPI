namespace ExchangeApi.Core.Transport.Protocol;

/// <summary>
/// エラーレスポンスのペイロードからコード/メッセージを抽出するためのパーサ。
/// </summary>
public interface IErrorPayloadParser
{
    ErrorPayload Parse(string content);
}

public sealed record ErrorPayload(string? ErrorCode, string? ErrorMessage);
