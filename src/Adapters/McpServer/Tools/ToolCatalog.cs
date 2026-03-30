using System.Text.Json;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Adapters.McpServer.Schema.Klines;
using ExchangeApi.Adapters.McpServer.Schema.Market;
using ExchangeApi.Exchanges.Binance.Vocabulary;

namespace ExchangeApi.Adapters.McpServer.Tools;

public static class ToolCatalog
{
    private static readonly string MarketSnapshotInputSchema = BuildMarketSnapshotInputSchema();

    private const string ListMarketsInputSchema = """
        {
          "type": "object",
          "additionalProperties": false
        }
        """;

    private static readonly string AccountSnapshotInputSchema = BuildAccountSnapshotInputSchema();

    private static readonly string KlinesInputSchema = BuildKlinesInputSchema();

    private static readonly string EvaluateOrderInputSchema = BuildEvaluateOrderInputSchema();

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
                "priceStep": { "type": "string" },
                "minSizeSourceKind": {
                  "type": "string",
                  "enum": ["official_documented", "official_api_contract", "adapter_inferred", "pinned_operational"]
                },
                "minSizeSourceRef": { "type": "string" },
                "sizeStepSourceKind": {
                  "type": "string",
                  "enum": ["official_documented", "official_api_contract", "adapter_inferred", "pinned_operational"]
                },
                "sizeStepSourceRef": { "type": "string" },
                "priceStepSourceKind": {
                  "type": "string",
                  "enum": ["official_documented", "official_api_contract", "adapter_inferred", "pinned_operational"]
                },
                "priceStepSourceRef": { "type": "string" }
              },
              "required": ["minSize", "sizeStep", "priceStep", "minSizeSourceKind", "minSizeSourceRef", "sizeStepSourceKind", "sizeStepSourceRef", "priceStepSourceKind", "priceStepSourceRef"],
              "additionalProperties": false
            },
            "status": { "type": "string" }
          },
          "required": ["symbol", "bid", "ask", "last", "timestamp", "rules", "status"],
          "additionalProperties": false
        }
        """;

    private const string ListMarketsOutputSchema = """
        {
          "type": "object",
          "properties": {
            "markets": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "venue": {
                    "type": "string",
                    "enum": ["binance", "bitflyer"]
                  },
                  "symbol": { "type": "string" },
                  "capabilities": {
                    "type": "array",
                    "items": {
                      "type": "string",
                      "enum": ["get_market_snapshot", "get_klines", "evaluate_order"]
                    }
                  }
                },
                "required": ["venue", "symbol", "capabilities"],
                "additionalProperties": false
              }
            }
          },
          "required": ["markets"],
          "additionalProperties": false
        }
        """;

    private const string AccountSnapshotOutputSchema = """
        {
          "type": "object",
          "properties": {
            "permissionModel": {
              "type": "string",
              "enum": ["bitflyer_private_read_v1"]
            },
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
          "required": ["permissionModel", "balance", "positions", "openOrdersSummary", "margin", "accountReadiness"],
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
                "feeCoverageOk": { "type": ["boolean", "null"] },
                "projectedExposureOk": { "type": "boolean" }
              },
              "required": ["symbolOk", "marketStatusOk", "sizeRuleOk", "priceRuleOk", "balanceOk", "feeCoverageOk", "projectedExposureOk"],
              "additionalProperties": false
            },
            "normalizedRequest": {
              "type": "object",
              "properties": {
                "venue": {
                  "type": "string",
                  "enum": ["bitflyer"]
                },
                "accountContext": {
                  "type": "string",
                  "enum": ["default"]
                },
                "symbol": { "type": "string" },
                "side": { "type": "string" },
                "orderType": { "type": "string" },
                "size": { "type": "string" },
                "price": { "type": ["string", "null"] }
              },
              "required": ["venue", "accountContext", "symbol", "side", "orderType", "size", "price"],
              "additionalProperties": false
            },
            "estimate": {
              "type": "object",
              "properties": {
                "referencePrice": { "type": "string" },
                "estimatedNotional": { "type": "string" },
                "estimatedFee": { "type": ["string", "null"] },
                "estimatedFeeSourceKind": {
                  "type": ["string", "null"],
                  "enum": ["pinned_operational", null]
                }
              },
              "required": ["referencePrice", "estimatedNotional", "estimatedFee", "estimatedFeeSourceKind"],
              "additionalProperties": false
            },
            "warnings": {
              "type": "array",
              "items": {
                "type": "string",
                "enum": ["estimated_fee_not_covered", "market_order_slippage_risk"]
              }
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
            "venue": {
              "type": "string",
              "enum": ["binance"]
            },
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
          "required": ["venue", "symbol", "interval", "candles"],
          "additionalProperties": false
        }
        """;

    private static string BuildMarketSnapshotInputSchema()
    {
        var symbols = BitflyerMarketRuleRegistry.Entries.Keys.OrderBy(x => x).ToArray();

        return SerializeSchema(
            new Dictionary<string, object?>
            {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, object?>
                {
                    ["symbol"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["description"] = "Supported bitFlyer v1 symbol.",
                        ["enum"] = symbols,
                    },
                },
                ["required"] = new[] { "symbol" },
                ["additionalProperties"] = false,
            });
    }

    private static string BuildKlinesInputSchema()
    {
        var symbols = BinanceKlineSymbolSet.Entries.OrderBy(x => x).ToArray();
        var intervals = Enum.GetValues<BinanceInterval>()
            .Select(BinanceApiStringEnum<BinanceInterval>.Format)
            .ToArray();

        return SerializeSchema(
            new Dictionary<string, object?>
            {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, object?>
                {
                    ["venue"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["description"] = "Venue identifier. v1 requires binance.",
                        ["enum"] = new[] { "binance" },
                    },
                    ["symbol"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["description"] = "Supported Binance v1 symbol.",
                        ["enum"] = symbols,
                    },
                    ["interval"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["description"] = "Binance kline interval literal.",
                        ["enum"] = intervals,
                    },
                    ["startTime"] = new Dictionary<string, object?>
                    {
                        ["type"] = new object[] { "string", "null" },
                        ["description"] = "RFC 3339 string with explicit Z or numeric offset. Normalized to UTC by the server.",
                    },
                    ["endTime"] = new Dictionary<string, object?>
                    {
                        ["type"] = new object[] { "string", "null" },
                        ["description"] = "RFC 3339 string with explicit Z or numeric offset. Normalized to UTC by the server.",
                    },
                    ["limit"] = new Dictionary<string, object?>
                    {
                        ["type"] = new object[] { "integer", "null" },
                        ["description"] = "1..1000",
                    },
                },
                ["required"] = new[] { "venue", "symbol", "interval" },
                ["additionalProperties"] = false,
            });
    }

    private static string BuildAccountSnapshotInputSchema()
    {
        return SerializeSchema(
            new Dictionary<string, object?>
            {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, object?>
                {
                    ["venue"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["description"] = "Venue identifier. v1 requires bitflyer.",
                        ["enum"] = new[] { McpVenueIds.Bitflyer },
                    },
                    ["accountContext"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["description"] = "Account context identifier. v1 requires default.",
                        ["enum"] = new[] { McpAccountContextIds.Default },
                    },
                },
                ["required"] = new[] { "venue", "accountContext" },
                ["additionalProperties"] = false,
            });
    }

    private static string BuildEvaluateOrderInputSchema()
    {
        return SerializeSchema(
            new Dictionary<string, object?>
            {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, object?>
                {
                    ["venue"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["description"] = "Venue identifier. v1 requires bitflyer.",
                        ["enum"] = new[] { McpVenueIds.Bitflyer },
                    },
                    ["accountContext"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["description"] = "Account context identifier. v1 requires default.",
                        ["enum"] = new[] { McpAccountContextIds.Default },
                    },
                    ["symbol"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["description"] = "Supported bitFlyer v1 symbol.",
                        ["enum"] = new[] { "BTC_JPY" },
                    },
                    ["side"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["enum"] = new[] { "buy", "sell" },
                    },
                    ["orderType"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["enum"] = new[] { "market", "limit" },
                    },
                    ["size"] = new Dictionary<string, object?>
                    {
                        ["type"] = "string",
                        ["description"] = "Positive decimal string.",
                    },
                    ["price"] = new Dictionary<string, object?>
                    {
                        ["type"] = new object[] { "string", "null" },
                        ["description"] = "Decimal string for limit orders, null for market orders.",
                    },
                },
                ["required"] = new[] { "venue", "accountContext", "symbol", "side", "orderType", "size" },
                ["additionalProperties"] = false,
            });
    }

    private static string SerializeSchema(object value)
    {
        return JsonSerializer.Serialize(value);
    }

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

    public static McpToolDefinition ListMarkets { get; } =
        new(
            Name: "list_markets",
            Description: "Return the current visible market capability set grouped as venue and symbol pairs.",
            RequestType: typeof(ListMarketsRequest),
            ResponseType: typeof(ListMarketsResponse),
            InputSchemaJson: ListMarketsInputSchema,
            OutputSchemaJson: ListMarketsOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: false);

    public static McpToolDefinition GetKlines { get; } =
        new(
            Name: "get_klines",
            Description: "Return Binance v1 public OHLCV kline candles for a supported symbol and interval.",
            RequestType: typeof(GetKlinesRequest),
            ResponseType: typeof(GetKlinesResponse),
            InputSchemaJson: KlinesInputSchema,
            OutputSchemaJson: KlinesOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: false);

    public static McpToolDefinition GetAccountSnapshot { get; } =
        new(
            Name: "get_account_snapshot",
            Description: "Return the bot-oriented bitFlyer v1 account snapshot for balances, positions, open order count, and read readiness.",
            RequestType: typeof(GetAccountSnapshotRequest),
            ResponseType: typeof(GetAccountSnapshotResponse),
            InputSchemaJson: AccountSnapshotInputSchema,
            OutputSchemaJson: AccountSnapshotOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: true);

    public static McpToolDefinition EvaluateOrder { get; } =
        new(
            Name: "evaluate_order",
            Description: "Evaluate whether a supported bitFlyer v1 spot order request can be placed mechanically under current rules and balances.",
            RequestType: typeof(EvaluateOrderRequest),
            ResponseType: typeof(EvaluateOrderResponse),
            InputSchemaJson: EvaluateOrderInputSchema,
            OutputSchemaJson: EvaluateOrderOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: true);

    public static IReadOnlyList<McpToolDefinition> All { get; } =
        [GetMarketSnapshot, ListMarkets, GetKlines, GetAccountSnapshot, EvaluateOrder];

    public static IReadOnlyList<McpToolDefinition> PublicOnly { get; } =
        All.Where(tool => !tool.RequiresCredentials).ToArray();
}
