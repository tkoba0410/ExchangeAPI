using System;
using System.Collections.Generic;
using ExchangeApi.Exchanges.Bittrade.Api.Raw;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Mappers;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeParsingTests
{
    [Fact]
    public void MapBalances_InvalidBalance_ThrowsWithContext()
    {
        var data = new RawPrivateDtos.RawBalanceData(
            Id: "1",
            Type: "spot",
            State: "working",
            List: new List<RawPrivateDtos.RawBalanceEntry>
            {
                new RawPrivateDtos.RawBalanceEntry(Currency: "btc", Type: "trade", Balance: "bad")
            });

        var ex = Assert.Throws<InvalidOperationException>(() => BittradeNormalizer.NormalizeBalances(data));

        Assert.Contains("RawBalanceEntry.balance", ex.Message);
        Assert.Contains("bad", ex.Message);
    }
}
