using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Normalize;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Wire.Types;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Facade;

public sealed class BitflyerNormalizedApi
{
    public BitflyerNormalizedMarketDataFacade MarketData { get; }
    public BitflyerNormalizedExchangeInfoFacade ExchangeInfo { get; }

    private BitflyerNormalizedApi(
        BitflyerNormalizedMarketDataFacade marketData,
        BitflyerNormalizedExchangeInfoFacade exchangeInfo)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
    }

    public static BitflyerNormalizedApi FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var wire = new WireTransport(restClient);
        var raw = new BitflyerRawApi(wire);

        return new BitflyerNormalizedApi(
            marketData: new BitflyerNormalizedMarketDataFacade(raw.MarketData),
            exchangeInfo: new BitflyerNormalizedExchangeInfoFacade(raw.MarketData));
    }
}

public sealed class BitflyerNormalizedMarketDataFacade
{
    private readonly IBitflyerRawMarketDataApi _raw;

    internal BitflyerNormalizedMarketDataFacade(IBitflyerRawMarketDataApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<BitflyerTickerNormalized> GetTickerAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var raw = await _raw.GetTickerAsync(new RawProductCode(productCode), cancellationToken: ct)
            .ConfigureAwait(false);
        return BitflyerTickerNormalizer.Normalize(raw);
    }

    public async Task<BitflyerNormalizedCall<BitflyerTickerNormalized, JsonElement>> GetTickerCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw.GetTickerCallAsync(new RawProductCode(productCode), cancellationToken: ct)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetTicker", new Dictionary<string, string?>
        {
            ["productCode"] = productCode,
        });

        return CreateCall(rawCall, request, BitflyerTickerNormalizer.Normalize);
    }

    public async Task<BitflyerOrderBookNormalized> GetOrderBookAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var raw = await _raw.GetBoardAsync(new RawProductCode(productCode), cancellationToken: ct)
            .ConfigureAwait(false);
        return BitflyerOrderBookNormalizer.Normalize(raw);
    }

    public async Task<BitflyerNormalizedCall<BitflyerOrderBookNormalized, JsonElement>> GetOrderBookCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw.GetBoardCallAsync(new RawProductCode(productCode), cancellationToken: ct)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetBoard", new Dictionary<string, string?>
        {
            ["productCode"] = productCode,
        });

        return CreateCall(rawCall, request, BitflyerOrderBookNormalizer.Normalize);
    }

    public async Task<IReadOnlyList<BitflyerExecutionNormalized>> GetExecutionsAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken ct = default)
    {
        var raw = await _raw.GetExecutionsAsync(
                new RawProductCode(productCode),
                count,
                before,
                after,
                cancellationToken: ct)
            .ConfigureAwait(false);

        return raw.Select(BitflyerExecutionNormalizer.Normalize).ToArray();
    }

    public async Task<BitflyerNormalizedCall<IReadOnlyList<BitflyerExecutionNormalized>, JsonElement>> GetExecutionsCallAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw.GetExecutionsCallAsync(
                new RawProductCode(productCode),
                count,
                before,
                after,
                cancellationToken: ct)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetExecutions", new Dictionary<string, string?>
        {
            ["productCode"] = productCode,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        });

        return CreateCall(
            rawCall,
            request,
            raw =>
            {
                IReadOnlyList<BitflyerExecutionNormalized> mapped = raw
                    .Select(BitflyerExecutionNormalizer.Normalize)
                    .ToArray();
                return mapped;
            });
    }

    public async Task<BitflyerHealthNormalized> GetHealthAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var raw = await _raw.GetHealthAsync(new RawProductCode(productCode), ct).ConfigureAwait(false);
        return BitflyerHealthNormalizer.Normalize(raw);
    }

    public async Task<BitflyerNormalizedCall<BitflyerHealthNormalized, JsonElement>> GetHealthCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw.GetHealthCallAsync(new RawProductCode(productCode), ct).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetHealth", new Dictionary<string, string?>
        {
            ["productCode"] = productCode,
        });

        return CreateCall(rawCall, request, BitflyerHealthNormalizer.Normalize);
    }

    public async Task<BitflyerBoardStateNormalized> GetBoardStateAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var raw = await _raw.GetBoardStateAsync(new RawProductCode(productCode), ct).ConfigureAwait(false);
        return BitflyerBoardStateNormalizer.Normalize(raw);
    }

    public async Task<BitflyerNormalizedCall<BitflyerBoardStateNormalized, JsonElement>> GetBoardStateCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw.GetBoardStateCallAsync(new RawProductCode(productCode), ct).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetBoardState", new Dictionary<string, string?>
        {
            ["productCode"] = productCode,
        });

        return CreateCall(rawCall, request, BitflyerBoardStateNormalizer.Normalize);
    }

    public async Task<IReadOnlyList<BitflyerChatNormalized>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken ct = default)
    {
        var raw = await _raw.GetChatsAsync(fromDate, region, ct).ConfigureAwait(false);
        return raw.Select(BitflyerChatNormalizer.Normalize).ToArray();
    }

    public async Task<BitflyerNormalizedCall<IReadOnlyList<BitflyerChatNormalized>, JsonElement>> GetChatsCallAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw.GetChatsCallAsync(fromDate, region, ct).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetChats", new Dictionary<string, string?>
        {
            ["fromDate"] = fromDate,
            ["region"] = region,
        });

        return CreateCall(
            rawCall,
            request,
            raw =>
            {
                IReadOnlyList<BitflyerChatNormalized> mapped = raw
                    .Select(BitflyerChatNormalizer.Normalize)
                    .ToArray();
                return mapped;
            });
    }

    private static BitflyerNormalizedRequest CreateRequest(
        string operation,
        IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BitflyerNormalizedCall<TOk, JsonElement> CreateCall<TRaw, TOk>(
        BitflyerRawCall<TRaw, JsonElement> rawCall,
        BitflyerNormalizedRequest request,
        Func<TRaw, TOk> mapper)
    {
        return rawCall.Result switch
        {
            Ok<TRaw, JsonElement> ok => new BitflyerNormalizedCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(mapper(ok.Value), ok.StatusCode),
                rawCall.Meta),
            Err<TRaw, JsonElement> err => new BitflyerNormalizedCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(err.Error, err.StatusCode),
                rawCall.Meta),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }
}

