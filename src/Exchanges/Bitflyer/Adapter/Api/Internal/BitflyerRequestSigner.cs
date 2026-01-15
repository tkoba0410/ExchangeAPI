using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;

internal sealed class BitflyerRequestSigner : IRequestSigner
{
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
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (request.RequestUri is null) throw new InvalidOperationException("RequestUri must be set before signing.");

        var timestamp = _clock.UtcNow.ToUnixTimeSeconds().ToString();
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

        request.Headers.Add("ACCESS-KEY", _apiKey);
        request.Headers.Add("ACCESS-TIMESTAMP", timestamp);
        request.Headers.Add("ACCESS-SIGN", sign);
        request.Headers.TryAddWithoutValidation("Content-Type", "application/json");
    }
}
