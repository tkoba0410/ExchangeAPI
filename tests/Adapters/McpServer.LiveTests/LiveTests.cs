using System.Globalization;
using System.Text.Json;
using ExchangeApi.Exchanges.Binance.Composition.Factory;
using ExchangeApi.Adapters.McpServer.Infrastructure;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Adapters.McpServer.Schema.Klines;
using ExchangeApi.Adapters.McpServer.Schema.Market;
using ExchangeApi.Adapters.McpServer.Tools;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;
using ExchangeApi.Tests.Adapters.McpServer.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Adapters.McpServer.LiveTests;

public sealed class LiveTests
{
    [McpServerPublicReadLiveFact]
    public async Task GetMarketSnapshot_ReturnsLiveSnapshotAndRegistryBaseline()
    {
        var result = await RunAsync(
            _ => new ExchangeApiMcpToolDispatcher(
                BitflyerClientFactory.CreateNativeClient(new BitflyerClientOptions()),
                BinanceClientFactory.CreateNativeClient()),
            BuildInitializeRequest(1),
            BuildToolsListRequest(2),
            BuildToolCallRequest(3, "get_market_snapshot", new { symbol = "BTC_JPY" }));

        Assert.Empty(result.StdErr);
        Assert.Equal(3, result.OutputLines.Count);

        var toolNames = ReadToolNames(result.OutputLines[1]);
        Assert.Contains("get_market_snapshot", toolNames);
        Assert.Contains("list_markets", toolNames);
        Assert.Contains("get_klines", toolNames);

        var snapshot = ReadStructuredContent<GetMarketSnapshotResponse>(result.OutputLines[2], out var isError);
        Assert.False(isError);
        Assert.Equal("BTC_JPY", snapshot.Symbol);
        Assert.True(decimal.Parse(snapshot.Bid, CultureInfo.InvariantCulture) > 0m);
        Assert.True(decimal.Parse(snapshot.Ask, CultureInfo.InvariantCulture) > 0m);
        Assert.True(decimal.Parse(snapshot.Last, CultureInfo.InvariantCulture) > 0m);
        Assert.True(DateTimeOffset.TryParse(snapshot.Timestamp, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _));
        Assert.True(new[] { "active", "restricted", "halted", "unknown" }.Contains(snapshot.Status));

        Assert.True(BitflyerMarketRuleRegistry.TryGet("BTC_JPY", out var rule));
        Assert.NotNull(rule);
        Assert.Equal(rule!.MinSize, snapshot.Rules.MinSize);
        Assert.Equal(rule.SizeStep, snapshot.Rules.SizeStep);
        Assert.Equal(rule.PriceStep, snapshot.Rules.PriceStep);
        Assert.Equal(rule.MinSizeSourceKind, snapshot.Rules.MinSizeSourceKind);
        Assert.Equal(rule.SizeStepSourceKind, snapshot.Rules.SizeStepSourceKind);
        Assert.Equal(rule.PriceStepSourceKind, snapshot.Rules.PriceStepSourceKind);
    }

    [McpServerPublicReadLiveFact]
    public async Task GetKlines_ReturnsLiveBinanceCandles()
    {
        var result = await RunAsync(
            _ => new ExchangeApiMcpToolDispatcher(
                BitflyerClientFactory.CreateNativeClient(new BitflyerClientOptions()),
                BinanceClientFactory.CreateNativeClient()),
            BuildInitializeRequest(1),
            BuildToolsListRequest(2),
            BuildToolCallRequest(
                3,
                "get_klines",
                new
                {
                    venue = "binance",
                    symbol = "BTCUSDT",
                    interval = "1h",
                    limit = 2,
                }));

        Assert.Equal(3, result.OutputLines.Count);
        var toolNames = ReadToolNames(result.OutputLines[1]);
        Assert.Contains("list_markets", toolNames);
        Assert.Contains("get_klines", toolNames);

        var response = ReadStructuredContent<GetKlinesResponse>(result.OutputLines[2], out var isError);
        Assert.False(isError);
        Assert.Equal("binance", response.Venue);
        Assert.Equal("BTCUSDT", response.Symbol);
        Assert.Equal("1h", response.Interval);
        AssertCandlesAreWellFormed(response.Candles);
    }

