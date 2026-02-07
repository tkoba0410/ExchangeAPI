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

public sealed class NormalizedApi : INormalizedApi
{
    private readonly NormalizedPublicApi _publicApi;
    private readonly NormalizedPrivateApi _privateApi;

    private NormalizedApi(
        NormalizedPublicApi publicApi,
        NormalizedPrivateApi privateApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
    }

    internal static NormalizedApi FromRaw(IRawApi raw, IMarketResolver markets)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        return new NormalizedApi(
            publicApi: new NormalizedPublicApi(raw),
            privateApi: new NormalizedPrivateApi(raw, markets));
    }

    public Task<Call<PublicRequests.GetMarketsRequest, IReadOnlyList<MarketNormalized>>> GetMarketsCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetMarketsCallAsync(cancellationToken);

    public Task<Call<PublicRequests.GetTickerRequest, TickerNormalized>> GetTickerCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTickerCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetBoardRequest, OrderBookNormalized>> GetBoardCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetExecutionsPublicRequest, IReadOnlyList<ExecutionNormalized>>> GetExecutionsPublicCallAsync(
        ProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetExecutionsPublicCallAsync(productCode, count, before, after, cancellationToken);

    public Task<Call<PublicRequests.GetHealthRequest, HealthNormalized>> GetHealthCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetHealthCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetBoardStateRequest, BoardStateNormalized>> GetBoardStateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardStateCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetCorporateLeverageRequest, CorporateLeverageNormalized>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetCorporateLeverageCallAsync(cancellationToken);

    public Task<Call<PublicRequests.GetFundingRateRequest, FundingRateNormalized>> GetFundingRateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetFundingRateCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetChatsRequest, IReadOnlyList<ChatNormalized>>> GetChatsCallAsync(
        FreeText? fromDate = null,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetChatsCallAsync(fromDate, cancellationToken);

    public Task<Call<PrivateRequests.SendChildOrderRequest, OrderResult>> SendChildOrderCallAsync(
        PrivateRequests.OrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.SendChildOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.CancelChildOrderRequest, CancelResult>> CancelChildOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _privateApi.CancelChildOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<PrivateRequests.GetChildOrdersRequest, IReadOnlyList<OpenOrderNormalized>>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetChildOrdersCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetChildOrdersByOrderKeyRequest, OrderStatus>> GetChildOrdersCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetChildOrdersCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<PrivateRequests.SendParentOrderRequest, ParentOrderAcceptance>> SendParentOrderCallAsync(
        PrivateRequests.SendParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.SendParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.CancelParentOrderRequest, ParentOrderCancelResult>> CancelParentOrderCallAsync(
        PrivateRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.CancelParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetParentOrdersRequest, IReadOnlyList<ParentOrderNormalized>>> GetParentOrdersCallAsync(
        PrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetParentOrdersCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetParentOrderRequest, ParentOrderDetailNormalized>> GetParentOrderCallAsync(
        PrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetBalanceRequest, IReadOnlyList<BalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetBalanceCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetPermissionsRequest, IReadOnlyList<FreeText>>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetPermissionsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralRequest, CollateralNormalized>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCollateralCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralAccountsRequest, IReadOnlyList<CollateralAccountNormalized>>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCollateralAccountsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetAddressesRequest, RawJsonNormalized>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetAddressesCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCoinInsRequest, RawJsonNormalized>> GetCoinInsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCoinInsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetCoinOutsRequest, RawJsonNormalized>> GetCoinOutsCallAsync(
        FreeText? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCoinOutsCallAsync(messageId, count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetBankAccountsRequest, RawJsonNormalized>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetBankAccountsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetDepositsRequest, RawJsonNormalized>> GetDepositsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetDepositsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.WithdrawRequest, WithdrawResultNormalized>> WithdrawCallAsync(
        CurrencyCode currencyCode,
        int bankAccountId,
        decimal amount,
        FreeText? code = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.WithdrawCallAsync(currencyCode, bankAccountId, amount, code, cancellationToken);

    public Task<Call<PrivateRequests.GetWithdrawalsRequest, RawJsonNormalized>> GetWithdrawalsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetWithdrawalsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetExecutionsPrivateRequest, IReadOnlyList<ExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetExecutionsPrivateCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetBalanceHistoryRequest, RawJsonNormalized>> GetBalanceHistoryCallAsync(
        CurrencyCode? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetBalanceHistoryCallAsync(currencyCode, count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetPositionsRequest, IReadOnlyList<PositionNormalized>>> GetPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetPositionsCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralHistoryRequest, RawJsonNormalized>> GetCollateralHistoryCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCollateralHistoryCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetTradingCommissionRequest, TradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetTradingCommissionCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.CancelAllChildOrdersRequest, CancelResult>> CancelAllChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.CancelAllChildOrdersCallAsync(symbol, cancellationToken);
}