using ExchangeApi.Tests.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

internal sealed class BitflyerPublicReadLiveFactAttribute : FactAttribute
{
}

internal sealed class BitflyerPrivateReadLiveFactAttribute : FactAttribute
{
    public BitflyerPrivateReadLiveFactAttribute()
    {
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
        if (!BitflyerCredentialResolver.HasConfiguredCredentialsSource())
        {
            Skip = "Set EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH and EXCHANGEAPI_AGE_IDENTITY_FILE_PATH to run Stage10 write live tests.";
            return;
        }

        if (!BitflyerLiveTestPolicy.HasWriteOptInMarker())
        {
            Skip = "Create local/bitflyer-live-write-enabled to run Stage10 write live tests.";
        }
    }
}

internal static class BitflyerLiveTestPolicy
{
    public static bool HasWriteOptInMarker()
    {
        return LiveTestLocalPolicy.HasMarker("bitflyer-live-write-enabled");
    }
}
