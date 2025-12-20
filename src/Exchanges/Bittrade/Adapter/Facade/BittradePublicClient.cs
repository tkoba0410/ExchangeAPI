using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Facade;

/// <summary>
/// Bittrade の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BittradePublicClient : IMarketDataApi, IExchangeInfoApi
{
    private readonly IMarketDataApi _marketApi;
    private readonly IExchangeInfoApi _exchangeInfoApi;
    private readonly BittradeMarketDataApi? _bittradeMarketApi;
    private readonly BittradeExchangeInfoApi? _bittradeExchangeInfoApi;

    public BittradePublicClient(IRestClient restClient)
        : this(new BittradeMarketDataApi(restClient), new BittradeExchangeInfoApi(restClient))
    {
    }

    public BittradePublicClient(IMarketDataApi marketApi, IExchangeInfoApi exchangeInfoApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _exchangeInfoApi = exchangeInfoApi ?? throw new ArgumentNullException(nameof(exchangeInfoApi));
        _bittradeMarketApi = marketApi as BittradeMarketDataApi;
        _bittradeExchangeInfoApi = exchangeInfoApi as BittradeExchangeInfoApi;
    }

    public Task<TimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default)
    {
        if (_bittradeMarketApi is null)
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "Timestamp");
        }

        return _bittradeMarketApi.GetTimestampAsync(cancellationToken);
    }

    public Task<SymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default)
    {
        if (_bittradeExchangeInfoApi is null)
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "Symbols");
        }

        return _bittradeExchangeInfoApi.GetSymbolsAsync(cancellationToken);
    }

    public Task<Ticker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<OrderBook> GetOrderBookAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(Symbol symbol, TimeSpan timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, timescale, from, to, cancellationToken);

    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);
}
