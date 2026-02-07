using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Adapter.Internal.Operations;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Static;
using ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Compose;
using ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Dynamic;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;
using MarketsCall = ExchangeApi.Primitives.CallCommon.Call<ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Requests.GetMarketsRequest, System.Collections.Generic.IReadOnlyList<ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos.BitflyerMarketNormalized>>;
using TradingCommissionCall = ExchangeApi.Primitives.CallCommon.Call<ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Requests.GetTradingCommissionRequest, ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos.BitflyerTradingCommissionNormalized>;
using HealthCall = ExchangeApi.Primitives.CallCommon.Call<ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Requests.GetHealthRequest, ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos.BitflyerHealthNormalized>;
using BoardStateCall = ExchangeApi.Primitives.CallCommon.Call<ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Requests.GetBoardStateRequest, ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos.BitflyerBoardStateNormalized>;
namespace ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Adapter.Public.Api;

/// <summary>
/// bitFlyer の ExchangeInfo 実装（/v1/getmarkets を使用）。
/// </summary>
public sealed class BitflyerExchangeInfoApi : IExchangeInfoProvider
{
    private readonly Func<CancellationToken, Task<MarketsCall>>? _getMarkets;
    private readonly Func<Symbol, CancellationToken, Task<TradingCommissionCall>>? _getTradingCommission;
    private readonly Func<ProductCode, CancellationToken, Task<HealthCall>>? _getHealth;
    private readonly Func<ProductCode, CancellationToken, Task<BoardStateCall>>? _getBoardState;

    public BitflyerExchangeInfoApi() { }

    internal BitflyerExchangeInfoApi(BitflyerNormalizedPublicApi normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _getMarkets = normalized.GetMarketsCallAsync;
        _getHealth = normalized.GetHealthCallAsync;
        _getBoardState = normalized.GetBoardStateCallAsync;
    }

    internal BitflyerExchangeInfoApi(IBitflyerNormalizedApi normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _getMarkets = normalized.GetMarketsCallAsync;
        _getTradingCommission = normalized.GetTradingCommissionCallAsync;
        _getHealth = normalized.GetHealthCallAsync;
        _getBoardState = normalized.GetBoardStateCallAsync;
    }

