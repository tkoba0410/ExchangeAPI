using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Transport;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

/// <summary>
/// bitFlyer 公開 REST API の Wire 実装。
/// </summary>
internal sealed class BitflyerPublicApi : IBitflyerPublicApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bitflyer;
    private readonly IRestClient _restClient;

    public BitflyerPublicApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<WireResponse> GetTickerRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            useAliasPath ? Raw.BitflyerRawConstants.Paths.Ticker : Raw.BitflyerRawConstants.Paths.GetTicker,
            CreateProductCodeQuery(productCode),
            cancellationToken);

    public Task<WireResponse> GetBoardRawAsync(
        RawProductCode productCode,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            useAliasPath ? Raw.BitflyerRawConstants.Paths.Board : Raw.BitflyerRawConstants.Paths.GetBoard,
            CreateProductCodeQuery(productCode),
            cancellationToken);

    public Task<WireResponse> GetExecutionsRawAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            useAliasPath ? Raw.BitflyerRawConstants.Paths.Executions : Raw.BitflyerRawConstants.Paths.GetExecutions,
            CreateExecutionsQuery(productCode, count, before, after),
            cancellationToken);

    public Task<WireResponse> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default)
    {
        var path = useAliasPath ? Raw.BitflyerRawConstants.Paths.Markets : Raw.BitflyerRawConstants.Paths.GetMarkets;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        return GetAsync(path, query: null, cancellationToken);
    }

    public Task<WireResponse> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default)
    {
        var path = Raw.BitflyerRawConstants.Paths.GetChats;
        if (!string.IsNullOrWhiteSpace(region))
        {
            path = $"{path}/{region}";
        }

        IReadOnlyDictionary<string, string?> query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [Raw.BitflyerRawConstants.QueryKeys.FromDate] = fromDate,
        };

        return GetAsync(path, query, cancellationToken);
    }

    public Task<WireResponse> GetHealthAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            Raw.BitflyerRawConstants.Paths.GetHealth,
            CreateProductCodeQuery(productCode),
            cancellationToken);

    public Task<WireResponse> GetBoardStateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            Raw.BitflyerRawConstants.Paths.GetBoardState,
            CreateProductCodeQuery(productCode),
            cancellationToken);

    public Task<WireResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default) =>
        GetAsync(Raw.BitflyerRawConstants.Paths.GetCorporateLeverage, query: null, cancellationToken);

    public Task<WireResponse> GetFundingRateAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            Raw.BitflyerRawConstants.Paths.GetFundingRate,
            CreateProductCodeQuery(productCode),
            cancellationToken);

    private async Task<WireResponse> GetAsync(
        string path,
        IReadOnlyDictionary<string, string?>? query,
        CancellationToken cancellationToken)
    {
        var meta = await _restClient.GetRawAsync(path, query, cancellationToken).ConfigureAwait(false);
        return ToWire(meta);
    }

    private static IReadOnlyDictionary<string, string?> CreateProductCodeQuery(RawProductCode productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        return new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [Raw.BitflyerRawConstants.QueryKeys.ProductCode] = productCode.Value,
        };
    }

    private static IReadOnlyDictionary<string, string?> CreateExecutionsQuery(
        RawProductCode productCode,
        int? count,
        long? before,
        long? after)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        return new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [Raw.BitflyerRawConstants.QueryKeys.ProductCode] = productCode.Value,
            [Raw.BitflyerRawConstants.QueryKeys.Count] = count?.ToString(),
            [Raw.BitflyerRawConstants.QueryKeys.Before] = before?.ToString(),
            [Raw.BitflyerRawConstants.QueryKeys.After] = after?.ToString(),
        };
    }

    private static WireResponse ToWire(HttpResponseMeta meta)
    {
        var headers = meta.Headers is null
            ? null
            : new Dictionary<string, string>(meta.Headers, StringComparer.OrdinalIgnoreCase);
        return new WireResponse(
            Exchange,
            meta.StatusCode,
            meta.Body ?? string.Empty,
            headers);
    }
}
