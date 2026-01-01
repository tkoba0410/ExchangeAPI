using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;
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

    public BittradeRawApi(IBittradeWireApi wire)
        : this(
            publicApi: new BittradePublicApi(wire ?? throw new ArgumentNullException(nameof(wire))),
            privateApi: new BittradePrivateApi(wire.Account),
            privateTradingApi: new BittradePrivateTradingApi(wire.Trading))
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
    public Task<RawSymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default) =>
        _publicApi.GetSymbolsAsync(cancellationToken);

    public Task<RawCurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default) =>
        _publicApi.GetCurrenciesAsync(cancellationToken);

    public Task<RawTimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default) =>
        _publicApi.GetTimestampAsync(cancellationToken);

    public Task<RawKlinesResponse> GetKlinesAsync(RawSymbol symbol, string period, int? size = null, CancellationToken cancellationToken = default) =>
        _publicApi.GetKlinesAsync(symbol, period, size, cancellationToken);

    public Task<RawMergedResponse> GetMergedTickerAsync(RawSymbol symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetMergedTickerAsync(symbol, cancellationToken);

    public Task<RawTickersResponse> GetTickersAsync(CancellationToken cancellationToken = default) =>
        _publicApi.GetTickersAsync(cancellationToken);

    public Task<RawDepthResponse> GetDepthAsync(RawSymbol symbol, string? type = null, CancellationToken cancellationToken = default) =>
        _publicApi.GetDepthAsync(symbol, type, cancellationToken);

    public Task<RawTradeResponse> GetTradesAsync(RawSymbol symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetTradesAsync(symbol, cancellationToken);

    public Task<RawTradeHistoryResponse> GetTradeHistoryAsync(RawSymbol symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetTradeHistoryAsync(symbol, cancellationToken);

    public Task<RawRetailMaintainTimeResponse> GetRetailMaintainTimeAsync(CancellationToken cancellationToken = default) =>
        _publicApi.GetRetailMaintainTimeAsync(cancellationToken);

    // Private GET
    public Task<RawAccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default) =>
        _privateApi.GetAccountsAsync(cancellationToken);

    public Task<RawBalancesResponse> GetAccountBalanceAsync(string accountId, CancellationToken cancellationToken = default) =>
        _privateApi.GetAccountBalanceAsync(accountId, cancellationToken);

    public Task<BittradeRawCall<RawSymbolsResponse, JsonElement>> GetSymbolsCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetSymbolsCallAsync(cancellationToken);

    public Task<BittradeRawCall<RawBalancesResponse, JsonElement>> GetAccountBalanceCallAsync(
        string accountId,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetAccountBalanceCallAsync(accountId, cancellationToken);

    public Task<RawOpenOrdersResponse> GetOpenOrdersAsync(RawSymbol symbol, string accountId, CancellationToken cancellationToken = default) =>
        _privateApi.GetOpenOrdersAsync(symbol, accountId, cancellationToken);

    public Task<RawOrderDetailResponse> GetOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default) =>
        _privateApi.GetOrderAsync(orderId, cancellationToken);

    public Task<RawOrderMatchResultsResponse> GetOrderMatchResultsAsync(RawOrderId orderId, CancellationToken cancellationToken = default) =>
        _privateApi.GetOrderMatchResultsAsync(orderId, cancellationToken);

    public Task<RawOrdersResponse> GetOrdersAsync(
        RawSymbol symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrdersAsync(symbol, states, startDate, endDate, from, direct, size, cancellationToken);

    public Task<RawMatchResultsResponse> GetMatchResultsAsync(
        RawSymbol? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetMatchResultsAsync(symbol, types, startDate, endDate, from, direct, size, cancellationToken);

    public Task<RawDepositWithdrawsResponse> GetDepositWithdrawsAsync(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetDepositWithdrawsAsync(type, currency, from, size, direct, cancellationToken);

    public Task<RawRetailOrdersResponse> GetRetailOrdersAsync(
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetRetailOrdersAsync(direct, status, startTime, endTime, cancellationToken);

    // Private POST
    public Task<RawPlaceOrderResponse> CreateOrderAsync(RawCreateOrderRequest request, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateOrderAsync(request, cancellationToken);

    public Task<RawCancelOrderResponse> CancelOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOrderAsync(orderId, cancellationToken);

    public Task<RawCancelOrdersResponse> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOrdersAsync(request, cancellationToken);

    public Task<RawCancelOpenOrdersResponse> CancelOpenOrdersAsync(RawCancelOpenOrdersRequest request, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOpenOrdersAsync(request, cancellationToken);

    public Task<RawCreateWithdrawResponse> CreateWithdrawAsync(RawCreateWithdrawRequest request, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateWithdrawAsync(request, cancellationToken);

    public Task<RawCancelWithdrawResponse> CancelWithdrawAsync(string withdrawId, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelWithdrawAsync(withdrawId, cancellationToken);

    public Task<RawRetailOrderResponse> CreateRetailOrderAsync(RawCreateRetailOrderRequest request, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateRetailOrderAsync(request, cancellationToken);
}
