using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Adapters.McpServer.Mapping;

public static class BitflyerMarketStatusMapper
{
    public static string Map(BitflyerTradingState state, BitflyerHealthStatus health)
    {
        if (state is TradingStates.Closed or TradingStates.Matured
            || health is HealthStatuses.NoOrder or HealthStatuses.Stop)
        {
            return "halted";
        }

        if (state is TradingStates.Running
            && health is HealthStatuses.Normal or HealthStatuses.Busy)
        {
            return "active";
        }

        if (state is TradingStates.Starting or TradingStates.Preopen or TradingStates.CircuitBreak
            || health is HealthStatuses.VeryBusy or HealthStatuses.SuperBusy)
        {
            return "restricted";
        }

        return "unknown";
    }
}
