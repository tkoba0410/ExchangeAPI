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
    Task<Call<GetMarketsRequest, IReadOnlyList<Market>>> GetMarketsCallAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardRequest, Board>> GetBoardCallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsRequest, IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsPublicCallAsync(
        GetExecutionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardStateRequest, BoardStateResponse>> GetBoardStateCallAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetHealthRequest, HealthResponse>> GetHealthCallAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetFundingRateRequest, FundingRateResponse>> GetFundingRateCallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCorporateLeverageRequest, CorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetChatsRequest, IReadOnlyList<Chat>>> GetChatsCallAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetPermissionsRequest, IReadOnlyList<FreeText>>> GetPermissionsCallAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalancesRequest, IReadOnlyList<BalanceResponse>>> GetBalanceCallAsync(
        GetBalancesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralRequest, CollateralResponse>> GetCollateralCallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralAccountsRequest, IReadOnlyList<CollateralAccount>>> GetCollateralAccountsCallAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAddressesRequest, RawJsonResponse>> GetAddressesCallAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinInsRequest, RawJsonResponse>> GetCoinInsCallAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinOutsRequest, RawJsonResponse>> GetCoinOutsCallAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBankAccountsRequest, RawJsonResponse>> GetBankAccountsCallAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetDepositsRequest, RawJsonResponse>> GetDepositsCallAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateWithdrawalRequest, CreateWithdrawalResponse>> WithdrawCallAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetWithdrawalsRequest, RawJsonResponse>> GetWithdrawalsCallAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateChildOrderRequest, RawSendChildOrderResponse>> SendChildOrderCallAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateParentOrderRequest, RawSendParentOrderResponse>> SendParentOrderCallAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelChildOrderRequest, RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelParentOrderRequest, RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelAllChildOrdersRequest, RawCancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetChildOrdersRequest, IReadOnlyList<RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrdersRequest, IReadOnlyList<RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrderRequest, RawGetParentOrderResponse>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsPrivateCallAsync(
        GetAccountExecutionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalanceHistoryRequest, RawJsonResponse>> GetBalanceHistoryCallAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetPositionsRequest, IReadOnlyList<PositionResponse>>> GetPositionsCallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralHistoryRequest, RawJsonResponse>> GetCollateralHistoryCallAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTradingCommissionRequest, RawJsonResponse>> GetTradingCommissionCallAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default);
}
