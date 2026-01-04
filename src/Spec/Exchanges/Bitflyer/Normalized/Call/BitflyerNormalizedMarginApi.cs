using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Exchanges.Bitflyer.Normalize;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
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

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default)
    {
        var call = await GetBalancesCallAsync(cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetBalances");
    }

    public async Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var call = await GetAccountExecutionsCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetExecutions");
    }

    public async Task<IReadOnlyList<Position>> GetOpenPositionsAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var call = await GetOpenPositionsCallAsync(symbol, cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetPositions");
    }

    public async Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default)
    {
        var call = await GetCollateralCallAsync(cancellationToken).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetCollateral");
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

        var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var rawCall = await _accountApi
            .GetExecutionsAsync(new RawRequests.GetAccountExecutionsRequest(productCode), cancellationToken)
            .ConfigureAwait(false);
        var request = new GetAccountExecutionsRequest(symbol);

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

        var productCode = await ToApiProductCodeAsync(symbol, cancellationToken).ConfigureAwait(false);
        var rawCall = await _accountApi
            .GetPositionsAsync(new RawRequests.GetPositionsRequest(productCode), cancellationToken)
            .ConfigureAwait(false);
        var request = new GetOpenPositionsRequest(symbol);

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

    private async Task<string> ToApiProductCodeAsync(Symbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode;
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

    private static TRes Unwrap<TReq, TRes>(Call<TReq, TRes> call, string operation)
    {
        return call.Result switch
        {
            CallResult<TRes>.Ok ok => ok.Response,
            CallResult<TRes>.Err err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: ExchangeCode.Bitflyer,
                operation: operation,
                statusCode: err.Error.HttpStatus is int status ? (HttpStatusCode?)status : null,
                innerException: err.Error.Exception),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }
}
