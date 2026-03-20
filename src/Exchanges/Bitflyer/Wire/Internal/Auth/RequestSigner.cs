using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Internal.Auth;

public sealed class RequestSigner : IRequestSigner
{
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly IExchangeClock _clock;

    public RequestSigner(string apiKey, string apiSecret, IExchangeClock clock)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _apiSecret = apiSecret ?? throw new ArgumentNullException(nameof(apiSecret));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task SignAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (request.RequestUri is null) throw new InvalidOperationException("RequestUri must be set before signing.");

        var timestamp = _clock.UtcNow.ToUnixTimeMilliseconds().ToString();
        var method = request.Method.Method.ToUpperInvariant();
        var pathAndQuery = request.RequestUri.PathAndQuery;

        var body = request.Content is null
            ? string.Empty
            : await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var canonical = $"{timestamp}{method}{pathAndQuery}{body}";

        var keyBytes = Encoding.UTF8.GetBytes(_apiSecret);
        var textBytes = Encoding.UTF8.GetBytes(canonical);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(textBytes);
        var sign = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        request.Headers.Add(AuthKeys.AccessKey, _apiKey);
        request.Headers.Add(AuthKeys.AccessTimestamp, timestamp);
        request.Headers.Add(AuthKeys.AccessSign, sign);
        request.Headers.TryAddWithoutValidation(AuthKeys.ContentType, AuthKeys.JsonContentType);
    }
}
