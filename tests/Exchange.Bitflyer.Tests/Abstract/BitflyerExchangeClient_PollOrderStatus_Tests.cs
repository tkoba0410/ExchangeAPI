using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Abstract;
using Exchange.Bitflyer.Raw;
using RawProductCode = Exchange.Bitflyer.Raw.ProductCode;
using Exchange.Bitflyer.Tests.Fakes;
using Common.Contract.Dtos;
using Common.Contract.Enums;
using ExecutionResponse = Exchange.Bitflyer.Raw.BitflyerExecutionPrivateResponse;
using Xunit;

namespace Exchange.Bitflyer.Tests;

public sealed class BitflyerExchangeClient_PollOrderStatus_Tests
{
    [Fact]
    public async Task PollOrderStatusAsync_CompletesWhenStateTransitions()
    {
        var acceptanceId = "ACCEPT-1";
        var active = new BitflyerChildOrderResponse
        {
            ProductCode = RawProductCode.BtcJpy,
            ChildOrderAcceptanceId = acceptanceId,
            ChildOrderStatusState = "ACTIVE",
            ExecutedSize = 0m,
            OutstandingSize = 0.01m,
            Price = 3000000m,
            AveragePrice = 0m,
            Side = Side.Buy,
            ChildOrderType = ChildOrderType.Limit,
            Size = 0.01m,
        };
        var completed = new BitflyerChildOrderResponse
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

        var publicApi = new FakeBitflyerPublicApi(new BitflyerTicker());
        var sequenceApi = new SequenceChildOrderApi(new[] { active }, new[] { completed });
        var tradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
        var client = new BitflyerExchangeClient(publicApi, sequenceApi, tradingApi);

        var status = await client.PollOrderStatusAsync(
            productCode: "BTC_JPY",
            childOrderAcceptanceId: acceptanceId,
            pollInterval: TimeSpan.FromMilliseconds(1),
            maxAttempts: 5);

        Assert.Equal(OrderState.Completed, status.Status);
        Assert.Equal(0m, status.OutstandingSize);
        Assert.Equal(0.01m, status.ExecutedSize);
        Assert.Equal(3000000m, status.AveragePrice);
    }

    private sealed class SequenceChildOrderApi : IBitflyerPrivateApi
    {
        private readonly Queue<IReadOnlyList<BitflyerChildOrderResponse>> _queue;

        public SequenceChildOrderApi(params IReadOnlyList<BitflyerChildOrderResponse>[] snapshots)
        {
            _queue = new Queue<IReadOnlyList<BitflyerChildOrderResponse>>(snapshots);
        }

        public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<IReadOnlyList<ExecutionResponse>> GetExecutionsAsync(string productCode, string? childOrderId = null, string? childOrderAcceptanceId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<BitflyerCollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<IReadOnlyList<BitflyerChildOrderResponse>> GetOrdersAsync(string productCode, string? childOrderStatusState = null, string? childOrderAcceptanceId = null, string? childOrderId = null, string? parentOrderId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default)
        {
            if (_queue.Count == 0)
            {
                return Task.FromResult<IReadOnlyList<BitflyerChildOrderResponse>>(Array.Empty<BitflyerChildOrderResponse>());
            }

            return Task.FromResult(_queue.Dequeue());
        }

        public Task<IReadOnlyList<string>> GetPermissionsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        public Task<IReadOnlyList<BitflyerCollateralAccount>> GetCollateralAccountsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<BitflyerCollateralAccount>>(Array.Empty<BitflyerCollateralAccount>());
        public Task<IReadOnlyList<BitflyerParentOrderResponse>> GetParentOrdersAsync(string productCode, int? count = null, long? before = null, long? after = null, string? parentOrderStatusState = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<BitflyerParentOrderResponse>>(Array.Empty<BitflyerParentOrderResponse>());
        public Task<BitflyerParentOrderDetailResponse> GetParentOrderAsync(string? parentOrderId = null, string? parentOrderAcceptanceId = null, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BitflyerParentOrderDetailResponse());
        public Task<IReadOnlyList<JsonElement>> GetBalanceHistoryAsync(string? currencyCode = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetCollateralHistoryAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<JsonElement> GetTradingCommissionAsync(string productCode, CancellationToken cancellationToken = default) => Task.FromResult(JsonDocument.Parse("{}").RootElement);
        public Task<IReadOnlyList<JsonElement>> GetAddressesAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetCoinInsAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetCoinOutsAsync(string? messageId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetDepositsAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetWithdrawalsAsync(string? messageId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
        public Task<IReadOnlyList<JsonElement>> GetBankAccountsAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
    }
}
