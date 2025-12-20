using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.RawApi;

/// <summary>
/// Raw API を外部提供するための薄いファサード。
/// </summary>
public sealed class BitflyerRawApiFacade
{
    private readonly BitflyerRawApi _raw;

    public BitflyerRawApiFacade(BitflyerRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public Task<Ticker> GetTickerAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetTickerAsync(productCode, cancellationToken);

    public Task<Board> GetBoardAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetBoardAsync(productCode, cancellationToken);

    public Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _raw.GetBalancesAsync(cancellationToken);

    public Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetExecutionsAsync(productCode, cancellationToken);

    public Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetPositionsAsync(productCode, cancellationToken);

    public Task<CollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default) =>
        _raw.GetCollateralAsync(cancellationToken);

    public Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        _raw.GetChildOrdersAsync(productCode, childOrderStatusState, childOrderAcceptanceId, cancellationToken);

    public Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _raw.CreateChildOrderAsync(request, cancellationToken);

    public Task<EmptyResponse> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _raw.CancelChildOrderAsync(request, cancellationToken);

    public Task<EmptyResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _raw.CancelAllChildOrdersAsync(request, cancellationToken);
}
