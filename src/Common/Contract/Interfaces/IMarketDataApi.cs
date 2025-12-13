using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Contract.Dtos;
using Common.Contract.Enums;

namespace Common.Contract.Interfaces;

/// <summary>
/// 市場データ（REST）を取得するための抽象インターフェース。
/// </summary>
public interface IMarketDataApi
{
    Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default);

    Task<OrderBook> GetOrderBookAsync(string symbol, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MarketExecution>> GetMarketExecutionsAsync(string symbol, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(
        string symbol,
        string timescale,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        CancellationToken cancellationToken = default);
}
