namespace ExchangeApi.Primitives.Credentials;

public interface IApiCredentialProvider
{
    ValueTask<IApiCredentialSession> OpenSessionAsync(
        CancellationToken cancellationToken = default);
}
