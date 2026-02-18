using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
namespace ExchangeApi.Transport.Observability;

/// <summary>
/// 標準化されたログを出力するロガー（サンプル実装）。
/// 機密情報は出力しない前提で最小項目のみ記録する。
/// </summary>
public sealed class StructuredRestClientLogger : IRestClientLogger
{
    private const string Mask = "***";
    private const string OrderIdPseudoPrefix = "oidp_v1_";
    private static readonly byte[] PseudonymizationKey = CreatePseudonymizationKey();
    private static readonly HashSet<string> AllowedQueryKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "symbol",
        "product_code",
        "type",
        "types",
        "period",
        "size",
        "count",
        "before",
        "after",
        "from",
        "direct",
        "status",
        "currency",
        "currency_code",
        "start-date",
        "end-date",
        "start_time",
        "end_time",
        "from_date",
        "child_order_state",
        "parent_order_state"
    };

    private static readonly HashSet<string> AlwaysMaskedKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "signature",
        "signaturemethod",
        "signatureversion",
        "accesskeyid",
        "access-key",
        "access-sign",
        "access-timestamp",
        "api_key",
        "apikey",
        "secret",
        "token",
        "authorization",
        "passphrase",
        "nonce",
        "timestamp",
        "account-id",
        "account_id",
        "uid",
        "sub_account",
        "sub_account_id",
        "message_id"
    };

    private static readonly HashSet<string> OrderIdKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "order_id",
        "client_order_id",
        "child_order_id",
        "parent_order_id",
        "child_order_acceptance_id",
        "parent_order_acceptance_id"
    };

    private static readonly char[] Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();
    private readonly Action<string> _write;

    public StructuredRestClientLogger(Action<string> write)
    {
        _write = write ?? throw new ArgumentNullException(nameof(write));
    }

    public void LogRequest(HttpRequestMessage request)
    {
        var uri = SanitizeUri(request.RequestUri);
        _write($"timestamp={DateTimeOffset.UtcNow:O} event_type=request method={request.Method.Method} uri={uri}");
    }

    public void LogResponse(HttpResponseMessage response, string content)
    {
        var reason = response.ReasonPhrase ?? "";
        var contentLength = content?.Length ?? 0;
        _write($"timestamp={DateTimeOffset.UtcNow:O} event_type=response status={(int)response.StatusCode} reason={reason} content_length={contentLength}");
    }

    public void LogError(Exception exception, HttpRequestMessage request)
    {
        var uri = SanitizeUri(request.RequestUri);
        _write($"timestamp={DateTimeOffset.UtcNow:O} event_type=error method={request.Method.Method} uri={uri} error={exception.GetType().Name} message={exception.Message}");
    }

    private static string SanitizeUri(Uri? uri)
    {
        if (uri is null)
        {
            return "<null>";
        }

        var baseUri = uri.GetLeftPart(UriPartial.Path);
        var rawQuery = uri.Query;
        if (string.IsNullOrEmpty(rawQuery))
        {
            return baseUri;
        }

        var sanitizedPairs = new List<string>();
        var pairs = rawQuery.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var pair in pairs)
        {
            var splitIndex = pair.IndexOf('=');
            var encodedKey = splitIndex >= 0 ? pair[..splitIndex] : pair;
            var encodedValue = splitIndex >= 0 ? pair[(splitIndex + 1)..] : string.Empty;

            var key = Uri.UnescapeDataString(encodedKey);
            var value = Uri.UnescapeDataString(encodedValue);
            var sanitizedValue = SanitizeQueryValue(uri.Host, key, value);
            sanitizedPairs.Add($"{key}={sanitizedValue}");
        }

        return sanitizedPairs.Count == 0
            ? baseUri
            : $"{baseUri}?{string.Join("&", sanitizedPairs)}";
    }

    private static string SanitizeQueryValue(string host, string key, string value)
    {
        if (OrderIdKeys.Contains(key))
        {
            return string.IsNullOrEmpty(value) ? Mask : ToOrderIdPseudo(host, key, value);
        }

        if (AlwaysMaskedKeys.Contains(key))
        {
            return Mask;
        }

        if (!AllowedQueryKeys.Contains(key))
        {
            return Mask;
        }

        return value;
    }

    private static string ToOrderIdPseudo(string host, string key, string value)
    {
        var payload = Encoding.UTF8.GetBytes($"{host}|{key}|{value}");
        using var hmac = new HMACSHA256(PseudonymizationKey);
        var hash = hmac.ComputeHash(payload);
        var token = ToBase32(hash);
        return $"{OrderIdPseudoPrefix}{token[..16]}";
    }

    private static string ToBase32(byte[] data)
    {
        if (data.Length == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder((data.Length * 8 + 4) / 5);
        var buffer = (int)data[0];
        var next = 1;
        var bitsLeft = 8;

        while (bitsLeft > 0 || next < data.Length)
        {
            if (bitsLeft < 5)
            {
                if (next < data.Length)
                {
                    buffer <<= 8;
                    buffer |= data[next++] & 0xff;
                    bitsLeft += 8;
                }
                else
                {
                    var pad = 5 - bitsLeft;
                    buffer <<= pad;
                    bitsLeft += pad;
                }
            }

            var index = (buffer >> (bitsLeft - 5)) & 0x1f;
            bitsLeft -= 5;
            builder.Append(Base32Alphabet[index]);
        }

        return builder.ToString();
    }

    private static byte[] CreatePseudonymizationKey()
    {
        var keyFromEnvironment = Environment.GetEnvironmentVariable("EXCHANGEAPI_LOG_MASK_KEY");
        if (!string.IsNullOrWhiteSpace(keyFromEnvironment))
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(keyFromEnvironment));
        }

        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }
}
