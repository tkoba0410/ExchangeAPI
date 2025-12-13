using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Raw.PrivateGet;
using Exchange.Bitflyer.Raw.PrivateGet.Models;
using Exchange.Bitflyer.Raw.PrivatePost;
using Exchange.Bitflyer.Raw.PrivatePost.Models;
using Exchange.Bitflyer.Raw.PublicGet;
using Exchange.Bitflyer.Raw.PublicGet.Models;

namespace Exchange.Bitflyer.Raw.RawApi;

/// <summary>
/// bitFlyer の Raw API アクセス。抽象化しづらい/詳細情報をそのまま返す用途向け。
/// </summary>
public sealed class BitflyerRawApiClient
{
    private readonly IBitflyerPublicApi _publicApi;
    private readonly IBitflyerPrivateApi _privateApi;
    private readonly IBitflyerPrivateTradingApi _privateTradingApi;

    public BitflyerRawApiClient(
        IBitflyerPublicApi publicApi,
        IBitflyerPrivateApi privateApi,
        IBitflyerPrivateTradingApi privateTradingApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
    }

    public Task<BitflyerTickerRaw> GetTickerAsync(string productCode, CancellationToken cancellationToken = default) =>
        _publicApi.GetTickerRawAsync(productCode, cancellationToken: cancellationToken);

    public Task<BitflyerBoardRaw> GetBoardAsync(string productCode, CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardRawAsync(productCode, cancellationToken: cancellationToken);

    public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _privateApi.GetBalancesAsync(cancellationToken);

    public Task<IReadOnlyList<BitflyerExecutionPrivateResponse>> GetExecutionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _privateApi.GetExecutionsAsync(productCode, cancellationToken: cancellationToken);

    public Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _privateApi.GetPositionsAsync(productCode, cancellationToken);

    public Task<BitflyerCollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default) =>
        _privateApi.GetCollateralAsync(cancellationToken);

    public Task<IReadOnlyList<BitflyerChildOrderResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderState = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetChildOrdersAsync(productCode, childOrderState, childOrderAcceptanceId, cancellationToken: cancellationToken);

    public Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(
        BitflyerSendChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.SendChildOrderAsync(request, cancellationToken);

    public Task<BitflyerEmptyResponse> CancelChildOrderAsync(
        BitflyerCancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelChildOrderAsync(request, cancellationToken);

    public Task<BitflyerEmptyResponse> CancelAllChildOrdersAsync(
        BitflyerCancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelAllChildOrdersAsync(request, cancellationToken);
}
