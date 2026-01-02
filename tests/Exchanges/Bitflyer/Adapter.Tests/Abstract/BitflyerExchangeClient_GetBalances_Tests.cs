using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Wire.Types.RawProductCode;
using RawTicker = ExchangeApi.Exchanges.Bitflyer.Raw.Ticker;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests
{
    public class BitflyerExchangeClient_GetBalances_Tests
    {
        [Fact]
        public async Task GetBalancesAsync_ReturnsMappedBalanceList()
        {
            // Arrange
            var rawBalances = new[]
            {
                new BalanceResponse
                {
                    CurrencyCode = "JPY",
                    Amount = 100000m,
                    Available = 80000m,
                },
                new BalanceResponse
                {
                    CurrencyCode = "BTC",
                    Amount = 0.01m,
                    Available = 0.009m,
                },
            };

            var fakePublicApi = CreateDummyPublicApi();
            var fakePrivateApi = new FakeBitflyerPrivateApi(rawBalances);
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());

            var client = CreateClient(fakePublicApi, fakePrivateApi, fakeTradingApi);

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
            var rawBalances = Array.Empty<BalanceResponse>();

            var fakePublicApi = CreateDummyPublicApi();
            var fakePrivateApi = new FakeBitflyerPrivateApi(rawBalances);
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());

            var client = CreateClient(fakePublicApi, fakePrivateApi, fakeTradingApi);

            // Act
            IReadOnlyList<Balance> result = await client.GetBalancesAsync();

            // Assert
            Assert.Empty(result);
        }

        private static BitflyerExchangeClient CreateClient(
            IBitflyerRawMarketDataApi marketData,
            IBitflyerRawAccountApi accountApi,
            IBitflyerRawPrivateTradingApi tradingApi)
        {
            var markets = BitflyerTestHelpers.CreateResolver();
            var normalizedMarket = BitflyerTestHelpers.CreateMarketData(marketData);
            var normalizedAccount = BitflyerTestHelpers.CreateAccountApi(accountApi, markets);
            var normalizedMargin = BitflyerTestHelpers.CreateMarginApi(accountApi, markets);
            var normalizedTrading = BitflyerTestHelpers.CreateTradingApi(tradingApi, accountApi, markets);

            return new BitflyerExchangeClient(normalizedMarket, normalizedAccount, normalizedMargin, normalizedTrading);
        }

        private static IBitflyerRawMarketDataApi CreateDummyPublicApi()
        {
            // 既存の Ticker 用テストと揃えるため、適当な値のダミーを作って流用する。
            var rawTicker = new RawTicker
            {
                ProductCode = new RawProductCode("BTC_JPY"),
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
