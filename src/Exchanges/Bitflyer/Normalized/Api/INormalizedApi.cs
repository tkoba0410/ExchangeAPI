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

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Api;

public interface INormalizedApi
{
    Task<Call<GetMarketsRequest, IReadOnlyList<MarketNormalized>>> GetMarketsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetTickerRequest, TickerNormalized>> GetTickerCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardRequest, OrderBookNormalized>> GetBoardCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsPublicRequest, IReadOnlyList<ExecutionNormalized>>> GetExecutionsPublicCallAsync(
        ProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetHealthRequest, HealthNormalized>> GetHealthCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardStateRequest, BoardStateNormalized>> GetBoardStateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetCorporateLeverageRequest, CorporateLeverageNormalized>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetFundingRateRequest, FundingRateNormalized>> GetFundingRateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetChatsRequest, IReadOnlyList<ChatNormalized>>> GetChatsCallAsync(
        FreeText? fromDate = null,
        CancellationToken cancellationToken = default);

    Task<Call<SendChildOrderRequest, OrderResult>> SendChildOrderCallAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelChildOrderRequest, CancelResult>> CancelChildOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

    Task<Call<GetChildOrdersRequest, IReadOnlyList<OpenOrderNormalized>>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetChildOrdersByOrderKeyRequest, OrderStatus>> GetChildOrdersCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

    Task<Call<SendParentOrderRequest, ParentOrderAcceptance>> SendParentOrderCallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelParentOrderRequest, ParentOrderCancelResult>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrderNormalized>>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrderRequest, ParentOrderDetailNormalized>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalanceRequest, IReadOnlyList<BalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetPermissionsRequest, IReadOnlyList<FreeText>>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralRequest, CollateralNormalized>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralAccountsRequest, IReadOnlyList<CollateralAccountNormalized>>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetAddressesRequest, RawJsonNormalized>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinInsRequest, RawJsonNormalized>> GetCoinInsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinOutsRequest, RawJsonNormalized>> GetCoinOutsCallAsync(
        FreeText? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetBankAccountsRequest, RawJsonNormalized>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetDepositsRequest, RawJsonNormalized>> GetDepositsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<WithdrawRequest, WithdrawResultNormalized>> WithdrawCallAsync(
        CurrencyCode currencyCode,
        int bankAccountId,
        decimal amount,
        FreeText? code = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetWithdrawalsRequest, RawJsonNormalized>> GetWithdrawalsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsPrivateRequest, IReadOnlyList<ExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalanceHistoryRequest, RawJsonNormalized>> GetBalanceHistoryCallAsync(
        CurrencyCode? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetPositionsRequest, IReadOnlyList<PositionNormalized>>> GetPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralHistoryRequest, RawJsonNormalized>> GetCollateralHistoryCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetTradingCommissionRequest, TradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<CancelAllChildOrdersRequest, CancelResult>> CancelAllChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);
}