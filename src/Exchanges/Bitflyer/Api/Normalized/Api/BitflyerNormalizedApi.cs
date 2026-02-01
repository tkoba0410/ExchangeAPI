using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Markets;
using PublicRequests = ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Requests;
using PrivateRequests = ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;

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

    public static BitflyerNormalizedApi FromRestClient(IRestClient restClient, IBitflyerMarketResolver markets)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));
        var wire = new WireTransport(restClient);
        var raw = new BitflyerRawApi(wire);

        return FromRaw(raw, markets);
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
        string productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTickerCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetOrderBookRequest, BitflyerOrderBookNormalized>> GetBoardCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetExecutionsRequest, IReadOnlyList<BitflyerExecutionNormalized>>> GetExecutionsPublicCallAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetExecutionsPublicCallAsync(productCode, count, before, after, cancellationToken);

    public Task<Call<PublicRequests.GetHealthRequest, BitflyerHealthNormalized>> GetHealthCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetHealthCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetBoardStateRequest, BitflyerBoardStateNormalized>> GetBoardStateCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetBoardStateCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetCorporateLeverageRequest, BitflyerCorporateLeverageNormalized>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetCorporateLeverageCallAsync(cancellationToken);

    public Task<Call<PublicRequests.GetFundingRateRequest, BitflyerFundingRateNormalized>> GetFundingRateCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetFundingRateCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        string? fromDate = null,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetChatsCallAsync(fromDate, cancellationToken);

    public Task<Call<PrivateRequests.PlaceOrderRequest, BitflyerOrderResult>> SendChildOrderCallAsync(
        PrivateRequests.BitflyerOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.SendChildOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.CancelOrderRequest, BitflyerCancelResult>> CancelChildOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _privateApi.CancelChildOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<PrivateRequests.GetOpenOrdersRequest, IReadOnlyList<BitflyerOpenOrder>>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetChildOrdersCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetOrderRequest, BitflyerOrderStatus>> GetChildOrdersCallAsync(
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

    public Task<Call<PrivateRequests.GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetBalanceCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
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
        string? messageId = null,
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
        string currencyCode,
        int bankAccountId,
        decimal amount,
        string? code = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.WithdrawCallAsync(currencyCode, bankAccountId, amount, code, cancellationToken);

    public Task<Call<PrivateRequests.GetWithdrawalsRequest, BitflyerRawJsonNormalized>> GetWithdrawalsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetWithdrawalsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetExecutionsPrivateCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetBalanceHistoryRequest, BitflyerRawJsonNormalized>> GetBalanceHistoryCallAsync(
        string? currencyCode = null,
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
