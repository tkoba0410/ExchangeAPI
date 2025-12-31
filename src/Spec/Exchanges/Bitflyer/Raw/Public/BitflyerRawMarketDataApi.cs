using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer Public REST API の Mirror Raw 実装。
/// </summary>
internal sealed class BitflyerRawMarketDataApi : IBitflyerRawMarketDataApi
{
    private readonly IBitflyerWireApi _wire;

    public BitflyerRawMarketDataApi(IBitflyerWireApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<Ticker> GetTickerAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
            throw new ArgumentException("Product code must not be null or whitespace.", nameof(productCode));

        var response = await _wire.MarketData
            .GetTickerRawAsync(productCode, useAliasPath, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<Ticker>(response.Json, "Bitflyer.GetTicker");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetTicker", response.StatusCode, response.Json);
    }

    public async Task<Board> GetBoardAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
            throw new ArgumentException("Product code must not be null or whitespace.", nameof(productCode));

        var response = await _wire.MarketData
            .GetBoardRawAsync(productCode, useAliasPath, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<Board>(response.Json, "Bitflyer.GetBoard");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetBoard", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
            throw new ArgumentException("productCode is required.", nameof(productCode));

        var response = await _wire.MarketData
            .GetExecutionsRawAsync(productCode, count, before, after, useAliasPath, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ExecutionPublicResponse>>(
                response.Json,
                "Bitflyer.GetExecutions");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetExecutions", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<Market>> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.ExchangeInfo
            .GetMarketsAsync(region, useAliasPath, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<Market>>(response.Json, "Bitflyer.GetMarkets");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetMarkets", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<Chat>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.ExchangeInfo
            .GetChatsAsync(fromDate, region, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<Chat>>(response.Json, "Bitflyer.GetChats");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetChats", response.StatusCode, response.Json);
    }

    public async Task<HealthResponse> GetHealthAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        var response = await _wire.MarketData
            .GetHealthAsync(productCode, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<HealthResponse>(response.Json, "Bitflyer.GetHealth");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetHealth", response.StatusCode, response.Json);
    }

    public async Task<BoardStateResponse> GetBoardStateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        var response = await _wire.MarketData
            .GetBoardStateAsync(productCode, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<BoardStateResponse>(response.Json, "Bitflyer.GetBoardState");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetBoardState", response.StatusCode, response.Json);
    }

    public async Task<CorporateLeverageResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.MarketData
            .GetCorporateLeverageAsync(cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<CorporateLeverageResponse>(
                response.Json,
                "Bitflyer.GetCorporateLeverage");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.GetCorporateLeverage",
            response.StatusCode,
            response.Json);
    }

    public async Task<FundingRateResponse> GetFundingRateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        var response = await _wire.MarketData
            .GetFundingRateAsync(productCode, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<FundingRateResponse>(
                response.Json,
                "Bitflyer.GetFundingRate");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.GetFundingRate",
            response.StatusCode,
            response.Json);
    }
}
