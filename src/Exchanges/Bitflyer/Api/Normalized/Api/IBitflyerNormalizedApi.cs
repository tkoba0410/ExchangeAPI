using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Requests;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;

public interface IBitflyerNormalizedApi
{
    Task<Call<GetMarketsRequest, IReadOnlyList<BitflyerMarketNormalized>>> GetMarketsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetTickerRequest, BitflyerTickerNormalized>> GetTickerCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderBookRequest, BitflyerOrderBookNormalized>> GetBoardCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsRequest, IReadOnlyList<BitflyerExecutionNormalized>>> GetExecutionsPublicCallAsync(
        ProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetHealthRequest, BitflyerHealthNormalized>> GetHealthCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardStateRequest, BitflyerBoardStateNormalized>> GetBoardStateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetCorporateLeverageRequest, BitflyerCorporateLeverageNormalized>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetFundingRateRequest, BitflyerFundingRateNormalized>> GetFundingRateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<Call<GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        FreeText? fromDate = null,
        CancellationToken cancellationToken = default);

    Task<Call<PlaceOrderRequest, BitflyerOrderResult>> SendChildOrderCallAsync(
        BitflyerOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOrderRequest, BitflyerCancelResult>> CancelChildOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

    Task<Call<GetOpenOrdersRequest, IReadOnlyList<BitflyerOpenOrder>>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderRequest, BitflyerOrderStatus>> GetChildOrdersCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

    Task<Call<SendParentOrderRequest, BitflyerParentOrderAcceptance>> SendParentOrderCallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelParentOrderRequest, BitflyerParentOrderCancelResult>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetPermissionsRequest, IReadOnlyList<FreeText>>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralRequest, BitflyerCollateralNormalized>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralAccountsRequest, IReadOnlyList<BitflyerCollateralAccountNormalized>>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetAddressesRequest, BitflyerRawJsonNormalized>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinInsRequest, BitflyerRawJsonNormalized>> GetCoinInsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetCoinOutsRequest, BitflyerRawJsonNormalized>> GetCoinOutsCallAsync(
        FreeText? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetBankAccountsRequest, BitflyerRawJsonNormalized>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetDepositsRequest, BitflyerRawJsonNormalized>> GetDepositsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<WithdrawRequest, BitflyerWithdrawResultNormalized>> WithdrawCallAsync(
        CurrencyCode currencyCode,
        int bankAccountId,
        decimal amount,
        FreeText? code = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetWithdrawalsRequest, BitflyerRawJsonNormalized>> GetWithdrawalsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalanceHistoryRequest, BitflyerRawJsonNormalized>> GetBalanceHistoryCallAsync(
        CurrencyCode? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>> GetPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetCollateralHistoryRequest, BitflyerRawJsonNormalized>> GetCollateralHistoryCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<CancelAllChildOrdersRequest, BitflyerCancelResult>> CancelAllChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);
}
