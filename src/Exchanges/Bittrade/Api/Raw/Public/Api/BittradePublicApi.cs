using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Constants;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Internal;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Public.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Api;

/// <summary>
/// Bittrade Public REST API の Raw 実装。
/// </summary>
internal sealed class BittradePublicApi
{
    private readonly IBittradeWireCallExecutor _wire;
    private readonly BittradeRawCallExecutor _executor;

    public BittradePublicApi(IBittradeWireCallExecutor wire, BittradeRawCallExecutor executor)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
    }

    public Task<Call<GetMergedTickerRequest, RawMergedResponse>> GetDetailMergedCallAsync(
        GetMergedTickerRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetDetailMerged),
            BittradePublicEndpoints.GetDetailMerged(request.Symbol.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawMergedResponse>(json, Component(BittradeEndpointIds.GetDetailMerged)));

    public Task<Call<GetDepthRequest, RawDepthResponse>> GetDepthCallAsync(
        GetDepthRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetDepth),
            BittradePublicEndpoints.GetDepth(request.Symbol.Value, request.Type?.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawDepthResponse>(json, Component(BittradeEndpointIds.GetDepth)));

    public Task<Call<GetTradesRequest, RawTradeResponse>> GetTradeCallAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetTrade),
            BittradePublicEndpoints.GetTrade(request.Symbol.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTradeResponse>(json, Component(BittradeEndpointIds.GetTrade)));

    public Task<Call<GetSymbolsRequest, RawSymbolsResponse>> GetSymbolsCallAsync(
        GetSymbolsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetSymbols),
            BittradePublicEndpoints.GetSymbols(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawSymbolsResponse>(json, Component(BittradeEndpointIds.GetSymbols)));

    public Task<Call<GetCurrenciesRequest, RawCurrenciesResponse>> GetCurrencysCallAsync(
        GetCurrenciesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetCurrencys),
            BittradePublicEndpoints.GetCurrencys(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCurrenciesResponse>(json, Component(BittradeEndpointIds.GetCurrencys)));

    public Task<Call<GetTimestampRequest, RawTimestampResponse>> GetTimestampCallAsync(
        GetTimestampRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetTimestamp),
            BittradePublicEndpoints.GetTimestamp(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTimestampResponse>(json, Component(BittradeEndpointIds.GetTimestamp)));

    public Task<Call<GetKlinesRequest, RawKlinesResponse>> GetHistoryKlineCallAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetHistoryKline),
            BittradePublicEndpoints.GetHistoryKline(
                request.Symbol.Value,
                request.Period.Value,
                request.Size?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawKlinesResponse>(json, Component(BittradeEndpointIds.GetHistoryKline)));

    public Task<Call<GetTickersRequest, RawTickersResponse>> GetTickersCallAsync(
        GetTickersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetTickers),
            BittradePublicEndpoints.GetTickers(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTickersResponse>(json, Component(BittradeEndpointIds.GetTickers)));

    public Task<Call<GetTradeHistoryRequest, RawTradeHistoryResponse>> GetHistoryTradeCallAsync(
        GetTradeHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetHistoryTrade),
            BittradePublicEndpoints.GetHistoryTrade(request.Symbol.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTradeHistoryResponse>(json, Component(BittradeEndpointIds.GetHistoryTrade)));

    private async Task<Call<TReq, TRes>> SendAndParse<TReq, TRes>(
        TReq request,
        string component,
        WireCallSpec spec,
        CancellationToken cancellationToken,
        Func<string, TRes> parse)
    {
        var wireCall = await _wire.SendAsync(spec, cancellationToken).ConfigureAwait(false);
        return _executor.Parse(request, component, wireCall, parse);
    }

    private static string Component(string endpointId) => $"Bittrade.{endpointId}";
}
