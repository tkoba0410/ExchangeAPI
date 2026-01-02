using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Wire.Types;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer Public REST API の Mirror Raw 実装。
/// </summary>
internal sealed class BitflyerRawMarketDataApi : IBitflyerRawMarketDataApi
{
    private readonly IWireTransport _wire;

    public BitflyerRawMarketDataApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<Ticker> GetTickerAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        var call = await GetTickerCallAsync(productCode, useAliasPath, cancellationToken).ConfigureAwait(false);
        return UnwrapOk(call, "Bitflyer.GetTicker");
    }

    public async Task<BitflyerRawCall<Ticker, JsonElement>> GetTickerCallAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
            throw new ArgumentException("Product code must not be null or whitespace.", nameof(productCode));

        var wireCall = await SendAsync(BitflyerEndpoints.GetTicker(productCode.Value, useAliasPath), cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetTicker", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
            ["useAliasPath"] = useAliasPath.ToString(),
        });
        return CreateCall<Ticker>(request, wireCall, "Bitflyer.GetTicker");
    }

    public async Task<Board> GetBoardAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        var call = await GetBoardCallAsync(productCode, useAliasPath, cancellationToken).ConfigureAwait(false);
        return UnwrapOk(call, "Bitflyer.GetBoard");
    }

    public async Task<BitflyerRawCall<Board, JsonElement>> GetBoardCallAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
            throw new ArgumentException("Product code must not be null or whitespace.", nameof(productCode));

        var wireCall = await SendAsync(BitflyerEndpoints.GetBoard(productCode.Value, useAliasPath), cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetBoard", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
            ["useAliasPath"] = useAliasPath.ToString(),
        });
        return CreateCall<Board>(request, wireCall, "Bitflyer.GetBoard");
    }

    public async Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        var call = await GetExecutionsCallAsync(productCode, count, before, after, useAliasPath, cancellationToken)
            .ConfigureAwait(false);
        return UnwrapOk(call, "Bitflyer.GetExecutions");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<ExecutionPublicResponse>, JsonElement>> GetExecutionsCallAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
            throw new ArgumentException("productCode is required.", nameof(productCode));

        var wireCall = await SendAsync(
                BitflyerEndpoints.GetExecutions(productCode.Value, count, before, after, useAliasPath),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetExecutions", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
            ["useAliasPath"] = useAliasPath.ToString(),
        });
        return CreateCall<IReadOnlyList<ExecutionPublicResponse>>(request, wireCall, "Bitflyer.GetExecutions");
    }

    public async Task<IReadOnlyList<Market>> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        var call = await GetMarketsCallAsync(region, useAliasPath, cancellationToken).ConfigureAwait(false);
        return UnwrapOk(call, "Bitflyer.GetMarkets");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<Market>, JsonElement>> GetMarketsCallAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BitflyerEndpoints.GetMarkets(region, useAliasPath), cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetMarkets", new Dictionary<string, string?>
        {
            ["region"] = region,
            ["useAliasPath"] = useAliasPath.ToString(),
        });
        return CreateCall<IReadOnlyList<Market>>(request, wireCall, "Bitflyer.GetMarkets");
    }

    public async Task<IReadOnlyList<Chat>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default)
    {
        var call = await GetChatsCallAsync(fromDate, region, cancellationToken).ConfigureAwait(false);
        return UnwrapOk(call, "Bitflyer.GetChats");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<Chat>, JsonElement>> GetChatsCallAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BitflyerEndpoints.GetChats(fromDate, region), cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetChats", new Dictionary<string, string?>
        {
            ["fromDate"] = fromDate,
            ["region"] = region,
        });
        return CreateCall<IReadOnlyList<Chat>>(request, wireCall, "Bitflyer.GetChats");
    }

    public async Task<HealthResponse> GetHealthAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var call = await GetHealthCallAsync(productCode, cancellationToken).ConfigureAwait(false);
        return UnwrapOk(call, "Bitflyer.GetHealth");
    }

    public async Task<BitflyerRawCall<HealthResponse, JsonElement>> GetHealthCallAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        var wireCall = await SendAsync(BitflyerEndpoints.GetHealth(productCode.Value), cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetHealth", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
        });
        return CreateCall<HealthResponse>(request, wireCall, "Bitflyer.GetHealth");
    }

    public async Task<BoardStateResponse> GetBoardStateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var call = await GetBoardStateCallAsync(productCode, cancellationToken).ConfigureAwait(false);
        return UnwrapOk(call, "Bitflyer.GetBoardState");
    }

    public async Task<BitflyerRawCall<BoardStateResponse, JsonElement>> GetBoardStateCallAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        var wireCall = await SendAsync(BitflyerEndpoints.GetBoardState(productCode.Value), cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetBoardState", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
        });
        return CreateCall<BoardStateResponse>(request, wireCall, "Bitflyer.GetBoardState");
    }

    public async Task<CorporateLeverageResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default)
    {
        var call = await GetCorporateLeverageCallAsync(cancellationToken).ConfigureAwait(false);
        return UnwrapOk(call, "Bitflyer.GetCorporateLeverage");
    }

    public async Task<BitflyerRawCall<CorporateLeverageResponse, JsonElement>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BitflyerEndpoints.GetCorporateLeverage(), cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetCorporateLeverage", new Dictionary<string, string?>());
        return CreateCall<CorporateLeverageResponse>(request, wireCall, "Bitflyer.GetCorporateLeverage");
    }

    public async Task<FundingRateResponse> GetFundingRateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var call = await GetFundingRateCallAsync(productCode, cancellationToken).ConfigureAwait(false);
        return UnwrapOk(call, "Bitflyer.GetFundingRate");
    }

    public async Task<BitflyerRawCall<FundingRateResponse, JsonElement>> GetFundingRateCallAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        var wireCall = await SendAsync(BitflyerEndpoints.GetFundingRate(productCode.Value), cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetFundingRate", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
        });
        return CreateCall<FundingRateResponse>(request, wireCall, "Bitflyer.GetFundingRate");
    }

    private static T UnwrapOk<T>(BitflyerRawCall<T, JsonElement> call, string context)
    {
        return call.Result switch
        {
            Ok<T, JsonElement> ok => ok.Value,
            Err<T, JsonElement> err => throw BitflyerRawJson.CreateStatusException(
                context,
                err.StatusCode,
                err.Error.ValueKind == JsonValueKind.Undefined ? string.Empty : err.Error.GetRawText()),
            _ => throw new InvalidOperationException("Unexpected call result.")
        };
    }

    private static BitflyerRawRequest CreateRequest(string operation, IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BitflyerRawCall<TOk, JsonElement> CreateCall<TOk>(
        BitflyerRawRequest request,
        WireCall call,
        string context)
    {
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            var ok = BitflyerRawJson.DeserializeOrThrow<TOk>(response.Json, context);
            return new BitflyerRawCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(ok, response.StatusCode),
                call.Meta);
        }

        if (BitflyerRawJson.TryDeserialize<JsonElement>(response.Json, out var error, out _))
        {
            return new BitflyerRawCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(error!, response.StatusCode),
                call.Meta);
        }

        return new BitflyerRawCall<TOk, JsonElement>(
            request,
            new Err<TOk, JsonElement>(default, response.StatusCode),
            call.Meta);
    }

    private Task<WireCall> SendAsync(WireRequest request, CancellationToken ct) =>
        _wire.SendAsync(ExchangeCode.Bitflyer, request, ct);
}
