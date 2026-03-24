namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

internal sealed class BitflyerPublicReadLiveFactAttribute : FactAttribute
{
    public BitflyerPublicReadLiveFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("BITFLYER_STAGE10_LIVE") != "1")
        {
            Skip = "Set BITFLYER_STAGE10_LIVE=1 to run Stage10 public live tests.";
        }
    }
}

internal sealed class BitflyerPrivateReadLiveFactAttribute : FactAttribute
{
    public BitflyerPrivateReadLiveFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("BITFLYER_STAGE10_LIVE") != "1")
        {
            Skip = "Set BITFLYER_STAGE10_LIVE=1 to run Stage10 private live tests.";
            return;
        }

        if (!BitflyerCredentialResolver.HasConfiguredCredentialsSource())
        {
            Skip = "Set EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH and EXCHANGEAPI_AGE_IDENTITY_FILE_PATH to run Stage10 private live tests.";
        }
    }
}

internal sealed class BitflyerWriteLiveFactAttribute : FactAttribute
{
    public BitflyerWriteLiveFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("BITFLYER_STAGE10_LIVE") != "1" ||
            Environment.GetEnvironmentVariable("BITFLYER_STAGE10_ALLOW_WRITE") != "1")
        {
            Skip = "Set BITFLYER_STAGE10_LIVE=1 and BITFLYER_STAGE10_ALLOW_WRITE=1 to run Stage10 write live tests.";
            return;
        }

        if (!BitflyerCredentialResolver.HasConfiguredCredentialsSource())
        {
            Skip = "Set EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH and EXCHANGEAPI_AGE_IDENTITY_FILE_PATH to run Stage10 write live tests.";
        }
    }
}
