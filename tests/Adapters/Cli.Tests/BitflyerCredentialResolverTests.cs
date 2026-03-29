using ExchangeApi.Adapters.Cli.Configuration;

namespace ExchangeApi.Adapters.Cli.Tests;

public sealed class BitflyerCredentialResolverTests
{
    [Fact]
    public void Resolve_ReturnsNoneWhenAgeBackedSourceIsNotConfigured()
    {
        var environment = new FakeEnvironment();

        var result = BitflyerCredentialResolver.Resolve(environment, new FakeAgeCredentialDecryptor(false, ""));

        Assert.False(result.HasFailure);
        Assert.Null(result.Credentials);
    }

    [Fact]
    public void Resolve_LoadsCredentialsFromAgeBackedFiles()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var identityFilePath = Path.Combine(tempDirectory.FullName, "age.key");
            var credentialsFilePath = Path.Combine(tempDirectory.FullName, "credentials.enc.json");
            File.WriteAllText(identityFilePath, "identity");
            File.WriteAllText(credentialsFilePath, "ciphertext");

            var environment = new FakeEnvironment(new Dictionary<string, string?>
            {
                [BitflyerCredentialResolver.AgeIdentityFileEnvName] = identityFilePath,
                [BitflyerCredentialResolver.CredentialsAgeFileEnvName] = credentialsFilePath,
            });

            var result = BitflyerCredentialResolver.Resolve(
                environment,
                new FakeAgeCredentialDecryptor(
                    true,
                    """{"version":1,"venue":"bitflyer","apiKey":"age-key","apiSecret":"age-secret","label":"main-trade","generatedAt":"2026-03-29T10:00:00+09:00","expiresAt":"2026-06-30T00:00:00+09:00","note":"main trading key"}"""));

            Assert.False(result.HasFailure);
            Assert.NotNull(result.Credentials);
            Assert.Equal("age-key", result.Credentials!.ApiKey);
            Assert.Equal("age-secret", result.Credentials.ApiSecret);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Resolve_FailsWhenAgeSourceIsPartiallyConfigured()
    {
        var environment = new FakeEnvironment(new Dictionary<string, string?>
        {
            [BitflyerCredentialResolver.AgeIdentityFileEnvName] = "/tmp/age.key",
        });

        var result = BitflyerCredentialResolver.Resolve(environment, new FakeAgeCredentialDecryptor(false, ""));

        Assert.True(result.HasFailure);
        Assert.Contains(BitflyerCredentialResolver.AgeIdentityFileEnvName, result.ErrorMessage);
        Assert.Contains(BitflyerCredentialResolver.CredentialsAgeFileEnvName, result.ErrorMessage);
    }

    [Fact]
    public void Resolve_FailsWhenCanonicalVenueDoesNotMatch()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var identityFilePath = Path.Combine(tempDirectory.FullName, "age.key");
            var credentialsFilePath = Path.Combine(tempDirectory.FullName, "credentials.enc.json");
            File.WriteAllText(identityFilePath, "identity");
            File.WriteAllText(credentialsFilePath, "ciphertext");

            var environment = new FakeEnvironment(new Dictionary<string, string?>
            {
                [BitflyerCredentialResolver.AgeIdentityFileEnvName] = identityFilePath,
                [BitflyerCredentialResolver.CredentialsAgeFileEnvName] = credentialsFilePath,
            });

            var result = BitflyerCredentialResolver.Resolve(
                environment,
                new FakeAgeCredentialDecryptor(true, """{"version":1,"venue":"binance","apiKey":"age-key","apiSecret":"age-secret"}"""));

            Assert.True(result.HasFailure);
            Assert.Contains("venue", result.ErrorMessage);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Resolve_FailsWhenLegacyFormatIsUsed()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var identityFilePath = Path.Combine(tempDirectory.FullName, "age.key");
            var credentialsFilePath = Path.Combine(tempDirectory.FullName, "credentials.enc.json");
            File.WriteAllText(identityFilePath, "identity");
            File.WriteAllText(credentialsFilePath, "ciphertext");

            var environment = new FakeEnvironment(new Dictionary<string, string?>
            {
                [BitflyerCredentialResolver.AgeIdentityFileEnvName] = identityFilePath,
                [BitflyerCredentialResolver.CredentialsAgeFileEnvName] = credentialsFilePath,
            });

            var result = BitflyerCredentialResolver.Resolve(
                environment,
                new FakeAgeCredentialDecryptor(true, """{"bitflyer":{"apiKey":"legacy-key","apiSecret":"legacy-secret"}}"""));

            Assert.True(result.HasFailure);
            Assert.Contains("version", result.ErrorMessage);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    private sealed class FakeAgeCredentialDecryptor : IAgeCredentialDecryptor
    {
        private readonly bool _isAvailable;
        private readonly string _decryptedJson;

        public FakeAgeCredentialDecryptor(bool isAvailable, string decryptedJson)
        {
            _isAvailable = isAvailable;
            _decryptedJson = decryptedJson;
        }

        public bool IsAvailable()
        {
            return _isAvailable;
        }

        public string Decrypt(string identityFilePath, string credentialsFilePath)
        {
            return _decryptedJson;
        }
    }
}
