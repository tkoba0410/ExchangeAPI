using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Requests;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Internal;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;

public sealed class BitflyerRawApi : IBitflyerRawApi
{
    private readonly BitflyerPublicApi _publicApi;
    private readonly BitflyerRawPrivateClient _privateClient;

    public BitflyerRawApi(IBitflyerWireCallExecutor wire)
    {
        var executor = new BitflyerRawCallExecutor();
        _publicApi = new BitflyerPublicApi(wire, executor);
        _privateClient = new BitflyerRawPrivateClient(wire, executor);
    }

    public Task<Call<GetMarketsRequest, GetMarketsResponse>> GetMarketsCallAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetMarketsCallAsync(request, cancellationToken);

    public Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardCallAsync(request, cancellationToken);

    public Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTickerCallAsync(request, cancellationToken);

    public Task<Call<GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        GetExecutionsPublicRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetExecutionsPublicCallAsync(request, cancellationToken);

    public Task<Call<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateCallAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardStateCallAsync(request, cancellationToken);

    public Task<Call<GetHealthRequest, GetHealthResponse>> GetHealthCallAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetHealthCallAsync(request, cancellationToken);

    public Task<Call<GetFundingRateRequest, GetFundingRateResponse>> GetFundingRateCallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetFundingRateCallAsync(request, cancellationToken);

    public Task<Call<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetCorporateLeverageCallAsync(request, cancellationToken);

    public Task<Call<GetChatsRequest, GetChatsResponse>> GetChatsCallAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetChatsCallAsync(request, cancellationToken);

    public Task<Call<GetPermissionsRequest, GetPermissionsResponse>> GetPermissionsCallAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetPermissionsCallAsync(request, cancellationToken);

    public Task<Call<GetBalanceRequest, GetBalanceResponse>> GetBalanceCallAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetBalanceCallAsync(request, cancellationToken);

    public Task<Call<GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetCollateralCallAsync(request, cancellationToken);

    public Task<Call<GetCollateralAccountsRequest, GetCollateralAccountsResponse>> GetCollateralAccountsCallAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetCollateralAccountsCallAsync(request, cancellationToken);

    public Task<Call<GetAddressesRequest, GetAddressesResponse>> GetAddressesCallAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetAddressesCallAsync(request, cancellationToken);

    public Task<Call<GetCoinInsRequest, GetCoinInsResponse>> GetCoinInsCallAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetCoinInsCallAsync(request, cancellationToken);

    public Task<Call<GetCoinOutsRequest, GetCoinOutsResponse>> GetCoinOutsCallAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetCoinOutsCallAsync(request, cancellationToken);

    public Task<Call<GetBankAccountsRequest, GetBankAccountsResponse>> GetBankAccountsCallAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetBankAccountsCallAsync(request, cancellationToken);

    public Task<Call<GetDepositsRequest, GetDepositsResponse>> GetDepositsCallAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetDepositsCallAsync(request, cancellationToken);

    public Task<Call<WithdrawRequest, WithdrawResponse>> WithdrawCallAsync(
        WithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.WithdrawCallAsync(request, cancellationToken);

    public Task<Call<GetWithdrawalsRequest, GetWithdrawalsResponse>> GetWithdrawalsCallAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetWithdrawalsCallAsync(request, cancellationToken);

    public Task<Call<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.SendChildOrderCallAsync(request, cancellationToken);

    public Task<Call<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderCallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.SendParentOrderCallAsync(request, cancellationToken);

    public Task<Call<CancelChildOrderRequest, CancelChildOrderResponse>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.CancelChildOrderCallAsync(request, cancellationToken);

    public Task<Call<CancelParentOrderRequest, CancelParentOrderResponse>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.CancelParentOrderCallAsync(request, cancellationToken);

    public Task<Call<CancelAllChildOrdersRequest, CancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.CancelAllChildOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetChildOrdersRequest, GetChildOrdersResponse>> GetChildOrdersCallAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetChildOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetParentOrdersRequest, GetParentOrdersResponse>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetParentOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetParentOrderCallAsync(request, cancellationToken);

    public Task<Call<GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>> GetExecutionsPrivateCallAsync(
        GetExecutionsPrivateRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetExecutionsPrivateCallAsync(request, cancellationToken);

    public Task<Call<GetBalanceHistoryRequest, GetBalanceHistoryResponse>> GetBalanceHistoryCallAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetBalanceHistoryCallAsync(request, cancellationToken);

    public Task<Call<GetPositionsRequest, GetPositionsResponse>> GetPositionsCallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetPositionsCallAsync(request, cancellationToken);

    public Task<Call<GetCollateralHistoryRequest, GetCollateralHistoryResponse>> GetCollateralHistoryCallAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetCollateralHistoryCallAsync(request, cancellationToken);

    public Task<Call<GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionCallAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetTradingCommissionCallAsync(request, cancellationToken);
}
