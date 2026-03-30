using System.Text.Json;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Adapters.McpServer.Schema.Klines;
using ExchangeApi.Adapters.McpServer.Schema.Market;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Tools;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class ToolCatalogTests
{
    [Fact]
    public void All_ContainsTheDocumentedToolSurface()
    {
        var tools = ToolCatalog.All;

        Assert.Equal(4, tools.Count);
        Assert.Equal("get_market_snapshot", tools[0].Name);
        Assert.Equal(typeof(GetMarketSnapshotRequest), tools[0].RequestType);
        Assert.Equal(typeof(GetMarketSnapshotResponse), tools[0].ResponseType);
        Assert.NotNull(tools[0].OutputSchemaJson);
        Assert.False(tools[0].RequiresCredentials);
        Assert.Equal("get_klines", tools[1].Name);
        Assert.Equal(typeof(GetKlinesRequest), tools[1].RequestType);
        Assert.Equal(typeof(GetKlinesResponse), tools[1].ResponseType);
        Assert.NotNull(tools[1].OutputSchemaJson);
        Assert.False(tools[1].RequiresCredentials);
        Assert.Equal("get_account_snapshot", tools[2].Name);
        Assert.Equal(typeof(GetAccountSnapshotRequest), tools[2].RequestType);
        Assert.Equal(typeof(GetAccountSnapshotResponse), tools[2].ResponseType);
        Assert.NotNull(tools[2].OutputSchemaJson);
        Assert.True(tools[2].RequiresCredentials);
        Assert.Equal("evaluate_order", tools[3].Name);
        Assert.Equal(typeof(EvaluateOrderRequest), tools[3].RequestType);
        Assert.Equal(typeof(EvaluateOrderResponse), tools[3].ResponseType);
        Assert.NotNull(tools[3].OutputSchemaJson);
        Assert.True(tools[3].RequiresCredentials);
    }

    [Fact]
    public void PublicOnly_ContainsOnlyThePublicReadTools()
    {
        var tools = ToolCatalog.PublicOnly;

        Assert.Equal(["get_market_snapshot", "get_klines"], tools.Select(tool => tool.Name).ToArray());
    }

    [Fact]
    public void GetKlines_InputSchema_ExposesVenueAndClosedSets()
    {
        using var document = JsonDocument.Parse(ToolCatalog.GetKlines.InputSchemaJson);
        var root = document.RootElement;
        var required = root.GetProperty("required").EnumerateArray().Select(x => x.GetString()!).ToArray();

        Assert.Equal(["venue", "symbol", "interval"], required);

        var properties = root.GetProperty("properties");
        var venues = properties.GetProperty("venue").GetProperty("enum").EnumerateArray().Select(x => x.GetString()!).ToArray();
        var symbols = properties.GetProperty("symbol").GetProperty("enum").EnumerateArray().Select(x => x.GetString()!).ToArray();
        var intervals = properties.GetProperty("interval").GetProperty("enum").EnumerateArray().Select(x => x.GetString()!).ToArray();

        Assert.Equal(["binance"], venues);
        Assert.Equal(BinanceKlineSymbolSet.Entries.OrderBy(x => x).ToArray(), symbols);
        Assert.Contains("1h", intervals);
    }

    [Fact]
    public void GetMarketSnapshot_OutputSchema_ExposesRuleSourceKinds()
    {
        using var document = JsonDocument.Parse(ToolCatalog.GetMarketSnapshot.OutputSchemaJson!);
        var rules = document.RootElement.GetProperty("properties").GetProperty("rules");
        var required = rules.GetProperty("required").EnumerateArray().Select(x => x.GetString()!).ToArray();
        var sourceKinds = rules.GetProperty("properties").GetProperty("priceStepSourceKind").GetProperty("enum").EnumerateArray().Select(x => x.GetString()!).ToArray();

        Assert.Contains("minSizeSourceKind", required);
        Assert.Equal(
            [MarketRuleSourceKinds.OfficialDocumented, MarketRuleSourceKinds.OfficialApiContract, MarketRuleSourceKinds.AdapterInferred, MarketRuleSourceKinds.PinnedOperational],
            sourceKinds);
    }
}
