using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Public.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Api;

/// <summary>
/// Bittrade Public REST API の Raw 実装。
/// </summary>
internal sealed class BittradePublicApi
{
    private readonly BittradeRawCallExecutor _executor;

    public BittradePublicApi(BittradeRawCallExecutor executor)
    {
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
    }

    public Task<Call<GetMergedTickerRequest, RawMergedResponse>> GetDetailMergedCallAsync(
        GetMergedTickerRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetMergedTicker",
            BittradePublicEndpoints.GetDetailMerged(request.Symbol),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawMergedResponse>(json, "Bittrade.GetMergedTicker"));

    public Task<Call<GetDepthRequest, RawDepthResponse>> GetDepthCallAsync(
        GetDepthRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetDepth",
            BittradePublicEndpoints.GetDepth(request.Symbol, request.Type),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawDepthResponse>(json, "Bittrade.GetDepth"));

    public Task<Call<GetTradesRequest, RawTradeResponse>> GetTradeCallAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetTrades",
            BittradePublicEndpoints.GetTrade(request.Symbol),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTradeResponse>(json, "Bittrade.GetTrades"));

    public Task<Call<GetSymbolsRequest, RawSymbolsResponse>> GetSymbolsCallAsync(
        GetSymbolsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetSymbols",
            BittradePublicEndpoints.GetSymbols(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawSymbolsResponse>(json, "Bittrade.GetSymbols"));

    public Task<Call<GetCurrenciesRequest, RawCurrenciesResponse>> GetCurrencysCallAsync(
        GetCurrenciesRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetCurrencies",
            BittradePublicEndpoints.GetCurrencys(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCurrenciesResponse>(json, "Bittrade.GetCurrencies"));

    public Task<Call<GetTimestampRequest, RawTimestampResponse>> GetTimestampCallAsync(
        GetTimestampRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetTimestamp",
            BittradePublicEndpoints.GetTimestamp(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTimestampResponse>(json, "Bittrade.GetTimestamp"));

    public Task<Call<GetKlinesRequest, RawKlinesResponse>> GetHistoryKlineCallAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
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
        _executor.SendAndParse(
            request,
            "Bittrade.GetTickers",
            BittradePublicEndpoints.GetTickers(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTickersResponse>(json, "Bittrade.GetTickers"));

    public Task<Call<GetTradeHistoryRequest, RawTradeHistoryResponse>> GetHistoryTradeCallAsync(
        GetTradeHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetTradeHistory",
            BittradePublicEndpoints.GetHistoryTrade(request.Symbol),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawTradeHistoryResponse>(json, "Bittrade.GetTradeHistory"));
}
