using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using Xunit;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract
{
    public class BitflyerExchangeClient_GetBalances_Tests
    {
        [Fact]
        public async Task GetBalanceCallAsync_ReturnsMappedBalanceList()
        {
            // Arrange
            var rawBalances = new[]
            {
                new RawPrivateDtos.BalanceResponse
                {
                    CurrencyCode = "JPY",
                    Amount = 100000m,
                    Available = 80000m,
                },
                new RawPrivateDtos.BalanceResponse
                {
                    CurrencyCode = "BTC",
                    Amount = 0.01m,
                    Available = 0.009m,
                },
            };

            var fakePrivateApi = new FakeBitflyerPrivateApi(rawBalances);
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateDtos.SendChildOrderResponse());

            var raw = CreateDummyPublicApi(fakePrivateApi, fakeTradingApi);
            var client = CreateClient(raw);

            // Act
            var call = await client.GetBalanceCallAsync();
            var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<GetBalanceResponse>.Ok>(call.Result);
            IReadOnlyList<Balance> result = ok.Response.Value;

            // Assert
            Assert.Equal(2, result.Count);

            var jpy = result[0];
            Assert.Equal(CurrencyCode.Jpy, jpy.Currency);
            Assert.Equal(100000m, jpy.Amount);
            Assert.Equal(80000m, jpy.Available);

            var btc = result[1];
            Assert.Equal(CurrencyCode.Btc, btc.Currency);
            Assert.Equal(0.01m, btc.Amount);
            Assert.Equal(0.009m, btc.Available);
        }

        [Fact]
        public async Task GetBalanceCallAsync_WhenRawReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var rawBalances = Array.Empty<RawPrivateDtos.BalanceResponse>();

            var fakePrivateApi = new FakeBitflyerPrivateApi(rawBalances);
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateDtos.SendChildOrderResponse());

            var raw = CreateDummyPublicApi(fakePrivateApi, fakeTradingApi);
            var client = CreateClient(raw);

            // Act
            var call = await client.GetBalanceCallAsync();
            var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<GetBalanceResponse>.Ok>(call.Result);
            IReadOnlyList<Balance> result = ok.Response.Value;

            // Assert
            Assert.Empty(result);
        }

        private static BitflyerExchangeClient CreateClient(IBitflyerRawApi raw)
        {
            var markets = BitflyerTestHelpers.CreateResolver();
            var normalized = BitflyerTestHelpers.CreateNormalizedApi(raw, markets);
            return new BitflyerExchangeClient(normalized);
        }

        private static IBitflyerRawApi CreateDummyPublicApi(
            FakeBitflyerPrivateApi privateApi,
            FakeBitflyerPrivateTradingApi tradingApi)
        {
            // 既存の RawPublicDtos.GetTickerResponse 用テストと揃えるため、適当な値のダミーを作って流用する。
            var rawTicker = new RawPublicDtos.GetTickerResponse
            {
                ProductCode = "BTC_JPY",
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

            return new FakeBitflyerPublicApi(rawTicker, privateApi: privateApi, tradingApi: tradingApi);
        }
    }
}
