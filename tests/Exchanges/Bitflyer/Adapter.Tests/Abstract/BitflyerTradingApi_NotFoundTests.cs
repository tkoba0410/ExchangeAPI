using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Trading;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Core.Services;
using ExchangeApi.Contracts.Dtos;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerTradingApi_NotFoundTests
{
    [Fact]
    public async Task GetOrderAsync_ByAcceptanceId_NotFound_Throws()
    {
        var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
        var api = new BitflyerTradingApi(tradingApi, privateApi, CreateResolver());

        var key = new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-404");
        await Assert.ThrowsAsync<ExchangeOrderNotFoundException>(() =>
            api.GetOrderAsync(new Symbol("BTC/JPY"), key));
    }

    [Fact]
    public async Task GetOrderAsync_ByExchangeOrderId_NotFound_Throws()
    {
        var privateApi = new RecordingPrivateApi(Array.Empty<ChildOrderResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
        var api = new BitflyerTradingApi(tradingApi, privateApi, CreateResolver());

        var key = new OrderKey(OrderIdKind.ExchangeOrderId, "JRF-404");
        await Assert.ThrowsAsync<ExchangeOrderNotFoundException>(() =>
            api.GetOrderAsync(new Symbol("BTC/JPY"), key));

        Assert.Equal(key.Value, privateApi.LastChildOrderId);
        Assert.Null(privateApi.LastChildOrderAcceptanceId);
    }

    private sealed class RecordingPrivateApi : IBitflyerPrivateApi
    {
        private readonly IReadOnlyList<ChildOrderResponse> _orders;

        public string? LastChildOrderId { get; private set; }
        public string? LastChildOrderAcceptanceId { get; private set; }

        public RecordingPrivateApi(IReadOnlyList<ChildOrderResponse> orders)
        {
            _orders = orders;
        }

        public Task<IReadOnlyList<string>> GetPermissionsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

        public Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<BalanceResponse>>(Array.Empty<BalanceResponse>());

        public Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(RawProductCode productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<PositionResponse>>(Array.Empty<PositionResponse>());

        public Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(RawProductCode productCode, string? childOrderId = null, string? childOrderAcceptanceId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ExecutionPrivateResponse>>(Array.Empty<ExecutionPrivateResponse>());

        public Task<CollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new CollateralResponse());

        public Task<IReadOnlyList<CollateralAccount>> GetCollateralAccountsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<CollateralAccount>>(Array.Empty<CollateralAccount>());

        public Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(
            RawProductCode productCode,
            string? childOrderStatusState = null,
            string? childOrderAcceptanceId = null,
            string? childOrderId = null,
            string? parentOrderId = null,
            int? count = null,
            long? before = null,
            long? after = null,
            CancellationToken cancellationToken = default)
        {
            LastChildOrderId = childOrderId;
            LastChildOrderAcceptanceId = childOrderAcceptanceId;

            if (!string.IsNullOrEmpty(childOrderAcceptanceId))
            {
                return Task.FromResult<IReadOnlyList<ChildOrderResponse>>(Array.Empty<ChildOrderResponse>());
            }

            if (!string.IsNullOrEmpty(childOrderId))
            {
                return Task.FromResult<IReadOnlyList<ChildOrderResponse>>(Array.Empty<ChildOrderResponse>());
            }

            return Task.FromResult(_orders);
        }

        public Task<IReadOnlyList<ParentOrderResponse>> GetParentOrdersAsync(RawProductCode productCode, int? count = null, long? before = null, long? after = null, string? parentOrderStatusState = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ParentOrderResponse>>(Array.Empty<ParentOrderResponse>());

        public Task<ParentOrderDetailResponse> GetParentOrderAsync(string? parentOrderId = null, string? parentOrderAcceptanceId = null, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ParentOrderDetailResponse());

        public Task<IReadOnlyList<JsonElement>> GetBalanceHistoryAsync(string? currencyCode = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());

        public Task<IReadOnlyList<JsonElement>> GetCollateralHistoryAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());

        public Task<JsonElement> GetTradingCommissionAsync(RawProductCode productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(JsonDocument.Parse("{}").RootElement);

        public Task<IReadOnlyList<JsonElement>> GetAddressesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());

        public Task<IReadOnlyList<JsonElement>> GetCoinInsAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());

        public Task<IReadOnlyList<JsonElement>> GetCoinOutsAsync(string? messageId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());

        public Task<IReadOnlyList<JsonElement>> GetDepositsAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());

        public Task<IReadOnlyList<JsonElement>> GetWithdrawalsAsync(string? messageId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());

        public Task<IReadOnlyList<JsonElement>> GetBankAccountsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());
    }

    private static IExchangeMarketResolver CreateResolver() =>
        new ExchangeInfoMarketResolver(new StubExchangeInfoApi(new ExchangeInfo(
            new[] { new ExchangeMarketInfo("BTC/JPY", "BTC_JPY", "Spot") },
            null,
            null,
            null)));

    private sealed class StubExchangeInfoApi : IExchangeInfoApi
    {
        private readonly ExchangeInfo _info;

        public StubExchangeInfoApi(ExchangeInfo info) => _info = info;

        public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_info);
    }
}
