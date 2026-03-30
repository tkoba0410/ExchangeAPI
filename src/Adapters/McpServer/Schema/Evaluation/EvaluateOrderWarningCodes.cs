namespace ExchangeApi.Adapters.McpServer.Schema.Evaluation;

public static class EvaluateOrderWarningCodes
{
    public const string MarketOrderSlippageRisk = "market_order_slippage_risk";
    public const string EstimatedFeeNotCovered = "estimated_fee_not_covered";

    public static IReadOnlyList<string> All { get; } =
        [EstimatedFeeNotCovered, MarketOrderSlippageRisk];
}
