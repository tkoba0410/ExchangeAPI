using ExchangeApi.Tests.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Exchanges.Binance.LiveTests.Infrastructure;

internal sealed class BinancePublicReadLiveFactAttribute : FactAttribute
{
    public BinancePublicReadLiveFactAttribute()
    {
        if (!LiveTestLocalPolicy.HasLiveOptIn())
        {
            Skip = LiveTestLocalPolicy.LiveOptInMessage("Binance public live tests");
        }
    }
}
