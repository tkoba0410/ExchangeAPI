using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Account;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Trading;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;
using PublicRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Requests;
using PrivateRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Api;

public sealed class BitflyerNormalizedApi : IBitflyerNormalizedApi
{
    private readonly BitflyerNormalizedMarketDataApi _marketData;
    private readonly BitflyerNormalizedExchangeInfoApi _exchangeInfo;
    private readonly BitflyerNormalizedTradingApi _trading;
    private readonly BitflyerNormalizedAccountApi _account;

    private BitflyerNormalizedApi(
        BitflyerNormalizedMarketDataApi marketData,
        BitflyerNormalizedExchangeInfoApi exchangeInfo,
        BitflyerNormalizedTradingApi trading,
        BitflyerNormalizedAccountApi account)
    {
        _marketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        _exchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
        _account = account ?? throw new ArgumentNullException(nameof(account));
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
            marketData: new BitflyerNormalizedMarketDataApi(raw),
            exchangeInfo: new BitflyerNormalizedExchangeInfoApi(raw),
            trading: new BitflyerNormalizedTradingApi(raw, markets),
            account: new BitflyerNormalizedAccountApi(raw, markets));
    }

    public Task<Call<PublicRequests.GetMarketsRequest, IReadOnlyList<BitflyerMarketNormalized>>> GetMarketsCallAsync(
        CancellationToken cancellationToken = default) =>
        _exchangeInfo.GetMarketsCallAsync(cancellationToken);

    public Task<Call<PublicRequests.GetTickerRequest, BitflyerTickerNormalized>> GetTickerCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetTickerCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetOrderBookRequest, BitflyerOrderBookNormalized>> GetBoardCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetBoardCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetExecutionsRequest, IReadOnlyList<BitflyerExecutionNormalized>>> GetExecutionsPublicCallAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _marketData.GetExecutionsPublicCallAsync(productCode, count, before, after, cancellationToken);

    public Task<Call<PublicRequests.GetHealthRequest, BitflyerHealthNormalized>> GetHealthCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetHealthCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetBoardStateRequest, BitflyerBoardStateNormalized>> GetBoardStateCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetBoardStateCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetCorporateLeverageRequest, BitflyerCorporateLeverageNormalized>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default) =>
        _marketData.GetCorporateLeverageCallAsync(cancellationToken);

    public Task<Call<PublicRequests.GetFundingRateRequest, BitflyerFundingRateNormalized>> GetFundingRateCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetFundingRateCallAsync(productCode, cancellationToken);

    public Task<Call<PublicRequests.GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        string? fromDate = null,
        CancellationToken cancellationToken = default) =>
        _marketData.GetChatsCallAsync(fromDate, cancellationToken);

    public Task<Call<PrivateRequests.PlaceOrderRequest, BitflyerOrderResult>> SendChildOrderCallAsync(
        PrivateRequests.BitflyerOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.SendChildOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.CancelOrderRequest, BitflyerCancelResult>> CancelChildOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _trading.CancelChildOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<PrivateRequests.GetOpenOrdersRequest, IReadOnlyList<BitflyerOpenOrder>>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _trading.GetChildOrdersCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetOrderRequest, BitflyerOrderStatus>> GetChildOrdersCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _trading.GetChildOrdersCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<PrivateRequests.SendParentOrderRequest, BitflyerParentOrderAcceptance>> SendParentOrderCallAsync(
        PrivateRequests.SendParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.SendParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.CancelParentOrderRequest, BitflyerParentOrderCancelResult>> CancelParentOrderCallAsync(
        PrivateRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.CancelParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        PrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.GetParentOrdersCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        PrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.GetParentOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default) =>
        _account.GetBalanceCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default) =>
        _account.GetPermissionsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralRequest, BitflyerCollateralNormalized>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default) =>
        _account.GetCollateralCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralAccountsRequest, IReadOnlyList<BitflyerCollateralAccountNormalized>>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default) =>
        _account.GetCollateralAccountsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetAddressesRequest, BitflyerRawJsonNormalized>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default) =>
        _account.GetAddressesCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetCoinInsRequest, BitflyerRawJsonNormalized>> GetCoinInsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _account.GetCoinInsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetCoinOutsRequest, BitflyerRawJsonNormalized>> GetCoinOutsCallAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _account.GetCoinOutsCallAsync(messageId, count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetBankAccountsRequest, BitflyerRawJsonNormalized>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default) =>
        _account.GetBankAccountsCallAsync(cancellationToken);

    public Task<Call<PrivateRequests.GetDepositsRequest, BitflyerRawJsonNormalized>> GetDepositsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _account.GetDepositsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.WithdrawRequest, BitflyerWithdrawResultNormalized>> WithdrawCallAsync(
        string currencyCode,
        int bankAccountId,
        decimal amount,
        string? code = null,
        CancellationToken cancellationToken = default) =>
        _account.WithdrawCallAsync(currencyCode, bankAccountId, amount, code, cancellationToken);

    public Task<Call<PrivateRequests.GetWithdrawalsRequest, BitflyerRawJsonNormalized>> GetWithdrawalsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _account.GetWithdrawalsCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _account.GetExecutionsPrivateCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetBalanceHistoryRequest, BitflyerRawJsonNormalized>> GetBalanceHistoryCallAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _account.GetBalanceHistoryCallAsync(currencyCode, count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetPositionsRequest, IReadOnlyList<BitflyerPositionNormalized>>> GetPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _account.GetPositionsCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetCollateralHistoryRequest, BitflyerRawJsonNormalized>> GetCollateralHistoryCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _account.GetCollateralHistoryCallAsync(count, before, after, cancellationToken);

    public Task<Call<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _account.GetTradingCommissionCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.CancelAllChildOrdersRequest, BitflyerCancelResult>> CancelAllChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _trading.CancelAllChildOrdersCallAsync(symbol, cancellationToken);
}
