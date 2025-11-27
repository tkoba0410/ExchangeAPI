using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Dtos;

namespace ExchangeApi.Abstractions.Contracts;

/// <summary>
/// 発注系の抽象インターフェース。
/// Stage3 では MARKET 注文 1 本をサポートする。
/// </summary>
public interface IExchangeTradingClient
{
    /// <summary>
    /// 注文を送信する。
    /// </summary>
    Task<OrderResult> SendOrderAsync(OrderRequest request, CancellationToken cancellationToken = default);
}

