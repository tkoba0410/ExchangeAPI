using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Optional.Credentials.PlainText;

public sealed class BinancePlainTextApiCredentialProvider : IApiCredentialProvider
{
    private readonly string _apiKey;
    private readonly string _apiSecret;

    public BinancePlainTextApiCredentialProvider(string apiKey, string apiSecret)
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
