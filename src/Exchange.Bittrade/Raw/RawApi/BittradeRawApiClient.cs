using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bittrade.RawApi;

namespace ExchangeApi.Adapter.Bittrade;

/// <summary>
/// Bittrade の Raw API アクセス（Public/Private/Trading をまとめた薄いファサード）。
/// </summary>
public sealed class BittradeRawApiClient
{
    private readonly IBittradePublicApi _publicApi;
    private readonly IBittradePrivateApi _privateApi;
    private readonly IBittradePrivateTradingApi _privateTradingApi;

    public BittradeRawApiClient(
        IBittradePublicApi publicApi,
        IBittradePrivateApi privateApi,
        IBittradePrivateTradingApi privateTradingApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
    }

    public Task<BittradeMergedResponse> GetTickerAsync(string symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetTickerRawAsync(symbol, cancellationToken);

    public Task<BittradeDepthResponse> GetOrderBookAsync(string symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetOrderBookRawAsync(symbol, cancellationToken);

    public Task<BittradeTradeResponse> GetTradesAsync(string symbol, CancellationToken cancellationToken = default) =>
        _publicApi.GetTradesRawAsync(symbol, cancellationToken);

    public Task<BittradeSymbolsResponse> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _publicApi.GetSymbolsRawAsync(cancellationToken);

    public Task<BittradeAccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default) =>
        _privateApi.GetAccountsAsync(cancellationToken);

    public Task<BittradeBalancesResponse> GetBalancesAsync(string accountId, CancellationToken cancellationToken = default) =>
        _privateApi.GetBalancesAsync(accountId, cancellationToken);

    public Task<BittradeOpenOrdersResponse> GetOrdersAsync(string symbol, string accountId, CancellationToken cancellationToken = default) =>
        _privateApi.GetOrdersAsync(symbol, accountId, cancellationToken);

    public Task<BittradeOrderDetailResponse> GetOrderAsync(string orderId, CancellationToken cancellationToken = default) =>
        _privateApi.GetOrderAsync(orderId, cancellationToken);

    public Task<BittradePlaceOrderResponse> PlaceOrderAsync(Dictionary<string, object?> body, CancellationToken cancellationToken = default) =>
        _privateTradingApi.PlaceOrderAsync(body, cancellationToken);

    public Task<BittradeCancelOrderResponse> CancelOrderAsync(string orderId, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOrderAsync(orderId, cancellationToken);
}
