using System.Collections.Generic;
using System.Threading;
using ExchangeApi.Contracts.Dtos;

namespace ExchangeApi.Contracts.Contracts;

/// <summary>
/// リアルタイム市場データ（WS）の抽象インターフェース。
/// </summary>
public interface IRealtimeMarketDataApi
{
    IAsyncEnumerable<TickerTick> SubscribeTickerAsync(string symbol, CancellationToken cancellationToken = default);

    IAsyncEnumerable<OrderBookDelta> SubscribeOrderBookAsync(string symbol, CancellationToken cancellationToken = default);

    IAsyncEnumerable<ExecutionTick> SubscribeExecutionsAsync(string symbol, CancellationToken cancellationToken = default);
}