public sealed class BitflyerNormalizedExchangeInfoFacade
{
    private readonly IBitflyerRawMarketDataApi _raw;

    internal BitflyerNormalizedExchangeInfoFacade(IBitflyerRawMarketDataApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<IReadOnlyList<BitflyerMarketNormalized>> GetMarketsAsync(
        string? region = null,
        CancellationToken ct = default)
    {
        var raw = await _raw.GetMarketsAsync(region, cancellationToken: ct).ConfigureAwait(false);
        return raw.Select(BitflyerMarketNormalizer.Normalize).ToArray();
    }

    public async Task<BitflyerNormalizedCall<IReadOnlyList<BitflyerMarketNormalized>, JsonElement>> GetMarketsCallAsync(
        string? region = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw.GetMarketsCallAsync(region, cancellationToken: ct).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetMarkets", new Dictionary<string, string?>
        {
            ["region"] = region,
        });

        return CreateCall(
            rawCall,
            request,
            raw =>
            {
                IReadOnlyList<BitflyerMarketNormalized> mapped = raw
                    .Select(BitflyerMarketNormalizer.Normalize)
                    .ToArray();
                return mapped;
            });
    }

    private static BitflyerNormalizedRequest CreateRequest(
        string operation,
        IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BitflyerNormalizedCall<TOk, JsonElement> CreateCall<TRaw, TOk>(
        BitflyerRawCall<TRaw, JsonElement> rawCall,
        BitflyerNormalizedRequest request,
        Func<TRaw, TOk> mapper)
    {
        return rawCall.Result switch
        {
            Ok<TRaw, JsonElement> ok => new BitflyerNormalizedCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(mapper(ok.Value), ok.StatusCode),
                rawCall.Meta),
            Err<TRaw, JsonElement> err => new BitflyerNormalizedCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(err.Error, err.StatusCode),
                rawCall.Meta),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }
}
