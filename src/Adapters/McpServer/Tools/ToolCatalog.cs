using System.Text.Json;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Adapters.McpServer.Schema.Inspection;
using ExchangeApi.Adapters.McpServer.Schema.Klines;
using ExchangeApi.Adapters.McpServer.Schema.MarginEvaluation;
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
    private static readonly string BalanceHistoryInputSchema = BuildBalanceHistoryInputSchema();
    private static readonly string CollateralHistoryInputSchema = BuildPagedPrivateReadInputSchema();
    private static readonly string ChildOrdersInputSchema = BuildChildOrdersInputSchema();

    private static readonly string KlinesInputSchema = BuildKlinesInputSchema();
    private static readonly string KlinesOutputSchema = BuildKlinesOutputSchema();

    private static readonly string EvaluateOrderInputSchema = BuildEvaluateOrderInputSchema();
    private static readonly string EvaluateMarginOrderInputSchema = BuildEvaluateMarginOrderInputSchema();

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
                      "enum": ["get_market_snapshot", "get_klines", "evaluate_order", "evaluate_margin_order"]
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

    private const string CollateralAccountsOutputSchema = """
        {
          "type": "object",
          "properties": {
            "accounts": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "currencyCode": { "type": "string" },
                  "amount": { "type": "string" }
                },
                "required": ["currencyCode", "amount"],
                "additionalProperties": false
              }
            }
          },
          "required": ["accounts"],
          "additionalProperties": false
        }
        """;

    private const string BalanceHistoryOutputSchema = """
        {
          "type": "object",
          "properties": {
            "items": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "id": { "type": "integer" },
                  "tradeDate": { "type": "string" },
                  "eventDate": { "type": "string" },
                  "productCode": { "type": ["string", "null"] },
                  "currencyCode": { "type": "string" },
                  "tradeType": { "type": "string" },
                  "price": { "type": "string" },
                  "amount": { "type": "string" },
                  "quantity": { "type": "string" },
                  "commission": { "type": "string" },
                  "balance": { "type": "string" },
                  "orderId": { "type": ["string", "null"] }
                },
                "required": ["id", "tradeDate", "eventDate", "productCode", "currencyCode", "tradeType", "price", "amount", "quantity", "commission", "balance", "orderId"],
                "additionalProperties": false
              }
            }
          },
          "required": ["items"],
          "additionalProperties": false
        }
        """;

    private const string CollateralHistoryOutputSchema = """
        {
          "type": "object",
          "properties": {
            "items": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "id": { "type": "integer" },
                  "currencyCode": { "type": "string" },
                  "change": { "type": "string" },
                  "amount": { "type": "string" },
                  "reasonCode": { "type": "string" },
                  "date": { "type": "string" }
                },
                "required": ["id", "currencyCode", "change", "amount", "reasonCode", "date"],
                "additionalProperties": false
              }
            }
          },
          "required": ["items"],
          "additionalProperties": false
        }
        """;

    private const string ChildOrdersOutputSchema = """
        {
          "type": "object",
          "properties": {
            "orders": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "id": { "type": "integer" },
                  "childOrderId": { "type": "string" },
                  "productCode": { "type": "string" },
                  "side": { "type": "string" },
                  "childOrderType": { "type": "string" },
                  "price": { "type": "string" },
                  "averagePrice": { "type": "string" },
                  "size": { "type": "string" },
                  "childOrderState": { "type": "string" },
                  "expireDate": { "type": "string" },
                  "childOrderDate": { "type": "string" },
                  "childOrderAcceptanceId": { "type": "string" },
                  "outstandingSize": { "type": "string" },
                  "cancelSize": { "type": "string" },
                  "executedSize": { "type": "string" },
                  "totalCommission": { "type": "string" },
                  "timeInForce": { "type": "string" }
                },
                "required": ["id", "childOrderId", "productCode", "side", "childOrderType", "price", "averagePrice", "size", "childOrderState", "expireDate", "childOrderDate", "childOrderAcceptanceId", "outstandingSize", "cancelSize", "executedSize", "totalCommission", "timeInForce"],
                "additionalProperties": false
              }
            }
          },
          "required": ["orders"],
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

    private const string EvaluateMarginOrderOutputSchema = """
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
                "collateralCoverageOk": { "type": "boolean" },
                "feeCoverageOk": { "type": ["boolean", "null"] },
                "projectedMarginExposureOk": { "type": "boolean" },
                "currentMaintenanceOk": { "type": "boolean" }
              },
              "required": ["symbolOk", "marketStatusOk", "sizeRuleOk", "priceRuleOk", "collateralCoverageOk", "feeCoverageOk", "projectedMarginExposureOk", "currentMaintenanceOk"],
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
                "estimatedRequiredCollateral": { "type": "string" },
                "currentMaxLeverage": { "type": "string" },
                "currentKeepRate": { "type": "string" },
                "minimumKeepRate": { "type": "string" },
                "estimatedFee": { "type": ["string", "null"] },
                "estimatedFeeSourceKind": {
                  "type": ["string", "null"],
                  "enum": ["pinned_operational", null]
                }
              },
              "required": ["referencePrice", "estimatedNotional", "estimatedRequiredCollateral", "currentMaxLeverage", "currentKeepRate", "minimumKeepRate", "estimatedFee", "estimatedFeeSourceKind"],
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
                ["oneOf"] = new object[]
                {
                    new Dictionary<string, object?>
                    {
                        ["type"] = "object",
                        ["properties"] = new Dictionary<string, object?>
                        {
                            ["venue"] = new Dictionary<string, object?>
                            {
                                ["const"] = McpVenueIds.Binance,
                                ["description"] = "Venue identifier. v1 requires binance.",
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
                    }
                },
            });
    }

    private static string BuildKlinesOutputSchema()
    {
        return SerializeSchema(
            new Dictionary<string, object?>
            {
                ["oneOf"] = new object[]
                {
                    new Dictionary<string, object?>
                    {
                        ["type"] = "object",
                        ["properties"] = new Dictionary<string, object?>
                        {
                            ["venue"] = new Dictionary<string, object?>
                            {
                                ["const"] = McpVenueIds.Binance,
                            },
                            ["symbol"] = new Dictionary<string, object?>
                            {
                                ["type"] = "string",
                                ["enum"] = BinanceKlineSymbolSet.Entries.OrderBy(x => x).ToArray(),
                            },
                            ["interval"] = new Dictionary<string, object?>
                            {
                                ["type"] = "string",
                                ["enum"] = Enum.GetValues<BinanceInterval>().Select(BinanceApiStringEnum<BinanceInterval>.Format).ToArray(),
                            },
                            ["candles"] = new Dictionary<string, object?>
                            {
                                ["type"] = "array",
                                ["items"] = new Dictionary<string, object?>
                                {
                                    ["type"] = "object",
                                    ["properties"] = new Dictionary<string, object?>
                                    {
                                        ["openTime"] = new Dictionary<string, object?> { ["type"] = "string" },
                                        ["closeTime"] = new Dictionary<string, object?> { ["type"] = "string" },
                                        ["open"] = new Dictionary<string, object?> { ["type"] = "string" },
                                        ["high"] = new Dictionary<string, object?> { ["type"] = "string" },
                                        ["low"] = new Dictionary<string, object?> { ["type"] = "string" },
                                        ["close"] = new Dictionary<string, object?> { ["type"] = "string" },
                                        ["volume"] = new Dictionary<string, object?> { ["type"] = "string" },
                                        ["quoteVolume"] = new Dictionary<string, object?> { ["type"] = "string" },
                                        ["tradeCount"] = new Dictionary<string, object?> { ["type"] = "integer" },
                                        ["takerBuyBaseVolume"] = new Dictionary<string, object?> { ["type"] = "string" },
                                        ["takerBuyQuoteVolume"] = new Dictionary<string, object?> { ["type"] = "string" },
                                    },
                                    ["required"] = new[] { "openTime", "closeTime", "open", "high", "low", "close", "volume", "quoteVolume", "tradeCount", "takerBuyBaseVolume", "takerBuyQuoteVolume" },
                                    ["additionalProperties"] = false,
                                },
                            },
                        },
                        ["required"] = new[] { "venue", "symbol", "interval", "candles" },
                        ["additionalProperties"] = false,
                    }
                },
            });
    }

    private static string BuildAccountSnapshotInputSchema()
    {
        return BuildPrivateReadInputSchema([]);
    }

    private static string BuildBalanceHistoryInputSchema()
    {
        return SerializeSchema(
            BuildPrivateReadSchemaObject(
                new Dictionary<string, object?>
                {
                    ["currencyCode"] = new Dictionary<string, object?>
                    {
                        ["type"] = new object[] { "string", "null" },
                    },
                    ["count"] = new Dictionary<string, object?> { ["type"] = new object[] { "integer", "null" } },
                    ["before"] = new Dictionary<string, object?> { ["type"] = new object[] { "integer", "null" } },
                    ["after"] = new Dictionary<string, object?> { ["type"] = new object[] { "integer", "null" } },
                },
                ["venue", "accountContext"]));
    }

    private static string BuildPagedPrivateReadInputSchema()
    {
        return BuildPrivateReadInputSchema(
            new Dictionary<string, object?>
            {
                ["count"] = new Dictionary<string, object?> { ["type"] = new object[] { "integer", "null" } },
                ["before"] = new Dictionary<string, object?> { ["type"] = new object[] { "integer", "null" } },
                ["after"] = new Dictionary<string, object?> { ["type"] = new object[] { "integer", "null" } },
            });
    }

    private static string BuildChildOrdersInputSchema()
    {
        return SerializeSchema(
            BuildPrivateReadSchemaObject(
                new Dictionary<string, object?>
                {
                    ["productCode"] = new Dictionary<string, object?> { ["type"] = new object[] { "string", "null" } },
                    ["count"] = new Dictionary<string, object?> { ["type"] = new object[] { "integer", "null" } },
                    ["before"] = new Dictionary<string, object?> { ["type"] = new object[] { "integer", "null" } },
                    ["after"] = new Dictionary<string, object?> { ["type"] = new object[] { "integer", "null" } },
                    ["childOrderState"] = new Dictionary<string, object?>
                    {
                        ["type"] = new object[] { "string", "null" },
                        ["enum"] = new object?[] { "ACTIVE", "COMPLETED", "CANCELED", "EXPIRED", "REJECTED", null },
                    },
                    ["childOrderId"] = new Dictionary<string, object?> { ["type"] = new object[] { "string", "null" } },
                    ["childOrderAcceptanceId"] = new Dictionary<string, object?> { ["type"] = new object[] { "string", "null" } },
                    ["parentOrderId"] = new Dictionary<string, object?> { ["type"] = new object[] { "string", "null" } },
                },
                ["venue", "accountContext"]));
    }

    private static string BuildPrivateReadInputSchema(Dictionary<string, object?> extraProperties)
    {
        return SerializeSchema(BuildPrivateReadSchemaObject(extraProperties, ["venue", "accountContext"]));
    }

    private static Dictionary<string, object?> BuildPrivateReadSchemaObject(
        Dictionary<string, object?> extraProperties,
        string[] required)
    {
        var properties = new Dictionary<string, object?>
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
        };

        foreach (var property in extraProperties)
        {
            properties[property.Key] = property.Value;
        }

        return new Dictionary<string, object?>
        {
            ["type"] = "object",
            ["properties"] = properties,
            ["required"] = required,
            ["additionalProperties"] = false,
        };
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

    private static string BuildEvaluateMarginOrderInputSchema()
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
                        ["description"] = "Supported bitFlyer v1 margin symbol.",
                        ["enum"] = new[] { "FX_BTC_JPY" },
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
            Description: "Read market price, venue status, and fixed trading rules for a supported symbol. Read-only. Does not place orders.",
            RequestType: typeof(GetMarketSnapshotRequest),
            ResponseType: typeof(GetMarketSnapshotResponse),
            InputSchemaJson: MarketSnapshotInputSchema,
            OutputSchemaJson: MarketSnapshotOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: false);

    public static McpToolDefinition ListMarkets { get; } =
        new(
            Name: "list_markets",
            Description: "Read the current visible venue and symbol capability set for discovery. Read-only. Does not place orders.",
            RequestType: typeof(ListMarketsRequest),
            ResponseType: typeof(ListMarketsResponse),
            InputSchemaJson: ListMarketsInputSchema,
            OutputSchemaJson: ListMarketsOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: false);

    public static McpToolDefinition GetKlines { get; } =
        new(
            Name: "get_klines",
            Description: "Read Binance v1 public OHLCV kline candles for a supported symbol and interval. Read-only. Does not place orders.",
            RequestType: typeof(GetKlinesRequest),
            ResponseType: typeof(GetKlinesResponse),
            InputSchemaJson: KlinesInputSchema,
            OutputSchemaJson: KlinesOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: false);

    public static McpToolDefinition GetAccountSnapshot { get; } =
        new(
            Name: "get_account_snapshot",
            Description: "Read a bot-oriented bitFlyer v1 account snapshot for balances, positions, open order count, and read readiness. Read-only. Does not place orders.",
            RequestType: typeof(GetAccountSnapshotRequest),
            ResponseType: typeof(GetAccountSnapshotResponse),
            InputSchemaJson: AccountSnapshotInputSchema,
            OutputSchemaJson: AccountSnapshotOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: true);

    public static McpToolDefinition GetCollateralAccounts { get; } =
        new(
            Name: "get_collateral_accounts",
            Description: "Read bitFlyer v1 collateral account balances. Read-only. Does not place, cancel, deposit, or withdraw.",
            RequestType: typeof(GetCollateralAccountsRequest),
            ResponseType: typeof(GetCollateralAccountsResponse),
            InputSchemaJson: AccountSnapshotInputSchema,
            OutputSchemaJson: CollateralAccountsOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: true);

    public static McpToolDefinition GetBalanceHistory { get; } =
        new(
            Name: "get_balance_history",
            Description: "Read bitFlyer v1 balance history with optional pagination. Read-only. Does not place, cancel, deposit, or withdraw.",
            RequestType: typeof(GetBalanceHistoryRequest),
            ResponseType: typeof(GetBalanceHistoryResponse),
            InputSchemaJson: BalanceHistoryInputSchema,
            OutputSchemaJson: BalanceHistoryOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: true);

    public static McpToolDefinition GetCollateralHistory { get; } =
        new(
            Name: "get_collateral_history",
            Description: "Read bitFlyer v1 collateral history with optional pagination. Read-only. Does not place, cancel, deposit, or withdraw.",
            RequestType: typeof(GetCollateralHistoryRequest),
            ResponseType: typeof(GetCollateralHistoryResponse),
            InputSchemaJson: CollateralHistoryInputSchema,
            OutputSchemaJson: CollateralHistoryOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: true);

    public static McpToolDefinition GetChildOrders { get; } =
        new(
            Name: "get_child_orders",
            Description: "Read bitFlyer v1 child orders with optional filters. Read-only. Does not place or cancel orders.",
            RequestType: typeof(GetChildOrdersRequest),
            ResponseType: typeof(GetChildOrdersResponse),
            InputSchemaJson: ChildOrdersInputSchema,
            OutputSchemaJson: ChildOrdersOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: true);

    public static McpToolDefinition EvaluateOrder { get; } =
        new(
            Name: "evaluate_order",
            Description: "Evaluate whether a supported bitFlyer v1 spot order request can be placed mechanically under current rules and balances. Evaluate-only. Does not place orders.",
            RequestType: typeof(EvaluateOrderRequest),
            ResponseType: typeof(EvaluateOrderResponse),
            InputSchemaJson: EvaluateOrderInputSchema,
            OutputSchemaJson: EvaluateOrderOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: true);

    public static McpToolDefinition EvaluateMarginOrder { get; } =
        new(
            Name: "evaluate_margin_order",
            Description: "Evaluate whether a supported bitFlyer v1 margin order request can be placed mechanically under current rules, collateral, and maintenance state. Evaluate-only. Does not place orders.",
            RequestType: typeof(EvaluateMarginOrderRequest),
            ResponseType: typeof(EvaluateMarginOrderResponse),
            InputSchemaJson: EvaluateMarginOrderInputSchema,
            OutputSchemaJson: EvaluateMarginOrderOutputSchema,
            ReadOnlyHint: true,
            RequiresCredentials: true);

    public static IReadOnlyList<McpToolDefinition> All { get; } =
        [
            GetMarketSnapshot,
            ListMarkets,
            GetKlines,
            GetAccountSnapshot,
            GetCollateralAccounts,
            GetBalanceHistory,
            GetCollateralHistory,
            GetChildOrders,
            EvaluateOrder,
            EvaluateMarginOrder,
        ];

    public static IReadOnlyList<McpToolDefinition> PublicOnly { get; } =
        All.Where(tool => !tool.RequiresCredentials).ToArray();
}
