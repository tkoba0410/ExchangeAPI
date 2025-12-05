using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Dtos;

namespace ExchangeApi.Core.Contracts;

/// <summary>
/// 市場データ（REST）を取得するための抽象インターフェース。
/// </summary>
public interface IMarketDataApi
{
    Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default);

    Task<OrderBook> GetOrderBookAsync(string symbol, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Execution>> GetExecutionsAsync(string symbol, CancellationToken cancellationToken = default);
}
