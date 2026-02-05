using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using System;
using ExchangeApi.Exchanges.Bittrade.Api.Raw;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Mappers;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeFailFastMapperTests
{
    [Fact]
    public void ToOpenOrder_UnknownSide_Throws()
    {
        var raw = CreateOpenOrdersResponse("unknown-type");

        var ok = BittradeTradingMapper.TryToOpenOrders(new Symbol("BTC/JPY"), raw, out var orders, out var error);
        Assert.False(ok);
        Assert.Null(orders);
        Assert.NotNull(error);
    }

    [Fact]
    public void ToOpenOrder_UnknownType_Throws()
    {
        var raw = CreateOpenOrdersResponse("unknown-type");

        var ok = BittradeTradingMapper.TryToOpenOrders(new Symbol("BTC/JPY"), raw, out var orders, out var error);
        Assert.False(ok);
        Assert.Null(orders);
        Assert.NotNull(error);
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

        var ok = BittradeTradingMapper.TryToOrderStatus(
            ProductCode.Parse("BTC_JPY"),
            raw,
            new OrderKey(OrderIdKind.ExchangeOrderId, "1"),
            out var status,
            out var error);
        Assert.False(ok);
        Assert.Null(status);
        Assert.NotNull(error);
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
