using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Static;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;

namespace ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Adapter.Public.Api;

/// <summary>
/// Bittrade の ExchangeInfo API 実装。
/// </summary>
public sealed class BittradeExchangeInfoApi : IExchangeInfoProvider
{
    internal BittradeExchangeInfoApi(NormalizedPublicApi normalized)
    {
        _ = normalized ?? throw new ArgumentNullException(nameof(normalized));
    }

    public Task<Call<ExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoAsync(
        ExchangeInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var staticInfo = BittradeStaticExchangeInfoLoader.Load();
            var response = MapExchangeInfo(staticInfo);
            var meta = new CallMeta(
                Layer: CallMetaVocabulary.Layer.Contracts,
                Component: BittradeExchangeInfoOperations.GetExchangeInfo,
                EndpointId: CallMeta.InternalEndpointId,
                Tags: null,
                Children: null);
            var call = new Call<ExchangeInfoRequest, ExchangeInfoDto>(
                Id: CallId.New(),
                StartedAt: startedAt,
                Duration: DateTimeOffset.UtcNow - startedAt,
                Request: request,
                Result: new CallResult<ExchangeInfoDto>.Ok(response),
                Meta: meta);
            return Task.FromResult(call);
        }
        catch (Exception ex)
        {
            var call = ExchangeInfoCallMapper.FromException<ExchangeInfoRequest, ExchangeInfoDto>(
                request,
                startedAt,
                BittradeExchangeInfoOperations.GetExchangeInfo,
                ex);
            return Task.FromResult(call);
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
}
