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
        Assert.NotNull(tools[0].OutputSchemaJson);
        Assert.False(tools[0].RequiresCredentials);
        Assert.Equal("get_account_snapshot", tools[1].Name);
        Assert.Equal(typeof(GetAccountSnapshotRequest), tools[1].RequestType);
        Assert.Equal(typeof(GetAccountSnapshotResponse), tools[1].ResponseType);
        Assert.NotNull(tools[1].OutputSchemaJson);
        Assert.True(tools[1].RequiresCredentials);
        Assert.Equal("evaluate_order", tools[2].Name);
        Assert.Equal(typeof(EvaluateOrderRequest), tools[2].RequestType);
        Assert.Equal(typeof(EvaluateOrderResponse), tools[2].ResponseType);
        Assert.NotNull(tools[2].OutputSchemaJson);
        Assert.True(tools[2].RequiresCredentials);
    }

    [Fact]
    public void PublicOnly_ContainsOnlyThePublicReadTool()
    {
        var tools = ToolCatalog.PublicOnly;

        var only = Assert.Single(tools);
        Assert.Equal("get_market_snapshot", only.Name);
    }
}
