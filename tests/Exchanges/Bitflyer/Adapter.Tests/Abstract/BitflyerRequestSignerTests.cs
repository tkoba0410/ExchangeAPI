using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Exchanges.Bitflyer.Wire.Internal.Auth;
using ExchangeApi.Transport.Time;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerRequestSignerTests
{
    [Fact]
    public async Task SignAsync_uses_unix_time_milliseconds_for_timestamp_and_signature()
    {
        var now = new DateTimeOffset(2024, 1, 2, 3, 4, 5, 678, TimeSpan.Zero);
        var signer = new RequestSigner("key-1", "secret-1", new FixedClock(now));
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://api.bitflyer.com/v1/me/getbalance?product_code=BTC_JPY");

        await signer.SignAsync(request);

        var expectedTimestamp = now.ToUnixTimeMilliseconds().ToString();
        Assert.Equal(expectedTimestamp, Assert.Single(request.Headers.GetValues(AuthKeys.AccessTimestamp)));
        Assert.Equal("key-1", Assert.Single(request.Headers.GetValues(AuthKeys.AccessKey)));

        var expectedSignature = ComputeHexDigest(
            "secret-1",
            $"{expectedTimestamp}GET/v1/me/getbalance?product_code=BTC_JPY");
        Assert.Equal(expectedSignature, Assert.Single(request.Headers.GetValues(AuthKeys.AccessSign)));
    }

    [Fact]
    public async Task SignAsync_includes_body_in_signature_for_post_requests()
    {
        var now = new DateTimeOffset(2024, 1, 2, 3, 4, 5, 678, TimeSpan.Zero);
        var signer = new RequestSigner("key-1", "secret-1", new FixedClock(now));
        const string body = "{\"product_code\":\"BTC_JPY\",\"child_order_acceptance_id\":\"JRF-1\"}";
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.bitflyer.com/v1/me/cancelchildorder")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };

        await signer.SignAsync(request);

        var expectedTimestamp = now.ToUnixTimeMilliseconds().ToString();
        var expectedSignature = ComputeHexDigest(
            "secret-1",
            $"{expectedTimestamp}POST/v1/me/cancelchildorder{body}");
        Assert.Equal(expectedSignature, Assert.Single(request.Headers.GetValues(AuthKeys.AccessSign)));
    }

    private static string ComputeHexDigest(string secret, string canonical)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(canonical));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    private sealed class FixedClock(DateTimeOffset now) : IExchangeClock
    {
        public DateTimeOffset UtcNow { get; } = now;
    }
}
