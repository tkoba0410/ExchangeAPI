namespace ExchangeApi.Optional.Credentials.AgeFile;

public interface IAgeCredentialFileDecryptor
{
    string Decrypt(string identityFilePath, string credentialsFilePath);
}
