using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Auth;

public sealed class BitflyerRequestSigner : IRequestSigner
{
    private const string AccessKeyHeader = "ACCESS-KEY";
    private const string AccessTimestampHeader = "ACCESS-TIMESTAMP";
    private const string AccessSignHeader = "ACCESS-SIGN";
    private const string JsonContentType = "application/json";

    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly IExchangeClock _clock;

    public BitflyerRequestSigner(string apiKey, string apiSecret, IExchangeClock clock)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _apiSecret = apiSecret ?? throw new ArgumentNullException(nameof(apiSecret));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task SignAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.RequestUri is null)
        {
            throw new InvalidOperationException("RequestUri must be set before signing.");
        }

        var timestamp = _clock.UtcNow.ToUnixTimeMilliseconds().ToString();
        var method = request.Method.Method.ToUpperInvariant();
        var pathAndQuery = request.RequestUri.PathAndQuery;
        var body = request.Content is null
            ? string.Empty
            : await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var canonical = $"{timestamp}{method}{pathAndQuery}{body}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret));
        var signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(canonical));
        var signatureText = Convert.ToHexString(signature).ToLowerInvariant();

        request.Headers.Add(AccessKeyHeader, _apiKey);
        request.Headers.Add(AccessTimestampHeader, timestamp);
        request.Headers.Add(AccessSignHeader, signatureText);

        if (request.Content is not null)
        {
            request.Content.Headers.ContentType ??=
                new System.Net.Http.Headers.MediaTypeHeaderValue(JsonContentType);
        }
    }
}
