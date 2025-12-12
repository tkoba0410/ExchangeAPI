using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Facade;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Adapter.Bitflyer.Tests.Fakes;
using ExchangeApi.Contracts.Dtos;
using Xunit;

namespace ExchangeApi.Adapter.Bitflyer.Tests;

public sealed class BitflyerExchangeClient_PollOrderStatus_Tests
{
    [Fact]
    public async Task PollOrderStatusAsync_CompletesWhenStateTransitions()
    {
        var acceptanceId = "ACCEPT-1";
        var active = new BitflyerChildOrderResponse
        {
            ProductCode = "BTC_JPY",
            ChildOrderAcceptanceId = acceptanceId,
            ChildOrderState = "ACTIVE",
            ExecutedSize = 0m,
            OutstandingSize = 0.01m,
            Price = 3000000m,
            AveragePrice = 0m,
            Side = "BUY",
            ChildOrderType = "LIMIT",
            Size = 0.01m,
        };
        var completed = new BitflyerChildOrderResponse
        {
            ProductCode = active.ProductCode,
            ChildOrderAcceptanceId = acceptanceId,
            ChildOrderState = "COMPLETED",
            ExecutedSize = 0.01m,
            OutstandingSize = 0m,
            Price = active.Price,
            AveragePrice = 3000000m,
            Side = active.Side,
            ChildOrderType = active.ChildOrderType,
            Size = active.Size,
        };

        var publicApi = new FakeBitflyerPublicApi(new BitflyerTickerRaw());
        var sequenceApi = new SequenceChildOrderApi(new[] { active }, new[] { completed });
        var tradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
        var client = new BitflyerExchangeClient(publicApi, sequenceApi, tradingApi);

        var status = await client.PollOrderStatusAsync(
            productCode: "BTC_JPY",
            childOrderAcceptanceId: acceptanceId,
            pollInterval: TimeSpan.FromMilliseconds(1),
            maxAttempts: 5);

        Assert.Equal(OrderStatusType.Completed, status.Status);
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

        public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(System.Threading.CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(string productCode, System.Threading.CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<IReadOnlyList<BitflyerExecutionResponse>> GetExecutionsAsync(string productCode, System.Threading.CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<BitflyerCollateralResponse> GetCollateralAsync(System.Threading.CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<IReadOnlyList<BitflyerChildOrderResponse>> GetChildOrdersAsync(string productCode, string? childOrderState = null, string? childOrderAcceptanceId = null, System.Threading.CancellationToken cancellationToken = default)
        {
            if (_queue.Count == 0)
            {
                return Task.FromResult<IReadOnlyList<BitflyerChildOrderResponse>>(Array.Empty<BitflyerChildOrderResponse>());
            }

            return Task.FromResult(_queue.Dequeue());
        }
    }
}
