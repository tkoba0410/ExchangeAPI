using System.Security.Cryptography;
using System.Text;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Auth;

internal static class BitflyerRequestSigner
{
    internal static void ApplyPrivateHeaders(
        HttpRequestMessage requestMessage,
        string method,
        string pathAndQuery,
        string bodyText,
        IApiCredentialSession credentialSession)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var payload = string.Concat(timestamp, method, pathAndQuery, bodyText);
        var signature = credentialSession.Sign(payload);

        requestMessage.Headers.TryAddWithoutValidation("ACCESS-KEY", credentialSession.ApiKey);
        requestMessage.Headers.TryAddWithoutValidation("ACCESS-TIMESTAMP", timestamp);
        requestMessage.Headers.TryAddWithoutValidation("ACCESS-SIGN", signature);
    }
}
