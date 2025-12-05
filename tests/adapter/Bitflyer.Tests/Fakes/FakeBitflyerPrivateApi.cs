using System;
using System.Collections.Generic;
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
    }

    public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
        => Task.FromResult(_response);

    public Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default)
        => Task.FromResult(_positions);

    public Task<IReadOnlyList<BitflyerExecutionResponse>> GetExecutionsAsync(string productCode, CancellationToken cancellationToken = default)
        => Task.FromResult(_executions);

    public Task<BitflyerCollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_collateral);

    public Task<IReadOnlyList<BitflyerChildOrderResponse>> GetChildOrdersAsync(string productCode, string? childOrderState = null, CancellationToken cancellationToken = default)
        => Task.FromResult(_childOrders);
}
