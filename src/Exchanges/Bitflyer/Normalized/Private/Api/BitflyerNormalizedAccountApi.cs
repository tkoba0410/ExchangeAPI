using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Markets;
using NormalizedRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Account;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using RawPrivateModels = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Api;

internal sealed class BitflyerNormalizedAccountApi
{
    private readonly IBitflyerRawApi _raw;
    private readonly IBitflyerMarketResolver _markets;

    public BitflyerNormalizedAccountApi(IBitflyerRawApi raw, IBitflyerMarketResolver markets)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<Call<NormalizedRequests.GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBalanceCallAsync(new RawPrivateModels.GetBalancesRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetBalancesRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetBalances", BitflyerAccountMapper.MapBalances);
    }

    public async Task<Call<NormalizedRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new NormalizedRequests.GetAccountExecutionsRequest(symbol);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<NormalizedRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
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
            return CreateCallError<NormalizedRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>(
                marketCall,
                request,
                "Bitflyer.GetExecutions",
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetExecutionsPrivateCallAsync(new RawPrivateModels.GetAccountExecutionsRequest(productCode), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetExecutions",
            raw => BitflyerAccountMapper.MapAccountExecutions(symbol, raw));
    }

    public async Task<Call<NormalizedRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        var request = new NormalizedRequests.GetTradingCommissionRequest(symbol);
        if (marketCall.Result is CallResult<BitflyerMarketInfo>.Err marketError)
        {
            return CreateCallError<NormalizedRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
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
            return CreateCallError<NormalizedRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>(
                marketCall,
                request,
                "Bitflyer.GetTradingCommission",
                new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code."));
        }

        var rawCall = await _raw
            .GetTradingCommissionCallAsync(new RawPrivateModels.GetTradingCommissionRequest(productCode), cancellationToken)
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
