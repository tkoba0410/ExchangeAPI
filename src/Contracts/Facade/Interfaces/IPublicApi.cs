using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Interfaces;

/// <summary>
/// Public API (no signature). Market data + exchange info.
/// </summary>
public interface IPublicApi
{
    Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetMarketExecutionsCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetHistoryKlineRequest, IReadOnlyList<Candlestick>>> GetHistoryKlineCallAsync(
        Symbol symbol,
        Period period,
        int? size = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetTickersRequest, IReadOnlyList<Ticker>>> GetTickersCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>> GetHistoryTradeCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetCurrencysRequest, IReadOnlyList<CurrencyCode>>> GetCurrencysCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default);
}
