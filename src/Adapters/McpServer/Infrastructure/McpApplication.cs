using System.Text.Json;
using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Tools;

namespace ExchangeApi.Adapters.McpServer.Infrastructure;

public sealed class McpApplication
{
    private const string JsonRpcVersion = "2.0";
    private const string McpProtocolVersion = "2025-11-25";
    private const int ParseErrorCode = -32700;
    private const int InvalidRequestErrorCode = -32600;
    private const int MethodNotFoundErrorCode = -32601;
    private const int InvalidParamsErrorCode = -32602;
    private const int InternalErrorCode = -32603;
    private const int ServerNotInitializedErrorCode = -32002;

    private readonly IMcpConsole _console;
    private readonly IMcpToolDispatcher? _dispatcher;
    private readonly IReadOnlyList<McpToolDefinition>? _toolOverrides;

    private bool _initialized;

    public McpApplication(
        IMcpToolDispatcher? dispatcher = null,
        IReadOnlyList<McpToolDefinition>? tools = null,
        IMcpConsole? console = null)
    {
        _console = console ?? new SystemMcpConsole();
        _dispatcher = dispatcher;
        _toolOverrides = tools;
    }

    public static Task<int> RunAsync(string[] args, CancellationToken cancellationToken = default)
    {
        return new McpApplication().RunAsync(args, cancellationToken);
    }

