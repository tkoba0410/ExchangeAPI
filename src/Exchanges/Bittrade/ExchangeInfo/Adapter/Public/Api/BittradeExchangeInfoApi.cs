using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Compose;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Dynamic;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Static;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using SymbolsCall = ExchangeApi.Primitives.CallCommon.Call<ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Requests.GetSymbolsRequest, System.Collections.Generic.IReadOnlyList<ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos.BittradeSymbolNormalized>>;

namespace ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Adapter.Public.Api;

/// <summary>
/// Bittrade の ExchangeInfo API 実装（/v1/common/symbols を使用）。
/// </summary>
public sealed class BittradeExchangeInfoApi : IExchangeInfoProvider
{
    private readonly BittradeNormalizedPublicApi _normalized;
    private readonly Func<CancellationToken, Task<SymbolsCall>>? _getSymbols;

    internal BittradeExchangeInfoApi(BittradeNormalizedPublicApi normalized)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
        _getSymbols = normalized.GetSymbolsCallAsync;
    }

    public async Task<Call<GetExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetExchangeInfoRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var staticInfo = BittradeStaticExchangeInfoLoader.Load();
            var dynamicInfo = await GetDynamicInfoAsync(cancellationToken).ConfigureAwait(false);
            var composed = BittradeExchangeInfoComposer.Compose(staticInfo, dynamicInfo);
            var info = MapExchangeInfo(composed);
            var meta = new CallMeta(
                Layer: "Contracts",
                Component: BittradeExchangeInfoOperations.GetExchangeInfo,
                EndpointId: CallMeta.InternalEndpointId,
                Tags: null,
                Children: null);
            return new Call<GetExchangeInfoRequest, ExchangeInfoDto>(
                Id: CallId.New(),
                StartedAt: startedAt,
                Duration: DateTimeOffset.UtcNow - startedAt,
                Request: request,
                Result: new CallResult<ExchangeInfoDto>.Ok(info),
                Meta: meta);
        }
        catch (Exception ex)
        {
            return ExchangeInfoCallMapper.FromException<GetExchangeInfoRequest, ExchangeInfoDto>(
                request,
                startedAt,
                BittradeExchangeInfoOperations.GetExchangeInfo,
                ex);
        }
    }

    public async Task<Call<GetCurrencysRequest, IReadOnlyList<string>>> GetCurrencysCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetCurrencysRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetCurrencysCallAsync(cancellationToken).ConfigureAwait(false);
            return ExchangeInfoCallMapper.MapCall(
                request,
                call,
                BittradeExchangeInfoOperations.GetCurrencys,
                ok => ok);
        }
        catch (Exception ex)
        {
            return ExchangeInfoCallMapper.FromException<GetCurrencysRequest, IReadOnlyList<string>>(
                request,
                startedAt,
                BittradeExchangeInfoOperations.GetCurrencys,
                ex);
        }
    }

    public async Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetTimestampRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetTimestampCallAsync(cancellationToken).ConfigureAwait(false);
            return ExchangeInfoCallMapper.MapCall(
                request,
                call,
                BittradeExchangeInfoOperations.GetTimestamp,
                ok => ok);
        }
        catch (Exception ex)
        {
            return ExchangeInfoCallMapper.FromException<GetTimestampRequest, DateTimeOffset>(
                request,
                startedAt,
                BittradeExchangeInfoOperations.GetTimestamp,
                ex);
        }
    }

    internal static string ToApiSymbol(ExchangeMarketInfo market) =>
        BittradeSymbol.Normalize(market.ProductCode.Value);

    private static ExchangeInfoDto MapExchangeInfo(BittradeStaticExchangeInfo info)
    {
        var mapped = info.Markets.Select(MapMarket).ToList();
        return new ExchangeInfoDto(
            Markets: mapped,
            Features: MapFeatures(info.Features),
            RateLimits: MapRateLimits(info.RateLimits),
            Maintenance: MapMaintenance(info.Maintenance));
    }

    private static ExchangeMarketInfo MapMarket(BittradeStaticMarketInfo market) =>
        new(
            Symbol: Symbol.ParseOrThrow(market.Symbol),
            ProductCode: ProductCode.ParseOrThrow(market.ProductCode),
            Type: MarketType.ParseOrThrow(market.Type),
            MinSize: ToSize(market.MinSize),
            MaxSize: ToSize(market.MaxSize),
            MinNotional: market.MinNotional,
            PriceIncrement: ToPrice(market.PriceIncrement),
            SizeIncrement: ToSize(market.SizeIncrement),
            MakerFeeRate: market.MakerFeeRate,
            TakerFeeRate: market.TakerFeeRate,
            FeeCurrency: ToCurrencyCode(market.FeeCurrency),
            FeeType: MapFeeType(market.FeeType),
            IsSupported: market.IsSupported,
            StatusNote: market.StatusNote);

    private static CurrencyCode? ToCurrencyCode(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : CurrencyCodeConverter.FromString(value);

    private static ExchangeFeatureFlags? MapFeatures(BittradeStaticFeatureFlags? features) =>
        features is null
            ? null
            : new ExchangeFeatureFlags(
                SupportsWebSocket: features.SupportsWebSocket,
                SupportsMargin: features.SupportsMargin,
                SupportsStopOrder: features.SupportsStopOrder,
                SupportsParentOrder: features.SupportsParentOrder,
                SupportsCandlestick: features.SupportsCandlestick,
                SupportsOrderBookDelta: features.SupportsOrderBookDelta,
                SupportsRealtimeExecutions: features.SupportsRealtimeExecutions,
                SupportsWithdraw: features.SupportsWithdraw);

    private static ExchangeRateLimits? MapRateLimits(BittradeStaticRateLimits? limits) =>
        limits is null ? null : new ExchangeRateLimits(limits.RequestsPerMinute, limits.OrdersPerMinute);

    private static ExchangeMaintenance? MapMaintenance(BittradeStaticMaintenance? maintenance) =>
        maintenance is null
            ? null
            : new ExchangeMaintenance(
                Status: MapMaintenanceStatus(maintenance.Status),
                PlannedUntil: maintenance.PlannedUntil,
                Message: maintenance.Message);

    private static ExchangeMaintenanceStatus? MapMaintenanceStatus(BittradeStaticMaintenanceStatus? status) =>
        status switch
        {
            null => null,
            BittradeStaticMaintenanceStatus.Normal => ExchangeMaintenanceStatus.Normal,
            BittradeStaticMaintenanceStatus.Planned => ExchangeMaintenanceStatus.Planned,
            BittradeStaticMaintenanceStatus.Unplanned => ExchangeMaintenanceStatus.Unplanned,
            _ => null
        };

    private static FeeType? MapFeeType(string? feeType) =>
        feeType?.ToUpperInvariant() switch
        {
            "PERCENTAGE" => FeeType.Percentage,
            "FLAT" => FeeType.Flat,
            _ => null
        };

    private static Size? ToSize(decimal? value) =>
        value is null ? null : new Size(value.Value);

    private static Price? ToPrice(decimal? value) =>
        value is null ? null : new Price(value.Value);

    private async Task<BittradeDynamicExchangeInfo?> GetDynamicInfoAsync(CancellationToken cancellationToken)
    {
        if (_getSymbols is null) return null;

        var symbolsCall = await _getSymbols(cancellationToken).ConfigureAwait(false);
        if (symbolsCall.Result is CallResult<IReadOnlyList<BittradeSymbolNormalized>>.Err)
        {
            return null;
        }

        var symbols = ((CallResult<IReadOnlyList<BittradeSymbolNormalized>>.Ok)symbolsCall.Result).Response;
        var markets = symbols.Select(MapDynamicMarket).ToList();
        return new BittradeDynamicExchangeInfo { Markets = markets };
    }

    private static BittradeDynamicMarketInfo MapDynamicMarket(BittradeSymbolNormalized symbol)
    {
        var displaySymbol = $"{symbol.BaseCurrency.ToUpperInvariant()}/{symbol.QuoteCurrency.ToUpperInvariant()}";
        var product = BittradeSymbol.Normalize(symbol.Symbol);
        var priceIncrement = Pow10(-symbol.PricePrecision);
        var sizeIncrement = Pow10(-symbol.AmountPrecision);
        var minSize = symbol.MinOrderAmount;
        var minNotional = symbol.MinOrderValue;
        var supported = string.Equals(symbol.State, "online", StringComparison.OrdinalIgnoreCase);

        return new BittradeDynamicMarketInfo
        {
            Symbol = displaySymbol,
            ProductCode = product,
            Type = "Spot",
            MinSize = minSize,
            MinNotional = minNotional,
            PriceIncrement = priceIncrement,
            SizeIncrement = sizeIncrement,
            IsSupported = supported,
            StatusNote = symbol.State
        };
    }

    private static decimal Pow10(int power) =>
        (decimal)Math.Pow(10, power);
}
