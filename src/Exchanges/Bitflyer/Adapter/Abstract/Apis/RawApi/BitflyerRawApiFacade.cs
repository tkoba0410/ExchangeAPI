using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Raw;
namespace Exchange.Bitflyer.Abstract.Apis.RawApi;

/// <summary>
/// Raw API を外部提供するための薄いファサード。
/// </summary>
public sealed class BitflyerRawApiFacade
{
    private readonly BitflyerRawApiClient _raw;

    public BitflyerRawApiFacade(BitflyerRawApiClient raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public Task<BitflyerTicker> GetTickerAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetTickerAsync(productCode, cancellationToken);

    public Task<BitflyerBoard> GetBoardAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetBoardAsync(productCode, cancellationToken);

    public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _raw.GetBalancesAsync(cancellationToken);

    public Task<IReadOnlyList<BitflyerExecutionPrivateResponse>> GetExecutionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetExecutionsAsync(productCode, cancellationToken);

    public Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetPositionsAsync(productCode, cancellationToken);

    public Task<BitflyerCollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default) =>
        _raw.GetCollateralAsync(cancellationToken);

    public Task<IReadOnlyList<BitflyerChildOrderResponse>> GetOrdersAsync(
        string productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        _raw.GetOrdersAsync(productCode, childOrderStatusState, childOrderAcceptanceId, cancellationToken);

    public Task<BitflyerSendChildOrderResponse> PlaceChildOrderAsync(
        BitflyerSendChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _raw.PlaceChildOrderAsync(request, cancellationToken);

    public Task<BitflyerEmptyResponse> CancelChildOrderAsync(
        BitflyerCancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _raw.CancelChildOrderAsync(request, cancellationToken);

    public Task<BitflyerEmptyResponse> CancelAllOrdersAsync(
        BitflyerCancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _raw.CancelAllOrdersAsync(request, cancellationToken);
}
