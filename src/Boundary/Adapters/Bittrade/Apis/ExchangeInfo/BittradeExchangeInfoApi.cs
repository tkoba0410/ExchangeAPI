using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
using ExchangeInfoDto = ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Spec.CallCommon;
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
        var call = await GetExchangeInfoCallAsync(cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.ExchangeInfo.GetExchangeInfo");
    }

    public async Task<Call<GetExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetExchangeInfoRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetSymbolsCallAsync(cancellationToken).ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                "Bittrade.ExchangeInfo.GetExchangeInfo",
                ok => new ExchangeInfoDto(ok.Select(MapSymbol).ToList(), Features: null, RateLimits: null, Maintenance: null));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetExchangeInfoRequest, ExchangeInfoDto>(
                request,
                startedAt,
                "Bittrade.ExchangeInfo.GetExchangeInfo",
                ex);
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

    private static TOk Unwrap<TReq, TOk>(Call<TReq, TOk> call, string operation)
    {
        return call.Result switch
        {
            CallResult<TOk>.Ok ok => ok.Response,
            CallResult<TOk>.Err err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: Exchange,
                operation: operation,
                statusCode: ApiCallMapper.ToStatusCode(err.Error.HttpStatus),
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(err.Error)),
            _ => throw new ExchangeApiException(
                message: "Unknown call result.",
                exchange: Exchange,
                operation: operation,
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(new CallError(CallErrorKind.Unknown, "Unknown call result.")))
        };
    }
}
