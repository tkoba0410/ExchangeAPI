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
                return ToToolCallResult(result);
            }
            case "get_klines":
            {
                if (_klinesTool is null)
                {
                    throw new InvalidOperationException("Binance public kline tool is not configured.");
                }

                var request = Deserialize<GetKlinesRequest>(arguments);
                var result = await _klinesTool.ExecuteAsync(request, cancellationToken);
                return ToToolCallResult(result);
            }
            case "get_account_snapshot":
            {
                if (_accountTool is null)
                {
                    return McpToolCallResult.ToolError(BuildMissingPrivateToolError());
                }

                var request = Deserialize<GetAccountSnapshotRequest>(arguments);
                var result = await _accountTool.ExecuteAsync(request, cancellationToken);
                return ToToolCallResult(result);
            }
            case "evaluate_order":
            {
                if (_evaluateOrderTool is null)
                {
                    return McpToolCallResult.ToolError(BuildMissingPrivateToolError());
                }

                var request = Deserialize<EvaluateOrderRequest>(arguments);
                var result = await _evaluateOrderTool.ExecuteAsync(request, cancellationToken);
                return ToToolCallResult(result);
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

    private static McpToolCallResult ToToolCallResult<TResponse>(McpToolExecutionResult<TResponse> result)
        where TResponse : class
    {
        return result.IsSuccess
            ? McpToolCallResult.Success(result.Response!)
            : McpToolCallResult.ToolError(result.Error!);
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
