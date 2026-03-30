using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Adapters.McpServer.Schema.Klines;
using ExchangeApi.Adapters.McpServer.Schema.Market;

namespace ExchangeApi.Adapters.McpServer.Tools;

public static class ToolCatalog
{
    private const string MarketSnapshotInputSchema = """
        {
          "type": "object",
          "properties": {
            "symbol": {
              "type": "string",
              "description": "Supported bitFlyer symbol."
            }
          },
          "required": ["symbol"],
          "additionalProperties": false
        }
        """;

    private const string AccountSnapshotInputSchema = """
        {
          "type": "object",
          "additionalProperties": false
        }
        """;

    private const string KlinesInputSchema = """
        {
          "type": "object",
          "properties": {
            "symbol": {
              "type": "string",
              "description": "Supported Binance symbol."
            },
            "interval": {
              "type": "string",
              "description": "Binance kline interval literal."
            },
            "startTime": {
              "type": ["string", "null"],
              "description": "UTC ISO 8601 string."
            },
            "endTime": {
              "type": ["string", "null"],
              "description": "UTC ISO 8601 string."
            },
            "limit": {
              "type": ["integer", "null"],
              "description": "1..1000"
            }
          },
          "required": ["symbol", "interval"],
          "additionalProperties": false
        }
        """;

    private const string EvaluateOrderInputSchema = """
        {
          "type": "object",
          "properties": {
            "symbol": {
              "type": "string",
              "description": "Supported bitFlyer symbol."
            },
            "side": {
              "type": "string",
              "enum": ["buy", "sell"]
            },
            "orderType": {
              "type": "string",
              "enum": ["market", "limit"]
            },
            "size": {
              "type": "string",
              "description": "Positive decimal string."
            },
            "price": {
              "type": ["string", "null"],
              "description": "Decimal string for limit orders, null for market orders."
            }
          },
          "required": ["symbol", "side", "orderType", "size"],
          "additionalProperties": false
        }
        """;

    private const string MarketSnapshotOutputSchema = """
        {
          "type": "object",
          "properties": {
            "symbol": { "type": "string" },
            "bid": { "type": "string" },
            "ask": { "type": "string" },
            "last": { "type": "string" },
            "timestamp": { "type": "string" },
            "rules": {
              "type": "object",
              "properties": {
                "minSize": { "type": "string" },
                "sizeStep": { "type": "string" },
                "priceStep": { "type": "string" }
              },
              "required": ["minSize", "sizeStep", "priceStep"],
              "additionalProperties": false
            },
            "status": { "type": "string" }
          },
          "required": ["symbol", "bid", "ask", "last", "timestamp", "rules", "status"],
          "additionalProperties": false
        }
        """;

    private const string AccountSnapshotOutputSchema = """
        {
          "type": "object",
          "properties": {
            "balance": {
              "type": "object",
              "additionalProperties": { "type": "string" }
            },
            "positions": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "symbol": { "type": "string" },
                  "side": { "type": "string" },
                  "size": { "type": "string" },
                  "avgPrice": { "type": "string" }
                },
                "required": ["symbol", "side", "size", "avgPrice"],
                "additionalProperties": false
              }
            },
            "openOrdersSummary": {
              "type": "object",
              "properties": {
                "count": { "type": "integer" }
              },
              "required": ["count"],
              "additionalProperties": false
            },
            "margin": {
              "type": "object",
              "properties": {
                "derivedAvailable": {
                  "type": ["string", "null"]
                }
              },
              "required": ["derivedAvailable"],
              "additionalProperties": false
            },
            "accountReadiness": { "type": "string" }
          },
          "required": ["balance", "positions", "openOrdersSummary", "margin", "accountReadiness"],
          "additionalProperties": false
        }
        """;

