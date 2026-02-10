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
internal sealed class PublicApi
{
    private readonly IBittradeWireCallExecutor _wire;
    private readonly RawCallExecutor _executor;

    public PublicApi(IBittradeWireCallExecutor wire, RawCallExecutor executor)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
    }

    public Task<Call<GetDetailMergedRequest, GetDetailMergedResponse>> GetDetailMergedCallAsync(
        GetDetailMergedRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetDetailMerged),
            PublicEndpoints.GetDetailMerged(request.Symbol.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetDetailMergedResponse>(json, Component(EndpointIds.GetDetailMerged)));

    public Task<Call<GetDepthRequest, GetDepthResponse>> GetDepthCallAsync(
        GetDepthRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetDepth),
            PublicEndpoints.GetDepth(request.Symbol.Value, request.Type?.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetDepthResponse>(json, Component(EndpointIds.GetDepth)));

    public Task<Call<GetTradeRequest, GetTradeResponse>> GetTradeCallAsync(
        GetTradeRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetTrade),
            PublicEndpoints.GetTrade(request.Symbol.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetTradeResponse>(json, Component(EndpointIds.GetTrade)));

    public Task<Call<GetSymbolsRequest, GetSymbolsResponse>> GetSymbolsCallAsync(
        GetSymbolsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetSymbols),
            PublicEndpoints.GetSymbols(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetSymbolsResponse>(json, Component(EndpointIds.GetSymbols)));

    public Task<Call<GetCurrenciesRequest, GetCurrenciesResponse>> GetCurrenciesCallAsync(
        GetCurrenciesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetCurrencies),
            PublicEndpoints.GetCurrencies(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetCurrenciesResponse>(json, Component(EndpointIds.GetCurrencies)));

    public Task<Call<GetTimestampRequest, GetTimestampResponse>> GetTimestampCallAsync(
        GetTimestampRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetTimestamp),
            PublicEndpoints.GetTimestamp(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetTimestampResponse>(json, Component(EndpointIds.GetTimestamp)));

    public Task<Call<GetHistoryKlineRequest, GetHistoryKlineResponse>> GetHistoryKlineCallAsync(
        GetHistoryKlineRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetHistoryKline),
            PublicEndpoints.GetHistoryKline(
                request.Symbol.Value,
                request.Period.Value,
                request.Size?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetHistoryKlineResponse>(json, Component(EndpointIds.GetHistoryKline)));

    public Task<Call<GetTickersRequest, GetTickersResponse>> GetTickersCallAsync(
        GetTickersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetTickers),
            PublicEndpoints.GetTickers(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetTickersResponse>(json, Component(EndpointIds.GetTickers)));

    public Task<Call<GetHistoryTradeRequest, GetHistoryTradeResponse>> GetHistoryTradeCallAsync(
        GetHistoryTradeRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetHistoryTrade),
            PublicEndpoints.GetHistoryTrade(request.Symbol.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetHistoryTradeResponse>(json, Component(EndpointIds.GetHistoryTrade)));

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
