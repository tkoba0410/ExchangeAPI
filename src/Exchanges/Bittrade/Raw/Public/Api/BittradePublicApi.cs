using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Wire.Public.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;

/// <summary>
/// Bittrade Public REST API の Raw 実装。
/// </summary>
internal sealed class BittradePublicApi
{
    private readonly IWireTransport _wire;

    public BittradePublicApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Call<GetMergedTickerRequest, RawMergedResponse>> GetDetailMergedCallAsync(
        GetMergedTickerRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetMergedTicker",
            BittradePublicEndpoints.GetDetailMerged(request.Symbol),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawMergedResponse>(json, "Bittrade.GetMergedTicker"));

    public Task<Call<GetDepthRequest, RawDepthResponse>> GetDepthCallAsync(
        GetDepthRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetDepth",
            BittradePublicEndpoints.GetDepth(request.Symbol, request.Type),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawDepthResponse>(json, "Bittrade.GetDepth"));

    public Task<Call<GetTradesRequest, RawTradeResponse>> GetTradeCallAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetTrades",
            BittradePublicEndpoints.GetTrade(request.Symbol),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTradeResponse>(json, "Bittrade.GetTrades"));

    public Task<Call<GetSymbolsRequest, RawSymbolsResponse>> GetSymbolsCallAsync(
        GetSymbolsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetSymbols",
            BittradePublicEndpoints.GetSymbols(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawSymbolsResponse>(json, "Bittrade.GetSymbols"));

    public Task<Call<GetCurrenciesRequest, RawCurrenciesResponse>> GetCurrencysCallAsync(
        GetCurrenciesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetCurrencies",
            BittradePublicEndpoints.GetCurrencys(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCurrenciesResponse>(json, "Bittrade.GetCurrencies"));

    public Task<Call<GetTimestampRequest, RawTimestampResponse>> GetTimestampCallAsync(
        GetTimestampRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetTimestamp",
            BittradePublicEndpoints.GetTimestamp(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTimestampResponse>(json, "Bittrade.GetTimestamp"));

    public Task<Call<GetKlinesRequest, RawKlinesResponse>> GetHistoryKlineCallAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetKlines",
            BittradePublicEndpoints.GetHistoryKline(
                request.Symbol,
                request.Period,
                request.Size?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawKlinesResponse>(json, "Bittrade.GetKlines"));

    public Task<Call<GetTickersRequest, RawTickersResponse>> GetTickersCallAsync(
        GetTickersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetTickers",
            BittradePublicEndpoints.GetTickers(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTickersResponse>(json, "Bittrade.GetTickers"));

    public Task<Call<GetTradeHistoryRequest, RawTradeHistoryResponse>> GetHistoryTradeCallAsync(
        GetTradeHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.GetTradeHistory",
            BittradePublicEndpoints.GetHistoryTrade(request.Symbol),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTradeHistoryResponse>(json, "Bittrade.GetTradeHistory"));

    private async Task<Call<TReq, TRes>> SendAndParse<TReq, TRes>(
        TReq request,
        string component,
        WireCallSpec spec,
        CancellationToken cancellationToken,
        Func<string, TRes> parse)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (parse is null) throw new ArgumentNullException(nameof(parse));

        var wireCall = await _wire.SendAsync(ExchangeCode.Bittrade, spec, cancellationToken).ConfigureAwait(false);
        return CreateCall(request, component, wireCall, parse);
    }

    private static Call<TReq, TRes> CreateCall<TReq, TRes>(
        TReq request,
        string component,
        Call<WireCallSpec, WireResponse> wireCall,
        Func<string, TRes> parse)
    {
        return wireCall.Result switch
        {
            CallResult<WireResponse>.Err err => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(err.Error),
                Meta: wireCall.Meta),
            CallResult<WireResponse>.Ok ok => CreateOkCall(request, component, ok.Response, wireCall, parse),
            _ => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(new CallError(CallErrorKind.Unknown, "Wire call returned unknown result.")),
                Meta: wireCall.Meta)
        };
    }

    private static Call<TReq, TRes> CreateOkCall<TReq, TRes>(
        TReq request,
        string component,
        WireResponse response,
        Call<WireCallSpec, WireResponse> wireCall,
        Func<string, TRes> parse)
    {
        if (response.StatusCode is < 200 or >= 300)
        {
            var error = new CallError(
                CallErrorKind.Http,
                $"{component} failed with status {response.StatusCode}.",
                HttpStatus: response.StatusCode,
                BodySnippet: Snip(response.Json));
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(error),
                Meta: wireCall.Meta);
        }

        try
        {
            var parsed = parse(response.Json);
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Ok(parsed),
                Meta: wireCall.Meta);
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            var error = new CallError(
                CallErrorKind.Codec,
                $"{component} failed to parse response.",
                ex,
                response.StatusCode,
                Snip(response.Json));
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(error),
                Meta: wireCall.Meta);
        }
    }

    private static string? Snip(string? json)
    {
        if (string.IsNullOrEmpty(json)) return json;
        return json.Length <= 512 ? json : json[..512];
    }
}
