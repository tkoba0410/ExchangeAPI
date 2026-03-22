using ExchangeApi.Stage10.Bitflyer.Composition.Factory;
using ExchangeApi.Stage10.Bitflyer.Composition.Options;
using ExchangeApi.Tests.Stage10.Bitflyer.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Stage10.Bitflyer.LiveTests;

public sealed class Stage10LiveTestSkeletons
{
    [Fact(Skip = "Stage10 live test skeleton. Configure BITFLYER_API_KEY / BITFLYER_API_SECRET and remove Skip to run.")]
    public async Task GetTicker_PublicWireAndNormalized()
    {
        using var bundle = BitflyerStage10ClientFactory.CreateNormalizedClient(new BitflyerStage10ClientOptions
        {
            BaseUri = ResolveBaseUri(),
        });

        _ = await bundle.Wire.Public.GetTickerAsync(BitflyerStage10LiveTestSettings.DefaultProductCode);
        _ = await bundle.Public.GetTickerAsync(new ExchangeApi.Stage10.Bitflyer.Normalized.Public.Requests.GetTickerRequest
        {
            ProductCode = BitflyerStage10LiveTestSettings.DefaultProductCode,
        });
    }

    [Fact(Skip = "Stage10 live test skeleton. Configure BITFLYER_API_KEY / BITFLYER_API_SECRET and remove Skip to run.")]
    public async Task GetBalance_And_SendChildOrder_PrivateNormalized()
    {
        using var bundle = BitflyerStage10ClientFactory.CreateNormalizedClient(new BitflyerStage10ClientOptions
        {
            BaseUri = ResolveBaseUri(),
            Credentials = new BitflyerApiCredentials
            {
                ApiKey = Environment.GetEnvironmentVariable(BitflyerStage10LiveTestSettings.ApiKeyEnvironmentVariable) ?? string.Empty,
                ApiSecret = Environment.GetEnvironmentVariable(BitflyerStage10LiveTestSettings.ApiSecretEnvironmentVariable) ?? string.Empty,
            },
        });

        _ = await bundle.Private!.GetBalanceAsync(new ExchangeApi.Stage10.Bitflyer.Normalized.Private.Requests.GetBalanceRequest());
        _ = await bundle.Private.SendChildOrderAsync(new ExchangeApi.Stage10.Bitflyer.Normalized.Private.Requests.SendChildOrderRequest
        {
            ProductCode = BitflyerStage10LiveTestSettings.DefaultProductCode,
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.001m,
            Price = 1m,
        });
    }

    private static Uri? ResolveBaseUri()
    {
        var configured = Environment.GetEnvironmentVariable(BitflyerStage10LiveTestSettings.ApiBaseUriEnvironmentVariable);
        return string.IsNullOrWhiteSpace(configured) ? null : new Uri(configured);
    }
}
