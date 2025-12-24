using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.Types.RawProductCode;

namespace ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;

public sealed class FakeBitflyerPrivateApi : IBitflyerPrivateApi
{
    private readonly IReadOnlyList<BalanceResponse> _response;
    private readonly IReadOnlyList<PositionResponse> _positions;
    private readonly IReadOnlyList<ExecutionPrivateResponse> _executions;
    private readonly CollateralResponse _collateral;
    private readonly IReadOnlyList<ChildOrderResponse> _childOrders;
    private readonly IReadOnlyList<CollateralAccount> _collateralAccounts;
    private readonly IReadOnlyList<JsonElement> _genericList = Array.Empty<JsonElement>();
    private readonly IReadOnlyList<ParentOrderResponse> _parentOrders = Array.Empty<ParentOrderResponse>();

    public FakeBitflyerPrivateApi(
        IReadOnlyList<BalanceResponse> response,
        IReadOnlyList<PositionResponse>? positions = null,
        IReadOnlyList<ExecutionPrivateResponse>? executions = null,
        CollateralResponse? collateral = null,
        IReadOnlyList<ChildOrderResponse>? childOrders = null,
        IReadOnlyList<ParentOrderResponse>? parentOrders = null)
    {
        _response = response;
        _positions = positions ?? Array.Empty<PositionResponse>();
        _executions = executions ?? Array.Empty<ExecutionPrivateResponse>();
        _collateral = collateral ?? new CollateralResponse();
        _childOrders = childOrders ?? Array.Empty<ChildOrderResponse>();
        _collateralAccounts = Array.Empty<CollateralAccount>();
        _parentOrders = parentOrders ?? Array.Empty<ParentOrderResponse>();
    }

    public Task<IReadOnlyList<string>> GetPermissionsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

    public Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
        => Task.FromResult(_response);

    public Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(RawProductCode productCode, CancellationToken cancellationToken = default)
        => Task.FromResult(_positions);

    public Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(RawProductCode productCode, string? childOrderId = null, string? childOrderAcceptanceId = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default)
        => Task.FromResult(_executions);

    public Task<CollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_collateral);

    public Task<IReadOnlyList<CollateralAccount>> GetCollateralAccountsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_collateralAccounts);

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
        if (!string.IsNullOrEmpty(childOrderAcceptanceId))
        {
            var filtered = _childOrders
                .Where(o => o.ChildOrderAcceptanceId == childOrderAcceptanceId)
                .ToArray();
            return Task.FromResult<IReadOnlyList<ChildOrderResponse>>(filtered);
        }

        return Task.FromResult(_childOrders);
    }

    public Task<IReadOnlyList<ParentOrderResponse>> GetParentOrdersAsync(RawProductCode productCode, int? count = null, long? before = null, long? after = null, string? parentOrderStatusState = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(_parentOrders);

    public Task<ParentOrderDetailResponse> GetParentOrderAsync(string? parentOrderId = null, string? parentOrderAcceptanceId = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(new ParentOrderDetailResponse
        {
            Id = 1,
            ParentOrderId = parentOrderId ?? "PARENT-ID",
            OrderMethod = OrderMethod.Simple,
            ExpireDate = System.DateTimeOffset.UtcNow,
            TimeInForce = TimeInForce.Gtc,
            ParentOrderAcceptanceId = parentOrderAcceptanceId ?? "PARENT-ACCEPT",
            Parameters = new[]
            {
                new ParentOrderDetailParameter
                {
                    ProductCode = new RawProductCode("BTC_JPY"),
                    ConditionType = ConditionType.Limit,
                    Side = Side.Buy,
                    Size = 0.1m,
                    Price = 30000m,
                    TriggerPrice = 0m,
                    Offset = 0m,
                }
            }
        });

    public Task<IReadOnlyList<JsonElement>> GetBalanceHistoryAsync(string? currencyCode = null, int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(_genericList);

    public Task<IReadOnlyList<JsonElement>> GetCollateralHistoryAsync(int? count = null, long? before = null, long? after = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(_genericList);

    public Task<JsonElement> GetTradingCommissionAsync(RawProductCode productCode, CancellationToken cancellationToken = default) =>
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
