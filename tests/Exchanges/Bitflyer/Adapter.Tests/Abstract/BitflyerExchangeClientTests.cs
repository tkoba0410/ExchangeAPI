using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ContractTicker = ExchangeApi.Contracts.Common.Dtos.Ticker;
using ContractOrderBook = ExchangeApi.Contracts.Common.Dtos.OrderBook;
using ContractBalance = ExchangeApi.Contracts.Common.Dtos.Balance;
using ContractCancelResult = ExchangeApi.Contracts.Common.Dtos.CancelResult;
using Xunit;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;


namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract
{
    public class BitflyerExchangeClientTests
    {
        [Fact]
        public async Task GetTickerCallAsync_BtcJpy_ReturnsMappedTicker()
        {
            // Arrange
            var raw = new RawPublicDtos.Ticker
            {
                ProductCode = "BTC_JPY",
                Timestamp = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                TickId = 123,
                BestBid = 100m,
                BestAsk = 101m,
                BestBidSize = 1.0m,
                BestAskSize = 2.0m,
                TotalBidDepth = 10m,
                TotalAskDepth = 20m,
                LastTradedPrice = 100.5m,
                Volume = 123.45m,
                VolumeByProduct = 200.0m
            };

            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateDtos.BalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateDtos.RawSendChildOrderResponse());
            var rawApi = new FakeBitflyerPublicApi(raw, privateApi: fakePrivateApi, tradingApi: fakeTradingApi);
            var client = CreateClient(rawApi);

            // Act
            var call = await client.GetTickerCallAsync(new Symbol("BTC/JPY"));
            var ok = Assert.IsType<CallResult<ContractTicker>.Ok>(call.Result);
            ContractTicker ticker = ok.Response;

            Assert.Equal(new Symbol("BTC/JPY"), ticker.Symbol);
            Assert.Equal(new Price(raw.LastTradedPrice), ticker.LastTradedPrice);
            Assert.Equal(raw.Timestamp /* 正規化 */, ticker.Timestamp);
        }

        [Fact]
        public async Task GetTickerCallAsync_UnsupportedSymbol_ThrowsSymbolNotSupportedException()
        {
            // Arrange
            var raw = new RawPublicDtos.Ticker
            {
                ProductCode = "BTC_JPY",
                Timestamp = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                TickId = 123,
                BestBid = 100m,
                BestAsk = 101m,
                BestBidSize = 1.0m,
                BestAskSize = 2.0m,
                TotalBidDepth = 10m,
                TotalAskDepth = 20m,
                LastTradedPrice = 100.5m,
                Volume = 123.45m,
                VolumeByProduct = 200.0m
            };

            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateDtos.BalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateDtos.RawSendChildOrderResponse());
            var rawApi = new FakeBitflyerPublicApi(raw, privateApi: fakePrivateApi, tradingApi: fakeTradingApi);
            var client = CreateClient(rawApi);

            var call = await client.GetTickerCallAsync(Symbol.Empty);
            var err = Assert.IsType<CallResult<ContractTicker>.Err>(call.Result);
            Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);


        }

        [Fact]
        public async Task GetOrderBookAsync_ReturnsMappedOrderBook()
        {
            var rawTicker = new RawPublicDtos.Ticker
            {
                ProductCode = "BTC_JPY",
                Timestamp = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
                TickId = 123,
                BestBid = 100m,
                BestAsk = 101m,
                BestBidSize = 1.0m,
                BestAskSize = 2.0m,
                TotalBidDepth = 10m,
                TotalAskDepth = 20m,
                LastTradedPrice = 100.5m,
                Volume = 123.45m,
                VolumeByProduct = 200.0m
            };

            var boardRaw = new RawPublicDtos.Board
            {
                MidPrice = 100.5m,
                Bids = new[]
                {
                    new RawPublicDtos.BoardEntry { Price = 100m, Size = 0.1m },
                },
                Asks = new[]
                {
                    new RawPublicDtos.BoardEntry { Price = 101m, Size = 0.2m },
                }
            };

            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateDtos.BalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateDtos.RawSendChildOrderResponse());
            var rawApi = new FakeBitflyerPublicApi(rawTicker, boardRaw, fakePrivateApi, fakeTradingApi);
            var client = CreateClient(rawApi);

            var call = await client.GetBoardCallAsync(new Symbol("BTC/JPY"));
            var ok = Assert.IsType<CallResult<ContractOrderBook>.Ok>(call.Result);
            ContractOrderBook board = ok.Response;

            Assert.Single(board.Bids);
            Assert.Single(board.Asks);
            Assert.Equal(new Price(100m), board.Bids[0].Price);
            Assert.Equal(new Size(0.1m), board.Bids[0].Size);
        }

        [Fact]
        public async Task GetBalanceCallAsync_ReturnsMappedBalances()
        {
            var rawTicker = new RawPublicDtos.Ticker { ProductCode = "BTC_JPY" };
            var balances = new[]
            {
                new RawPrivateDtos.BalanceResponse { CurrencyCode = "JPY", Amount = 10000m, Available = 8000m },
                new RawPrivateDtos.BalanceResponse { CurrencyCode = "BTC", Amount = 1.5m, Available = 1.2m },
            };

            var privateApi = new FakeBitflyerPrivateApi(balances);
            var tradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateDtos.RawSendChildOrderResponse());
            var rawApi = new FakeBitflyerPublicApi(rawTicker, privateApi: privateApi, tradingApi: tradingApi);
            var client = CreateClient(rawApi);

            var call = await client.GetBalanceCallAsync();
            var ok = Assert.IsType<CallResult<IReadOnlyList<ContractBalance>>.Ok>(call.Result);
            IReadOnlyList<ContractBalance> result = ok.Response;

            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Currency == CurrencyCode.Jpy && b.Amount == 10000m && b.Available == 8000m);
            Assert.Contains(result, b => b.Currency == CurrencyCode.Btc && b.Amount == 1.5m && b.Available == 1.2m);
        }

        [Fact]
        public async Task CancelOrderAsync_NullResponse_Throws()
        {
            var rawTicker = new RawPublicDtos.Ticker { ProductCode = "BTC_JPY" };
            var accountApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateDtos.BalanceResponse>());
            var exception = new ExchangeApiException(
                message: "cancel failed",
                statusCode: System.Net.HttpStatusCode.InternalServerError,
                exchangeErrorCode: "INTERNAL_SERVER_ERROR");
            var tradingApi = new FakeBitflyerPrivateTradingApi(
                new RawPrivateDtos.RawSendChildOrderResponse(),
                exceptionToThrow: exception);
            var rawApi = new FakeBitflyerPublicApi(rawTicker, new RawPublicDtos.Board { Bids = Array.Empty<RawPublicDtos.BoardEntry>(), Asks = Array.Empty<RawPublicDtos.BoardEntry>() }, accountApi, tradingApi);
            var client = CreateClient(rawApi);

            var call = await client.CancelOrderCallAsync(new Symbol("BTC/JPY"), new OrderKey(OrderIdKind.AcceptanceId, "id-1"));
            var err = Assert.IsType<CallResult<ContractCancelResult>.Err>(call.Result);
            Assert.Equal(CallErrorKind.Http, err.Error.Kind);
        }

        private static BitflyerExchangeClient CreateClient(IBitflyerRawApi raw)
        {
            var markets = BitflyerTestHelpers.CreateResolver();
            var normalized = BitflyerTestHelpers.CreateNormalizedApi(raw, markets);
            return new BitflyerExchangeClient(normalized);
        }
    }
}