    public async Task<int> RunAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
    {
        try
        {
            return await RunInternalAsync(args, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _console.WriteErrorLine($"unexpected internal error: {ex.Message}");
            return McpExitCode.UnexpectedInternalError;
        }
    }

    private async Task<int> RunInternalAsync(IReadOnlyList<string> args, CancellationToken cancellationToken)
    {
        if (args.Count == 0)
        {
            return await RunStdioAsync(cancellationToken);
        }

        if (args.Count == 1 && (args[0] == "--help" || args[0] == "-h"))
        {
            WriteHelp();
            return McpExitCode.Success;
        }

        _console.WriteErrorLine($"unknown argument: {string.Join(" ", args)}");
        _console.WriteErrorLine("Use --help to inspect the current scaffold.");
        return McpExitCode.ArgumentError;
    }

    private async Task<int> RunStdioAsync(CancellationToken cancellationToken)
    {
        using var ownedDispatcher = _dispatcher is null
            ? ExchangeApiMcpToolDispatcher.CreateDefault(_console)
            : null;

        var dispatcher = _dispatcher ?? ownedDispatcher;
        if (dispatcher is null)
        {
            throw new InvalidOperationException("MCP tool dispatcher was not configured.");
        }

        while (true)
        {
            var line = await _console.ReadInLineAsync(cancellationToken);
            if (line is null)
            {
                return McpExitCode.Success;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            await ProcessMessageAsync(line, dispatcher, cancellationToken);
        }
    }

    private async Task ProcessMessageAsync(
        string line,
        IMcpToolDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        JsonDocument? document = null;
        JsonElement? requestId = null;
        try
        {
            try
            {
                document = JsonDocument.Parse(line);
            }
            catch (JsonException ex)
            {
                WriteProtocolError(id: null, ParseErrorCode, "Parse error.", ex.Message);
                return;
            }

            using (document)
            {
                var root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                {
                    WriteProtocolError(id: null, InvalidRequestErrorCode, "Invalid Request.");
                    return;
                }

                if (!root.TryGetProperty("jsonrpc", out var jsonRpcElement) ||
                    jsonRpcElement.ValueKind != JsonValueKind.String ||
                    !string.Equals(jsonRpcElement.GetString(), JsonRpcVersion, StringComparison.Ordinal))
                {
                    WriteProtocolError(ExtractResponseId(root), InvalidRequestErrorCode, "Invalid Request.");
                    return;
                }

                if (!root.TryGetProperty("method", out var methodElement))
                {
                    return;
                }

                if (methodElement.ValueKind != JsonValueKind.String)
                {
                    WriteProtocolError(ExtractResponseId(root), InvalidRequestErrorCode, "Invalid Request.");
                    return;
                }

                var method = methodElement.GetString()!;
                requestId = ExtractResponseId(root);
                var hasRequestId = requestId.HasValue;

                switch (method)
                {
                    case "initialize":
                        if (!hasRequestId)
                        {
                            return;
                        }

                        if (!TryGetObjectParams(root, required: true, out _))
                        {
                            WriteProtocolError(requestId, InvalidParamsErrorCode, "Invalid params.");
                            return;
                        }

                        _initialized = true;
                        WriteResult(
                            requestId!.Value,
                            new
                            {
                                protocolVersion = McpProtocolVersion,
                                capabilities = new
                                {
                                    tools = new
                                    {
                                        listChanged = false,
                                    },
                                },
                                serverInfo = new
                                {
                                    name = "exchangeapi-mcp",
                                    version = GetServerVersion(),
                                },
                                instructions =
                                    "ExchangeAPI bitFlyer v1 MCP server. This server is read/evaluate-only and exposes get_market_snapshot, get_account_snapshot, and evaluate_order.",
                            });
                        return;

                    case "notifications/initialized":
                    case "notifications/cancelled":
                        return;

                    case "ping":
                        if (!hasRequestId)
                        {
                            return;
                        }

                        WriteResult(requestId!.Value, new { });
                        return;
                }

                if (!_initialized)
                {
                    if (hasRequestId)
                    {
                        WriteProtocolError(requestId, ServerNotInitializedErrorCode, "Server is not initialized.");
                    }

                    return;
                }

                switch (method)
                {
                    case "tools/list":
                        if (!hasRequestId)
                        {
                            return;
                        }

                        if (!TryGetObjectParams(root, required: false, out _))
                        {
                            WriteProtocolError(requestId, InvalidParamsErrorCode, "Invalid params.");
                            return;
                        }

                        WriteResult(
                            requestId!.Value,
                            new
                            {
                                tools = GetVisibleTools(dispatcher).Select(BuildToolDescriptor).ToArray(),
                            });
                        return;

                    case "tools/call":
                        if (!hasRequestId)
                        {
                            return;
                        }

                        if (!TryGetObjectParams(root, required: true, out var callParams))
                        {
                            WriteProtocolError(requestId, InvalidParamsErrorCode, "Invalid params.");
                            return;
                        }

                        if (!callParams.TryGetProperty("name", out var toolNameElement) ||
                            toolNameElement.ValueKind != JsonValueKind.String)
                        {
                            WriteProtocolError(requestId, InvalidParamsErrorCode, "Invalid params.");
                            return;
                        }

                        var toolName = toolNameElement.GetString()!;
                        if (!GetVisibleTools(dispatcher).Any(tool => string.Equals(tool.Name, toolName, StringComparison.Ordinal)))
                        {
                            WriteProtocolError(requestId, InvalidParamsErrorCode, $"Unknown tool: {toolName}");
                            return;
                        }

                        var arguments = EmptyObject();
                        if (callParams.TryGetProperty("arguments", out var argumentsElement))
                        {
                            if (argumentsElement.ValueKind != JsonValueKind.Object)
                            {
                                WriteProtocolError(requestId, InvalidParamsErrorCode, "Invalid params.");
                                return;
                            }

                            arguments = argumentsElement.Clone();
                        }

                        try
                        {
                            var toolResult = await dispatcher.DispatchAsync(toolName, arguments, cancellationToken);
                            var structuredContent = toolResult.StructuredContent;
                            WriteResult(
                                requestId!.Value,
                                new
                                {
                                    content = new[]
                                    {
                                        new
                                        {
                                            type = "text",
                                            text = SerializeJson(structuredContent),
                                        },
                                    },
                                    structuredContent,
                                    isError = toolResult.IsError,
                                });
                        }
                        catch (JsonException ex)
                        {
                            WriteProtocolError(requestId, InvalidParamsErrorCode, "Invalid params.", ex.Message);
                        }
                        catch (NotSupportedException ex)
                        {
                            WriteProtocolError(requestId, InvalidParamsErrorCode, "Invalid params.", ex.Message);
                        }

                        return;

                    default:
                        if (hasRequestId)
                        {
                            WriteProtocolError(requestId, MethodNotFoundErrorCode, "Method not found.");
                        }

                        return;
                }
            }
        }
        catch (Exception ex)
        {
            _console.WriteErrorLine($"request handling failure: {ex.Message}");
            if (requestId.HasValue)
            {
                WriteProtocolError(requestId, InternalErrorCode, "Internal error.");
            }
        }
    }

    private void WriteHelp()
    {
        _console.WriteErrorLine("exchangeapi-mcp");
        _console.WriteErrorLine("ExchangeAPI MCP server for bitFlyer v1.");
        _console.WriteErrorLine("Current tools:");

        foreach (var tool in _toolOverrides ?? _dispatcher?.Tools ?? ToolCatalog.All)
        {
            _console.WriteErrorLine($"- {tool.Name}: {tool.Description}");
        }

        _console.WriteErrorLine("Transport: stdio, one JSON-RPC message per line on stdout/stdin.");
    }

    private void WriteResult(JsonElement id, object result)
    {
        _console.WriteOutLine(
            SerializeJson(
                new
                {
                    jsonrpc = JsonRpcVersion,
                    id,
                    result,
                }));
    }

    private void WriteProtocolError(JsonElement? id, int code, string message, object? data = null)
    {
        _console.WriteOutLine(
            SerializeJson(
                new
                {
                    jsonrpc = JsonRpcVersion,
                    id = id,
                    error = new
                    {
                        code,
                        message,
                        data,
                    },
                }));
    }

    private static bool TryGetObjectParams(
        JsonElement root,
        bool required,
        out JsonElement paramsElement)
    {
        if (!root.TryGetProperty("params", out paramsElement))
        {
            paramsElement = default;
            return !required;
        }

        if (paramsElement.ValueKind != JsonValueKind.Object)
        {
            paramsElement = default;
            return false;
        }

        paramsElement = paramsElement.Clone();
        return true;
    }

    private static JsonElement? ExtractResponseId(JsonElement root)
    {
        if (!root.TryGetProperty("id", out var idElement))
        {
            return null;
        }

        return idElement.ValueKind is JsonValueKind.String or JsonValueKind.Number
            ? idElement.Clone()
            : null;
    }

    private static JsonElement ParseJsonElement(string json)
    {
        return JsonSerializer.Deserialize<JsonElement>(json);
    }

    private IReadOnlyList<McpToolDefinition> GetVisibleTools(IMcpToolDispatcher dispatcher)
    {
        return _toolOverrides ?? dispatcher.Tools;
    }

    private object BuildToolDescriptor(McpToolDefinition tool)
    {
        var descriptor = new Dictionary<string, object?>
        {
            ["name"] = tool.Name,
            ["description"] = tool.Description,
            ["inputSchema"] = ParseJsonElement(tool.InputSchemaJson),
            ["annotations"] = new
            {
                readOnlyHint = tool.ReadOnlyHint,
                destructiveHint = false,
                idempotentHint = true,
            },
        };

        if (tool.OutputSchemaJson is not null)
        {
            descriptor["outputSchema"] = ParseJsonElement(tool.OutputSchemaJson);
        }

        return descriptor;
    }

    private static JsonElement EmptyObject()
    {
        return JsonSerializer.Deserialize<JsonElement>("{}");
    }

    private static string SerializeJson(object value)
    {
        return JsonSerializer.Serialize(value, value.GetType());
    }

    private static string GetServerVersion()
    {
        return typeof(McpApplication).Assembly.GetName().Version?.ToString() ?? "1.0.0";
    }
}
