namespace ExchangeApi.Tests.Exchanges.Binance.LiveTests.Infrastructure;

internal sealed class BinancePublicReadLiveFactAttribute : FactAttribute
{
    public BinancePublicReadLiveFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("BINANCE_STAGE10_LIVE") != "1")
        {
            Skip = "Set BINANCE_STAGE10_LIVE=1 to run Binance Stage10 public live tests.";
        }
    }
}
