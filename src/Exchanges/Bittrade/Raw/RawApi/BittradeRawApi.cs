using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade の Raw API アクセス（Public/Private/Trading をまとめた単一入口）。
/// </summary>
public sealed class BittradeRawApi : IBittradeRawApi
{
    private readonly IBittradePublicApi _publicApi;
    private readonly IBittradePrivateApi _privateApi;
    private readonly IBittradePrivateTradingApi _privateTradingApi;
    public IBittradeRawMarketDataApi MarketData { get; }
    public IBittradeRawTradingApi Trading { get; }

    public BittradeRawApi(IRestClient restClient)
        : this(
            publicApi: new BittradePublicApi(restClient ?? throw new ArgumentNullException(nameof(restClient))),
            privateApi: new BittradePrivateApi(restClient),
            privateTradingApi: new BittradePrivateTradingApi(restClient))
    {
    }

    internal BittradeRawApi(
        IBittradePublicApi publicApi,
        IBittradePrivateApi privateApi,
        IBittradePrivateTradingApi privateTradingApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
        MarketData = new BittradeRawMarketDataApi(_publicApi);
        Trading = new BittradeRawTradingApi(_privateApi, _privateTradingApi);
    }

    // Public
    public Task<SymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default) =>
        _publicApi.GetSymbolsAsync(cancellationToken);

    public Task<CurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default) =>
        _publicApi.GetCurrenciesAsync(cancellationToken);

    public Task<TimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default) =>
        _publicApi.GetTimestampAsync(cancellationToken);

    public Task<KlinesResponse> GetKlinesAsync(Symbol symbol, string period, int? size = null, CancellationToken cancellationToken = default) =>
        _publicApi.GetKlinesAsync(symbol, period, size, cancellationToken);

    public Task<MergedResponse> GetMergedTickerAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetMergedTickerAsync(symbol, cancellationToken);

    public Task<TickersResponse> GetTickersAsync(CancellationToken cancellationToken = default) =>
        _publicApi.GetTickersAsync(cancellationToken);

    public Task<DepthResponse> GetDepthAsync(Symbol symbol, string? type = null, CancellationToken cancellationToken = default) =>
        _publicApi.GetDepthAsync(symbol, type, cancellationToken);

    public Task<TradeResponse> GetTradesAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetTradesAsync(symbol, cancellationToken);

    public Task<TradeHistoryResponse> GetTradeHistoryAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetTradeHistoryAsync(symbol, cancellationToken);

    public Task<RetailMaintainTimeResponse> GetRetailMaintainTimeAsync(CancellationToken cancellationToken = default) =>
        _publicApi.GetRetailMaintainTimeAsync(cancellationToken);

    // Private GET
    public Task<AccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default) =>
        _privateApi.GetAccountsAsync(cancellationToken);

    public Task<BalancesResponse> GetAccountBalanceAsync(string accountId, CancellationToken cancellationToken = default) =>
        _privateApi.GetAccountBalanceAsync(accountId, cancellationToken);

    public Task<OpenOrdersResponse> GetOpenOrdersAsync(Symbol symbol, string accountId, CancellationToken cancellationToken = default) =>
        _privateApi.GetOpenOrdersAsync(symbol, accountId, cancellationToken);

    public Task<OrderDetailResponse> GetOrderAsync(OrderId orderId, CancellationToken cancellationToken = default) =>
        _privateApi.GetOrderAsync(orderId, cancellationToken);

    public Task<OrderMatchResultsResponse> GetOrderMatchResultsAsync(OrderId orderId, CancellationToken cancellationToken = default) =>
        _privateApi.GetOrderMatchResultsAsync(orderId, cancellationToken);

    public Task<OrdersResponse> GetOrdersAsync(
        Symbol symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrdersAsync(symbol, states, startDate, endDate, from, direct, size, cancellationToken);

    public Task<MatchResultsResponse> GetMatchResultsAsync(
        Symbol? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetMatchResultsAsync(symbol, types, startDate, endDate, from, direct, size, cancellationToken);

    public Task<DepositWithdrawsResponse> GetDepositWithdrawsAsync(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetDepositWithdrawsAsync(type, currency, from, size, direct, cancellationToken);

    public Task<RetailOrdersResponse> GetRetailOrdersAsync(
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetRetailOrdersAsync(direct, status, startTime, endTime, cancellationToken);

    // Private POST
    public Task<PlaceOrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateOrderAsync(request, cancellationToken);

    public Task<CancelOrderResponse> CancelOrderAsync(OrderId orderId, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOrderAsync(orderId, cancellationToken);

    public Task<CancelOrdersResponse> CancelOrdersAsync(CancelOrdersRequest request, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOrdersAsync(request, cancellationToken);

    public Task<CancelOpenOrdersResponse> CancelOpenOrdersAsync(CancelOpenOrdersRequest request, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOpenOrdersAsync(request, cancellationToken);

    public Task<CreateWithdrawResponse> CreateWithdrawAsync(CreateWithdrawRequest request, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateWithdrawAsync(request, cancellationToken);

    public Task<CancelWithdrawResponse> CancelWithdrawAsync(string withdrawId, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelWithdrawAsync(withdrawId, cancellationToken);

    public Task<RetailOrderResponse> CreateRetailOrderAsync(CreateRetailOrderRequest request, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateRetailOrderAsync(request, cancellationToken);
}
