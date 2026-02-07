using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;
using PublicRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Requests;
using PrivateRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Api;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Api;

public sealed class BitflyerNormalizedApi : IBitflyerNormalizedApi
{
    private readonly BitflyerNormalizedPublicApi _publicApi;
    private readonly BitflyerNormalizedPrivateApi _privateApi;

    private BitflyerNormalizedApi(
        BitflyerNormalizedPublicApi publicApi,
        BitflyerNormalizedPrivateApi privateApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
    }

    internal static BitflyerNormalizedApi FromRaw(IBitflyerRawApi raw, IBitflyerMarketResolver markets)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        return new BitflyerNormalizedApi(
            publicApi: new BitflyerNormalizedPublicApi(raw),
            privateApi: new BitflyerNormalizedPrivateApi(raw, markets));
    }

    public Task<Call<PublicRequests.GetMarketsRequest, IReadOnlyList<BitflyerMarketNormalized>>> GetMarketsCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetMarketsCallAsync(cancellationToken);

    public Task<Call<PublicRequests.GetTickerRequest, BitflyerTickerNormalized>> GetTickerCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTickerCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetBoardRequest, BitflyerOrderBookNormalized>> GetBoardCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetExecutionsPublicRequest, IReadOnlyList<BitflyerExecutionNormalized>>> GetExecutionsPublicCallAsync(
        ProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetExecutionsPublicCallAsync(productCode, count, before, after, cancellationToken);

    public Task<Call<PublicRequests.GetHealthRequest, BitflyerHealthNormalized>> GetHealthCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetHealthCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetBoardStateRequest, BitflyerBoardStateNormalized>> GetBoardStateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardStateCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetCorporateLeverageRequest, BitflyerCorporateLeverageNormalized>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetCorporateLeverageCallAsync(cancellationToken);

    public Task<Call<PublicRequests.GetFundingRateRequest, BitflyerFundingRateNormalized>> GetFundingRateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetFundingRateCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        FreeText? fromDate = null,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetChatsCallAsync(fromDate, cancellationToken);

    public Task<Call<PrivateRequests.SendChildOrderRequest, BitflyerOrderResult>> SendChildOrderCallAsync(
        PrivateRequests.BitflyerOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.SendChildOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.CancelChildOrderRequest, BitflyerCancelResult>> CancelChildOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _privateApi.CancelChildOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<PrivateRequests.GetChildOrdersRequest, IReadOnlyList<BitflyerOpenOrder>>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetChildOrdersCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetChildOrdersByOrderKeyRequest, BitflyerOrderStatus>> GetChildOrdersCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetChildOrdersCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<PrivateRequests.SendParentOrderRequest, BitflyerParentOrderAcceptance>> SendParentOrderCallAsync(
        PrivateRequests.SendParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.SendParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.CancelParentOrderRequest, BitflyerParentOrderCancelResult>> CancelParentOrderCallAsync(
        PrivateRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.CancelParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        PrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetParentOrdersCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        PrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetBalanceRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetBalanceCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetPermissionsRequest, IReadOnlyList<FreeText>>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetPermissionsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralRequest, BitflyerCollateralNormalized>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCollateralCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralAccountsRequest, IReadOnlyList<BitflyerCollateralAccountNormalized>>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCollateralAccountsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetAddressesRequest, BitflyerRawJsonNormalized>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetAddressesCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCoinInsRequest, BitflyerRawJsonNormalized>> GetCoinInsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCoinInsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetCoinOutsRequest, BitflyerRawJsonNormalized>> GetCoinOutsCallAsync(
        FreeText? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCoinOutsCallAsync(messageId, count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetBankAccountsRequest, BitflyerRawJsonNormalized>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetBankAccountsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetDepositsRequest, BitflyerRawJsonNormalized>> GetDepositsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetDepositsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.WithdrawRequest, BitflyerWithdrawResultNormalized>> WithdrawCallAsync(
        CurrencyCode currencyCode,
        int bankAccountId,
        decimal amount,
        FreeText? code = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.WithdrawCallAsync(currencyCode, bankAccountId, amount, code, cancellationToken);

    public Task<Call<PrivateRequests.GetWithdrawalsRequest, BitflyerRawJsonNormalized>> GetWithdrawalsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetWithdrawalsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetExecutionsPrivateRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetExecutionsPrivateCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetBalanceHistoryRequest, BitflyerRawJsonNormalized>> GetBalanceHistoryCallAsync(
        CurrencyCode? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetBalanceHistoryCallAsync(currencyCode, count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>> GetPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetPositionsCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralHistoryRequest, BitflyerRawJsonNormalized>> GetCollateralHistoryCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCollateralHistoryCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetTradingCommissionCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.CancelAllChildOrdersRequest, BitflyerCancelResult>> CancelAllChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.CancelAllChildOrdersCallAsync(symbol, cancellationToken);
}