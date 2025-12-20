using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using System.Text.Json;
using ExchangeInfoDto = ExchangeApi.Common.Dtos.ExchangeInfo;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;

/// <summary>
/// Bittrade の ExchangeInfo API 実装（/v1/common/symbols を使用）。
/// </summary>
public sealed class BittradeExchangeInfoApi : IExchangeInfoApi
{
    private readonly IRestClient _restClient;

    public BittradeExchangeInfoApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public async Task<ExchangeInfoDto> GetExchangeInfoAsync(CancellationToken cancellationToken = default)
    {
        var response = await _restClient.GetAsync<SymbolsResponse>(
            "v1/common/symbols",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Data is null)
        {
            throw new ExchangeApiException("Bittrade symbols response invalid.");
        }

        var markets = response.Data.Select(MapSymbol).ToList();
        return new ExchangeInfoDto(markets, Features: null, RateLimits: null, Maintenance: null);
    }

    private static ExchangeMarketInfo MapSymbol(SymbolInfo s)
    {
        var symbol = $"{s.BaseCurrency.ToUpperInvariant()}/{s.QuoteCurrency.ToUpperInvariant()}";
        var product = s.Symbol.ToLowerInvariant();
        var priceIncrement = Pow10(-s.PricePrecision);
        var sizeIncrement = Pow10(-s.AmountPrecision);
        var minSize = ParseDecimalFlexible(s.MinOrderAmount);
        var minNotional = ParseNullableDecimalFlexible(s.MinOrderValue);
        var supported = string.Equals(s.State, "online", StringComparison.OrdinalIgnoreCase);

        return new ExchangeMarketInfo(
            Symbol: symbol,
            ProductCode: product,
            Type: "Spot",
            MinSize: minSize,
            MaxSize: null,
            MinNotional: minNotional,
            PriceIncrement: priceIncrement,
            SizeIncrement: sizeIncrement,
            MakerFeeRate: null,
            TakerFeeRate: null,
            FeeCurrency: null,
            FeeType: null,
            IsSupported: supported,
            StatusNote: s.State);
    }

    private static decimal ParseDecimalFlexible(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => ParseDecimal(element.GetString()!),
            JsonValueKind.Number => element.GetDecimal(),
            _ => throw new ExchangeApiException($"Unexpected JSON type for decimal: {element.ValueKind}")
        };
    }

    private static decimal? ParseNullableDecimalFlexible(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined) return null;
        if (element.ValueKind == JsonValueKind.String)
        {
            var s = element.GetString();
            return string.IsNullOrWhiteSpace(s) ? null : ParseDecimal(s);
        }

        if (element.ValueKind == JsonValueKind.Number) return element.GetDecimal();
        return null;
    }

    private static decimal Pow10(int power) =>
        (decimal)Math.Pow(10, power);

    private static decimal ParseDecimal(string s) =>
        decimal.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);
}
