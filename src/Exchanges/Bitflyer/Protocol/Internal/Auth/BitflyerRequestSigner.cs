using System.Security.Cryptography;
using System.Text;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Auth;

internal static class BitflyerRequestSigner
{
    internal static void ApplyPrivateHeaders(
        HttpRequestMessage requestMessage,
        string method,
        string pathAndQuery,
        string bodyText,
        string apiKey,
        string apiSecret)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var payload = string.Concat(timestamp, method, pathAndQuery, bodyText);
        var signature = Sign(payload, apiSecret);

        requestMessage.Headers.TryAddWithoutValidation("ACCESS-KEY", apiKey);
        requestMessage.Headers.TryAddWithoutValidation("ACCESS-TIMESTAMP", timestamp);
        requestMessage.Headers.TryAddWithoutValidation("ACCESS-SIGN", signature);
    }

    private static string Sign(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
