using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfo.ExchangeInfo;
using MarketsCall = ExchangeApi.Primitives.CallCommon.Call<ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Requests.GetMarketsRequest, System.Collections.Generic.IReadOnlyList<ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.BitflyerMarketNormalized>>;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;

/// <summary>
/// bitFlyer の ExchangeInfo 実装（/v1/getmarkets を使用）。
/// </summary>
public sealed class BitflyerExchangeInfoApi : IExchangeInfoProvider
{
    private readonly Func<CancellationToken, Task<MarketsCall>> _getMarkets;

    internal BitflyerExchangeInfoApi(BitflyerNormalizedPublicApi normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _getMarkets = normalized.GetMarketsCallAsync;
    }

    internal BitflyerExchangeInfoApi(IBitflyerNormalizedApi normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _getMarkets = normalized.GetMarketsCallAsync;
    }

    public async Task<Call<GetExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetExchangeInfoRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _getMarkets(cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                BitflyerOperations.ExchangeInfo.GetExchangeInfo,
                MapExchangeInfo);
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetExchangeInfoRequest, ExchangeInfoDto>(
                request,
                startedAt,
                BitflyerOperations.ExchangeInfo.GetExchangeInfo,
                ex);
        }
    }

    public Task<Call<GetCurrencysRequest, IReadOnlyList<string>>> GetCurrencysCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetCurrencysRequest();
        return Task.FromResult(NotSupportedCall.Create<GetCurrencysRequest, IReadOnlyList<string>>(
            "Contracts",
            BitflyerOperations.ExchangeInfo.GetCurrencys,
            request,
            "Currencys"));
    }

    public Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetTimestampRequest();
        return Task.FromResult(NotSupportedCall.Create<GetTimestampRequest, DateTimeOffset>(
            "Contracts",
            BitflyerOperations.ExchangeInfo.GetTimestamp,
            request,
            "Timestamp"));
    }

    private static ExchangeInfoDto MapExchangeInfo(IReadOnlyList<BitflyerMarketNormalized> markets)
    {
        var mapped = markets.Select(MapMarket).ToList();
        return new ExchangeInfoDto(mapped, Features: null, RateLimits: null, Maintenance: null);
    }

    private static ExchangeMarketInfo MapMarket(BitflyerMarketNormalized market) =>
        new(
            Symbol: NormalizeSymbol(market),
            ProductCode: market.ProductCode,
            Type: "Spot",
            IsSupported: true);

    private static string NormalizeSymbol(BitflyerMarketNormalized market)
    {
        var symbol = market.ProductCode;
        if (symbol.Contains('_', StringComparison.Ordinal))
        {
            return symbol.Replace('_', '/');
        }

        if (!string.IsNullOrWhiteSpace(market.Alias) &&
            market.Alias.Contains('_', StringComparison.Ordinal))
        {
            return market.Alias.Replace('_', '/');
        }

        return symbol;
    }
}
