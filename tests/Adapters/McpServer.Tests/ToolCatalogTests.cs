using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Adapters.McpServer.Schema.Market;
using ExchangeApi.Adapters.McpServer.Tools;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class ToolCatalogTests
{
    [Fact]
    public void All_ContainsTheDocumentedToolSurface()
    {
        var tools = ToolCatalog.All;

        Assert.Equal(3, tools.Count);
        Assert.Equal("get_market_snapshot", tools[0].Name);
        Assert.Equal(typeof(GetMarketSnapshotRequest), tools[0].RequestType);
        Assert.Equal(typeof(GetMarketSnapshotResponse), tools[0].ResponseType);
        Assert.Equal("get_account_snapshot", tools[1].Name);
        Assert.Equal(typeof(GetAccountSnapshotRequest), tools[1].RequestType);
        Assert.Equal(typeof(GetAccountSnapshotResponse), tools[1].ResponseType);
        Assert.Equal("evaluate_order", tools[2].Name);
        Assert.Equal(typeof(EvaluateOrderRequest), tools[2].RequestType);
        Assert.Equal(typeof(EvaluateOrderResponse), tools[2].ResponseType);
    }
}
