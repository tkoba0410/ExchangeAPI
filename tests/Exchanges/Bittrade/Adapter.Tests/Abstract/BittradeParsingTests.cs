using System;
using System.Collections.Generic;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeParsingTests
{
    [Fact]
    public void MapBalances_InvalidBalance_ThrowsWithContext()
    {
        var data = new RawPrivateDtos.GetAccountsBalanceByAccountIdData(
            Id: "1",
            Type: "spot",
            State: "working",
            List: new List<RawPrivateDtos.GetAccountsBalanceByAccountIdEntry>
            {
                new RawPrivateDtos.GetAccountsBalanceByAccountIdEntry(Currency: "btc", Type: "trade", Balance: "bad")
            });

        var ok = BittradeNormalizer.TryNormalizeBalances(data, out var balances, out var error);
        Assert.False(ok);
        Assert.Null(balances);
        Assert.NotNull(error);
        Assert.Contains("GetAccountsBalanceByAccountIdEntry.balance", error!.Message, StringComparison.Ordinal);
        Assert.Contains("bad", error.Message, StringComparison.Ordinal);
    }
}
