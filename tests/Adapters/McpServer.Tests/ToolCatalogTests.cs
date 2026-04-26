using System.Text.Json;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Adapters.McpServer.Schema.Inspection;
using ExchangeApi.Adapters.McpServer.Schema.Klines;
using ExchangeApi.Adapters.McpServer.Schema.MarginEvaluation;
using ExchangeApi.Adapters.McpServer.Schema.Market;
using ExchangeApi.Adapters.McpServer.Tools;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class ToolCatalogTests
{
    [Fact]
    public void All_ContainsTheDocumentedToolSurface()
    {
        var tools = ToolCatalog.All;

        Assert.Equal(10, tools.Count);
        Assert.Equal("get_market_snapshot", tools[0].Name);
        Assert.Equal(typeof(GetMarketSnapshotRequest), tools[0].RequestType);
        Assert.Equal(typeof(GetMarketSnapshotResponse), tools[0].ResponseType);
        Assert.NotNull(tools[0].OutputSchemaJson);
        Assert.False(tools[0].RequiresCredentials);
        Assert.Equal("list_markets", tools[1].Name);
        Assert.Equal(typeof(ListMarketsRequest), tools[1].RequestType);
        Assert.Equal(typeof(ListMarketsResponse), tools[1].ResponseType);
        Assert.NotNull(tools[1].OutputSchemaJson);
        Assert.False(tools[1].RequiresCredentials);
        Assert.Equal("get_klines", tools[2].Name);
        Assert.Equal(typeof(GetKlinesRequest), tools[2].RequestType);
        Assert.Equal(typeof(GetKlinesResponse), tools[2].ResponseType);
        Assert.NotNull(tools[2].OutputSchemaJson);
        Assert.False(tools[2].RequiresCredentials);
        Assert.Equal("get_account_snapshot", tools[3].Name);
        Assert.Equal(typeof(GetAccountSnapshotRequest), tools[3].RequestType);
        Assert.Equal(typeof(GetAccountSnapshotResponse), tools[3].ResponseType);
        Assert.NotNull(tools[3].OutputSchemaJson);
        Assert.True(tools[3].RequiresCredentials);
        Assert.Equal("get_collateral_accounts", tools[4].Name);
        Assert.Equal(typeof(GetCollateralAccountsRequest), tools[4].RequestType);
        Assert.Equal(typeof(GetCollateralAccountsResponse), tools[4].ResponseType);
        Assert.NotNull(tools[4].OutputSchemaJson);
        Assert.True(tools[4].RequiresCredentials);
        Assert.Equal("get_balance_history", tools[5].Name);
        Assert.Equal(typeof(GetBalanceHistoryRequest), tools[5].RequestType);
        Assert.Equal(typeof(GetBalanceHistoryResponse), tools[5].ResponseType);
        Assert.NotNull(tools[5].OutputSchemaJson);
        Assert.True(tools[5].RequiresCredentials);
        Assert.Equal("get_collateral_history", tools[6].Name);
        Assert.Equal(typeof(GetCollateralHistoryRequest), tools[6].RequestType);
        Assert.Equal(typeof(GetCollateralHistoryResponse), tools[6].ResponseType);
        Assert.NotNull(tools[6].OutputSchemaJson);
        Assert.True(tools[6].RequiresCredentials);
        Assert.Equal("get_child_orders", tools[7].Name);
        Assert.Equal(typeof(GetChildOrdersRequest), tools[7].RequestType);
        Assert.Equal(typeof(GetChildOrdersResponse), tools[7].ResponseType);
        Assert.NotNull(tools[7].OutputSchemaJson);
        Assert.True(tools[7].RequiresCredentials);
        Assert.Equal("evaluate_order", tools[8].Name);
        Assert.Equal(typeof(EvaluateOrderRequest), tools[8].RequestType);
        Assert.Equal(typeof(EvaluateOrderResponse), tools[8].ResponseType);
        Assert.NotNull(tools[8].OutputSchemaJson);
        Assert.True(tools[8].RequiresCredentials);
        Assert.Equal("evaluate_margin_order", tools[9].Name);
        Assert.Equal(typeof(EvaluateMarginOrderRequest), tools[9].RequestType);
        Assert.Equal(typeof(EvaluateMarginOrderResponse), tools[9].ResponseType);
        Assert.NotNull(tools[9].OutputSchemaJson);
        Assert.True(tools[9].RequiresCredentials);
    }

    [Fact]
    public void PublicOnly_ContainsOnlyThePublicReadTools()
    {
        var tools = ToolCatalog.PublicOnly;

        Assert.Equal(["get_market_snapshot", "list_markets", "get_klines"], tools.Select(tool => tool.Name).ToArray());
    }

    [Fact]
    public void GetKlines_InputSchema_ExposesVenueAndClosedSets()
    {
        using var document = JsonDocument.Parse(ToolCatalog.GetKlines.InputSchemaJson);
        var root = document.RootElement;
        var branch = root.GetProperty("oneOf")[0];
        var required = branch.GetProperty("required").EnumerateArray().Select(x => x.GetString()!).ToArray();

        Assert.Equal(["venue", "symbol", "interval"], required);

        var properties = branch.GetProperty("properties");
        var venue = properties.GetProperty("venue").GetProperty("const").GetString();
        var symbols = properties.GetProperty("symbol").GetProperty("enum").EnumerateArray().Select(x => x.GetString()!).ToArray();
        var intervals = properties.GetProperty("interval").GetProperty("enum").EnumerateArray().Select(x => x.GetString()!).ToArray();

        Assert.Equal("binance", venue);
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
        Assert.Contains("minSizeSourceRef", required);
        Assert.Contains("sizeStepSourceRef", required);
        Assert.Contains("priceStepSourceRef", required);
        Assert.Equal(
            [MarketRuleSourceKinds.OfficialDocumented, MarketRuleSourceKinds.OfficialApiContract, MarketRuleSourceKinds.AdapterInferred, MarketRuleSourceKinds.PinnedOperational],
            sourceKinds);
    }

    [Fact]
    public void GetAccountSnapshot_OutputSchema_ExposesPermissionModel()
    {
        using var document = JsonDocument.Parse(ToolCatalog.GetAccountSnapshot.OutputSchemaJson!);
        var properties = document.RootElement.GetProperty("properties");

        Assert.Equal("bitflyer_private_read_v1", properties.GetProperty("permissionModel").GetProperty("enum")[0].GetString());
    }

    [Fact]
    public void PrivateToolInputSchemas_ExposeBitflyerVenueAndDefaultAccountContext()
    {
        using var accountDocument = JsonDocument.Parse(ToolCatalog.GetAccountSnapshot.InputSchemaJson);
        using var collateralAccountsDocument = JsonDocument.Parse(ToolCatalog.GetCollateralAccounts.InputSchemaJson);
        using var balanceHistoryDocument = JsonDocument.Parse(ToolCatalog.GetBalanceHistory.InputSchemaJson);
        using var collateralHistoryDocument = JsonDocument.Parse(ToolCatalog.GetCollateralHistory.InputSchemaJson);
        using var childOrdersDocument = JsonDocument.Parse(ToolCatalog.GetChildOrders.InputSchemaJson);
        using var evaluateDocument = JsonDocument.Parse(ToolCatalog.EvaluateOrder.InputSchemaJson);
        using var evaluateMarginDocument = JsonDocument.Parse(ToolCatalog.EvaluateMarginOrder.InputSchemaJson);

        AssertBitflyerPrivateContextShape(accountDocument.RootElement);
        AssertBitflyerPrivateContextShape(collateralAccountsDocument.RootElement);
        AssertBitflyerPrivateContextShape(balanceHistoryDocument.RootElement);
        AssertBitflyerPrivateContextShape(collateralHistoryDocument.RootElement);
        AssertBitflyerPrivateContextShape(childOrdersDocument.RootElement);
        AssertBitflyerPrivateContextShape(evaluateDocument.RootElement);
        AssertBitflyerPrivateContextShape(evaluateMarginDocument.RootElement);
    }

    [Fact]
    public void EvaluateOrder_OutputSchema_ExposesClosedWarningTaxonomy()
    {
        using var document = JsonDocument.Parse(ToolCatalog.EvaluateOrder.OutputSchemaJson!);
        var properties = document.RootElement.GetProperty("properties");
        var warningEnum = properties
            .GetProperty("warnings")
            .GetProperty("items")
            .GetProperty("enum")
            .EnumerateArray()
            .Select(x => x.GetString()!)
            .ToArray();
        var checks = properties.GetProperty("checks").GetProperty("properties");
        var estimate = properties.GetProperty("estimate").GetProperty("properties");
        var normalizedRequest = properties.GetProperty("normalizedRequest");
        var normalizedRequired = normalizedRequest.GetProperty("required").EnumerateArray().Select(x => x.GetString()!).ToArray();

        Assert.Equal(EvaluateOrderWarningCodes.All, warningEnum);
        Assert.Equal("null", checks.GetProperty("feeCoverageOk").GetProperty("type")[1].GetString());
        Assert.Equal("null", estimate.GetProperty("estimatedFee").GetProperty("type")[1].GetString());
        Assert.Contains("venue", normalizedRequired);
        Assert.Contains("accountContext", normalizedRequired);
        Assert.Equal("bitflyer", normalizedRequest.GetProperty("properties").GetProperty("venue").GetProperty("enum")[0].GetString());
        Assert.Equal("default", normalizedRequest.GetProperty("properties").GetProperty("accountContext").GetProperty("enum")[0].GetString());
    }

    [Fact]
    public void EvaluateMarginOrder_OutputSchema_ExposesClosedWarningTaxonomy()
    {
        using var document = JsonDocument.Parse(ToolCatalog.EvaluateMarginOrder.OutputSchemaJson!);
        var properties = document.RootElement.GetProperty("properties");
        var warningEnum = properties
            .GetProperty("warnings")
            .GetProperty("items")
            .GetProperty("enum")
            .EnumerateArray()
            .Select(x => x.GetString()!)
            .ToArray();
        var checks = properties.GetProperty("checks").GetProperty("properties");
        var estimate = properties.GetProperty("estimate").GetProperty("properties");

        Assert.Equal(EvaluateMarginOrderWarningCodes.All, warningEnum);
        Assert.Equal("null", checks.GetProperty("feeCoverageOk").GetProperty("type")[1].GetString());
        Assert.Equal("null", estimate.GetProperty("estimatedFee").GetProperty("type")[1].GetString());
        Assert.Equal("bitflyer", properties.GetProperty("normalizedRequest").GetProperty("properties").GetProperty("venue").GetProperty("enum")[0].GetString());
        Assert.Equal("default", properties.GetProperty("normalizedRequest").GetProperty("properties").GetProperty("accountContext").GetProperty("enum")[0].GetString());
    }

    private static void AssertBitflyerPrivateContextShape(JsonElement root)
    {
        var required = root.GetProperty("required").EnumerateArray().Select(x => x.GetString()!).ToArray();
        var properties = root.GetProperty("properties");

        Assert.Contains("venue", required);
        Assert.Contains("accountContext", required);
        Assert.Equal("bitflyer", properties.GetProperty("venue").GetProperty("enum")[0].GetString());
        Assert.Equal("default", properties.GetProperty("accountContext").GetProperty("enum")[0].GetString());
    }
}
