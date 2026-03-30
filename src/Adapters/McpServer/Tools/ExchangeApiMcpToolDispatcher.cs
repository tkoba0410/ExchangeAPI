using System.Text.Json;
using ExchangeApi.Adapters.McpServer.Configuration;
using ExchangeApi.Adapters.McpServer.Infrastructure;
using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Adapters.McpServer.Schema.Klines;
using ExchangeApi.Adapters.McpServer.Schema.Market;
using ExchangeApi.Adapters.McpServer.Tools.Account;
using ExchangeApi.Adapters.McpServer.Tools.Evaluation;
using ExchangeApi.Adapters.McpServer.Tools.Klines;
using ExchangeApi.Adapters.McpServer.Tools.Market;
using ExchangeApi.Exchanges.Binance.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Adapters.McpServer.Tools;

public sealed class ExchangeApiMcpToolDispatcher : IMcpToolDispatcher, IDisposable
{
    private readonly BitflyerNativeBundle _bitflyerBundle;
    private readonly BinanceNativeBundle? _binanceBundle;
    private readonly GetMarketSnapshotTool _marketTool;
    private readonly ListMarketsTool _listMarketsTool;
    private readonly GetKlinesTool? _klinesTool;
    private readonly GetAccountSnapshotTool? _accountTool;
    private readonly EvaluateOrderTool? _evaluateOrderTool;
    private readonly string? _privateToolUnavailableReason;
    private readonly IReadOnlyList<McpToolDefinition> _tools;

    public ExchangeApiMcpToolDispatcher(
        BitflyerNativeBundle bitflyerBundle,
        BinanceNativeBundle? binanceBundle = null,
        string? privateToolUnavailableReason = null)
    {
        _bitflyerBundle = bitflyerBundle;
        _binanceBundle = binanceBundle;
        _privateToolUnavailableReason = privateToolUnavailableReason;
        _marketTool = new GetMarketSnapshotTool(new BitflyerNativeMarketSnapshotGateway(bitflyerBundle.Public));

        if (binanceBundle is not null)
        {
            _klinesTool = new GetKlinesTool(new BinanceNativeKlinesGateway(binanceBundle.Public));
        }

        if (bitflyerBundle.Private is not null)
        {
            _accountTool = new GetAccountSnapshotTool(new BitflyerNativeAccountSnapshotGateway(bitflyerBundle.Private));
            _evaluateOrderTool = new EvaluateOrderTool(new BitflyerNativeEvaluateOrderGateway(bitflyerBundle.Public, bitflyerBundle.Private));
        }

        _listMarketsTool = new ListMarketsTool(
            hasMarketSnapshot: true,
            hasKlines: _klinesTool is not null,
            hasEvaluateOrder: _evaluateOrderTool is not null);

        _tools = BuildVisibleTools();
    }

    public IReadOnlyList<McpToolDefinition> Tools => _tools;

    public static ExchangeApiMcpToolDispatcher CreateDefault(IMcpConsole console)
    {
        var credentialResolution = BitflyerCredentialResolver.Resolve();
        string? privateToolUnavailableReason = null;
        if (credentialResolution.HasFailure)
        {
            privateToolUnavailableReason = credentialResolution.ErrorMessage;
            console.WriteErrorLine($"bitFlyer private tools unavailable: {credentialResolution.ErrorMessage}");
        }

        var bundle = BitflyerClientFactory.CreateNativeClient(
            new BitflyerClientOptions
            {
                Credentials = credentialResolution.Credentials,
            });

        if (bundle.Private is null && privateToolUnavailableReason is null)
        {
            privateToolUnavailableReason =
                $"Configure {BitflyerCredentialResolver.AgeIdentityFileEnvName} and {BitflyerCredentialResolver.CredentialsAgeFileEnvName} to enable private tools.";
        }

        var binanceBundle = BinanceClientFactory.CreateNativeClient();
        return new ExchangeApiMcpToolDispatcher(bundle, binanceBundle, privateToolUnavailableReason);
    }

