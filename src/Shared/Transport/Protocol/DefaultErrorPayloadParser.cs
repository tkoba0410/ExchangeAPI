namespace ExchangeApi.Shared.Transport.Protocol;

/// <summary>
/// transport 既定のエラーペイロードパーサ。
/// TopSpec 9 により transport で JSON 解釈は行わないため、本文をそのまま返す。
/// </summary>
public sealed class DefaultErrorPayloadParser : IErrorPayloadParser
{
    public static readonly DefaultErrorPayloadParser Instance = new();

    public ErrorPayload Parse(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return new ErrorPayload(null, null);
        }

        // 非 JSON は本文をそのままメッセージとして返す（HTML 等は呼び出し側で適宜処理）
        var trimmed = content.Trim();
        return new ErrorPayload(null, trimmed.Length == 0 ? null : trimmed);
    }
}
