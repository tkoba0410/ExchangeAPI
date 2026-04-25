using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Optional.Credentials.PlainText;

public sealed class BitflyerPlainTextApiCredentialProvider : IApiCredentialProvider
{
    private readonly string _apiKey;
    private readonly string _apiSecret;

    public BitflyerPlainTextApiCredentialProvider(string apiKey, string apiSecret)
    {
        _apiKey = apiKey;
        _apiSecret = apiSecret;
    }

    public ValueTask<IApiCredentialSession> OpenSessionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult<IApiCredentialSession>(new PlainTextApiCredentialSession(_apiKey, _apiSecret));
    }
}
