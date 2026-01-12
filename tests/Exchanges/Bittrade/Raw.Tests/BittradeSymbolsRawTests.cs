using ExchangeApi.Exchanges.Bittrade.Raw;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Tests;

public sealed class BittradeSymbolsRawTests
{
    [Fact]
    public void Deserialize_Symbols_MinOrderFields_AreString()
    {
        const string json = """
            {
              "status": "ok",
              "data": [
                {
                  "symbol": "btcjpy",
                  "base-currency": "btc",
                  "quote-currency": "jpy",
                  "price-precision": 0,
                  "amount-precision": 4,
                  "value-precision": 0,
                  "min-order-amt": 0.0001,
                  "min-order-value": 1000,
                  "state": "online"
                }
              ]
            }
            """;

        var response = BittradeRawJson.DeserializeOrThrow<RawSymbolsResponse>(json, "Bittrade.GetSymbols");
        var symbol = Assert.Single(response.Data!);

        Assert.Equal("0.0001", symbol.MinOrderAmount);
        Assert.Equal("1000", symbol.MinOrderValue);
    }
}
