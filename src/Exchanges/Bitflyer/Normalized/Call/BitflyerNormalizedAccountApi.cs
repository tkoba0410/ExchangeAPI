using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Markets;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos.Account;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;
using ExchangeApi.Primitives.CallCommon;
using RawRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Call;

internal sealed class BitflyerNormalizedAccountApi : IBitflyerNormalizedAccountApi
{
    private readonly IBitflyerRawAccountApi _accountApi;
    private readonly IBitflyerMarketResolver _markets;

    public BitflyerNormalizedAccountApi(IBitflyerRawAccountApi accountApi, IBitflyerMarketResolver markets)
    {
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<Call<GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _accountApi
            .GetBalancesAsync(new RawRequests.GetBalancesRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new GetBalancesRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetBalances", BitflyerAccountMapper.MapBalances);
    }

    public async Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetAccountExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new GetAccountExecutionsRequest(symbol);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
                marketCall,
                request,
                "Bitflyer.GetExecutions",
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<BitflyerMarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : null;
        if (string.IsNullOrEmpty(productCode))
        {
            return CreateCallError<GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
                marketCall,
                request,
                "Bitflyer.GetExecutions",
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _accountApi
            .GetExecutionsAsync(new RawRequests.GetAccountExecutionsRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetExecutions",
            raw => BitflyerAccountMapper.MapAccountExecutions(symbol, raw));
    }

    public async Task<Call<GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new GetTradingCommissionRequest(symbol);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
                marketCall,
                request,
                "Bitflyer.GetTradingCommission",
                marketError.Error);
        }

        var productCode = marketCall.Result is CallResult<BitflyerMarketInfo>.Ok marketOk
            ? marketOk.Response.ProductCode
            : null;
        if (string.IsNullOrEmpty(productCode))
        {
            return CreateCallError<GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
                marketCall,
                request,
                "Bitflyer.GetTradingCommission",
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _accountApi
            .GetTradingCommissionAsync(new RawRequests.GetTradingCommissionRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetTradingCommission",
            raw => ParseTradingCommission(raw.RawJson, productCode));
    }

    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, TOk> mapper)
    {
        return rawCall.Result switch
        {
            CallResult<TRaw>.Err err => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(err.Error),
                Meta: rawCall.Meta),
            CallResult<TRaw>.Ok ok => MapOk(rawCall, request, component, ok.Response, mapper),
            _ => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Unknown, "Raw call returned unknown result.")),
                Meta: rawCall.Meta)
        };
    }

    private static BitflyerTradingCommissionNormalized ParseTradingCommission(string? rawJson, string productCode)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            throw new InvalidOperationException("Trading commission response is empty.");
        }

        using var doc = JsonDocument.Parse(rawJson);
        var root = doc.RootElement;
        string? parsedProductCode = null;
        decimal? commissionRate = null;

        if (root.ValueKind == JsonValueKind.Object)
        {
            if (root.TryGetProperty("product_code", out var productCodeElement)
                && productCodeElement.ValueKind == JsonValueKind.String)
            {
                parsedProductCode = productCodeElement.GetString();
            }

            if (root.TryGetProperty("commission_rate", out var commissionElement))
            {
                commissionRate = TryParseDecimal(commissionElement);
            }
        }

        return new BitflyerTradingCommissionNormalized(
            ProductCode: parsedProductCode ?? productCode,
            CommissionRate: commissionRate);
    }

    private static decimal? TryParseDecimal(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Number => element.GetDecimal(),
            JsonValueKind.String => TryParseDecimalString(element.GetString()),
            _ => null
        };
    }

    private static decimal? TryParseDecimalString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }

    private static Call<TReq, TOk> MapOk<TRawReq, TReq, TRaw, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        TRaw raw,
        Func<TRaw, TOk> mapper)
    {
        try
        {
            var mapped = mapper(raw);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Ok(mapped),
                Meta: rawCall.Meta);
        }
        catch (Exception ex)
        {
            var error = new CallError(CallErrorKind.Mapping, $"{component} failed to map normalized response.", ex);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(error),
                Meta: rawCall.Meta);
        }
    }

    private static Call<TReq, TOk> CreateCallError<TReq, TOk>(
        Call<ResolveBitflyerMarketRequest, BitflyerMarketInfo> marketCall,
        TReq request,
        string component,
        CallError error)
    {
        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: marketCall.StartedAt,
            Duration: marketCall.Duration,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: marketCall.Meta);
    }
}
