using System.Collections.Generic;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;
using ExchangeApi.Exchanges.Bittrade.Raw;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeParsingTests
{
    [Fact]
    public void MapBalances_InvalidBalance_ThrowsWithContext()
    {
        var data = new BalanceData(
            Id: "1",
            Type: "spot",
            State: "working",
            List: new List<BalanceEntry>
            {
                new BalanceEntry(Currency: "btc", Type: "trade", Balance: "bad")
            });

        var ex = Assert.Throws<ExchangeApiException>(() => BittradeMapper.MapBalances(data));

        Assert.Contains("BalanceEntry.balance", ex.Message);
        Assert.Contains("bad", ex.Message);
    }
}
