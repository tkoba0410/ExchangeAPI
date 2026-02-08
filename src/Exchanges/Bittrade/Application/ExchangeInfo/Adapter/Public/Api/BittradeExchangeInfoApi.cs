using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Compose;
using ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Dynamic;
using ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Static;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;
using SymbolsCall = ExchangeApi.Primitives.CallCommon.Call<ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests.GetSymbolsRequest, System.Collections.Generic.IReadOnlyList<ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos.SymbolNormalized>>;

namespace ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Adapter.Public.Api;

/// <summary>
/// Bittrade の ExchangeInfo API 実装（/v1/common/symbols を使用）。
/// </summary>
public sealed class BittradeExchangeInfoApi : IExchangeInfoProvider
{
    private readonly NormalizedPublicApi _normalized;
    private readonly Func<CancellationToken, Task<SymbolsCall>>? _getSymbols;

    internal BittradeExchangeInfoApi(NormalizedPublicApi normalized)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
        _getSymbols = normalized.GetSymbolsCallAsync;
    }

    public async Task<Call<ExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoAsync(
        ExchangeInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var staticInfo = BittradeStaticExchangeInfoLoader.Load();
            var dynamicInfo = await GetDynamicInfoAsync(cancellationToken).ConfigureAwait(false);
            var composed = BittradeExchangeInfoComposer.Compose(staticInfo, dynamicInfo);
            var response = MapExchangeInfo(composed);
            var meta = new CallMeta(
                Layer: "Contracts",
                Component: BittradeExchangeInfoOperations.GetExchangeInfo,
                EndpointId: CallMeta.InternalEndpointId,
                Tags: null,
                Children: null);
            return new Call<ExchangeInfoRequest, ExchangeInfoDto>(
                Id: CallId.New(),
                StartedAt: startedAt,
                Duration: DateTimeOffset.UtcNow - startedAt,
                Request: request,
                Result: new CallResult<ExchangeInfoDto>.Ok(response),
                Meta: meta);
        }
        catch (Exception ex)
        {
            return ExchangeInfoCallMapper.FromException<ExchangeInfoRequest, ExchangeInfoDto>(
                request,
                startedAt,
                BittradeExchangeInfoOperations.GetExchangeInfo,
                ex);
        }
    }

    internal static string ToApiSymbol(ExchangeMarketInfo market)
    {
        if (!ExchangeSymbol.TryParse(market.ProductCode.Value, out var parsed))
        {
            throw new ArgumentException(
                $"Bittrade symbol is invalid: '{market.ProductCode.Value}'. Expected lowercase alphanumeric like 'btcjpy'.",
                nameof(market));
        }

        return parsed.Value;
    }

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
            StatusNote: ToFreeText(market.StatusNote));

    private static CurrencyCode? ToCurrencyCode(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : CurrencyCodeConverter.FromString(value);

    private static FreeText? ToFreeText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : new FreeText(value);

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
                Message: ToFreeText(maintenance.Message));

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
        if (symbolsCall.Result is CallResult<IReadOnlyList<SymbolNormalized>>.Err)
        {
            return null;
        }

        var symbols = ((CallResult<IReadOnlyList<SymbolNormalized>>.Ok)symbolsCall.Result).Response;
        var markets = symbols.Select(MapDynamicMarket).ToList();
        return new BittradeDynamicExchangeInfo { Markets = markets };
    }

    private static BittradeDynamicMarketInfo MapDynamicMarket(SymbolNormalized symbol)
    {
        var baseCurrency = CurrencyCodeConverter.ToCurrencyString(symbol.BaseCurrency);
        var quoteCurrency = CurrencyCodeConverter.ToCurrencyString(symbol.QuoteCurrency);
        var displaySymbol = $"{baseCurrency.ToUpperInvariant()}/{quoteCurrency.ToUpperInvariant()}";
        if (!ExchangeSymbol.TryParse(symbol.Symbol.Value, out var parsed))
        {
            throw new ArgumentException(
                $"Bittrade symbol is invalid: '{symbol.Symbol.Value}'. Expected lowercase alphanumeric like 'btcjpy'.",
                nameof(symbol));
        }

        var product = parsed.Value;
        var priceIncrement = Pow10(-symbol.PricePrecision);
        var sizeIncrement = Pow10(-symbol.AmountPrecision);
        var minSize = symbol.MinOrderAmount;
        var minNotional = symbol.MinOrderValue;
        var supported = symbol.State.IsKnown && symbol.State.Known == ExchangeSymbolState.Online;

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
            StatusNote = symbol.State.ToString()
        };
    }

    private static decimal Pow10(int power) =>
        (decimal)Math.Pow(10, power);
}
