using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Contract.Dtos;
using Exchange.Bitflyer.Abstract;
using Exchange.Bitflyer.Raw;
using Exchange.Bitflyer.Tests.Fakes;
using Xunit;

namespace Exchange.Bitflyer.Tests
{
    public class BitflyerExchangeClient_GetBalances_Tests
    {
        [Fact]
        public async Task GetBalancesAsync_ReturnsMappedBalanceList()
        {
            // Arrange
            var rawBalances = new[]
            {
                new BitflyerBalanceResponse
                {
                    CurrencyCode = "JPY",
                    Amount = 100000m,
                    Available = 80000m,
                },
                new BitflyerBalanceResponse
                {
                    CurrencyCode = "BTC",
                    Amount = 0.01m,
                    Available = 0.009m,
                },
            };

            var fakePublicApi = CreateDummyPublicApi();
            var fakePrivateApi = new FakeBitflyerPrivateApi(rawBalances);
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());

            var client = new BitflyerExchangeClient(fakePublicApi, fakePrivateApi, fakeTradingApi);

            // Act
            IReadOnlyList<Balance> result = await client.GetBalancesAsync();

            // Assert
            Assert.Equal(2, result.Count);

            var jpy = result[0];
            Assert.Equal("JPY", jpy.Currency);
            Assert.Equal(100000m, jpy.Amount);
            Assert.Equal(80000m, jpy.Available);

            var btc = result[1];
            Assert.Equal("BTC", btc.Currency);
            Assert.Equal(0.01m, btc.Amount);
            Assert.Equal(0.009m, btc.Available);
        }

        [Fact]
        public async Task GetBalancesAsync_WhenRawReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var rawBalances = Array.Empty<BitflyerBalanceResponse>();

            var fakePublicApi = CreateDummyPublicApi();
            var fakePrivateApi = new FakeBitflyerPrivateApi(rawBalances);
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());

            var client = new BitflyerExchangeClient(fakePublicApi, fakePrivateApi, fakeTradingApi);

            // Act
            IReadOnlyList<Balance> result = await client.GetBalancesAsync();

            // Assert
            Assert.Empty(result);
        }

        private static IBitflyerPublicApi CreateDummyPublicApi()
        {
            // 既存の Ticker 用テストと揃えるため、適当な値のダミーを作って流用する。
            var rawTicker = new BitflyerTicker
            {
                ProductCode = ProductCode.BtcJpy,
                Timestamp = DateTimeOffset.UnixEpoch,
                TickId = 0,
                BestBid = 0m,
                BestAsk = 0m,
                BestBidSize = 0m,
                BestAskSize = 0m,
                TotalBidDepth = 0m,
                TotalAskDepth = 0m,
                LastTradedPrice = 0m,
                Volume = 0m,
                VolumeByProduct = 0m,
            };

            return new FakeBitflyerPublicApi(rawTicker);
        }
    }
}
