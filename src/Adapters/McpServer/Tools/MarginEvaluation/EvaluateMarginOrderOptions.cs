namespace ExchangeApi.Adapters.McpServer.Tools.MarginEvaluation;

public sealed class EvaluateMarginOrderOptions
{
    public decimal? MaxBaseSize { get; init; }

    public decimal? MarketFeeRate { get; init; }

    public decimal? LimitFeeRate { get; init; }
}
