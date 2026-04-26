namespace ExchangeApi.Primitives.Credentials;

public interface IApiCredentialSession : IAsyncDisposable
{
    string ApiKey { get; }

    string Sign(string payload);
}
