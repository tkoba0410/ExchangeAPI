using System.Text.Json;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Adapters.McpServer.Schema.Klines;
using ExchangeApi.Adapters.McpServer.Schema.MarginEvaluation;
using ExchangeApi.Adapters.McpServer.Schema.Market;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class SchemaContractTests
{
    [Fact]
    public void GetMarketSnapshotResponse_SerializesUsingDocumentedFieldNames()
    {
        var value = new GetMarketSnapshotResponse
        {
            Symbol = "BTC_JPY",
            Bid = "12345000",
            Ask = "12346000",
            Last = "12345500",
            Timestamp = "2026-03-29T10:00:00Z",
            Rules = new MarketSnapshotRules
            {
                MinSize = "0.001",
                SizeStep = "0.00000001",
                PriceStep = "1",
                MinSizeSourceKind = MarketRuleSourceKinds.OfficialDocumented,
                MinSizeSourceRef = "https://bitflyer.com/ja-jp/s/commission",
                SizeStepSourceKind = MarketRuleSourceKinds.OfficialDocumented,
                SizeStepSourceRef = "https://bitflyer.com/ja-jp/s/commission",
                PriceStepSourceKind = MarketRuleSourceKinds.AdapterInferred,
                PriceStepSourceRef = "adapter://bitflyer-jpy-price-step.v1",
            },
            Status = "active",
        };

        var json = JsonSerializer.Serialize(value);

        Assert.Equal("""{"symbol":"BTC_JPY","bid":"12345000","ask":"12346000","last":"12345500","timestamp":"2026-03-29T10:00:00Z","rules":{"minSize":"0.001","sizeStep":"0.00000001","priceStep":"1","minSizeSourceKind":"official_documented","minSizeSourceRef":"https://bitflyer.com/ja-jp/s/commission","sizeStepSourceKind":"official_documented","sizeStepSourceRef":"https://bitflyer.com/ja-jp/s/commission","priceStepSourceKind":"adapter_inferred","priceStepSourceRef":"adapter://bitflyer-jpy-price-step.v1"},"status":"active"}""", json);
    }

    [Fact]
    public void GetAccountSnapshotResponse_SerializesUsingDocumentedFieldNames()
    {
        var value = new GetAccountSnapshotResponse
        {
            PermissionModel = PermissionModelIds.BitflyerPrivateReadV1,
            Balance = new Dictionary<string, string>
            {
                ["JPY"] = "5000000",
            },
            Positions =
            [
                new AccountPositionSnapshot
                {
                    Symbol = "FX_BTC_JPY",
                    Side = "buy",
                    Size = "0.1",
                    AvgPrice = "12000000",
                },
            ],
            OpenOrdersSummary = new OpenOrdersSummary
            {
                Count = 0,
            },
            Margin = new AccountMarginSnapshot
            {
                DerivedAvailable = "4500000",
            },
            AccountReadiness = "ready",
        };

        var json = JsonSerializer.Serialize(value);

        Assert.Equal("""{"permissionModel":"bitflyer_private_read_v1","balance":{"JPY":"5000000"},"positions":[{"symbol":"FX_BTC_JPY","side":"buy","size":"0.1","avgPrice":"12000000"}],"openOrdersSummary":{"count":0},"margin":{"derivedAvailable":"4500000"},"accountReadiness":"ready"}""", json);
    }

    [Fact]
    public void EvaluateOrderResponse_SerializesUsingDocumentedFieldNames()
    {
        var value = new EvaluateOrderResponse
        {
            CanPlace = true,
            Checks = new EvaluateOrderChecks
            {
                SymbolOk = true,
                MarketStatusOk = true,
                SizeRuleOk = true,
                PriceRuleOk = true,
                BalanceOk = true,
                FeeCoverageOk = null,
                ProjectedExposureOk = true,
            },
            NormalizedRequest = new EvaluateOrderRequest
            {
                Venue = "bitflyer",
                AccountContext = "default",
                Symbol = "BTC_JPY",
                Side = "buy",
                OrderType = "market",
                Size = "0.300",
                Price = null,
            },
            Estimate = new EvaluateOrderEstimate
            {
                ReferencePrice = "12345678",
                EstimatedNotional = "3703703.4",
                EstimatedFee = null,
                EstimatedFeeSourceKind = null,
            },
            Warnings = ["market_order_slippage_risk"],
            Reasons = [],
        };

        var json = JsonSerializer.Serialize(value);

        Assert.Equal("""{"canPlace":true,"checks":{"symbolOk":true,"marketStatusOk":true,"sizeRuleOk":true,"priceRuleOk":true,"balanceOk":true,"feeCoverageOk":null,"projectedExposureOk":true},"normalizedRequest":{"venue":"bitflyer","accountContext":"default","symbol":"BTC_JPY","side":"buy","orderType":"market","size":"0.300","price":null},"estimate":{"referencePrice":"12345678","estimatedNotional":"3703703.4","estimatedFee":null,"estimatedFeeSourceKind":null},"warnings":["market_order_slippage_risk"],"reasons":[]}""", json);
    }

    [Fact]
    public void EvaluateMarginOrderResponse_SerializesUsingDocumentedFieldNames()
    {
        var value = new EvaluateMarginOrderResponse
        {
            CanPlace = true,
            Checks = new EvaluateMarginOrderChecks
            {
                SymbolOk = true,
                MarketStatusOk = true,
                SizeRuleOk = true,
                PriceRuleOk = true,
                CollateralCoverageOk = true,
                FeeCoverageOk = null,
                ProjectedMarginExposureOk = true,
                CurrentMaintenanceOk = true,
            },
            NormalizedRequest = new EvaluateMarginOrderRequest
            {
                Venue = "bitflyer",
                AccountContext = "default",
                Symbol = "FX_BTC_JPY",
                Side = "buy",
                OrderType = "market",
                Size = "0.100",
                Price = null,
            },
            Estimate = new EvaluateMarginOrderEstimate
            {
                ReferencePrice = "12345678",
                EstimatedNotional = "1234567.8",
                EstimatedRequiredCollateral = "493827.12",
                CurrentMaxLeverage = "2.5",
                CurrentKeepRate = "8",
                MinimumKeepRate = "1.5",
                EstimatedFee = null,
                EstimatedFeeSourceKind = null,
            },
            Warnings = [EvaluateMarginOrderWarningCodes.MarketOrderSlippageRisk],
            Reasons = [],
        };

        var json = JsonSerializer.Serialize(value);

        Assert.Equal("""{"canPlace":true,"checks":{"symbolOk":true,"marketStatusOk":true,"sizeRuleOk":true,"priceRuleOk":true,"collateralCoverageOk":true,"feeCoverageOk":null,"projectedMarginExposureOk":true,"currentMaintenanceOk":true},"normalizedRequest":{"venue":"bitflyer","accountContext":"default","symbol":"FX_BTC_JPY","side":"buy","orderType":"market","size":"0.100","price":null},"estimate":{"referencePrice":"12345678","estimatedNotional":"1234567.8","estimatedRequiredCollateral":"493827.12","currentMaxLeverage":"2.5","currentKeepRate":"8","minimumKeepRate":"1.5","estimatedFee":null,"estimatedFeeSourceKind":null},"warnings":["market_order_slippage_risk"],"reasons":[]}""", json);
    }

    [Fact]
    public void EvaluateOrderRequest_AllowsMissingPriceForMarketOrders()
    {
        var json = """{"venue":"bitflyer","accountContext":"default","symbol":"BTC_JPY","side":"buy","orderType":"market","size":"0.001"}""";

        var value = JsonSerializer.Deserialize<EvaluateOrderRequest>(json);

        Assert.NotNull(value);
        Assert.Equal("bitflyer", value.Venue);
        Assert.Equal("default", value.AccountContext);
        Assert.Equal("BTC_JPY", value.Symbol);
        Assert.Equal("buy", value.Side);
        Assert.Equal("market", value.OrderType);
        Assert.Equal("0.001", value.Size);
        Assert.Null(value.Price);
    }

    [Fact]
    public void GetKlinesResponse_SerializesUsingDocumentedFieldNames()
    {
        var value = new GetKlinesResponse
        {
            Venue = "binance",
            Symbol = "BTCUSDT",
            Interval = "1h",
            Candles =
            [
                new KlineCandle
                {
                    OpenTime = "2026-03-30T00:00:00Z",
                    CloseTime = "2026-03-30T00:59:59.999Z",
                    Open = "10700000",
                    High = "10750000",
                    Low = "10680000",
                    Close = "10720000",
                    Volume = "123.45",
                    QuoteVolume = "1323000000",
                    TradeCount = 12345,
                    TakerBuyBaseVolume = "61.72",
                    TakerBuyQuoteVolume = "662100000",
                },
            ],
        };

        var json = JsonSerializer.Serialize(value);

        Assert.Equal("""{"venue":"binance","symbol":"BTCUSDT","interval":"1h","candles":[{"openTime":"2026-03-30T00:00:00Z","closeTime":"2026-03-30T00:59:59.999Z","open":"10700000","high":"10750000","low":"10680000","close":"10720000","volume":"123.45","quoteVolume":"1323000000","tradeCount":12345,"takerBuyBaseVolume":"61.72","takerBuyQuoteVolume":"662100000"}]}""", json);
    }

    [Fact]
    public void ListMarketsResponse_SerializesUsingDocumentedFieldNames()
    {
        var value = new ListMarketsResponse
        {
            Markets =
            [
                new SupportedMarketDescriptor
                {
                    Venue = "bitflyer",
                    Symbol = "BTC_JPY",
                    Capabilities = ["get_market_snapshot", "evaluate_order"],
                },
            ],
        };

        var json = JsonSerializer.Serialize(value);

        Assert.Equal("""{"markets":[{"venue":"bitflyer","symbol":"BTC_JPY","capabilities":["get_market_snapshot","evaluate_order"]}]}""", json);
    }
}
