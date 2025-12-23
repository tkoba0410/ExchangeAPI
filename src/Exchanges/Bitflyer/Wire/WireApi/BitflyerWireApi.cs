using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;
namespace ExchangeApi.Exchanges.Bitflyer.Wire;

/// <summary>
/// bitFlyer の Wire API アクセス。正規化済みの詳細情報を返す用途向け。
/// </summary>
public sealed class BitflyerWireApi : IBitflyerWireApi
{
    private readonly IBitflyerPublicApi _publicApi;
    private readonly IBitflyerPrivateApi _privateApi;
    private readonly IBitflyerPrivateTradingApi _privateTradingApi;

    /// <summary>
    /// Wire API 用の RestClient を受け取って Wire API を生成します。
    /// </summary>
    /// <param name="restClient">
    /// 署名・認証・ポリシー設定済みの RestClient を渡してください。
    /// 署名や認証が不要な Public API のみを使う場合でも、呼び出し側で責務を分離します。
    /// </param>
    public BitflyerWireApi(IRestClient restClient)
        : this(
            publicApi: new BitflyerPublicApi(new Raw.BitflyerRawApi(restClient ?? throw new ArgumentNullException(nameof(restClient))).MarketData),
            privateApi: new BitflyerPrivateApi(restClient),
            privateTradingApi: new BitflyerPrivateTradingApi(restClient))
    {
    }

    internal BitflyerWireApi(
        IBitflyerPublicApi publicApi,
        IBitflyerPrivateApi privateApi,
        IBitflyerPrivateTradingApi privateTradingApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
    }

    public Task<Ticker> GetTickerAsync(string productCode, CancellationToken cancellationToken = default) =>
        _publicApi.GetTickerRawAsync(productCode, cancellationToken: cancellationToken);

    public Task<Board> GetBoardAsync(string productCode, CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardRawAsync(productCode, cancellationToken: cancellationToken);

    public Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        _privateApi.GetBalancesAsync(cancellationToken);

    public Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _privateApi.GetExecutionsAsync(productCode, cancellationToken: cancellationToken);

    public Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(string productCode, CancellationToken cancellationToken = default) =>
        _privateApi.GetPositionsAsync(productCode, cancellationToken);

    public Task<CollateralResponse> GetCollateralAsync(CancellationToken cancellationToken = default) =>
        _privateApi.GetCollateralAsync(cancellationToken);

    public Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetChildOrdersAsync(productCode, childOrderStatusState, childOrderAcceptanceId, cancellationToken: cancellationToken);

    public Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateChildOrderAsync(request, cancellationToken);

    public Task<EmptyResponse> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelChildOrderAsync(request, cancellationToken);

    public Task<EmptyResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelAllChildOrdersAsync(request, cancellationToken);

    public Task<CreateParentOrderResponse> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateParentOrderAsync(request, cancellationToken);

    public Task<EmptyResponse> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelParentOrderAsync(request, cancellationToken);

    public Task<CreateWithdrawalResponse> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateWithdrawalAsync(request, cancellationToken);
}
