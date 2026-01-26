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
using ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;
using NormalizedRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;
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

    public Task<Call<NormalizedRequests.GetMarketsRequest, IReadOnlyList<BitflyerMarketNormalized>>> GetMarketsCallAsync(
        string? region = null,
        CancellationToken cancellationToken = default) =>
        _exchangeInfo.GetMarketsCallAsync(region, cancellationToken);

    public Task<Call<NormalizedRequests.GetTickerRequest, BitflyerTickerNormalized>> GetTickerCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetTickerCallAsync(productCode, cancellationToken);

    public Task<Call<NormalizedRequests.GetOrderBookRequest, BitflyerOrderBookNormalized>> GetBoardCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetBoardCallAsync(productCode, cancellationToken);

    public Task<Call<NormalizedRequests.GetExecutionsRequest, IReadOnlyList<BitflyerExecutionNormalized>>> GetExecutionsPublicCallAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _marketData.GetExecutionsPublicCallAsync(productCode, count, before, after, cancellationToken);

    public Task<Call<NormalizedRequests.GetHealthRequest, BitflyerHealthNormalized>> GetHealthCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetHealthCallAsync(productCode, cancellationToken);

    public Task<Call<NormalizedRequests.GetBoardStateRequest, BitflyerBoardStateNormalized>> GetBoardStateCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetBoardStateCallAsync(productCode, cancellationToken);

    public Task<Call<NormalizedRequests.GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default) =>
        _marketData.GetChatsCallAsync(fromDate, region, cancellationToken);

    public Task<Call<NormalizedRequests.PlaceOrderRequest, BitflyerOrderResult>> PlaceOrderCallAsync(
        BitflyerOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.PlaceOrderCallAsync(request, cancellationToken);

    public Task<Call<NormalizedRequests.CancelOrderRequest, BitflyerCancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _trading.CancelOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<NormalizedRequests.GetOpenOrdersRequest, IReadOnlyList<BitflyerOpenOrder>>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _trading.GetChildOrdersCallAsync(symbol, cancellationToken);

    public Task<Call<NormalizedRequests.GetOrderRequest, BitflyerOrderStatus>> GetChildOrdersCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _trading.GetChildOrdersCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<NormalizedRequests.SendParentOrderRequest, BitflyerParentOrderAcceptance>> SendParentOrderCallAsync(
        NormalizedRequests.SendParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.SendParentOrderCallAsync(request, cancellationToken);

    public Task<Call<NormalizedRequests.CancelParentOrderRequest, BitflyerParentOrderCancelResult>> CancelParentOrderCallAsync(
        NormalizedRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.CancelParentOrderCallAsync(request, cancellationToken);

    public Task<Call<NormalizedRequests.GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        NormalizedRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.GetParentOrdersCallAsync(request, cancellationToken);

    public Task<Call<NormalizedRequests.GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        NormalizedRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.GetParentOrderCallAsync(request, cancellationToken);

    public Task<Call<NormalizedRequests.GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default) =>
        _account.GetBalanceCallAsync(cancellationToken);

    public Task<Call<NormalizedRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _account.GetExecutionsPrivateCallAsync(symbol, cancellationToken);

    public Task<Call<NormalizedRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _account.GetTradingCommissionCallAsync(symbol, cancellationToken);
}
