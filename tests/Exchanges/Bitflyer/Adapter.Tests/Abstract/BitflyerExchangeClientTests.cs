using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ContractSide = ExchangeApi.Primitives.DomainCommon.Enums.Side;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ContractTicker = ExchangeApi.Contracts.Common.Dtos.Market.Ticker;
using ContractOrderBook = ExchangeApi.Contracts.Common.Dtos.Market.OrderBook;
using ContractBalance = ExchangeApi.Contracts.Common.Dtos.Account.Balance;
using ContractCancelResult = ExchangeApi.Contracts.Common.Dtos.Trading.CancelResult;
using Xunit;


namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract
{
    public class BitflyerExchangeClientTests
    {
        [Fact]
        public async Task GetTickerCallAsync_BtcJpy_ReturnsMappedTicker()
        {
            // Arrange
            var raw = new RawPublicModels.Ticker
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

            var fakeApi = new FakeBitflyerPublicApi(raw);
            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateModels.BalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateModels.RawSendChildOrderResponse());
            var client = CreateClient(fakeApi, fakePrivateApi, fakeTradingApi);

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
            var raw = new RawPublicModels.Ticker
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

            var fakeApi = new FakeBitflyerPublicApi(raw);
            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateModels.BalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateModels.RawSendChildOrderResponse());
            var client = CreateClient(fakeApi, fakePrivateApi, fakeTradingApi);

            var call = await client.GetTickerCallAsync(Symbol.Empty);
            var err = Assert.IsType<CallResult<ContractTicker>.Err>(call.Result);
            Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);


        }

        [Fact]
        public async Task GetOrderBookAsync_ReturnsMappedOrderBook()
        {
            var rawTicker = new RawPublicModels.Ticker
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

            var boardRaw = new RawPublicModels.Board
            {
                MidPrice = 100.5m,
                Bids = new[]
                {
                    new RawPublicModels.BoardEntry { Price = 100m, Size = 0.1m },
                },
                Asks = new[]
                {
                    new RawPublicModels.BoardEntry { Price = 101m, Size = 0.2m },
                }
            };

            var fakeApi = new FakeBitflyerPublicApi(rawTicker, boardRaw);
            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateModels.BalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateModels.RawSendChildOrderResponse());
            var client = CreateClient(fakeApi, fakePrivateApi, fakeTradingApi);

            var call = await client.GetOrderBookCallAsync(new Symbol("BTC/JPY"));
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
            var rawTicker = new RawPublicModels.Ticker { ProductCode = "BTC_JPY" };
            var balances = new[]
            {
                new RawPrivateModels.BalanceResponse { CurrencyCode = "JPY", Amount = 10000m, Available = 8000m },
                new RawPrivateModels.BalanceResponse { CurrencyCode = "BTC", Amount = 1.5m, Available = 1.2m },
            };

            var publicApi = new FakeBitflyerPublicApi(rawTicker);
            var privateApi = new FakeBitflyerPrivateApi(balances);
            var tradingApi = new FakeBitflyerPrivateTradingApi(new RawPrivateModels.RawSendChildOrderResponse());
            var client = CreateClient(publicApi, privateApi, tradingApi);

            var call = await client.GetBalancesCallAsync();
            var ok = Assert.IsType<CallResult<IReadOnlyList<ContractBalance>>.Ok>(call.Result);
            IReadOnlyList<ContractBalance> result = ok.Response;

            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Currency == "JPY" && b.Amount == 10000m && b.Available == 8000m);
            Assert.Contains(result, b => b.Currency == "BTC" && b.Amount == 1.5m && b.Available == 1.2m);
        }

        [Fact]
        public async Task CancelOrderAsync_NullResponse_Throws()
        {
            var rawTicker = new RawPublicModels.Ticker { ProductCode = "BTC_JPY" };
            var publicApi = new FakeBitflyerPublicApi(rawTicker, new RawPublicModels.Board { Bids = Array.Empty<RawPublicModels.BoardEntry>(), Asks = Array.Empty<RawPublicModels.BoardEntry>() });
            var accountApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateModels.BalanceResponse>());
            var tradingApi = new NullCancelTradingApi();
            var client = CreateClient(publicApi, accountApi, tradingApi);

            var call = await client.CancelOrderCallAsync(new Symbol("BTC/JPY"), new OrderKey(OrderIdKind.AcceptanceId, "id-1"));
            var err = Assert.IsType<CallResult<ContractCancelResult>.Err>(call.Result);
            Assert.Equal(CallErrorKind.Http, err.Error.Kind);
        }

        private static BitflyerExchangeClient CreateClient(
            IBitflyerRawMarketDataApi marketData,
            IBitflyerPrivateApi accountApi,
            IBitflyerRawTradingApi tradingApi)
        {
            var markets = BitflyerTestHelpers.CreateResolver();
            var normalizedMarket = BitflyerTestHelpers.CreateMarketData(marketData);
            var normalizedAccount = BitflyerTestHelpers.CreateAccountApi(accountApi, markets);
            var normalizedTrading = BitflyerTestHelpers.CreateTradingApi(tradingApi, markets);

            return new BitflyerExchangeClient(normalizedMarket, normalizedAccount, normalizedTrading);
        }

        private sealed class NullCancelTradingApi : IBitflyerRawTradingApi
        {
            public Task<Call<string, RawPrivateModels.RawSendChildOrderResponse>> SendChildOrderCallAsync(
                string bodyJson,
                CancellationToken cancellationToken = default) =>
                Task.FromResult(OkCall(bodyJson, new RawPrivateModels.RawSendChildOrderResponse()));

            public Task<Call<string, RawPrivateModels.RawSendParentOrderResponse>> SendParentOrderCallAsync(
                string bodyJson,
                CancellationToken cancellationToken = default) =>
                Task.FromResult(OkCall(bodyJson, new RawPrivateModels.RawSendParentOrderResponse { ParentOrderAcceptanceId = "PARENT-1" }));

            public Task<Call<RawPrivateModels.CancelChildOrderRequest, RawPrivateModels.RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
                RawPrivateModels.CancelChildOrderRequest request,
                CancellationToken cancellationToken = default) =>
                Task.FromResult(ErrCall<RawPrivateModels.CancelChildOrderRequest, RawPrivateModels.RawCancelChildOrderResponse>(request, 500));

            public Task<Call<RawPrivateModels.CancelParentOrderRequest, RawPrivateModels.RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
                RawPrivateModels.CancelParentOrderRequest request,
                CancellationToken cancellationToken = default) =>
                Task.FromResult(ErrCall<RawPrivateModels.CancelParentOrderRequest, RawPrivateModels.RawCancelParentOrderResponse>(request, 500));

            public Task<Call<RawPrivateModels.GetChildOrdersRequest, IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
                RawPrivateModels.GetChildOrdersRequest request,
                CancellationToken cancellationToken = default) =>
                Task.FromResult(ErrCallList<RawPrivateModels.GetChildOrdersRequest, RawPrivateModels.RawGetChildOrdersResponse>(request, 500));

            public Task<Call<RawPrivateModels.GetParentOrdersRequest, IReadOnlyList<RawPrivateModels.RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
                RawPrivateModels.GetParentOrdersRequest request,
                CancellationToken cancellationToken = default) =>
                Task.FromResult(ErrCallList<RawPrivateModels.GetParentOrdersRequest, RawPrivateModels.RawGetParentOrdersResponse>(request, 500));

            public Task<Call<RawPrivateModels.GetParentOrderRequest, RawPrivateModels.RawGetParentOrderResponse>> GetParentOrderCallAsync(
                RawPrivateModels.GetParentOrderRequest request,
                CancellationToken cancellationToken = default) =>
                Task.FromResult(ErrCall<RawPrivateModels.GetParentOrderRequest, RawPrivateModels.RawGetParentOrderResponse>(request, 500));

            private static Call<TReq, TResponse> OkCall<TReq, TResponse>(TReq request, TResponse response)
            {
                var meta = CallMeta.CreateInternal("Raw", "NullCancelTradingApi");
                return new Call<TReq, TResponse>(
                    Id: CallId.New(),
                    StartedAt: DateTimeOffset.UtcNow,
                    Duration: TimeSpan.Zero,
                    Request: request,
                    Result: new CallResult<TResponse>.Ok(response),
                    Meta: meta);
            }

            private static Call<TReq, TResponse> ErrCall<TReq, TResponse>(TReq request, int statusCode)
            {
                var meta = CallMeta.CreateInternal("Raw", "NullCancelTradingApi");
                var error = new CallError(CallErrorKind.Http, "Test error.", null, statusCode);
                return new Call<TReq, TResponse>(
                    Id: CallId.New(),
                    StartedAt: DateTimeOffset.UtcNow,
                    Duration: TimeSpan.Zero,
                    Request: request,
                    Result: new CallResult<TResponse>.Err(error),
                    Meta: meta);
            }

            private static Call<TReq, IReadOnlyList<TItem>> ErrCallList<TReq, TItem>(TReq request, int statusCode)
            {
                var meta = CallMeta.CreateInternal("Raw", "NullCancelTradingApi");
                var error = new CallError(CallErrorKind.Http, "Test error.", null, statusCode);
                return new Call<TReq, IReadOnlyList<TItem>>(
                    Id: CallId.New(),
                    StartedAt: DateTimeOffset.UtcNow,
                    Duration: TimeSpan.Zero,
                    Request: request,
                    Result: new CallResult<IReadOnlyList<TItem>>.Err(error),
                    Meta: meta);
            }
        }
    }
}
