using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Wire.Constants;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Exchanges.Bittrade.Wire.Public.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;

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

    public Task<Call<GetDetailMergedRequest, GetDetailMergedResponse>> GetDetailMergedCallAsync(
        GetDetailMergedRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetDetailMerged),
            BittradePublicEndpoints.GetDetailMerged(request.Symbol.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetDetailMergedResponse>(json, Component(BittradeEndpointIds.GetDetailMerged)));

    public Task<Call<GetDepthRequest, GetDepthResponse>> GetDepthCallAsync(
        GetDepthRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetDepth),
            BittradePublicEndpoints.GetDepth(request.Symbol.Value, request.Type?.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetDepthResponse>(json, Component(BittradeEndpointIds.GetDepth)));

    public Task<Call<GetTradeRequest, GetTradeResponse>> GetTradeCallAsync(
        GetTradeRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetTrade),
            BittradePublicEndpoints.GetTrade(request.Symbol.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetTradeResponse>(json, Component(BittradeEndpointIds.GetTrade)));

    public Task<Call<GetSymbolsRequest, GetSymbolsResponse>> GetSymbolsCallAsync(
        GetSymbolsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetSymbols),
            BittradePublicEndpoints.GetSymbols(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetSymbolsResponse>(json, Component(BittradeEndpointIds.GetSymbols)));

    public Task<Call<GetCurrencysRequest, GetCurrencysResponse>> GetCurrencysCallAsync(
        GetCurrencysRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetCurrencys),
            BittradePublicEndpoints.GetCurrencys(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetCurrencysResponse>(json, Component(BittradeEndpointIds.GetCurrencys)));

    public Task<Call<GetTimestampRequest, GetTimestampResponse>> GetTimestampCallAsync(
        GetTimestampRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetTimestamp),
            BittradePublicEndpoints.GetTimestamp(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetTimestampResponse>(json, Component(BittradeEndpointIds.GetTimestamp)));

    public Task<Call<GetHistoryKlineRequest, GetHistoryKlineResponse>> GetHistoryKlineCallAsync(
        GetHistoryKlineRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetHistoryKline),
            BittradePublicEndpoints.GetHistoryKline(
                request.Symbol.Value,
                request.Period.Value,
                request.Size?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetHistoryKlineResponse>(json, Component(BittradeEndpointIds.GetHistoryKline)));

    public Task<Call<GetTickersRequest, GetTickersResponse>> GetTickersCallAsync(
        GetTickersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetTickers),
            BittradePublicEndpoints.GetTickers(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetTickersResponse>(json, Component(BittradeEndpointIds.GetTickers)));

    public Task<Call<GetHistoryTradeRequest, GetHistoryTradeResponse>> GetHistoryTradeCallAsync(
        GetHistoryTradeRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetHistoryTrade),
            BittradePublicEndpoints.GetHistoryTrade(request.Symbol.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetHistoryTradeResponse>(json, Component(BittradeEndpointIds.GetHistoryTrade)));

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
