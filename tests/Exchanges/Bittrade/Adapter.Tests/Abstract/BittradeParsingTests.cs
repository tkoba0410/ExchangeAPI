using System.Collections.Generic;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Call;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.Mappers;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

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

        var ex = Assert.Throws<BittradeNormalizedException>(() => BittradeNormalizer.NormalizeBalances(data));

        Assert.Contains("RawBalanceEntry.balance", ex.Message);
        Assert.Contains("bad", ex.Message);
    }
}
