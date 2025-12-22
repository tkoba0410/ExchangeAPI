using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
namespace ExchangeApi.Common.Interfaces;

/// <summary>
/// 取引（REST）の抽象インターフェース。
/// </summary>
public interface ITradingApi
{
    Task<OrderResult> PlaceLimitOrderAsync(
        Symbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default);

    Task<OrderResult> PlaceMarketOrderAsync(
        Symbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default);

    Task<OrderResult> PlaceStopOrderAsync(
        Symbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default);

    Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default);

    /// <summary>
    /// 注文の状態を単発で取得する。
    /// </summary>
    Task<OrderStatus> GetOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);
}
