using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Dtos;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
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
    private readonly IRestClient? _restClient;

    public BittradePublicClient(IRestClient restClient)
        : this(new BittradeMarketDataApi(restClient), new BittradeExchangeInfoApi(restClient))
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public BittradePublicClient(IMarketDataApi marketApi, IExchangeInfoApi exchangeInfoApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _exchangeInfoApi = exchangeInfoApi ?? throw new ArgumentNullException(nameof(exchangeInfoApi));
    }

    public Task<TimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default)
    {
        if (_restClient is null)
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "Timestamp");
        }

        return _restClient.GetAsync<TimestampResponse>("v1/common/timestamp", cancellationToken: cancellationToken);
    }

    public Task<SymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default)
    {
        if (_restClient is null)
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "Symbols");
        }

        return _restClient.GetAsync<SymbolsResponse>("v1/common/symbols", cancellationToken: cancellationToken);
    }

    public Task<Ticker> GetTickerAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetTickerAsync(symbol, cancellationToken);

    public Task<OrderBook> GetOrderBookAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetOrderBookAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(CommonSymbol symbol, CancellationToken cancellationToken = default) =>
        _marketApi.GetMarketExecutionsAsync(symbol, cancellationToken);

    public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(CommonSymbol symbol, TimeSpan timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) =>
        _marketApi.GetCandlesticksAsync(symbol, timescale, from, to, cancellationToken);

    public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
        _exchangeInfoApi.GetExchangeInfoAsync(cancellationToken);
}
