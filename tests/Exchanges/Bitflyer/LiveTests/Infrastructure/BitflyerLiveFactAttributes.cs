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
            Skip = "Set EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH and EXCHANGEAPI_AGE_IDENTITY_FILE_PATH to run private live tests.";
        }
    }
}

internal sealed class BitflyerWriteLiveFactAttribute : FactAttribute
{
    public BitflyerWriteLiveFactAttribute()
    {
        if (!BitflyerCredentialResolver.HasConfiguredCredentialsSource())
        {
            Skip = "Set EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH and EXCHANGEAPI_AGE_IDENTITY_FILE_PATH to run write live tests.";
            return;
        }

        if (!BitflyerLiveTestPolicy.HasWriteOptInMarker())
        {
            Skip = "Create local/bitflyer-live-write-enabled to run write live tests.";
        }
    }
}

internal sealed class BitflyerCancelAllWriteLiveFactAttribute : FactAttribute
{
    public BitflyerCancelAllWriteLiveFactAttribute()
    {
        if (!BitflyerCredentialResolver.HasConfiguredCredentialsSource())
        {
            Skip = "Set EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH and EXCHANGEAPI_AGE_IDENTITY_FILE_PATH to run write live tests.";
            return;
        }

        if (!BitflyerLiveTestPolicy.HasWriteOptInMarker())
        {
            Skip = "Create local/bitflyer-live-write-enabled to run write live tests.";
            return;
        }

        if (!BitflyerLiveTestPolicy.HasCancelAllWriteOptInMarker())
        {
            Skip = "Create local/bitflyer-live-cancel-all-enabled to run CancelAllChildOrders live tests.";
        }
    }
}

internal sealed class BitflyerWithdrawNegativeLiveFactAttribute : FactAttribute
{
    public BitflyerWithdrawNegativeLiveFactAttribute()
    {
        if (!BitflyerCredentialResolver.HasConfiguredCredentialsSource())
        {
            Skip = "Set EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH and EXCHANGEAPI_AGE_IDENTITY_FILE_PATH to run withdraw negative live tests.";
            return;
        }

        if (!BitflyerLiveTestPolicy.HasWriteOptInMarker())
        {
            Skip = "Create local/bitflyer-live-write-enabled to run write live tests.";
            return;
        }

        if (!BitflyerLiveTestPolicy.HasWithdrawNegativeOptInMarker())
        {
            Skip = "Create local/bitflyer-live-withdraw-negative-enabled to run Withdraw negative live tests.";
        }
    }
}

internal static class BitflyerLiveTestPolicy
{
    public static bool HasWriteOptInMarker()
    {
        return LiveTestLocalPolicy.HasMarker("bitflyer-live-write-enabled");
    }

    public static bool HasCancelAllWriteOptInMarker()
    {
        return LiveTestLocalPolicy.HasMarker("bitflyer-live-cancel-all-enabled");
    }

    public static bool HasWithdrawNegativeOptInMarker()
    {
        return LiveTestLocalPolicy.HasMarker("bitflyer-live-withdraw-negative-enabled");
    }
}
