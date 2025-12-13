using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Exchange.Bitflyer.Raw.Signer;
using ExchangeApi.Transport.Time;
using Xunit;

namespace Exchange.Bitflyer.Tests;

public sealed class BitflyerRequestSigner_Tests
{
    [Fact]
    public async Task SignAsync_AddsAccessHeadersWithSignature()
    {
        var clock = new FixedClock(1234567890);
        var signer = new BitflyerRequestSigner("api-key", "secret", clock);

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.bitflyer.com/v1/me/sendchildorder")
        {
            Content = new StringContent("{\"a\":1}", Encoding.UTF8, "application/json"),
        };

        await signer.SignAsync(request);

        Assert.Equal("api-key", Assert.Single(request.Headers.GetValues("ACCESS-KEY")));
        Assert.Equal("1234567890", Assert.Single(request.Headers.GetValues("ACCESS-TIMESTAMP")));
        Assert.NotNull(request.Headers.GetValues("ACCESS-SIGN"));

        // 期待値計算
        var expectedPrehash = "1234567890POST/v1/me/sendchildorder{\"a\":1}";
        var expectedSignature = TestHmac.Sha256Hex("secret", expectedPrehash);
        Assert.Equal(expectedSignature, Assert.Single(request.Headers.GetValues("ACCESS-SIGN")));
    }

    private static class TestHmac
    {
        public static string Sha256Hex(string secret, string data)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return System.BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }

    private sealed class FixedClock : IExchangeClock
    {
        private readonly long _timestamp;

        public FixedClock(long timestamp)
        {
            _timestamp = timestamp;
        }

        public System.DateTimeOffset UtcNow => System.DateTimeOffset.FromUnixTimeSeconds(_timestamp);
    }
}
