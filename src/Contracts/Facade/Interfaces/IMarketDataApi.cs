using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Contracts.Facade.Interfaces;

/// <summary>
/// 市場データ（REST）を取得するための抽象インターフェース。
/// </summary>
public interface IMarketDataApi
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
        string period,
        int? size = null,
        CancellationToken cancellationToken = default);

    Task<Call<GetTickersRequest, IReadOnlyList<Ticker>>> GetTickersCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>> GetHistoryTradeCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);
}
