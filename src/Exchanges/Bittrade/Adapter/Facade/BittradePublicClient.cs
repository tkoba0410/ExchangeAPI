using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Clients.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Wire;
using ExchangeApi.Exchanges.Bittrade.Wire.Public;
using ExchangeApi.Exchanges.Bittrade.Wire.Private;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Core.Services;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Facade;

/// <summary>
/// Bittrade の Public API だけを利用する軽量クライアント。
/// </summary>
public sealed class BittradePublicClient : IMarketDataApi, IExchangeInfoApi, IExchangeClient, IHasRawAccess, IHasWireAccess
{
    private readonly IMarketDataApi _marketApi;
    private readonly IExchangeInfoApi _exchangeInfoApi;
    private readonly IRestClient? _restClient;
    private readonly object? _rawBundle;
    private readonly object? _wireBundle;
    internal BittradeApiBundle? ApiBundle { get; }

    public ExchangeCode ExchangeCode { get; } = ExchangeCode.Bittrade;

    public BittradePublicClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        var exchangeInfo = new BittradeExchangeInfoApi(restClient);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        _marketApi = new BittradeMarketDataApi(restClient, markets);
        _exchangeInfoApi = exchangeInfo;
        _restClient = restClient;
        var raw = new BittradeRawApi(_restClient);
        _rawBundle = raw;
        _wireBundle = new BittradeWireApi(
            new BittradeWireMarketDataApi(raw.MarketData),
            new BittradeWireTradingApiNotSupported(),
            new BittradeWireCommonApi(raw));
    }

    public BittradePublicClient(IMarketDataApi marketApi, IExchangeInfoApi exchangeInfoApi)
    {
        _marketApi = marketApi ?? throw new ArgumentNullException(nameof(marketApi));
        _exchangeInfoApi = exchangeInfoApi ?? throw new ArgumentNullException(nameof(exchangeInfoApi));
    }

    internal BittradePublicClient(BittradeApiBundle bundle)
    {
        if (bundle is null) throw new ArgumentNullException(nameof(bundle));
        _marketApi = new BittradeMarketDataApi(bundle.RestClient, bundle.Markets);
        _exchangeInfoApi = bundle.ExchangeInfo;
        _restClient = bundle.RestClient;
        _rawBundle = bundle.RawBundle;
        _wireBundle = bundle.WireBundle;
        ApiBundle = bundle;
    }

    [Obsolete("Use Wire.Common via IHasWireAccess for raw endpoints. This API will be removed in a future major release.")]
    public Task<TimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default)
    {
        if (!TryGetWire<BittradeWireApi>(out var wire))
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "Timestamp");
        }

        return wire.Common.GetTimestampAsync(cancellationToken);
    }

    [Obsolete("Use Wire.Common via IHasWireAccess for raw endpoints. This API will be removed in a future major release.")]
    public Task<SymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default)
    {
        if (!TryGetWire<BittradeWireApi>(out var wire))
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, "Symbols");
        }

        return wire.Common.GetSymbolsAsync(cancellationToken);
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

    public bool TryGetRaw<T>(out T raw) where T : class
    {
        raw = _rawBundle as T ?? null!;
        return raw is not null;
    }

    public bool TryGetWire<T>(out T wire) where T : class
    {
        wire = _wireBundle as T ?? null!;
        return wire is not null;
    }
}
