using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

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
        var raw = new BitflyerRawApi(restClient);

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

    public async Task<BitflyerOrderBookNormalized> GetOrderBookAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var raw = await _raw.GetBoardAsync(new RawProductCode(productCode), cancellationToken: ct)
            .ConfigureAwait(false);
        return BitflyerOrderBookNormalizer.Normalize(raw);
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

    public async Task<BitflyerHealthNormalized> GetHealthAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var raw = await _raw.GetHealthAsync(new RawProductCode(productCode), ct).ConfigureAwait(false);
        return BitflyerHealthNormalizer.Normalize(raw);
    }

    public async Task<BitflyerBoardStateNormalized> GetBoardStateAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var raw = await _raw.GetBoardStateAsync(new RawProductCode(productCode), ct).ConfigureAwait(false);
        return BitflyerBoardStateNormalizer.Normalize(raw);
    }

    public async Task<IReadOnlyList<BitflyerChatNormalized>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken ct = default)
    {
        var raw = await _raw.GetChatsAsync(fromDate, region, ct).ConfigureAwait(false);
        return raw.Select(BitflyerChatNormalizer.Normalize).ToArray();
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
}
