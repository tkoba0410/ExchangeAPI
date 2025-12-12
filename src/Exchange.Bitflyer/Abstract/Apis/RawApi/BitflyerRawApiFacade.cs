using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Models;

namespace ExchangeApi.Adapter.Bitflyer.Apis.RawApi;

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

    public Task<BitflyerTickerRaw> GetTickerAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetTickerAsync(productCode, cancellationToken);

    public Task<BitflyerBoardRaw> GetBoardAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetBoardAsync(productCode, cancellationToken);

    public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _raw.GetBalancesAsync(cancellationToken);

    public Task<IReadOnlyList<BitflyerExecutionResponse>> GetExecutionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetExecutionsAsync(productCode, cancellationToken);

    public Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _raw.GetPositionsAsync(productCode, cancellationToken);

    public Task<BitflyerCollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default) =>
        _raw.GetCollateralAsync(cancellationToken);

    public Task<IReadOnlyList<BitflyerChildOrderResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderState = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        _raw.GetChildOrdersAsync(productCode, childOrderState, childOrderAcceptanceId, cancellationToken);

    public Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(
        BitflyerSendChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _raw.SendChildOrderAsync(request, cancellationToken);

    public Task<BitflyerEmptyResponse> CancelChildOrderAsync(
        BitflyerCancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _raw.CancelChildOrderAsync(request, cancellationToken);

    public Task<BitflyerEmptyResponse> CancelAllChildOrdersAsync(
        BitflyerCancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _raw.CancelAllChildOrdersAsync(request, cancellationToken);
}
