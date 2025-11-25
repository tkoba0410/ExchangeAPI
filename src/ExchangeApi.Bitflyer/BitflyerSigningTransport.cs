using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Infrastructure.Protocol;
using ExchangeApi.Infrastructure.Time;
using ExchangeApi.Infrastructure.Transport;

namespace ExchangeApi.Bitflyer;

/// <summary>
/// bitFlyer Private API 向けの署名付きトランスポート。
/// RestClient に手を入れず、Transport 層で認証を付与する。
/// </summary>
public sealed class BitflyerSigningTransport : IHttpTransport
{
    private readonly IHttpTransport _inner;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly IExchangeClock _clock;

    public BitflyerSigningTransport(
        IHttpTransport inner,
        string apiKey,
        string apiSecret,
        IExchangeClock clock)
    {
        _inner = inner;
        _apiKey = apiKey;
        _apiSecret = apiSecret;
        _clock = clock;
    }

    public async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        // Timestamp
        var timestamp = _clock.UtcNow.ToUnixTimeSeconds().ToString();

        // Method
        var method = request.Method.Method.ToUpperInvariant();

        // Path + Query
        var pathAndQuery = request.RequestUri!.PathAndQuery;

        // Body
        string body = string.Empty;
        if (request.Content != null)
        {
            body = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        // Sign text
        var text = $"{timestamp}{method}{pathAndQuery}{body}";

        var keyBytes = Encoding.UTF8.GetBytes(_apiSecret);
        var textBytes = Encoding.UTF8.GetBytes(text);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(textBytes);
        var sign = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        // Add headers
        request.Headers.Add("ACCESS-KEY", _apiKey);
        request.Headers.Add("ACCESS-TIMESTAMP", timestamp);
        request.Headers.Add("ACCESS-SIGN", sign);
        request.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        return await _inner.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
