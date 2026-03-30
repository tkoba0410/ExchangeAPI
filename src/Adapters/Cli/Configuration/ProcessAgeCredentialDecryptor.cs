using ExchangeApi.Exchanges.Bitflyer.Composition.Credentials;

namespace ExchangeApi.Adapters.Cli.Configuration;

public sealed class ProcessAgeCredentialDecryptor : IAgeCredentialDecryptor
{
    public static ProcessAgeCredentialDecryptor Instance { get; } = new();

    private ProcessAgeCredentialDecryptor()
    {
    }

    public bool IsAvailable()
    {
        return AgeProcessCredentialHelper.IsAvailable();
    }

    public string Decrypt(string identityFilePath, string credentialsFilePath)
    {
        return AgeProcessCredentialHelper.Decrypt(identityFilePath, credentialsFilePath);
    }
}
