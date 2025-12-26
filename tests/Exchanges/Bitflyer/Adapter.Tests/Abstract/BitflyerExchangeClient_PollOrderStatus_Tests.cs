using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Wire.Public;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.Types.RawProductCode;
using RawChildOrderType = ExchangeApi.Exchanges.Bitflyer.Raw.ChildOrderType;
using RawSide = ExchangeApi.Exchanges.Bitflyer.Raw.Side;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Domain.UseCases;
using ExecutionResponse = ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet.ExecutionPrivateResponse;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerExchangeClient_PollOrderStatus_Tests
{
    [Fact]
    public async Task WaitForOrderAsync_CompletesWhenStateTransitions()
    {
        var acceptanceId = "ACCEPT-1";
        var active = new ChildOrderResponse
        {
            ProductCode = new RawProductCode("BTC_JPY"),
            ChildOrderAcceptanceId = acceptanceId,
            ChildOrderStatusState = "ACTIVE",
            ExecutedSize = 0m,
            OutstandingSize = 0.01m,
            Price = 3000000m,
            AveragePrice = 0m,
            Side = RawSide.Buy,
            ChildOrderType = RawChildOrderType.Limit,
            Size = 0.01m,
        };
        var completed = new ChildOrderResponse
        {
            ProductCode = active.ProductCode,
            ChildOrderAcceptanceId = acceptanceId,
            ChildOrderStatusState = "COMPLETED",
            ExecutedSize = 0.01m,
            OutstandingSize = 0m,
            Price = active.Price,
            AveragePrice = 3000000m,
            Side = active.Side,
            ChildOrderType = active.ChildOrderType,
            Size = active.Size,
        };

        var publicApi = new FakeBitflyerPublicApi(new Ticker());
        var sequenceApi = new SequenceChildOrderApi(new[] { active }, new[] { completed });
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
        var client = new BitflyerExchangeClient(publicApi, sequenceApi, tradingApi, publicApi);

        var status = await OrderPolling.WaitForOrderAsync(
            api: client,
            symbol: new Symbol("BTC/JPY"),
            orderKey: new OrderKey(OrderIdKind.AcceptanceId, acceptanceId),
            options: new PollingOptions(TimeSpan.FromMilliseconds(1), 5));

        Assert.Equal(OrderState.Completed, status.Status);
        Assert.Equal(0m, status.OutstandingSize.Value);
        Assert.Equal(0.01m, status.ExecutedSize.Value);
        Assert.Equal(3000000m, status.AveragePrice!.Value.Value);
    }

    private sealed class SequenceChildOrderApi : IBitflyerPrivateApi
    {
        private readonly Queue<IReadOnlyList<ChildOrderResponse>> _queue;

        public SequenceChildOrderApi(params IReadOnlyList<ChildOrderResponse>[] snapshots)
        {
            _queue = new Queue<IReadOnlyList<ChildOrderResponse>>(snapshots);
        }

        public Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(RawProductCode productCode, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<IReadOnlyList<ExecutionResponse>> GetExecutionsAsync(RawProductCode productCode, string? childOrderId = null, string? childOrderAcceptanceId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<CollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(RawProductCode productCode, string? childOrderStatusState = null, string? childOrderAcceptanceId = null, string? childOrderId = null, string? parentOrderId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default)
        {
            if (_queue.Count == 0)
            {
                return Task.FromResult<IReadOnlyList<ChildOrderResponse>>(Array.Empty<ChildOrderResponse>());
            }

            return Task.FromResult(_queue.Dequeue());
        }

        public Task<IReadOnlyList<string>> GetPermissionsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        public Task<IReadOnlyList<CollateralAccount>> GetCollateralAccountsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<CollateralAccount>>(Array.Empty<CollateralAccount>());
        public Task<IReadOnlyList<ParentOrderResponse>> GetParentOrdersAsync(RawProductCode productCode, int? count = null, long? before = null, long? after = null, string? parentOrderStatusState = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ParentOrderResponse>>(Array.Empty<ParentOrderResponse>());
        public Task<ParentOrderDetailResponse> GetParentOrderAsync(string? parentOrderId = null, string? parentOrderAcceptanceId = null, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ParentOrderDetailResponse());
        public Task<IReadOnlyList<JsonElement>> GetBalanceHistoryAsync(string? currencyCode = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetCollateralHistoryAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<JsonElement> GetTradingCommissionAsync(RawProductCode productCode, CancellationToken cancellationToken = default) => Task.FromResult(JsonDocument.Parse("{}").RootElement);
        public Task<IReadOnlyList<JsonElement>> GetAddressesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetCoinInsAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetCoinOutsAsync(string? messageId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetDepositsAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetWithdrawalsAsync(string? messageId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetBankAccountsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
    }
}
