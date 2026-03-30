namespace ExchangeApi.Adapters.McpServer.Schema.Evaluation;

public static class EvaluateOrderWarningCodes
{
    public const string MarketOrderSlippageRisk = "market_order_slippage_risk";

    public static IReadOnlyList<string> All { get; } =
        [MarketOrderSlippageRisk];
}
