using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Normalize.Mappers;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeFailFastMapperTests
{
    [Fact]
    public void ToOpenOrder_UnknownSide_Throws()
    {
        var raw = CreateOpenOrdersResponse("unknown-type");

        Assert.Throws<ExchangeApiException>(() =>
            BittradeTradingMapper.ToOpenOrders(new Symbol("BTC/JPY"), raw));
    }

    [Fact]
    public void ToOpenOrder_UnknownType_Throws()
    {
        var raw = CreateOpenOrdersResponse("unknown-type");

        Assert.Throws<ExchangeApiException>(() =>
            BittradeTradingMapper.ToOpenOrders(new Symbol("BTC/JPY"), raw));
    }

    [Fact]
    public void ToOrderStatus_UnknownState_Throws()
    {
        var raw = new RawOrderDetailResponse(
            Status: "ok",
            Data: new RawOrderDetail(
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

        Assert.Throws<ExchangeApiException>(() =>
            BittradeTradingMapper.ToOrderStatus("BTC_JPY", raw, new OrderKey(OrderIdKind.ExchangeOrderId, "1")));
    }

    private static RawOpenOrdersResponse CreateOpenOrdersResponse(string type) =>
        new(
            Status: "ok",
            Data:
            [
                new RawOrderSummary(
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
