namespace ExchangeApi.Adapters.Cli.Configuration;

public interface IAgeCredentialDecryptor
{
    bool IsAvailable();

    string Decrypt(string identityFilePath, string credentialsFilePath);
}
