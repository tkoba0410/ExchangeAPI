using System.Collections.Generic;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Raw;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeParsingTests
{
    [Fact]
    public void MapBalances_InvalidBalance_ThrowsWithContext()
    {
        var data = new RawBalanceData(
            Id: "1",
            Type: "spot",
            State: "working",
            List: new List<RawBalanceEntry>
            {
                new RawBalanceEntry(Currency: "btc", Type: "trade", Balance: "bad")
            });

        var ex = Assert.Throws<ExchangeApiException>(() => BittradeNormalizer.NormalizeBalances(data));

        Assert.Contains("RawBalanceEntry.balance", ex.Message);
        Assert.Contains("bad", ex.Message);
    }
}
