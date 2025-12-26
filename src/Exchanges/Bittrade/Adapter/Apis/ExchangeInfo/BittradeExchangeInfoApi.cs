using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using System.Text.Json;
using ExchangeInfoDto = ExchangeApi.Contracts.Dtos.ExchangeInfo;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;

/// <summary>
/// Bittrade の ExchangeInfo API 実装（/v1/common/symbols を使用）。
/// </summary>
internal sealed class BittradeExchangeInfoApi : IExchangeInfoApi
{
    private readonly IRestClient _restClient;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public BittradeExchangeInfoApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public async Task<SymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default)
    {
        const string operation = "Bittrade.ExchangeInfo.GetSymbols";
        try
        {
            return await _restClient.GetAsync<SymbolsResponse>(
                "v1/common/symbols",
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (TransportException ex)
        {
            throw BittradeErrorMapper.FromTransportException(ex, Exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    public async Task<ExchangeInfoDto> GetExchangeInfoAsync(CancellationToken cancellationToken = default)
    {
        const string operation = "Bittrade.ExchangeInfo.GetExchangeInfo";
        try
        {
            var response = await GetSymbolsAsync(cancellationToken).ConfigureAwait(false);

            if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Data is null)
            {
                throw new ExchangeApiException(
                    message: "Bittrade symbols response invalid.",
                    exchange: Exchange,
                    operation: operation);
            }

            var markets = response.Data.Select(MapSymbol).ToList();
            return new ExchangeInfoDto(markets, Features: null, RateLimits: null, Maintenance: null);
        }
        catch (TransportException ex)
        {
            throw BittradeErrorMapper.FromTransportException(ex, Exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BittradeErrorMapper.EnrichBittradeException(ex, Exchange, operation);
        }
    }

    private static ExchangeMarketInfo MapSymbol(SymbolInfo s)
    {
        var symbol = $"{s.BaseCurrency.ToUpperInvariant()}/{s.QuoteCurrency.ToUpperInvariant()}";
        var product = s.Symbol.Value.ToLowerInvariant();
        var priceIncrement = Pow10(-s.PricePrecision);
        var sizeIncrement = Pow10(-s.AmountPrecision);
        var minSize = ParseDecimalFlexible(s.MinOrderAmount, "min-order-amt");
        var minNotional = ParseNullableDecimalFlexible(s.MinOrderValue, "min-order-value");
        var supported = string.Equals(s.State, "online", StringComparison.OrdinalIgnoreCase);

        return new ExchangeMarketInfo(
            Symbol: symbol,
            ProductCode: product,
            Type: "Spot",
            MinSize: new Size(minSize),
            MaxSize: null,
            MinNotional: minNotional,
            PriceIncrement: new Price(priceIncrement),
            SizeIncrement: new Size(sizeIncrement),
            MakerFeeRate: null,
            TakerFeeRate: null,
            FeeCurrency: null,
            FeeType: null,
            IsSupported: supported,
            StatusNote: s.State);
    }

    private static decimal ParseDecimalFlexible(JsonElement element, string field)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => ParseDecimalOrThrow(element.GetString()!, field),
            JsonValueKind.Number => element.GetDecimal(),
            _ => throw new ExchangeApiException($"Unexpected JSON type for {field}: {element.ValueKind}")
        };
    }

    private static decimal? ParseNullableDecimalFlexible(JsonElement element, string field)
    {
        if (element.ValueKind == JsonValueKind.Null || element.ValueKind == JsonValueKind.Undefined) return null;
        if (element.ValueKind == JsonValueKind.String)
        {
            var s = element.GetString();
            return string.IsNullOrWhiteSpace(s) ? null : ParseDecimalOrThrow(s, field);
        }

        if (element.ValueKind == JsonValueKind.Number) return element.GetDecimal();
        return null;
    }

    private static decimal Pow10(int power) =>
        (decimal)Math.Pow(10, power);

    private static decimal ParseDecimalOrThrow(string s, string field)
    {
        if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw new ExchangeApiException($"Invalid decimal for SymbolInfo.{field}: '{s}'.");
    }
}