    [McpServerPublicReadLiveFact]
    public async Task GetKlines_ReturnsLiveBinanceJpyCandles()
    {
        var result = await RunAsync(
            _ => new ExchangeApiMcpToolDispatcher(
                BitflyerClientFactory.CreateNativeClient(new BitflyerClientOptions()),
                BinanceClientFactory.CreateNativeClient()),
            BuildInitializeRequest(1),
            BuildToolCallRequest(
                2,
                "get_klines",
                new
                {
                    venue = "binance",
                    symbol = "BTCJPY",
                    interval = "1h",
                    limit = 2,
                }));

        Assert.Equal(2, result.OutputLines.Count);

        var response = ReadStructuredContent<GetKlinesResponse>(result.OutputLines[1], out var isError);
        Assert.False(isError);
        Assert.Equal("binance", response.Venue);
        Assert.Equal("BTCJPY", response.Symbol);
        Assert.Equal("1h", response.Interval);
        AssertCandlesAreWellFormed(response.Candles);
    }

    [McpServerPrivateReadLiveFact]
    public async Task ToolsList_AndGetAccountSnapshot_ReturnPrivateReadSurface()
    {
        var result = await RunAsync(
            console => ExchangeApiMcpToolDispatcher.CreateDefault(console),
            BuildInitializeRequest(1),
            BuildToolsListRequest(2),
            BuildToolCallRequest(
                3,
                "get_account_snapshot",
                new
                {
                    venue = McpVenueIds.Bitflyer,
                    accountContext = McpAccountContextIds.Default,
                }));

        Assert.Equal(3, result.OutputLines.Count);

        var toolNames = ReadToolNames(result.OutputLines[1]);
        Assert.Contains("get_market_snapshot", toolNames);
        Assert.Contains("list_markets", toolNames);
        Assert.Contains("get_account_snapshot", toolNames);
        Assert.Contains("evaluate_order", toolNames);
        Assert.Contains("evaluate_margin_order", toolNames);

        var snapshot = ReadStructuredContent<GetAccountSnapshotResponse>(result.OutputLines[2], out var isError);
        Assert.False(isError);
        Assert.Equal("bitflyer_private_read_v1", snapshot.PermissionModel);
        Assert.NotNull(snapshot.Balance);
        Assert.True(snapshot.OpenOrdersSummary.Count >= 0);
        Assert.True(decimal.TryParse(snapshot.Margin.DerivedAvailable, CultureInfo.InvariantCulture, out _));
        Assert.True(new[] { "ready", "restricted", "unknown" }.Contains(snapshot.AccountReadiness));
        Assert.All(snapshot.Positions, position => Assert.Equal("FX_BTC_JPY", position.Symbol));
    }

    [McpServerPrivateReadLiveFact]
    public async Task EvaluateOrder_ReturnsLiveStructuredEvaluation()
    {
        var result = await RunAsync(
            console => ExchangeApiMcpToolDispatcher.CreateDefault(console),
            BuildInitializeRequest(1),
            BuildToolCallRequest(
                2,
                "evaluate_order",
                new
                {
                    venue = McpVenueIds.Bitflyer,
                    accountContext = McpAccountContextIds.Default,
                    symbol = "BTC_JPY",
                    side = "buy",
                    orderType = "market",
                    size = "0.001",
                }));

        Assert.Equal(2, result.OutputLines.Count);

        var evaluation = ReadStructuredContent<EvaluateOrderResponse>(result.OutputLines[1], out var isError);
        Assert.False(isError);
        Assert.True(evaluation.Checks.SymbolOk);
        Assert.True(evaluation.Checks.SizeRuleOk);
        Assert.True(evaluation.Checks.PriceRuleOk);
        Assert.True(evaluation.Checks.ProjectedExposureOk);
        Assert.Equal(McpVenueIds.Bitflyer, evaluation.NormalizedRequest.Venue);
        Assert.Equal(McpAccountContextIds.Default, evaluation.NormalizedRequest.AccountContext);
        Assert.Equal("BTC_JPY", evaluation.NormalizedRequest.Symbol);
        Assert.Equal("buy", evaluation.NormalizedRequest.Side);
        Assert.Equal("market", evaluation.NormalizedRequest.OrderType);
        Assert.Equal("0.001", evaluation.NormalizedRequest.Size);
        Assert.Null(evaluation.NormalizedRequest.Price);
        Assert.True(decimal.Parse(evaluation.Estimate.ReferencePrice, CultureInfo.InvariantCulture) > 0m);
        Assert.True(decimal.Parse(evaluation.Estimate.EstimatedNotional, CultureInfo.InvariantCulture) > 0m);
        Assert.Contains("market_order_slippage_risk", evaluation.Warnings);
    }

