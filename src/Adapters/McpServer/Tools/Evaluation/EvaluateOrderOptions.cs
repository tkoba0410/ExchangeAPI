namespace ExchangeApi.Adapters.McpServer.Tools.Evaluation;

public sealed class EvaluateOrderOptions
{
    public decimal? MaxBaseSize { get; init; }

    public decimal? MarketFeeRate { get; init; }

    public decimal? LimitFeeRate { get; init; }
}
