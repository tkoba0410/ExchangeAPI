using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Exchanges.Bitflyer.Normalize;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Spec.CallCommon;
using RawRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Call;

internal sealed class BitflyerNormalizedMarginApi : IBitflyerNormalizedMarginApi
{
    private readonly IBitflyerRawAccountApi _accountApi;
    private readonly IExchangeMarketResolver _markets;

    public BitflyerNormalizedMarginApi(IBitflyerRawAccountApi accountApi, IExchangeMarketResolver markets)
    {
        _accountApi = accountApi ?? throw new ArgumentNullException(nameof(accountApi));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public async Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _accountApi
            .GetBalancesAsync(new RawRequests.GetBalancesRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new GetBalancesRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetBalances", BitflyerAccountMapper.MapBalances);
    }

    public async Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>> GetAccountExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var request = new GetAccountExecutionsRequest(symbol);
        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>(
                marketCall,
                request,
                "Bitflyer.GetExecutions",
                marketError!);
        }

        var rawCall = await _accountApi
            .GetExecutionsAsync(new RawRequests.GetAccountExecutionsRequest(productCode!), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetExecutions",
            raw => BitflyerAccountMapper.MapAccountExecutions(symbol, raw));
    }

    public async Task<Call<GetOpenPositionsRequest, IReadOnlyList<Position>>> GetOpenPositionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        var request = new GetOpenPositionsRequest(symbol);
        var marketCall = await _markets.ResolveCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        if (!TryGetProductCode(marketCall, out var productCode, out var marketError))
        {
            return CreateCallError<GetOpenPositionsRequest, IReadOnlyList<Position>>(
                marketCall,
                request,
                "Bitflyer.GetPositions",
                marketError!);
        }

        var rawCall = await _accountApi
            .GetPositionsAsync(new RawRequests.GetPositionsRequest(productCode!), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetPositions",
            raw => BitflyerMarginMapper.MapPositions(symbol, raw));
    }

    public async Task<Call<GetCollateralRequest, Collateral>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _accountApi
            .GetCollateralAsync(new RawRequests.GetCollateralRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new GetCollateralRequest();
        return CreateCall(rawCall, request, "Bitflyer.GetCollateral", BitflyerMarginMapper.MapCollateral);
    }

    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, TOk> mapper)
    {
        var meta = new CallMeta(
            Layer: "Normalized",
            Component: component,
            Tags: null,
            Children: new[] { rawCall.Id });

        return rawCall.Result switch
        {
            CallResult<TRaw>.Err err => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(err.Error),
                Meta: meta),
            CallResult<TRaw>.Ok ok => MapOk(rawCall, request, component, ok.Response, mapper, meta),
            _ => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Unknown, "Raw call returned unknown result.")),
                Meta: meta)
        };
    }

    private static Call<TReq, TOk> MapOk<TRawReq, TReq, TRaw, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        TRaw raw,
        Func<TRaw, TOk> mapper,
        CallMeta meta)
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
                Meta: meta);
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
                Meta: meta);
        }
    }

    private static bool TryGetProductCode(
        Call<ExchangeApi.Contracts.Requests.ResolveExchangeMarketRequest, ExchangeMarketInfo> marketCall,
        out string? productCode,
        out CallError? error)
    {
        if (marketCall.Result is CallResult<ExchangeMarketInfo>.Err err)
        {
            productCode = null;
            error = err.Error;
            return false;
        }

        if (marketCall.Result is CallResult<ExchangeMarketInfo>.Ok ok &&
            !string.IsNullOrWhiteSpace(ok.Response.ProductCode))
        {
            productCode = ok.Response.ProductCode;
            error = null;
            return true;
        }

        productCode = null;
        error = new CallError(CallErrorKind.Unknown, "Market resolution returned empty product code.");
        return false;
    }

    private static Call<TReq, TOk> CreateCallError<TReq, TOk>(
        Call<ExchangeApi.Contracts.Requests.ResolveExchangeMarketRequest, ExchangeMarketInfo> marketCall,
        TReq request,
        string component,
        CallError error)
    {
        var meta = new CallMeta(
            Layer: "Normalized",
            Component: component,
            Tags: null,
            Children: new[] { marketCall.Id });

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: marketCall.StartedAt,
            Duration: marketCall.Duration,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }

}
