using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bittrade.RawApi;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeInfoDto = ExchangeApi.Contracts.Dtos.ExchangeInfo;

namespace ExchangeApi.Adapter.Bittrade.Apis.ExchangeInfo;

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
        var response = await _restClient.GetAsync<BittradeSymbolsResponse>(
            "v1/common/symbols",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Data is null)
        {
            throw new ExchangeApiException("Bittrade symbols response invalid.");
        }

        var markets = response.Data.Select(MapSymbol).ToList();
        return new ExchangeInfoDto(markets, Features: null, RateLimits: null, Maintenance: null);
    }

    private static ExchangeMarketInfo MapSymbol(BittradeSymbolInfo s)
    {
        var symbol = $"{s.BaseCurrency.ToUpperInvariant()}/{s.QuoteCurrency.ToUpperInvariant()}";
        var product = s.Symbol.ToLowerInvariant();
        var priceIncrement = Pow10(-s.PricePrecision);
        var sizeIncrement = Pow10(-s.AmountPrecision);
        var minSize = ParseDecimal(s.MinOrderAmount);
        var minNotional = ParseNullableDecimal(s.MinOrderValue);
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

    private static decimal Pow10(int power) =>
        (decimal)Math.Pow(10, power);

    private static decimal ParseDecimal(string s) =>
        decimal.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);

    private static decimal? ParseNullableDecimal(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        return decimal.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);
    }
}
