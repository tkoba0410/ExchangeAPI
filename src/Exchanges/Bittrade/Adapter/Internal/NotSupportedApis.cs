using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal;

internal sealed class NotSupportedTradingApi : ITradingApi
{
    private readonly ExchangeCode _exchange;

    public NotSupportedTradingApi(ExchangeCode exchange) => _exchange = exchange;

    private ExchangeFeatureNotSupportedException NotSupported(string feature) => new(_exchange, feature);

    public Task<OrderResult> PlaceLimitOrderAsync(
        Symbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");

    public Task<OrderResult> PlaceMarketOrderAsync(
        Symbol symbol,
        Side side,
        Size size,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");

    public Task<OrderResult> PlaceStopOrderAsync(
        Symbol symbol,
        Side side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");

    public Task<CancelResult> CancelOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");

    public Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");

    public Task<OrderStatus> GetOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Trading");
}

internal sealed class NotSupportedAccountApi : IAccountApi
{
    private readonly ExchangeCode _exchange;

    public NotSupportedAccountApi(ExchangeCode exchange) => _exchange = exchange;

    private ExchangeFeatureNotSupportedException NotSupported(string feature) => new(_exchange, feature);

    public Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
        throw NotSupported("Account");

    public Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        throw NotSupported("Account");
}
