using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using System;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeFailFastMapperTests
{
    [Fact]
    public void ToOpenOrder_UnknownSide_Throws()
    {
        var raw = CreateOpenOrdersResponse("unknown-type");

        Assert.Throws<InvalidOperationException>(() =>
            BittradeTradingMapper.ToOpenOrders(new Symbol("BTC/JPY"), raw));
    }

    [Fact]
    public void ToOpenOrder_UnknownType_Throws()
    {
        var raw = CreateOpenOrdersResponse("unknown-type");

        Assert.Throws<InvalidOperationException>(() =>
            BittradeTradingMapper.ToOpenOrders(new Symbol("BTC/JPY"), raw));
    }

    [Fact]
    public void ToOrderStatus_UnknownState_Throws()
    {
        var raw = new RawPrivateDtos.RawOrderDetailResponse(
            Status: "ok",
            Data: new RawPrivateDtos.RawOrderDetail(
                Id: "1",
                Symbol: "btcjpy",
                AccountId: "1",
                Amount: "1",
                Price: "100",
                State: "mystery",
                Type: "buy-limit",
                ClientOrderId: null,
                CreatedAt: DateTimeOffset.UtcNow,
                FinishedAt: null,
                FilledAmount: "0",
                FilledCashAmount: "0",
                Fees: "0"));

        Assert.Throws<InvalidOperationException>(() =>
            BittradeTradingMapper.ToOrderStatus("BTC_JPY", raw, new OrderKey(OrderIdKind.ExchangeOrderId, "1")));
    }

    private static RawPrivateDtos.RawOpenOrdersResponse CreateOpenOrdersResponse(string type) =>
        new(
            Status: "ok",
            Data:
            [
                new RawPrivateDtos.RawOrderSummary(
                    Id: "1",
                    Symbol: "btcjpy",
                    AccountId: "1",
                    Amount: "1",
                    Price: "100",
                    State: "submitted",
                    Type: type,
                    ClientOrderId: null,
                    CreatedAt: DateTimeOffset.UtcNow,
                    FilledAmount: "0")
            ]);
}
