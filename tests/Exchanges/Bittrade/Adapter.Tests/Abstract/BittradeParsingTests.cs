using System.Collections.Generic;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeParsingTests
{
    [Fact]
    public void MapBalances_InvalidBalance_ThrowsWithContext()
    {
        var data = new RawPrivateModels.RawBalanceData(
            Id: "1",
            Type: "spot",
            State: "working",
            List: new List<RawPrivateModels.RawBalanceEntry>
            {
                new RawPrivateModels.RawBalanceEntry(Currency: "btc", Type: "trade", Balance: "bad")
            });

        var ex = Assert.Throws<BittradeNormalizedException>(() => BittradeNormalizer.NormalizeBalances(data));

        Assert.Contains("RawBalanceEntry.balance", ex.Message);
        Assert.Contains("bad", ex.Message);
    }
}