    public async Task<Call<ExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoAsync(
        ExchangeInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var staticInfo = BitflyerStaticExchangeInfoLoader.Load();
            var dynamicInfo = await GetDynamicInfoAsync(cancellationToken).ConfigureAwait(false);
            var composed = BitflyerExchangeInfoComposer.Compose(staticInfo, dynamicInfo);
            var response = MapExchangeInfo(composed);
            var meta = new CallMeta(
                Layer: "Contracts",
                Component: BitflyerExchangeInfoOperations.GetExchangeInfo,
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
                BitflyerExchangeInfoOperations.GetExchangeInfo,
                ex);
        }
    }

    public Task<Call<GetCurrencysRequest, IReadOnlyList<CurrencyCode>>> GetCurrencysCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetCurrencysRequest();
        return Task.FromResult(NotSupportedCall.Create<GetCurrencysRequest, IReadOnlyList<CurrencyCode>>(
            "Contracts",
            BitflyerExchangeInfoOperations.GetCurrencys,
            request,
            "Currencys"));
    }

    public Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetTimestampRequest();
        return Task.FromResult(NotSupportedCall.Create<GetTimestampRequest, DateTimeOffset>(
            "Contracts",
            BitflyerExchangeInfoOperations.GetTimestamp,
            request,
            "Timestamp"));
    }

    private static ExchangeInfoDto MapExchangeInfo(BitflyerStaticExchangeInfo info)
    {
        var mapped = info.Markets.Select(MapMarket).ToList();
        return new ExchangeInfoDto(
            Markets: mapped,
            Features: MapFeatures(info.Features),
            RateLimits: MapRateLimits(info.RateLimits),
            Maintenance: MapMaintenance(info.Maintenance));
    }

    private static ExchangeMarketInfo MapMarket(BitflyerStaticMarketInfo market) =>
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

    private static ExchangeFeatureFlags? MapFeatures(BitflyerStaticFeatureFlags? features) =>
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

    private static ExchangeRateLimits? MapRateLimits(BitflyerStaticRateLimits? limits) =>
        limits is null ? null : new ExchangeRateLimits(limits.RequestsPerMinute, limits.OrdersPerMinute);

    private static ExchangeMaintenance? MapMaintenance(BitflyerStaticMaintenance? maintenance) =>
        maintenance is null
            ? null
            : new ExchangeMaintenance(
                Status: MapMaintenanceStatus(maintenance.Status),
                PlannedUntil: maintenance.PlannedUntil,
                Message: ToFreeText(maintenance.Message));

    private static ExchangeMaintenanceStatus? MapMaintenanceStatus(BitflyerStaticMaintenanceStatus? status) =>
        status switch
        {
            null => null,
            BitflyerStaticMaintenanceStatus.Normal => ExchangeMaintenanceStatus.Normal,
            BitflyerStaticMaintenanceStatus.Planned => ExchangeMaintenanceStatus.Planned,
            BitflyerStaticMaintenanceStatus.Unplanned => ExchangeMaintenanceStatus.Unplanned,
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

    private async Task<BitflyerDynamicExchangeInfo?> GetDynamicInfoAsync(CancellationToken cancellationToken)
    {
        if (_getMarkets is null) return null;

        var marketsCall = await _getMarkets(cancellationToken).ConfigureAwait(false);
        if (marketsCall.Result is CallResult<IReadOnlyList<BitflyerMarketNormalized>>.Err)
        {
            return null;
        }

        var markets = ((CallResult<IReadOnlyList<BitflyerMarketNormalized>>.Ok)marketsCall.Result).Response;
        var marketInfos = markets.Select(MapDynamicMarket).ToList();

        if (_getTradingCommission is not null)
        {
            await EnrichTradingCommissionAsync(marketInfos, cancellationToken).ConfigureAwait(false);
        }

        BitflyerDynamicMaintenance? maintenance = null;
        if (marketInfos.Count > 0 && (_getHealth is not null || _getBoardState is not null))
        {
            var productCode = ProductCode.ParseNormalized(marketInfos[0].ProductCode);
            maintenance = await GetMaintenanceAsync(productCode, cancellationToken).ConfigureAwait(false);
        }

        return new BitflyerDynamicExchangeInfo
        {
            Markets = marketInfos,
            Maintenance = maintenance
        };
    }

    private static BitflyerDynamicMarketInfo MapDynamicMarket(BitflyerMarketNormalized market)
    {
        var productCodeText = market.ProductCode.Value;
        var symbol = productCodeText.Contains('_', StringComparison.Ordinal)
            ? productCodeText.Replace('_', '/')
            : productCodeText;

        return new BitflyerDynamicMarketInfo
        {
            ProductCode = productCodeText,
            Symbol = symbol,
            Type = market.Alias is null ? "Spot" : "Spot"
        };
    }

    private async Task EnrichTradingCommissionAsync(
        List<BitflyerDynamicMarketInfo> markets,
        CancellationToken cancellationToken)
    {
        foreach (var market in markets)
        {
            if (string.IsNullOrWhiteSpace(market.Symbol))
            {
                continue;
            }

            var symbol = new Symbol(market.Symbol);
            TradingCommissionCall call;
            try
            {
                call = await _getTradingCommission!(symbol, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                continue;
            }

            if (call.Result is CallResult<BitflyerTradingCommissionNormalized>.Err)
            {
                continue;
            }

            var ok = (CallResult<BitflyerTradingCommissionNormalized>.Ok)call.Result;
            if (ok.Response.CommissionRate is null)
            {
                continue;
            }

            market.TakerFeeRate = ok.Response.CommissionRate;
            market.FeeType = "Percentage";
        }
    }

    private async Task<BitflyerDynamicMaintenance?> GetMaintenanceAsync(
        ProductCode productCode,
        CancellationToken cancellationToken)
    {
        BitflyerDynamicMaintenance? maintenanceFromHealth = null;
        if (_getHealth is not null)
        {
            try
            {
                var call = await _getHealth(productCode, cancellationToken).ConfigureAwait(false);
                if (call.Result is CallResult<BitflyerHealthNormalized>.Ok ok)
                {
                    maintenanceFromHealth = MapMaintenanceFromHealth(ok.Response.Status);
                }
            }
            catch
            {
                // Ignore dynamic maintenance failures.
            }
        }

        if (_getBoardState is not null)
        {
            try
            {
                var call = await _getBoardState(productCode, cancellationToken).ConfigureAwait(false);
                if (call.Result is CallResult<BitflyerBoardStateNormalized>.Ok ok)
                {
                    var fromBoardState = MapMaintenanceFromBoardState(ok.Response);
                    if (fromBoardState is not null)
                    {
                        return fromBoardState;
                    }
                }
            }
            catch
            {
                // Ignore dynamic maintenance failures.
            }
        }

        return maintenanceFromHealth;
    }

    private static BitflyerDynamicMaintenance? MapMaintenanceFromHealth(FreeText? status)
    {
        if (status is null || status.Value.IsEmpty) return null;
        var statusText = status.Value.Value;
        var normalized = statusText.Trim().ToUpperInvariant();
        if (normalized is "NORMAL" || normalized.Contains("BUSY", StringComparison.Ordinal))
        {
            return new BitflyerDynamicMaintenance
            {
                Status = BitflyerDynamicMaintenanceStatus.Normal,
                Message = $"Health:{statusText}"
            };
        }

        if (normalized is "STOP" or "FAIL")
        {
            return new BitflyerDynamicMaintenance
            {
                Status = BitflyerDynamicMaintenanceStatus.Unplanned,
                Message = $"Health:{statusText}"
            };
        }

        return null;
    }

    private static BitflyerDynamicMaintenance? MapMaintenanceFromBoardState(BitflyerBoardStateNormalized state)
    {
        if (state.Health is not null && !state.Health.Value.IsEmpty)
        {
            var healthText = state.Health.Value.Value;
            var health = healthText.Trim().ToUpperInvariant();
            if (health is "NORMAL" || health.Contains("BUSY", StringComparison.Ordinal))
            {
                return new BitflyerDynamicMaintenance
                {
                    Status = BitflyerDynamicMaintenanceStatus.Normal,
                    Message = $"BoardState.Health:{healthText}"
                };
            }

            if (health is "STOP" or "FAIL")
            {
                return new BitflyerDynamicMaintenance
                {
                    Status = BitflyerDynamicMaintenanceStatus.Unplanned,
                    Message = $"BoardState.Health:{healthText}"
                };
            }
        }

        if (state.State is not null && !state.State.Value.IsEmpty)
        {
            var stateText = state.State.Value.Value;
            var boardState = stateText.Trim().ToUpperInvariant();
            if (boardState is "RUNNING")
            {
                return new BitflyerDynamicMaintenance
                {
                    Status = BitflyerDynamicMaintenanceStatus.Normal,
                    Message = $"BoardState.State:{stateText}"
                };
            }

            if (boardState is "CLOSED" or "STOP")
            {
                return new BitflyerDynamicMaintenance
                {
                    Status = BitflyerDynamicMaintenanceStatus.Unplanned,
                    Message = $"BoardState.State:{stateText}"
                };
            }
        }

        return null;
    }
}
