namespace ExchangeApi.Tests.Stage10.Bitflyer.LiveTests.Infrastructure;

internal sealed class BitflyerStage10LivePublicFactAttribute : FactAttribute
{
    public BitflyerStage10LivePublicFactAttribute()
    {
        Skip = BitflyerStage10LiveTestSettings.GetPublicSkipReason();
    }
}

internal sealed class BitflyerStage10LivePrivateFactAttribute : FactAttribute
{
    public BitflyerStage10LivePrivateFactAttribute()
    {
        Skip = BitflyerStage10LiveTestSettings.GetPrivateSkipReason();
    }
}
