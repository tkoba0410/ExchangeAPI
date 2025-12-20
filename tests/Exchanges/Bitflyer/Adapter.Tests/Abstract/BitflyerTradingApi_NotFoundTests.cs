using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Trading;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerTradingApi_NotFoundTests
{
    [Fact]
    public async Task GetOrderAsync_ByAcceptanceId_NotFound_Throws()
    {
        var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
        var api = new BitflyerTradingApi(tradingApi, privateApi);

        var key = new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-404");
        await Assert.ThrowsAsync<ExchangeOrderNotFoundException>(() =>
            api.GetOrderAsync(new Symbol("BTC/JPY"), key));
    }

    [Fact]
    public async Task GetOrderAsync_ByExchangeOrderId_NotFound_Throws()
    {
        var privateApi = new RecordingPrivateApi(Array.Empty<BitflyerChildOrderResponse>());
        var tradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
        var api = new BitflyerTradingApi(tradingApi, privateApi);

        var key = new OrderKey(OrderIdKind.ExchangeOrderId, "JRF-404");
        await Assert.ThrowsAsync<ExchangeOrderNotFoundException>(() =>
            api.GetOrderAsync(new Symbol("BTC/JPY"), key));

        Assert.Equal(key.Value, privateApi.LastChildOrderId);
        Assert.Null(privateApi.LastChildOrderAcceptanceId);
    }

    private sealed class RecordingPrivateApi : IBitflyerPrivateApi
    {
        private readonly IReadOnlyList<BitflyerChildOrderResponse> _orders;

        public string? LastChildOrderId { get; private set; }
        public string? LastChildOrderAcceptanceId { get; private set; }

        public RecordingPrivateApi(IReadOnlyList<BitflyerChildOrderResponse> orders)
        {
            _orders = orders;
        }

        public Task<IReadOnlyList<string>> GetPermissionsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

        public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<BitflyerBalanceResponse>>(Array.Empty<BitflyerBalanceResponse>());

        public Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<BitflyerPositionResponse>>(Array.Empty<BitflyerPositionResponse>());

        public Task<IReadOnlyList<BitflyerExecutionPrivateResponse>> GetExecutionsAsync(string productCode, string? childOrderId = null, string? childOrderAcceptanceId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<BitflyerExecutionPrivateResponse>>(Array.Empty<BitflyerExecutionPrivateResponse>());

        public Task<BitflyerCollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new BitflyerCollateralResponse());

        public Task<IReadOnlyList<BitflyerCollateralAccount>> GetCollateralAccountsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<BitflyerCollateralAccount>>(Array.Empty<BitflyerCollateralAccount>());

        public Task<IReadOnlyList<BitflyerChildOrderResponse>> GetOrdersAsync(
            string productCode,
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
                return Task.FromResult<IReadOnlyList<BitflyerChildOrderResponse>>(Array.Empty<BitflyerChildOrderResponse>());
            }

            if (!string.IsNullOrEmpty(childOrderId))
            {
                return Task.FromResult<IReadOnlyList<BitflyerChildOrderResponse>>(Array.Empty<BitflyerChildOrderResponse>());
            }

            return Task.FromResult(_orders);
        }

        public Task<IReadOnlyList<BitflyerParentOrderResponse>> GetParentOrdersAsync(string productCode, int? count = null, long? before = null, long? after = null, string? parentOrderStatusState = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<BitflyerParentOrderResponse>>(Array.Empty<BitflyerParentOrderResponse>());

        public Task<BitflyerParentOrderDetailResponse> GetParentOrderAsync(string? parentOrderId = null, string? parentOrderAcceptanceId = null, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BitflyerParentOrderDetailResponse());

        public Task<IReadOnlyList<JsonElement>> GetBalanceHistoryAsync(string? currencyCode = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());

        public Task<IReadOnlyList<JsonElement>> GetCollateralHistoryAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<JsonElement>>(Array.Empty<JsonElement>());

        public Task<JsonElement> GetTradingCommissionAsync(string productCode, CancellationToken cancellationToken = default) =>
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
}