    public async Task<McpToolCallResult> DispatchAsync(
        string name,
        JsonElement arguments,
        CancellationToken cancellationToken = default)
    {
        switch (name)
        {
            case "get_market_snapshot":
            {
                var request = Deserialize<GetMarketSnapshotRequest>(arguments);
                var result = await _marketTool.ExecuteAsync(request, cancellationToken);
                return ToToolCallResult(result, "get_market_snapshot");
            }
            case "list_markets":
            {
                var request = Deserialize<ListMarketsRequest>(arguments);
                var result = await _listMarketsTool.ExecuteAsync(request, cancellationToken);
                return ToToolCallResult(result, "list_markets");
            }
            case "get_klines":
            {
                if (_klinesTool is null)
                {
                    throw new InvalidOperationException("Binance public kline tool is not configured.");
                }

                var request = Deserialize<GetKlinesRequest>(arguments);
                var result = await _klinesTool.ExecuteAsync(request, cancellationToken);
                return ToToolCallResult(result, "get_klines");
            }
            case "get_account_snapshot":
            {
                if (_accountTool is null)
                {
                    return McpToolCallResult.ToolError(
                        BuildMissingPrivateToolError(),
                        BuildMeta("get_account_snapshot", content: null));
                }

                var request = Deserialize<GetAccountSnapshotRequest>(arguments);
                var result = await _accountTool.ExecuteAsync(request, cancellationToken);
                return ToToolCallResult(result, "get_account_snapshot");
            }
            case "evaluate_order":
            {
                if (_evaluateOrderTool is null)
                {
                    return McpToolCallResult.ToolError(
                        BuildMissingPrivateToolError(),
                        BuildMeta("evaluate_order", content: null));
                }

                var request = Deserialize<EvaluateOrderRequest>(arguments);
                var result = await _evaluateOrderTool.ExecuteAsync(request, cancellationToken);
                return ToToolCallResult(result, "evaluate_order");
            }
            default:
                throw new InvalidOperationException($"Unknown tool: {name}");
        }
    }

    public void Dispose()
    {
        _bitflyerBundle.Dispose();
        _binanceBundle?.Dispose();
    }

    private TRequest Deserialize<TRequest>(JsonElement arguments)
    {
        var json = arguments.ValueKind == JsonValueKind.Undefined
            ? "{}"
            : arguments.GetRawText();

        return JsonSerializer.Deserialize<TRequest>(json)
            ?? throw new JsonException($"Failed to deserialize tool arguments as {typeof(TRequest).Name}.");
    }

    private static McpToolCallResult ToToolCallResult<TResponse>(McpToolExecutionResult<TResponse> result, string toolName)
        where TResponse : class
    {
        return result.IsSuccess
            ? McpToolCallResult.Success(result.Response!, BuildMeta(toolName, result.Response))
            : McpToolCallResult.ToolError(result.Error!, BuildMeta(toolName, content: null));
    }

    private static McpToolCallMeta BuildMeta(string toolName, object? content)
    {
        return new McpToolCallMeta
        {
            SchemaVersion = $"exchangeapi.mcp.{toolName}.v1",
            DataVersion = toolName switch
            {
                "get_market_snapshot" => "bitflyer-market-rules.v1",
                "list_markets" => "exchangeapi-visible-markets.v1",
                "get_klines" => "binance-kline-support-set.v1",
                "get_account_snapshot" => "bitflyer-private-read.v1",
                "evaluate_order" => "bitflyer-evaluate-order.v1",
                _ => "exchangeapi.mcp.unknown.v1",
            },
            Degraded = content is GetAccountSnapshotResponse accountSnapshot
                && string.Equals(accountSnapshot.AccountReadiness, "unknown", StringComparison.Ordinal),
        };
    }

    private McpToolError BuildMissingPrivateToolError()
    {
        return new McpToolError
        {
            ErrorCategory = "upstream_error",
            ErrorCode = "account_unavailable",
            Message = "bitFlyer private tools are unavailable because credentials are not configured.",
            Details = new Dictionary<string, string?>
            {
                ["reason"] = _privateToolUnavailableReason,
                ["requiredEnv"] =
                    $"{BitflyerCredentialResolver.AgeIdentityFileEnvName}, {BitflyerCredentialResolver.CredentialsAgeFileEnvName}",
            },
            Retryable = false,
        };
    }

    private IReadOnlyList<McpToolDefinition> BuildVisibleTools()
    {
        var tools = new List<McpToolDefinition>
        {
            ToolCatalog.GetMarketSnapshot,
            ToolCatalog.ListMarkets,
        };

        if (_klinesTool is not null)
        {
            tools.Add(ToolCatalog.GetKlines);
        }

        if (_accountTool is not null)
        {
            tools.Add(ToolCatalog.GetAccountSnapshot);
        }

        if (_evaluateOrderTool is not null)
        {
            tools.Add(ToolCatalog.EvaluateOrder);
        }

        return tools;
    }
}
