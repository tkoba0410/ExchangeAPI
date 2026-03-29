using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Adapters.McpServer.Schema.Market;

namespace ExchangeApi.Adapters.McpServer.Tools;

public static class ToolCatalog
{
    public static IReadOnlyList<McpToolDefinition> All { get; } =
        new McpToolDefinition[]
        {
            new(
                Name: "get_market_snapshot",
                Description: "Return market price, market status, and fixed trading rules for a supported symbol.",
                RequestType: typeof(GetMarketSnapshotRequest),
                ResponseType: typeof(GetMarketSnapshotResponse)),
            new(
                Name: "get_account_snapshot",
                Description: "Return the bot-oriented account snapshot for balances, positions, open order count, and read readiness.",
                RequestType: typeof(GetAccountSnapshotRequest),
                ResponseType: typeof(GetAccountSnapshotResponse)),
            new(
                Name: "evaluate_order",
                Description: "Evaluate whether a supported order request can be placed mechanically under current rules and balances.",
                RequestType: typeof(EvaluateOrderRequest),
                ResponseType: typeof(EvaluateOrderResponse)),
        };
}
