using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
using ExchangeInfoDto = ExchangeApi.Contracts.Dtos.ExchangeInfo;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;

/// <summary>
/// Bittrade の ExchangeInfo API 実装（/v1/common/symbols を使用）。
/// </summary>
internal sealed class BittradeExchangeInfoApi : IExchangeInfoApi
{
    private readonly IBittradeNormalizedExchangeInfoApi _normalized;
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;

    public BittradeExchangeInfoApi(IBittradeNormalizedExchangeInfoApi normalized)
    {
        _normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
    }

    public async Task<ExchangeInfoDto> GetExchangeInfoAsync(CancellationToken cancellationToken = default)
    {
        const string operation = "Bittrade.ExchangeInfo.GetExchangeInfo";
        try
        {
            var symbols = await _normalized.GetSymbolsAsync(cancellationToken).ConfigureAwait(false);
            var markets = symbols.Select(MapSymbol).ToList();
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

    private static ExchangeMarketInfo MapSymbol(BittradeSymbolNormalized s)
    {
        var symbol = $"{s.BaseCurrency.ToUpperInvariant()}/{s.QuoteCurrency.ToUpperInvariant()}";
        var product = s.Symbol.ToLowerInvariant();
        var priceIncrement = Pow10(-s.PricePrecision);
        var sizeIncrement = Pow10(-s.AmountPrecision);
        var minSize = s.MinOrderAmount;
        var minNotional = s.MinOrderValue;
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

    private static decimal Pow10(int power) =>
        (decimal)Math.Pow(10, power);
}
