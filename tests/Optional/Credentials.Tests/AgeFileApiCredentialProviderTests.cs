using ExchangeApi.Optional.Credentials;
using ExchangeApi.Optional.Credentials.AgeFile;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Tests.Optional.Credentials.Tests;

public sealed class AgeFileApiCredentialProviderTests
{
    [Theory]
    [InlineData(ExchangeVenue.Bitflyer, "bitflyer")]
    [InlineData(ExchangeVenue.Binance, "binance")]
    public async Task OpenSessionAsync_DecryptsCanonicalCredentialJson(
        ExchangeVenue venue,
        string canonicalVenue)
    {
        using var tempFiles = TempCredentialFiles.Create();
        var provider = AgeFileApiCredentialProviderFactory.Create(
            venue,
            tempFiles.IdentityFilePath,
            tempFiles.CredentialsFilePath,
            new FakeAgeCredentialFileDecryptor(
                $$"""{"version":1,"venue":"{{canonicalVenue}}","apiKey":"age-key","apiSecret":"age-secret"}"""));

        await using var session = await provider.OpenSessionAsync();

        Assert.Equal("age-key", session.ApiKey);
        Assert.Equal("9e8aea03151a4229aafbadc8aa33f32870ef4d4cf31cc17d47de714b0943e586", session.Sign("payload"));
    }

    [Fact]
    public async Task OpenSessionAsync_FailsWhenVenueDoesNotMatch()
    {
        using var tempFiles = TempCredentialFiles.Create();
        var provider = new BitflyerAgeFileApiCredentialProvider(
            tempFiles.IdentityFilePath,
            tempFiles.CredentialsFilePath,
            new FakeAgeCredentialFileDecryptor(
                """{"version":1,"venue":"binance","apiKey":"age-key","apiSecret":"age-secret"}"""));

        var ex = await Assert.ThrowsAsync<ApiCredentialException>(async () => await provider.OpenSessionAsync());

        Assert.Equal(ApiCredentialErrorKind.VenueMismatch, ex.Kind);
    }

    [Theory]
    [InlineData("""{"version":2,"venue":"bitflyer","apiKey":"age-key","apiSecret":"age-secret"}""", ApiCredentialErrorKind.UnsupportedVersion)]
    [InlineData("""{"version":1,"venue":"bitflyer","apiSecret":"age-secret"}""", ApiCredentialErrorKind.MissingRequiredField)]
    [InlineData("""{"version":1,"venue":"bitflyer","apiKey":" age-key","apiSecret":"age-secret"}""", ApiCredentialErrorKind.InvalidApiKey)]
    [InlineData("""{"version":1,"venue":"bitflyer","apiKey":"age-key","apiSecret":" "}""", ApiCredentialErrorKind.InvalidApiSecret)]
    [InlineData("""not-json""", ApiCredentialErrorKind.JsonParseFailed)]
    public async Task OpenSessionAsync_MapsCredentialJsonFailures(
        string decryptedJson,
        ApiCredentialErrorKind expectedKind)
    {
        using var tempFiles = TempCredentialFiles.Create();
        var provider = new BitflyerAgeFileApiCredentialProvider(
            tempFiles.IdentityFilePath,
            tempFiles.CredentialsFilePath,
            new FakeAgeCredentialFileDecryptor(decryptedJson));

        var ex = await Assert.ThrowsAsync<ApiCredentialException>(async () => await provider.OpenSessionAsync());

        Assert.Equal(expectedKind, ex.Kind);
    }

    [Fact]
    public async Task OpenSessionAsync_FailsWhenCredentialFileIsMissing()
    {
        using var tempFiles = TempCredentialFiles.Create();
        File.Delete(tempFiles.CredentialsFilePath);
        var provider = new BitflyerAgeFileApiCredentialProvider(
            tempFiles.IdentityFilePath,
            tempFiles.CredentialsFilePath,
            new FakeAgeCredentialFileDecryptor("{}"));

        var ex = await Assert.ThrowsAsync<ApiCredentialException>(async () => await provider.OpenSessionAsync());

        Assert.Equal(ApiCredentialErrorKind.SourceUnavailable, ex.Kind);
    }

    private sealed class FakeAgeCredentialFileDecryptor : IAgeCredentialFileDecryptor
    {
        private readonly string _decryptedJson;

        public FakeAgeCredentialFileDecryptor(string decryptedJson)
        {
            _decryptedJson = decryptedJson;
        }

        public string Decrypt(string identityFilePath, string credentialsFilePath)
        {
            return _decryptedJson;
        }
    }

    private sealed class TempCredentialFiles : IDisposable
    {
        private readonly string _directoryPath;

        private TempCredentialFiles(string directoryPath, string identityFilePath, string credentialsFilePath)
        {
            _directoryPath = directoryPath;
            IdentityFilePath = identityFilePath;
            CredentialsFilePath = credentialsFilePath;
        }

        public string IdentityFilePath { get; }

        public string CredentialsFilePath { get; }

        public static TempCredentialFiles Create()
        {
            var directoryPath = Directory.CreateTempSubdirectory().FullName;
            var identityFilePath = Path.Combine(directoryPath, "identity.txt");
            var credentialsFilePath = Path.Combine(directoryPath, "credentials.age");
            File.WriteAllText(identityFilePath, "identity");
            File.WriteAllText(credentialsFilePath, "ciphertext");
            return new TempCredentialFiles(directoryPath, identityFilePath, credentialsFilePath);
        }

        public void Dispose()
        {
            Directory.Delete(_directoryPath, recursive: true);
        }
    }
}
