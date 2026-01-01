using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Call;
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
using System.Text.Json;
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

    public async Task<ApiCall<GetExchangeInfoRequest, ExchangeInfoDto, ApiError>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new GetExchangeInfoRequest();
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _normalized.GetSymbolsCallAsync(cancellationToken).ConfigureAwait(false);
            return call.Result switch
            {
                Ok<IReadOnlyList<BittradeSymbolNormalized>, JsonElement> ok => ApiCallMapper.Ok(
                    Exchange,
                    request,
                    call.Meta,
                    ok.StatusCode,
                    new ExchangeInfoDto(ok.Value.Select(MapSymbol).ToList(), Features: null, RateLimits: null, Maintenance: null)),
                Err<IReadOnlyList<BittradeSymbolNormalized>, JsonElement> err => ApiCallMapper.Err<GetExchangeInfoRequest, ExchangeInfoDto>(
                    Exchange,
                    request,
                    call.Meta,
                    err.StatusCode),
                _ => throw new InvalidOperationException("Unsupported CallResult type.")
            };
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<GetExchangeInfoRequest, ExchangeInfoDto>(Exchange, request, startedAt, ex);
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

    private static TOk Unwrap<TReq, TOk>(ApiCall<TReq, TOk, ApiError> call, string operation)
    {
        return call.Result switch
        {
            ApiOk<TOk, ApiError> ok => ok.Value,
            ApiErr<TOk, ApiError> err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: call.Exchange,
                operation: operation,
                statusCode: ApiCallMapper.ToStatusCode(err.StatusCode),
                errorCategory: ApiCallMapper.ToExchangeErrorCategory(err.Error.Kind)),
            _ => throw new InvalidOperationException("Unsupported ApiCallResult type.")
        };
    }
}
