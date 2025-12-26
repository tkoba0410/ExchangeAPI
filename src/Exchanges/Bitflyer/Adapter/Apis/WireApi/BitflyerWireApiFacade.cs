using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Wire.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Wire;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.WireApi;

/// <summary>
/// Wire API を外部提供するための薄いファサード。
/// </summary>
public sealed class BitflyerWireApiFacade
{
    private readonly IBitflyerWireApi _wire;

    public BitflyerWireApiFacade(IBitflyerWireApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Ticker> GetTickerAsync(RawProductCode productCode, CancellationToken cancellationToken = default) =>
        _wire.MarketData.GetTickerRawAsync(productCode, cancellationToken: cancellationToken);

    public Task<Board> GetBoardAsync(RawProductCode productCode, CancellationToken cancellationToken = default) =>
        _wire.MarketData.GetBoardRawAsync(productCode, cancellationToken: cancellationToken);

    public Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _wire.Account.GetBalancesAsync(cancellationToken);

    public Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(RawProductCode productCode, CancellationToken cancellationToken = default) =>
        _wire.Account.GetExecutionsAsync(productCode, cancellationToken: cancellationToken);

    public Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(RawProductCode productCode, CancellationToken cancellationToken = default) =>
        _wire.Account.GetPositionsAsync(productCode, cancellationToken);

    public Task<CollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default) =>
        _wire.Account.GetCollateralAsync(cancellationToken);

    public Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        _wire.Account.GetChildOrdersAsync(productCode, childOrderStatusState, childOrderAcceptanceId, cancellationToken: cancellationToken);

    public Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _wire.Trading.CreateChildOrderAsync(request, cancellationToken);

    public Task<EmptyResponse> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _wire.Trading.CancelChildOrderAsync(request, cancellationToken);

    public Task<EmptyResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _wire.Trading.CancelAllChildOrdersAsync(request, cancellationToken);
}
