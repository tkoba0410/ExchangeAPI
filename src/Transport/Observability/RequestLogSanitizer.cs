using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace ExchangeApi.Transport.Observability;

internal static class RequestLogSanitizer
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
        "parent_order_state",
    };

    private static readonly HashSet<string> AlwaysMaskedQueryKeys = new(StringComparer.OrdinalIgnoreCase)
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
        "message_id",
    };

    private static readonly HashSet<string> OrderIdKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "order_id",
        "client_order_id",
        "child_order_id",
        "parent_order_id",
        "child_order_acceptance_id",
        "parent_order_acceptance_id",
    };

    private static readonly HashSet<string> AlwaysMaskedHeaderKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "authorization",
        "proxy-authorization",
        "access-key",
        "access-sign",
        "access-timestamp",
        "accesskeyid",
        "x-api-key",
        "api-key",
        "apikey",
        "signature",
        "passphrase",
        "cookie",
    };

    private static readonly char[] Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

    public static string SanitizeUri(Uri? uri)
    {
        if (uri is null)
        {
            return "<null>";
        }

        var baseUri = uri.GetLeftPart(UriPartial.Path);
        var sanitizedPairs = BuildSanitizedQueryPairs(uri);
        return sanitizedPairs.Count == 0
            ? baseUri
            : $"{baseUri}?{string.Join("&", sanitizedPairs)}";
    }

    public static HttpRequestMessage CreateSanitizedRequest(HttpRequestMessage request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var sanitizedRequest = new HttpRequestMessage(request.Method, CreateSanitizedUri(request.RequestUri))
        {
            Version = request.Version,
            VersionPolicy = request.VersionPolicy,
        };

        foreach (var header in request.Headers)
        {
            if (AlwaysMaskedHeaderKeys.Contains(header.Key))
            {
                sanitizedRequest.Headers.TryAddWithoutValidation(header.Key, Mask);
                continue;
            }

            sanitizedRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return sanitizedRequest;
    }

    private static Uri? CreateSanitizedUri(Uri? uri)
    {
        if (uri is null)
        {
            return null;
        }

        var builder = new UriBuilder(uri.GetLeftPart(UriPartial.Path));
        var sanitizedPairs = BuildSanitizedQueryPairs(uri);
        if (sanitizedPairs.Count == 0)
        {
            builder.Query = string.Empty;
            return builder.Uri;
        }

        var encodedPairs = new List<string>(sanitizedPairs.Count);
        foreach (var pair in sanitizedPairs)
        {
            var splitIndex = pair.IndexOf('=');
            var key = splitIndex >= 0 ? pair[..splitIndex] : pair;
            var value = splitIndex >= 0 ? pair[(splitIndex + 1)..] : string.Empty;
            encodedPairs.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
        }

        builder.Query = string.Join("&", encodedPairs);
        return builder.Uri;
    }

    private static List<string> BuildSanitizedQueryPairs(Uri uri)
    {
        var rawQuery = uri.Query;
        if (string.IsNullOrEmpty(rawQuery))
        {
            return new List<string>();
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

        return sanitizedPairs;
    }

    private static string SanitizeQueryValue(string host, string key, string value)
    {
        if (OrderIdKeys.Contains(key))
        {
            return string.IsNullOrEmpty(value) ? Mask : ToOrderIdPseudo(host, key, value);
        }

        if (AlwaysMaskedQueryKeys.Contains(key))
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
