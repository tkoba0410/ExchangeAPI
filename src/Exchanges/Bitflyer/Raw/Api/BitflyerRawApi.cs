using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Api;

public sealed class BitflyerRawApi : IBitflyerRawApi
{
    private readonly BitflyerRawPublicClient _publicClient;
    private readonly BitflyerRawPrivateClient _privateClient;

    public BitflyerRawApi(IWireTransport wire)
    {
        var executor = new BitflyerRawCallExecutor(wire);
        _publicClient = new BitflyerRawPublicClient(executor);
        _privateClient = new BitflyerRawPrivateClient(executor);
    }

    public Task<Call<GetMarketsRequest, IReadOnlyList<Market>>> GetMarketsCallAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        _publicClient.GetMarketsCallAsync(request, cancellationToken);

    public Task<Call<GetBoardRequest, Board>> GetBoardCallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default) =>
        _publicClient.GetBoardCallAsync(request, cancellationToken);

    public Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        _publicClient.GetTickerCallAsync(request, cancellationToken);

    public Task<Call<GetExecutionsRequest, IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsPublicCallAsync(
        GetExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        _publicClient.GetExecutionsPublicCallAsync(request, cancellationToken);

    public Task<Call<GetBoardStateRequest, BoardStateResponse>> GetBoardStateCallAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        _publicClient.GetBoardStateCallAsync(request, cancellationToken);

    public Task<Call<GetHealthRequest, HealthResponse>> GetHealthCallAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        _publicClient.GetHealthCallAsync(request, cancellationToken);

    public Task<Call<GetFundingRateRequest, FundingRateResponse>> GetFundingRateCallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        _publicClient.GetFundingRateCallAsync(request, cancellationToken);

    public Task<Call<GetCorporateLeverageRequest, CorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        _publicClient.GetCorporateLeverageCallAsync(request, cancellationToken);

    public Task<Call<GetChatsRequest, IReadOnlyList<Chat>>> GetChatsCallAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default) =>
        _publicClient.GetChatsCallAsync(request, cancellationToken);

    public Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetPermissionsCallAsync(request, cancellationToken);

    public Task<Call<GetBalancesRequest, IReadOnlyList<BalanceResponse>>> GetBalanceCallAsync(
        GetBalancesRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetBalanceCallAsync(request, cancellationToken);

    public Task<Call<GetCollateralRequest, CollateralResponse>> GetCollateralCallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetCollateralCallAsync(request, cancellationToken);

    public Task<Call<GetCollateralAccountsRequest, IReadOnlyList<CollateralAccount>>> GetCollateralAccountsCallAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetCollateralAccountsCallAsync(request, cancellationToken);

    public Task<Call<GetAddressesRequest, RawJsonResponse>> GetAddressesCallAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetAddressesCallAsync(request, cancellationToken);

    public Task<Call<GetCoinInsRequest, RawJsonResponse>> GetCoinInsCallAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetCoinInsCallAsync(request, cancellationToken);

    public Task<Call<GetCoinOutsRequest, RawJsonResponse>> GetCoinOutsCallAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetCoinOutsCallAsync(request, cancellationToken);

    public Task<Call<GetBankAccountsRequest, RawJsonResponse>> GetBankAccountsCallAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetBankAccountsCallAsync(request, cancellationToken);

    public Task<Call<GetDepositsRequest, RawJsonResponse>> GetDepositsCallAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetDepositsCallAsync(request, cancellationToken);

    public Task<Call<CreateWithdrawalRequest, CreateWithdrawalResponse>> WithdrawCallAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.WithdrawCallAsync(request, cancellationToken);

    public Task<Call<GetWithdrawalsRequest, RawJsonResponse>> GetWithdrawalsCallAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetWithdrawalsCallAsync(request, cancellationToken);

    public Task<Call<string, RawSendChildOrderResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        _privateClient.SendChildOrderCallAsync(bodyJson, cancellationToken);

    public Task<Call<string, RawSendParentOrderResponse>> SendParentOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        _privateClient.SendParentOrderCallAsync(bodyJson, cancellationToken);

    public Task<Call<CancelChildOrderRequest, RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.CancelChildOrderCallAsync(request, cancellationToken);

    public Task<Call<CancelParentOrderRequest, RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.CancelParentOrderCallAsync(request, cancellationToken);

    public Task<Call<CancelAllChildOrdersRequest, RawCancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.CancelAllChildOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetChildOrdersRequest, IReadOnlyList<RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetChildOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetParentOrdersRequest, IReadOnlyList<RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetParentOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetParentOrderRequest, RawGetParentOrderResponse>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetParentOrderCallAsync(request, cancellationToken);

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsPrivateCallAsync(
        GetAccountExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetExecutionsPrivateCallAsync(request, cancellationToken);

    public Task<Call<GetBalanceHistoryRequest, RawJsonResponse>> GetBalanceHistoryCallAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetBalanceHistoryCallAsync(request, cancellationToken);

    public Task<Call<GetPositionsRequest, IReadOnlyList<PositionResponse>>> GetPositionsCallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetPositionsCallAsync(request, cancellationToken);

    public Task<Call<GetCollateralHistoryRequest, RawJsonResponse>> GetCollateralHistoryCallAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetCollateralHistoryCallAsync(request, cancellationToken);

    public Task<Call<GetTradingCommissionRequest, RawJsonResponse>> GetTradingCommissionCallAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetTradingCommissionCallAsync(request, cancellationToken);
}