    private static async Task<RunResult> RunAsync(
        Func<TestMcpConsole, ExchangeApiMcpToolDispatcher> dispatcherFactory,
        params string[] inputLines)
    {
        var console = new TestMcpConsole(inputLines);
        using var dispatcher = dispatcherFactory(console);
        var application = new McpApplication(dispatcher: dispatcher, console: console);

        var exitCode = await application.RunAsync([]);

        Assert.Equal(McpExitCode.Success, exitCode);
        return new RunResult(GetOutputLines(console.StdOut), console.StdErr);
    }

    private static string BuildInitializeRequest(int id)
    {
        return JsonSerializer.Serialize(
            new
            {
                jsonrpc = "2.0",
                id,
                method = "initialize",
                @params = new
                {
                    protocolVersion = "2025-11-25",
                    capabilities = new { },
                    clientInfo = new
                    {
                        name = "mcp-live-test",
                        version = "1.0.0",
                    },
                },
            });
    }

    private static string BuildToolsListRequest(int id)
    {
        return JsonSerializer.Serialize(
            new
            {
                jsonrpc = "2.0",
                id,
                method = "tools/list",
                @params = new { },
            });
    }

    private static string BuildToolCallRequest(int id, string name, object arguments)
    {
        return JsonSerializer.Serialize(
            new
            {
                jsonrpc = "2.0",
                id,
                method = "tools/call",
                @params = new
                {
                    name,
                    arguments,
                },
            });
    }

    private static IReadOnlyList<string> ReadToolNames(string line)
    {
        using var document = JsonDocument.Parse(line);
        return document.RootElement
            .GetProperty("result")
            .GetProperty("tools")
            .EnumerateArray()
            .Select(tool => tool.GetProperty("name").GetString())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .ToArray();
    }

    private static T ReadStructuredContent<T>(string line, out bool isError)
    {
        using var document = JsonDocument.Parse(line);
        var result = document.RootElement.GetProperty("result");
        isError = result.GetProperty("isError").GetBoolean();
        return JsonSerializer.Deserialize<T>(result.GetProperty("structuredContent").GetRawText())
            ?? throw new InvalidOperationException($"Failed to deserialize structuredContent as {typeof(T).Name}.");
    }

    private static IReadOnlyList<string> GetOutputLines(string stdout)
    {
        return stdout
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static void AssertCandlesAreWellFormed(IReadOnlyList<KlineCandle> candles)
    {
        Assert.True(candles.Count > 0);
        Assert.All(
            candles,
            candle =>
            {
                Assert.True(DateTimeOffset.TryParse(candle.OpenTime, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _));
                Assert.True(DateTimeOffset.TryParse(candle.CloseTime, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _));
                Assert.True(decimal.Parse(candle.Open, CultureInfo.InvariantCulture) > 0m);
                Assert.True(decimal.Parse(candle.High, CultureInfo.InvariantCulture) > 0m);
                Assert.True(decimal.Parse(candle.Low, CultureInfo.InvariantCulture) > 0m);
                Assert.True(decimal.Parse(candle.Close, CultureInfo.InvariantCulture) > 0m);
                Assert.True(decimal.Parse(candle.Volume, CultureInfo.InvariantCulture) >= 0m);
                Assert.True(decimal.Parse(candle.QuoteVolume, CultureInfo.InvariantCulture) >= 0m);
                Assert.True(candle.TradeCount >= 0);
            });
    }

    private sealed record RunResult(IReadOnlyList<string> OutputLines, string StdErr);
}
