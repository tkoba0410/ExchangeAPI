using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Requests;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;
using ExchangeApi.Primitives.CallCommon;
using CancelAllChildOrdersResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.CancelResult;
using CancelChildOrderResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.CancelResult;
using CancelParentOrderResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.ParentOrderCancelResult;
using GetAddressesResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.RawJsonNormalized;
using GetBalanceHistoryResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.RawJsonNormalized;
using GetBalanceResponse = global::System.Collections.Generic.IReadOnlyList<global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.BalanceEntryNormalized>;
using GetBankAccountsResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.RawJsonNormalized;
using GetBoardResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.OrderBookNormalized;
using GetBoardStateResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.BoardStateNormalized;
using GetChatsResponse = global::System.Collections.Generic.IReadOnlyList<global::ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.ChatNormalized>;
using GetChildOrdersResponse = global::System.Collections.Generic.IReadOnlyList<global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.OpenOrder>;
using GetCoinInsResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.RawJsonNormalized;
using GetCoinOutsResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.RawJsonNormalized;
using GetCollateralAccountsResponse = global::System.Collections.Generic.IReadOnlyList<global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.CollateralAccountNormalized>;
using GetCollateralHistoryResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.RawJsonNormalized;
using GetCollateralResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.CollateralNormalized;
using GetCorporateLeverageResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.CorporateLeverageNormalized;
using GetDepositsResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.RawJsonNormalized;
using GetExecutionsPrivateResponse = global::System.Collections.Generic.IReadOnlyList<global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.ExecutionAccountNormalized>;
using GetExecutionsPublicResponse = global::System.Collections.Generic.IReadOnlyList<global::ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.ExecutionNormalized>;
using GetFundingRateResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.FundingRateNormalized;
using GetHealthResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.HealthNormalized;
using GetMarketsResponse = global::System.Collections.Generic.IReadOnlyList<global::ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.MarketNormalized>;
using GetParentOrderResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.ParentOrderDetailNormalized;
using GetParentOrdersResponse = global::System.Collections.Generic.IReadOnlyList<global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.ParentOrderNormalized>;
using GetPermissionsResponse = global::System.Collections.Generic.IReadOnlyList<global::ExchangeApi.Primitives.DomainCommon.Types.FreeText>;
using GetPositionsResponse = global::System.Collections.Generic.IReadOnlyList<global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.PositionNormalized>;
using GetTickerResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.TickerNormalized;
using GetTradingCommissionResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.TradingCommissionNormalized;
using GetWithdrawalsResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.RawJsonNormalized;
using SendChildOrderResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.OrderResult;
using SendParentOrderResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.ParentOrderAcceptance;
using WithdrawResponse = global::ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.WithdrawResult;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Api;

public interface INormalizedApi
{
    Task<Call<GetMarketsRequest, GetMarketsResponse>> GetMarketsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        ProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetHealthRequest, GetHealthResponse>> GetHealthCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetFundingRateRequest, GetFundingRateResponse>> GetFundingRateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetChatsRequest, GetChatsResponse>> GetChatsCallAsync(
        FreeText? fromDate = null,
        CancellationToken cancellationToken = default);

    Task<Call<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelChildOrderRequest, CancelChildOrderResponse>> CancelChildOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

    Task<Call<GetChildOrdersRequest, GetChildOrdersResponse>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetChildOrdersByOrderKeyRequest, OrderStatus>> GetChildOrdersCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

    Task<Call<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderCallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelParentOrderRequest, CancelParentOrderResponse>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrdersRequest, GetParentOrdersResponse>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalanceRequest, GetBalanceResponse>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetPermissionsRequest, GetPermissionsResponse>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralAccountsRequest, GetCollateralAccountsResponse>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetAddressesRequest, GetAddressesResponse>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinInsRequest, GetCoinInsResponse>> GetCoinInsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinOutsRequest, GetCoinOutsResponse>> GetCoinOutsCallAsync(
        FreeText? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetBankAccountsRequest, GetBankAccountsResponse>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetDepositsRequest, GetDepositsResponse>> GetDepositsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<WithdrawRequest, WithdrawResponse>> WithdrawCallAsync(
        CurrencyCode currencyCode,
        int bankAccountId,
        decimal amount,
        FreeText? code = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetWithdrawalsRequest, GetWithdrawalsResponse>> GetWithdrawalsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalanceHistoryRequest, GetBalanceHistoryResponse>> GetBalanceHistoryCallAsync(
        CurrencyCode? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetPositionsRequest, GetPositionsResponse>> GetPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralHistoryRequest, GetCollateralHistoryResponse>> GetCollateralHistoryCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<CancelAllChildOrdersRequest, CancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);
}