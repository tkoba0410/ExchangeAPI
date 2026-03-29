using System.Text.Json;
using ExchangeApi.Adapters.McpServer.Schema.Account;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
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
            },
            Status = "active",
        };

        var json = JsonSerializer.Serialize(value);

        Assert.Equal("""{"symbol":"BTC_JPY","bid":"12345000","ask":"12346000","last":"12345500","timestamp":"2026-03-29T10:00:00Z","rules":{"minSize":"0.001","sizeStep":"0.00000001","priceStep":"1"},"status":"active"}""", json);
    }

    [Fact]
    public void GetAccountSnapshotResponse_SerializesUsingDocumentedFieldNames()
    {
        var value = new GetAccountSnapshotResponse
        {
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

        Assert.Equal("""{"balance":{"JPY":"5000000"},"positions":[{"symbol":"FX_BTC_JPY","side":"buy","size":"0.1","avgPrice":"12000000"}],"openOrdersSummary":{"count":0},"margin":{"derivedAvailable":"4500000"},"accountReadiness":"ready"}""", json);
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
                PositionLimitOk = true,
            },
            NormalizedRequest = new EvaluateOrderRequest
            {
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
            },
            Warnings = ["market_order_slippage_risk"],
            Reasons = [],
        };

        var json = JsonSerializer.Serialize(value);

        Assert.Equal("""{"canPlace":true,"checks":{"symbolOk":true,"marketStatusOk":true,"sizeRuleOk":true,"priceRuleOk":true,"balanceOk":true,"positionLimitOk":true},"normalizedRequest":{"symbol":"BTC_JPY","side":"buy","orderType":"market","size":"0.300","price":null},"estimate":{"referencePrice":"12345678","estimatedNotional":"3703703.4"},"warnings":["market_order_slippage_risk"],"reasons":[]}""", json);
    }
}
