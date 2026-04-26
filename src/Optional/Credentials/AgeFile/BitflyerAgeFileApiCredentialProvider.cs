using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Optional.Credentials.AgeFile;

public sealed class BitflyerAgeFileApiCredentialProvider : IApiCredentialProvider
{
    private readonly string _identityFilePath;
    private readonly string _credentialsFilePath;
    private readonly IAgeCredentialFileDecryptor _decryptor;

    public BitflyerAgeFileApiCredentialProvider(
        string identityFilePath,
        string credentialsFilePath,
        IAgeCredentialFileDecryptor decryptor)
    {
        _identityFilePath = identityFilePath;
        _credentialsFilePath = credentialsFilePath;
        _decryptor = decryptor;
    }

    public ValueTask<IApiCredentialSession> OpenSessionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ValidateConfigured(_identityFilePath, "identityFilePath");
        ValidateConfigured(_credentialsFilePath, "credentialsFilePath");
        var decryptedJson = _decryptor.Decrypt(_identityFilePath, _credentialsFilePath);
        return ValueTask.FromResult(AgeFileCredentialJsonParser.ParseSession(decryptedJson, "bitflyer"));
    }

    private static void ValidateConfigured(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ApiCredentialException(ApiCredentialErrorKind.NotConfigured, $"{name} is not configured.");
        }

        if (!File.Exists(value))
        {
            throw new ApiCredentialException(ApiCredentialErrorKind.SourceUnavailable, $"{name} is unavailable.");
        }
    }
}
