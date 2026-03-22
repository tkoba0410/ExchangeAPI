using ExchangeApi.Stage10.Bitflyer.Vocabulary;

namespace ExchangeApi.Tests.Stage10.Bitflyer.LiveTests.Infrastructure;

internal static class BitflyerStage10LiveTestSettings
{
    public const string ApiBaseUriEnvironmentVariable = "BITFLYER_API_BASE_URI";
    public const string ApiKeyEnvironmentVariable = "BITFLYER_API_KEY";
    public const string ApiSecretEnvironmentVariable = "BITFLYER_API_SECRET";
    public const string DefaultProductCode = ProductCodes.Default;
}
