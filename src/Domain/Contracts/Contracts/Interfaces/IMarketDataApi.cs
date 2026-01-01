using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
namespace ExchangeApi.Contracts.Interfaces;

/// <summary>
/// 市場データ（REST）を取得するための抽象インターフェース。
/// </summary>
public interface IMarketDataApi
{
    Task<Ticker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<OrderBook> GetOrderBookAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(
        Symbol symbol,
        TimeSpan timescale,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default);

    Task<ApiCall<GetTickerRequest, Ticker, ApiError>> GetTickerCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<ApiCall<GetOrderBookRequest, OrderBook, ApiError>> GetOrderBookCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<ApiCall<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>, ApiError>> GetMarketExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);
}
