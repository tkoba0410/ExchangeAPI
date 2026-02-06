using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;

public interface IBitflyerRawApi
{
    Task<Call<GetMarketsRequest, GetMarketsResponse>> GetMarketsCallAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        GetExecutionsPublicRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateCallAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetHealthRequest, GetHealthResponse>> GetHealthCallAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetFundingRateRequest, GetFundingRateResponse>> GetFundingRateCallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetChatsRequest, GetChatsResponse>> GetChatsCallAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetPermissionsRequest, GetPermissionsResponse>> GetPermissionsCallAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalanceRequest, GetBalanceResponse>> GetBalanceCallAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralAccountsRequest, GetCollateralAccountsResponse>> GetCollateralAccountsCallAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAddressesRequest, GetAddressesResponse>> GetAddressesCallAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinInsRequest, GetCoinInsResponse>> GetCoinInsCallAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinOutsRequest, GetCoinOutsResponse>> GetCoinOutsCallAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBankAccountsRequest, GetBankAccountsResponse>> GetBankAccountsCallAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetDepositsRequest, GetDepositsResponse>> GetDepositsCallAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<WithdrawRequest, WithdrawResponse>> WithdrawCallAsync(
        WithdrawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetWithdrawalsRequest, GetWithdrawalsResponse>> GetWithdrawalsCallAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderCallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelChildOrderRequest, CancelChildOrderResponse>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelParentOrderRequest, CancelParentOrderResponse>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelAllChildOrdersRequest, CancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetChildOrdersRequest, GetChildOrdersResponse>> GetChildOrdersCallAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrdersRequest, GetParentOrdersResponse>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>> GetExecutionsPrivateCallAsync(
        GetExecutionsPrivateRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalanceHistoryRequest, GetBalanceHistoryResponse>> GetBalanceHistoryCallAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetPositionsRequest, GetPositionsResponse>> GetPositionsCallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralHistoryRequest, GetCollateralHistoryResponse>> GetCollateralHistoryCallAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionCallAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default);
}
