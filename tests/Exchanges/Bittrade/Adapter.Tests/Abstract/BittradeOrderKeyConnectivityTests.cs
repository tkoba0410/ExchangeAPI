using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Raw;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeOrderKeyConnectivityTests
{
    [Fact]
    public async Task GetOrderAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var rest = new RecordingRestClient(
            getResponse: new OrderDetailResponse(
                Status: "ok",
                Data: CreateOrderDetail(1001)));
        var api = new BittradeTradingApi(rest, accountId: "account-1");

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1001");
        var status = await api.GetOrderAsync(new Symbol("BTC/JPY"), key);

        Assert.Equal("v1/order/orders/1001", rest.LastGetPath);
        Assert.Equal(OrderIdKind.AcceptanceId, status.Key.Kind);
        Assert.Equal("1001", status.Key.Value);
    }

    [Fact]
    public async Task CancelOrderAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var rest = new RecordingRestClient(
            postResponse: new CancelOrderResponse("ok", "1002"));
        var api = new BittradeTradingApi(rest, accountId: "account-1");

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1002");
        var result = await api.CancelOrderAsync(new Symbol("BTC/JPY"), key);

        Assert.True(result.IsSuccess);
        Assert.Equal("v1/order/orders/1002/submitcancel", rest.LastPostPath);
    }

    private static OrderDetail CreateOrderDetail(long id) =>
        new(
            Id: id,
            Symbol: "btcjpy",
            AccountId: "account-1",
            Amount: "0.01",
            Price: "100",
            State: BittradeOrderState.Filled,
            Type: BittradeOrderType.BuyLimit,
            ClientOrderId: null,
            CreatedAt: DateTimeOffset.FromUnixTimeMilliseconds(1),
            FinishedAt: DateTimeOffset.FromUnixTimeMilliseconds(2),
            FilledAmount: "0.01",
            FilledCashAmount: "1",
            Fees: "0");

    private sealed class RecordingRestClient : IRestClient
    {
        private readonly object? _getResponse;
        private readonly object? _postResponse;

        public string? LastGetPath { get; private set; }
        public string? LastPostPath { get; private set; }

        public RecordingRestClient(object? getResponse = null, object? postResponse = null)
        {
            _getResponse = getResponse;
            _postResponse = postResponse;
        }

        public Task<TResponse> GetAsync<TResponse>(
            string path,
            IReadOnlyDictionary<string, string?>? query = null,
            CancellationToken cancellationToken = default)
        {
            LastGetPath = path;
            return Task.FromResult((TResponse)_getResponse!);
        }

        public Task<TResponse> PostAsync<TRequest, TResponse>(
            string path,
            TRequest body,
            CancellationToken cancellationToken = default)
        {
            LastPostPath = path;
            return Task.FromResult((TResponse)_postResponse!);
        }
    }
}
