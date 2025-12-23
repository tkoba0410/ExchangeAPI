using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Wire;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.WireApi;

/// <summary>
/// Wire API を外部提供するための薄いファサード。
/// </summary>
public sealed class BitflyerWireApiFacade
{
    private readonly BitflyerWireApi _wire;

    public BitflyerWireApiFacade(BitflyerWireApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Ticker> GetTickerAsync(string productCode, CancellationToken cancellationToken = default) =>
        _wire.GetTickerAsync(productCode, cancellationToken);

    public Task<Board> GetBoardAsync(string productCode, CancellationToken cancellationToken = default) =>
        _wire.GetBoardAsync(productCode, cancellationToken);

    public Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _wire.GetBalancesAsync(cancellationToken);

    public Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _wire.GetExecutionsAsync(productCode, cancellationToken);

    public Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _wire.GetPositionsAsync(productCode, cancellationToken);

    public Task<CollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default) =>
        _wire.GetCollateralAsync(cancellationToken);

    public Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        _wire.GetChildOrdersAsync(productCode, childOrderStatusState, childOrderAcceptanceId, cancellationToken);

    public Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _wire.CreateChildOrderAsync(request, cancellationToken);

    public Task<EmptyResponse> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _wire.CancelChildOrderAsync(request, cancellationToken);

    public Task<EmptyResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _wire.CancelAllChildOrdersAsync(request, cancellationToken);
}
