using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer;
using ExchangeApi.Adapter.Bitflyer.Models;

namespace ExchangeApi.Adapter.Bitflyer.Tests.Fakes;

public sealed class FakeBitflyerPrivateApi : IBitflyerPrivateApi
{
    private readonly IReadOnlyList<BitflyerBalanceResponse> _response;
    private readonly IReadOnlyList<BitflyerPositionResponse> _positions;
    private readonly IReadOnlyList<BitflyerExecutionResponse> _executions;
    private readonly BitflyerCollateralResponse _collateral;
    private readonly IReadOnlyList<BitflyerChildOrderResponse> _childOrders;
    private readonly IReadOnlyList<BitflyerCollateralAccount> _collateralAccounts;
    private readonly IReadOnlyList<JsonElement> _genericList = Array.Empty<JsonElement>();

    public FakeBitflyerPrivateApi(
        IReadOnlyList<BitflyerBalanceResponse> response,
        IReadOnlyList<BitflyerPositionResponse>? positions = null,
        IReadOnlyList<BitflyerExecutionResponse>? executions = null,
        BitflyerCollateralResponse? collateral = null,
        IReadOnlyList<BitflyerChildOrderResponse>? childOrders = null)
    {
        _response = response;
        _positions = positions ?? Array.Empty<BitflyerPositionResponse>();
        _executions = executions ?? Array.Empty<BitflyerExecutionResponse>();
        _collateral = collateral ?? new BitflyerCollateralResponse();
        _childOrders = childOrders ?? Array.Empty<BitflyerChildOrderResponse>();
        _collateralAccounts = Array.Empty<BitflyerCollateralAccount>();
    }

    public Task<IReadOnlyList<string>> GetPermissionsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

    public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
        => Task.FromResult(_response);

    public Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default)
        => Task.FromResult(_positions);

    public Task<IReadOnlyList<BitflyerExecutionResponse>> GetExecutionsAsync(string productCode, string? childOrderId = null, string? childOrderAcceptanceId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default)
        => Task.FromResult(_executions);

    public Task<BitflyerCollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_collateral);

    public Task<IReadOnlyList<BitflyerCollateralAccount>> GetCollateralAccountsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_collateralAccounts);

    public Task<IReadOnlyList<BitflyerChildOrderResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(childOrderAcceptanceId))
        {
            var filtered = _childOrders
                .Where(o => o.ChildOrderAcceptanceId == childOrderAcceptanceId)
                .ToArray();
            return Task.FromResult<IReadOnlyList<BitflyerChildOrderResponse>>(filtered);
        }

        return Task.FromResult(_childOrders);
    }

    public Task<IReadOnlyList<JsonElement>> GetParentOrdersAsync(string productCode, int? count = null, long? before = null, long? after = null, string? parentOrderState = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(_genericList);

    public Task<JsonElement> GetParentOrderAsync(string? parentOrderId = null, string? parentOrderAcceptanceId = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(JsonDocument.Parse("{}").RootElement);

    public Task<IReadOnlyList<JsonElement>> GetBalanceHistoryAsync(string? currencyCode = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(_genericList);

    public Task<IReadOnlyList<JsonElement>> GetCollateralHistoryAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(_genericList);

    public Task<JsonElement> GetTradingCommissionAsync(string productCode, CancellationToken cancellationToken = default) =>
        Task.FromResult(JsonDocument.Parse("{}").RootElement);

    public Task<IReadOnlyList<JsonElement>> GetAddressesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_genericList);

    public Task<IReadOnlyList<JsonElement>> GetCoinInsAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(_genericList);

    public Task<IReadOnlyList<JsonElement>> GetCoinOutsAsync(string? messageId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(_genericList);

    public Task<IReadOnlyList<JsonElement>> GetDepositsAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(_genericList);

    public Task<IReadOnlyList<JsonElement>> GetWithdrawalsAsync(string? messageId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(_genericList);

    public Task<IReadOnlyList<JsonElement>> GetBankAccountsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_genericList);
}
