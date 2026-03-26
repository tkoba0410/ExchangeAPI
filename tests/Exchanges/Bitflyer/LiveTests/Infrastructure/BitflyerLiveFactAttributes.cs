using ExchangeApi.Tests.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

internal sealed class BitflyerPublicReadLiveFactAttribute : FactAttribute
{
    public BitflyerPublicReadLiveFactAttribute()
    {
        Skip = BitflyerLiveTestPolicy.GetPublicReadSkipReason();
    }
}

internal sealed class BitflyerPrivateReadLiveFactAttribute : FactAttribute
{
    public BitflyerPrivateReadLiveFactAttribute()
    {
        Skip = BitflyerLiveTestPolicy.GetPrivateReadSkipReason();
    }
}

internal sealed class BitflyerWriteLiveFactAttribute : FactAttribute
{
    public BitflyerWriteLiveFactAttribute()
    {
        Skip = BitflyerLiveTestPolicy.GetWriteSkipReason();
    }
}

internal sealed class BitflyerCancelAllWriteLiveFactAttribute : FactAttribute
{
    public BitflyerCancelAllWriteLiveFactAttribute()
    {
        Skip = BitflyerLiveTestPolicy.GetCancelAllWriteSkipReason();
    }
}

internal sealed class BitflyerWithdrawNegativeLiveFactAttribute : FactAttribute
{
    public BitflyerWithdrawNegativeLiveFactAttribute()
    {
        Skip = BitflyerLiveTestPolicy.GetWithdrawNegativeSkipReason();
    }
}

internal static class BitflyerLiveTestPolicy
{
    public static string? GetPublicReadSkipReason()
    {
        return GlobalLiveOptInSkipReason("bitFlyer public live tests");
    }

    public static string? GetPrivateReadSkipReason()
    {
        var skip = GlobalLiveOptInSkipReason("bitFlyer private live tests");
        if (skip is not null)
        {
            return skip;
        }

        if (!BitflyerCredentialResolver.HasConfiguredCredentialsSource())
        {
            return "Set EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH and EXCHANGEAPI_AGE_IDENTITY_FILE_PATH to run private live tests.";
        }

        return null;
    }

    public static string? GetWriteSkipReason()
    {
        var skip = GlobalLiveOptInSkipReason("bitFlyer write live tests");
        if (skip is not null)
        {
            return skip;
        }

        if (!BitflyerCredentialResolver.HasConfiguredCredentialsSource())
        {
            return "Set EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH and EXCHANGEAPI_AGE_IDENTITY_FILE_PATH to run write live tests.";
        }

        if (!HasWriteOptInMarker())
        {
            return "Create local/bitflyer-live-write-enabled to run write live tests.";
        }

        return null;
    }

    public static string? GetCancelAllWriteSkipReason()
    {
        var skip = GetWriteSkipReason();
        if (skip is not null)
        {
            return skip;
        }

        if (!HasCancelAllWriteOptInMarker())
        {
            return "Create local/bitflyer-live-cancel-all-enabled to run CancelAllChildOrders live tests.";
        }

        return null;
    }

    public static string? GetWithdrawNegativeSkipReason()
    {
        var skip = GetWriteSkipReason();
        if (skip is not null)
        {
            return skip;
        }

        if (!HasWithdrawNegativeOptInMarker())
        {
            return "Create local/bitflyer-live-withdraw-negative-enabled to run Withdraw negative live tests.";
        }

        return null;
    }

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

    private static string? GlobalLiveOptInSkipReason(string target)
    {
        if (!LiveTestLocalPolicy.HasLiveOptIn())
        {
            return LiveTestLocalPolicy.LiveOptInMessage(target);
        }

        return null;
    }
}
