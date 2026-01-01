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
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Spec.CallCommon;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerTradingApi_NotFoundTests
{
    [Fact]
    public async Task GetOrderAsync_ByAcceptanceId_NotFound_Throws()
    {
        var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerTestHelpers.CreateTradingApi(tradingApi, privateApi, markets);
        var api = new BitflyerTradingApi(normalized);

        var key = new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-404");
        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() =>
            api.GetOrderAsync(new Symbol("BTC/JPY"), key));
        Assert.Contains("Order not found", ex.Message);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public async Task GetOrderAsync_ByExchangeOrderId_NotFound_Throws()
    {
        var privateApi = new RecordingPrivateApi(Array.Empty<ChildOrderResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerTestHelpers.CreateTradingApi(tradingApi, privateApi, markets);
        var api = new BitflyerTradingApi(normalized);

        var key = new OrderKey(OrderIdKind.ExchangeOrderId, "JRF-404");
        var ex = await Assert.ThrowsAsync<ExchangeApiException>(() =>
            api.GetOrderAsync(new Symbol("BTC/JPY"), key));
        Assert.Contains("Order not found", ex.Message);
        Assert.Null(ex.InnerException);

        Assert.Equal(key.Value, privateApi.LastChildOrderId);
        Assert.Null(privateApi.LastChildOrderAcceptanceId);
    }

    private sealed class RecordingPrivateApi : IBitflyerPrivateApi
    {
        private readonly IReadOnlyList<ChildOrderResponse> _orders;
        private static readonly BitflyerRawRequest DefaultRequest =
            new BitflyerRawRequest("test", new Dictionary<string, string?>());

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

        public Task<BitflyerRawCall<IReadOnlyList<BalanceResponse>, JsonElement>> GetBalancesCallAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(MakeOkCall<IReadOnlyList<BalanceResponse>>(Array.Empty<BalanceResponse>()));

        public Task<BitflyerRawCall<IReadOnlyList<ExecutionPrivateResponse>, JsonElement>> GetExecutionsCallAsync(
            RawProductCode productCode,
            string? childOrderId = null,
            string? childOrderAcceptanceId = null,
            int? count = null,
            long? before = null,
            long? after = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(MakeOkCall<IReadOnlyList<ExecutionPrivateResponse>>(Array.Empty<ExecutionPrivateResponse>()));

        public Task<BitflyerRawCall<IReadOnlyList<PositionResponse>, JsonElement>> GetPositionsCallAsync(
            RawProductCode productCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(MakeOkCall<IReadOnlyList<PositionResponse>>(Array.Empty<PositionResponse>()));

        public Task<BitflyerRawCall<CollateralResponse, JsonElement>> GetCollateralCallAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(MakeOkCall(new CollateralResponse()));

        public Task<BitflyerRawCall<IReadOnlyList<ChildOrderResponse>, JsonElement>> GetChildOrdersCallAsync(
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
            var result = GetChildOrdersAsync(
                productCode,
                childOrderStatusState,
                childOrderAcceptanceId,
                childOrderId,
                parentOrderId,
                count,
                before,
                after,
                cancellationToken);
            return result.ContinueWith(task => MakeOkCall(task.Result), cancellationToken);
        }

        public Task<BitflyerRawCall<JsonElement, JsonElement>> GetTradingCommissionCallAsync(
            RawProductCode productCode,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(MakeOkCall(JsonDocument.Parse("{}").RootElement));

        private static BitflyerRawCall<TResponse, JsonElement> MakeOkCall<TResponse>(TResponse response) =>
            new(
                DefaultRequest,
                new Ok<TResponse, JsonElement>(response, 200),
                new CallMeta(DateTimeOffset.UtcNow, TimeSpan.Zero, null));
    }

}
