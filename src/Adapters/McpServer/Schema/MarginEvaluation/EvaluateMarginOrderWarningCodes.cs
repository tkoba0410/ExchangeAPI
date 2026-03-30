namespace ExchangeApi.Adapters.McpServer.Schema.MarginEvaluation;

public static class EvaluateMarginOrderWarningCodes
{
    public const string EstimatedFeeNotCovered = "estimated_fee_not_covered";
    public const string MarketOrderSlippageRisk = "market_order_slippage_risk";

    public static IReadOnlyList<string> All { get; } =
        [EstimatedFeeNotCovered, MarketOrderSlippageRisk];
}
