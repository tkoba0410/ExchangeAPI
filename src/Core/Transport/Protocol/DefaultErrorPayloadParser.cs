using System;
using System.Text.Json;
namespace Core.Transport.Protocol;

/// <summary>
/// 一般的な JSON エラーフォーマットを対象にしたデフォルトパーサ。
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

        // JSON オブジェクトを試行
        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                var code = TryGetString(root, "error_code")
                           ?? TryGetString(root, "code");
                var message = TryGetString(root, "error_message")
                              ?? TryGetString(root, "message");
                return new ErrorPayload(code, message);
            }
        }
        catch (JsonException)
        {
            // JSON でない場合はフォールバックへ
        }

        // 非 JSON は本文をそのままメッセージとして返す（HTML 等は呼び出し側で適宜処理）
        var trimmed = content.Trim();
        return new ErrorPayload(null, trimmed.Length == 0 ? null : trimmed);
    }

    private static string? TryGetString(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String)
        {
            return value.GetString();
        }

        return null;
    }
}
