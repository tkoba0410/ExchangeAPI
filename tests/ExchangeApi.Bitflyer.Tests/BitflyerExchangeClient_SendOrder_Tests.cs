using System.Threading.Tasks;
using ExchangeApi.Abstractions.Dtos;
using ExchangeApi.Bitflyer.Models;
using ExchangeApi.Bitflyer.Tests.Fakes;
using Xunit;

namespace ExchangeApi.Bitflyer.Tests;

public sealed class BitflyerExchangeClient_SendOrder_Tests
{
    [Fact]
    public async Task SendOrderAsync_MapsDomainToDtoAndReturnsResult()
    {
        var fakePublic = new FakeBitflyerPublicApi(new BitflyerTickerRaw());
        var fakeAccount = new FakeBitflyerPrivateApi(new BitflyerBalanceResponse[0]);
        var tradingResponse = new BitflyerSendChildOrderResponse { ChildOrderAcceptanceId = "ACCEPT-123" };
        var fakeTrading = new FakeBitflyerPrivateTradingApi(tradingResponse);

        var client = new BitflyerExchangeClient(fakePublic, fakeAccount, fakeTrading);

        var order = new OrderRequest(
            ProductCode: "BTC_JPY",
            Side: OrderSide.Buy,
            OrderType: OrderType.Market,
            Size: 0.01m);

        var result = await client.SendOrderAsync(order);

        Assert.Equal("ACCEPT-123", result.OrderId);
        Assert.NotNull(fakeTrading.LastRequest);
        Assert.Equal("BTC_JPY", fakeTrading.LastRequest!.ProductCode);
        Assert.Equal("BUY", fakeTrading.LastRequest!.Side);
        Assert.Equal("MARKET", fakeTrading.LastRequest!.ChildOrderType);
        Assert.Equal(0.01m, fakeTrading.LastRequest!.Size);
    }
}

