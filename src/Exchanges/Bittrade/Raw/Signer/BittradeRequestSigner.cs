using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade (Huobi 系) HmacSHA256 署名。
/// </summary>
public sealed class BittradeRequestSigner : IRequestSigner
{
    private readonly string _accessKey;
    private readonly string _secretKey;

    public BittradeRequestSigner(string accessKey, string secretKey)
    {
        _accessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
        _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
    }

    public Task SignAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (request.RequestUri is null) throw new InvalidOperationException("RequestUri is required.");

        var uri = request.RequestUri;
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

        var query = HttpUtility.ParseQueryString(uri.Query ?? string.Empty);
        query["AccessKeyId"] = _accessKey;
        query["SignatureMethod"] = "HmacSHA256";
        query["SignatureVersion"] = "2";
        query["Timestamp"] = timestamp;

        var sorted = query.AllKeys!
            .Where(k => k is not null)
            .OrderBy(k => k, StringComparer.Ordinal)
            .Select(k => $"{k}={HttpUtility.UrlEncode(query[k])}");

        var canonicalQuery = string.Join("&", sorted);
        var canonical = $"{request.Method.Method}\n{uri.Host}\n{uri.AbsolutePath}\n{canonicalQuery}";

        var signature = Sign(canonical, _secretKey);
        query["Signature"] = signature;

        var builder = new UriBuilder(uri)
        {
            Query = query.ToString() ?? string.Empty
        };

        request.RequestUri = builder.Uri;
        return Task.CompletedTask;
    }

    private static string Sign(string text, string secret)
    {
        var payload = Encoding.UTF8.GetBytes(text);
        var key = Encoding.UTF8.GetBytes(secret);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(payload);
        return Convert.ToBase64String(hash);
    }
}
