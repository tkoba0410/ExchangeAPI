using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;

namespace ExchangeApi.Adapter.Bitflyer;

/// <summary>
/// bitFlyer のリアルタイム API スタブ。現状は未実装を明示する。
/// </summary>
public sealed class BitflyerRealtimeClient : IRealtimeMarketDataApi
{
    private readonly string _exchangeId;

    public BitflyerRealtimeClient(string exchangeId = "bitFlyer")
    {
        _exchangeId = exchangeId;
    }

    public IAsyncEnumerable<TickerTick> SubscribeTickerAsync(string symbol, CancellationToken cancellationToken = default)
    {
        return ThrowNotSupported<TickerTick>("SubscribeTicker");
    }

    public IAsyncEnumerable<OrderBookDelta> SubscribeOrderBookAsync(string symbol, CancellationToken cancellationToken = default)
    {
        return ThrowNotSupported<OrderBookDelta>("SubscribeOrderBook");
    }

    public IAsyncEnumerable<ExecutionTick> SubscribeExecutionsAsync(string symbol, CancellationToken cancellationToken = default)
    {
        return ThrowNotSupported<ExecutionTick>("SubscribeExecutions");
    }

    private IAsyncEnumerable<T> ThrowNotSupported<T>(string operation)
    {
        throw new ExchangeApiException(
            message: $"Realtime operation '{operation}' is not supported by bitFlyer adapter.",
            exchangeId: _exchangeId,
            operation: operation,
            statusCode: System.Net.HttpStatusCode.NotImplemented,
            exchangeErrorCode: "UNSUPPORTED_OPERATION",
            errorCategory: ExchangeErrorCategory.Request);
    }
}