    private const string EvaluateOrderOutputSchema = """
        {
          "type": "object",
          "properties": {
            "canPlace": { "type": "boolean" },
            "checks": {
              "type": "object",
              "properties": {
                "symbolOk": { "type": "boolean" },
                "marketStatusOk": { "type": "boolean" },
                "sizeRuleOk": { "type": "boolean" },
                "priceRuleOk": { "type": "boolean" },
                "balanceOk": { "type": "boolean" },
                "positionLimitOk": { "type": "boolean" }
              },
              "required": ["symbolOk", "marketStatusOk", "sizeRuleOk", "priceRuleOk", "balanceOk", "positionLimitOk"],
              "additionalProperties": false
            },
            "normalizedRequest": {
              "type": "object",
              "properties": {
                "symbol": { "type": "string" },
                "side": { "type": "string" },
                "orderType": { "type": "string" },
                "size": { "type": "string" },
                "price": { "type": ["string", "null"] }
              },
              "required": ["symbol", "side", "orderType", "size", "price"],
              "additionalProperties": false
            },
            "estimate": {
              "type": "object",
              "properties": {
                "referencePrice": { "type": "string" },
                "estimatedNotional": { "type": "string" }
              },
              "required": ["referencePrice", "estimatedNotional"],
              "additionalProperties": false
            },
            "warnings": {
              "type": "array",
              "items": { "type": "string" }
            },
            "reasons": {
              "type": "array",
              "items": { "type": "string" }
            }
          },
          "required": ["canPlace", "checks", "normalizedRequest", "estimate", "warnings", "reasons"],
          "additionalProperties": false
        }
        """;

    private const string KlinesOutputSchema = """
        {
          "type": "object",
          "properties": {
            "symbol": { "type": "string" },
            "interval": { "type": "string" },
            "candles": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "openTime": { "type": "string" },
                  "closeTime": { "type": "string" },
                  "open": { "type": "string" },
                  "high": { "type": "string" },
                  "low": { "type": "string" },
                  "close": { "type": "string" },
                  "volume": { "type": "string" },
                  "quoteVolume": { "type": "string" },
                  "tradeCount": { "type": "integer" },
                  "takerBuyBaseVolume": { "type": "string" },
                  "takerBuyQuoteVolume": { "type": "string" }
                },
                "required": ["openTime", "closeTime", "open", "high", "low", "close", "volume", "quoteVolume", "tradeCount", "takerBuyBaseVolume", "takerBuyQuoteVolume"],
                "additionalProperties": false
              }
            }
          },
          "required": ["symbol", "interval", "candles"],
          "additionalProperties": false
        }
        """;

    public static McpToolDefinition GetMarketSnapshot { get; } =
        new(
            Name: "get_market_snapshot",
            Description: "Return market price, market status, and fixed trading rules for a supported symbol.",
            RequestType: typeof(GetMarketSnapshotRequest),
            ResponseType: typeof(GetMarketSnapshotResponse),
            InputSchemaJson: MarketSnapshotInputSchema,
            OutputSchemaJson: MarketSnapshotOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: false);

    public static McpToolDefinition GetKlines { get; } =
        new(
            Name: "get_klines",
            Description: "Return Binance public OHLCV kline candles for a supported symbol and interval.",
            RequestType: typeof(GetKlinesRequest),
            ResponseType: typeof(GetKlinesResponse),
            InputSchemaJson: KlinesInputSchema,
            OutputSchemaJson: KlinesOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: false);

    public static McpToolDefinition GetAccountSnapshot { get; } =
        new(
            Name: "get_account_snapshot",
            Description: "Return the bot-oriented account snapshot for balances, positions, open order count, and read readiness.",
            RequestType: typeof(GetAccountSnapshotRequest),
            ResponseType: typeof(GetAccountSnapshotResponse),
            InputSchemaJson: AccountSnapshotInputSchema,
            OutputSchemaJson: AccountSnapshotOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: true);

    public static McpToolDefinition EvaluateOrder { get; } =
        new(
            Name: "evaluate_order",
            Description: "Evaluate whether a supported order request can be placed mechanically under current rules and balances.",
            RequestType: typeof(EvaluateOrderRequest),
            ResponseType: typeof(EvaluateOrderResponse),
            InputSchemaJson: EvaluateOrderInputSchema,
            OutputSchemaJson: EvaluateOrderOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: true);

    public static IReadOnlyList<McpToolDefinition> All { get; } =
        [GetMarketSnapshot, GetKlines, GetAccountSnapshot, EvaluateOrder];

    public static IReadOnlyList<McpToolDefinition> PublicOnly { get; } =
        All.Where(tool => !tool.RequiresCredentials).ToArray();
}
