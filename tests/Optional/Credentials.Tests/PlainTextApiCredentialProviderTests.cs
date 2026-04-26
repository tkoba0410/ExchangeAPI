using ExchangeApi.Optional.Credentials;
using ExchangeApi.Optional.Credentials.PlainText;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Tests.Optional.Credentials.Tests;

public sealed class PlainTextApiCredentialProviderTests
{
    [Theory]
    [InlineData(ExchangeVenue.Bitflyer)]
    [InlineData(ExchangeVenue.Binance)]
    public async Task OpenSessionAsync_ExposesApiKeyAndSignsPayload(ExchangeVenue venue)
    {
        var provider = PlainTextApiCredentialProviderFactory.Create(venue, "api-key", "api-secret");

        await using var session = await provider.OpenSessionAsync();

        Assert.Equal("api-key", session.ApiKey);
        Assert.Equal("3a4fcb53468ee879669dc72e2d75cc3bc55abebecf44fdb8341fde712020a1db", session.Sign("payload"));
    }

    [Theory]
    [InlineData("", "api-secret", ApiCredentialErrorKind.InvalidApiKey)]
    [InlineData(" api-key", "api-secret", ApiCredentialErrorKind.InvalidApiKey)]
    [InlineData("api-key", "", ApiCredentialErrorKind.InvalidApiSecret)]
    [InlineData("api-key", "api-secret ", ApiCredentialErrorKind.InvalidApiSecret)]
    public async Task OpenSessionAsync_RejectsInvalidPlainTextValues(
        string apiKey,
        string apiSecret,
        ApiCredentialErrorKind expectedKind)
    {
        var provider = new BitflyerPlainTextApiCredentialProvider(apiKey, apiSecret);

        var ex = await Assert.ThrowsAsync<ApiCredentialException>(async () => await provider.OpenSessionAsync());

        Assert.Equal(expectedKind, ex.Kind);
    }
}
