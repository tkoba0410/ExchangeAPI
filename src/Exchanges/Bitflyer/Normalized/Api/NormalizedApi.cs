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
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api.Markets;
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

    public Task<Call<PublicRequests.GetMarketsRequest, GetMarketsResponse>> GetMarketsCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetMarketsCallAsync(cancellationToken);

    public Task<Call<PublicRequests.GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTickerCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        ProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetExecutionsPublicCallAsync(productCode, count, before, after, cancellationToken);

    public Task<Call<PublicRequests.GetHealthRequest, GetHealthResponse>> GetHealthCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetHealthCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardStateCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetCorporateLeverageCallAsync(cancellationToken);

    public Task<Call<PublicRequests.GetFundingRateRequest, GetFundingRateResponse>> GetFundingRateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetFundingRateCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetChatsRequest, GetChatsResponse>> GetChatsCallAsync(
        FreeText? fromDate = null,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetChatsCallAsync(fromDate, cancellationToken);

    public Task<Call<PrivateRequests.SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        PrivateRequests.OrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.SendChildOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.CancelChildOrderRequest, CancelChildOrderResponse>> CancelChildOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _privateApi.CancelChildOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<PrivateRequests.GetChildOrdersRequest, GetChildOrdersResponse>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetChildOrdersCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderCallAsync(
        PrivateRequests.SendParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.SendParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.CancelParentOrderRequest, CancelParentOrderResponse>> CancelParentOrderCallAsync(
        PrivateRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.CancelParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetParentOrdersRequest, GetParentOrdersResponse>> GetParentOrdersCallAsync(
        PrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetParentOrdersCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderCallAsync(
        PrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetBalanceRequest, GetBalanceResponse>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetBalanceCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetPermissionsRequest, GetPermissionsResponse>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetPermissionsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCollateralCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralAccountsRequest, GetCollateralAccountsResponse>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCollateralAccountsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetAddressesRequest, GetAddressesResponse>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetAddressesCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCoinInsRequest, GetCoinInsResponse>> GetCoinInsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCoinInsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetCoinOutsRequest, GetCoinOutsResponse>> GetCoinOutsCallAsync(
        FreeText? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCoinOutsCallAsync(messageId, count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetBankAccountsRequest, GetBankAccountsResponse>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetBankAccountsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetDepositsRequest, GetDepositsResponse>> GetDepositsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetDepositsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.WithdrawRequest, WithdrawResponse>> WithdrawCallAsync(
        CurrencyCode currencyCode,
        int bankAccountId,
        decimal amount,
        FreeText? code = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.WithdrawCallAsync(currencyCode, bankAccountId, amount, code, cancellationToken);

    public Task<Call<PrivateRequests.GetWithdrawalsRequest, GetWithdrawalsResponse>> GetWithdrawalsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetWithdrawalsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetExecutionsPrivateCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetBalanceHistoryRequest, GetBalanceHistoryResponse>> GetBalanceHistoryCallAsync(
        CurrencyCode? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetBalanceHistoryCallAsync(currencyCode, count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetPositionsRequest, GetPositionsResponse>> GetPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetPositionsCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralHistoryRequest, GetCollateralHistoryResponse>> GetCollateralHistoryCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetCollateralHistoryCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetTradingCommissionCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.CancelAllChildOrdersRequest, CancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.CancelAllChildOrdersCallAsync(symbol, cancellationToken);
}
