using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Normalize.Mappers;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal sealed class BittradeNormalizedTradingApi : IBittradeNormalizedTradingApi
{
    private readonly IBittradeRawTradingApi _trading;
    private readonly IExchangeMarketResolver _markets;
    private readonly string _accountId;

    public BittradeNormalizedTradingApi(
        IBittradeRawTradingApi trading,
        IExchangeMarketResolver markets,
        string accountId)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
        _markets = markets ?? throw new ArgumentNullException(nameof(markets));
        _accountId = string.IsNullOrWhiteSpace(accountId)
            ? throw new ArgumentException("accountId is required.", nameof(accountId))
            : accountId;
    }

    public async Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        var apiSymbol = await ToApiSymbolAsync(request.Symbol, ct).ConfigureAwait(false);
        var rawRequest = BittradeTradingMapper.ToRaw(_accountId, apiSymbol, request);
        var raw = await _trading.CreateOrderAsync(rawRequest, ct).ConfigureAwait(false);
        return BittradeTradingMapper.ToOrderResult(raw);
    }

    public async Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken ct = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        if (orderKey.Kind is not (OrderIdKind.ExchangeOrderId or OrderIdKind.AcceptanceId))
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, $"CancelOrderBy{orderKey.Kind}");
        }

        await _trading.CancelOrderAsync(RawOrderId.From(orderKey.Value), ct).ConfigureAwait(false);
        return new CancelResult(true);
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(Symbol symbol, CancellationToken ct = default)
    {
        var apiSymbol = await ToApiSymbolAsync(symbol, ct).ConfigureAwait(false);
        var raw = await _trading.GetOpenOrdersAsync(RawSymbol.From(apiSymbol), _accountId, ct).ConfigureAwait(false);
        return BittradeTradingMapper.ToOpenOrders(symbol, raw);
    }

    public async Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken ct = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        if (orderKey.Kind is not (OrderIdKind.ExchangeOrderId or OrderIdKind.AcceptanceId))
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, $"GetOrderBy{orderKey.Kind}");
        }

        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        var key = orderKey.Kind == OrderIdKind.AcceptanceId
            ? new OrderKey(OrderIdKind.AcceptanceId, orderKey.Value)
            : new OrderKey(OrderIdKind.ExchangeOrderId, orderKey.Value);

        var raw = await _trading.GetOrderAsync(RawOrderId.From(orderKey.Value), ct).ConfigureAwait(false);
        return BittradeTradingMapper.ToOrderStatus(market.ProductCode, raw, key);
    }

    private async Task<string> ToApiSymbolAsync(Symbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode.Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant();
    }
}
