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
using ExchangeApi.Exchanges.Bitflyer.Normalized.Markets;
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
    private readonly BitflyerNormalizedMarketDataFacade _marketData;
    private readonly BitflyerNormalizedExchangeInfoFacade _exchangeInfo;
    private readonly BitflyerNormalizedTradingApi _trading;
    private readonly BitflyerNormalizedAccountApi _account;

    private BitflyerNormalizedApi(
        BitflyerNormalizedMarketDataFacade marketData,
        BitflyerNormalizedExchangeInfoFacade exchangeInfo,
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
            marketData: new BitflyerNormalizedMarketDataFacade(raw),
            exchangeInfo: new BitflyerNormalizedExchangeInfoFacade(raw),
            trading: new BitflyerNormalizedTradingApi(raw, markets),
            account: new BitflyerNormalizedAccountApi(raw, markets));
    }

    public Task<Call<PublicRequests.GetMarketsRequest, IReadOnlyList<BitflyerMarketNormalized>>> GetMarketsCallAsync(
        string? region = null,
        CancellationToken cancellationToken = default) =>
        _exchangeInfo.GetMarketsCallAsync(region, cancellationToken);

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

    public Task<Call<PublicRequests.GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default) =>
        _marketData.GetChatsCallAsync(fromDate, region, cancellationToken);

    public Task<Call<PrivateRequests.PlaceOrderRequest, BitflyerOrderResult>> PlaceOrderCallAsync(
        PrivateRequests.BitflyerOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.PlaceOrderCallAsync(request, cancellationToken);

    public Task<Call<PrivateRequests.CancelOrderRequest, BitflyerCancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _trading.CancelOrderCallAsync(symbol, orderKey, cancellationToken);

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

    public Task<Call<PrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _account.GetExecutionsPrivateCallAsync(symbol, cancellationToken);

    public Task<Call<PrivateRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _account.GetTradingCommissionCallAsync(symbol, cancellationToken);
}
