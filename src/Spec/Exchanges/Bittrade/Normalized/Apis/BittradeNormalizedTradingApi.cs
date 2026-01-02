using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Mappers;
using ExchangeApi.Exchanges.Bittrade.Wire.Types;
using ExchangeApi.Spec.CallCommon;
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

    public async Task<BittradeNormalizedCall<OrderResult, JsonElement>> PlaceOrderCallAsync(
        OrderRequest request,
        CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var apiSymbol = await ToApiSymbolAsync(request.Symbol, ct).ConfigureAwait(false);
        var rawRequest = BittradeTradingMapper.ToRaw(_accountId, apiSymbol, request);
        var rawCall = await _trading.CreateOrderCallAsync(rawRequest, ct).ConfigureAwait(false);
        var requestMeta = CreateRequest("Bittrade.PlaceOrder", new Dictionary<string, string?>
        {
            ["symbol"] = request.Symbol.ToString(),
            ["side"] = request.Side.ToString(),
            ["orderType"] = request.OrderType.ToString(),
            ["price"] = request.Price?.ToString(),
            ["size"] = request.Size.ToString(),
        });

        return CreateCall(rawCall, requestMeta, BittradeTradingMapper.ToOrderResult);
    }

    public async Task<BittradeNormalizedCall<CancelResult, JsonElement>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default)
    {
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        if (orderKey.Kind is not (OrderIdKind.ExchangeOrderId or OrderIdKind.AcceptanceId))
        {
            throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bittrade, $"CancelOrderBy{orderKey.Kind}");
        }

        var rawCall = await _trading.CancelOrderCallAsync(RawOrderId.From(orderKey.Value), ct).ConfigureAwait(false);
        var requestMeta = CreateRequest("Bittrade.CancelOrder", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.ToString(),
            ["orderKey"] = orderKey.ToString(),
        });

        return CreateCall(rawCall, requestMeta, _ => new CancelResult(true));
    }

    public async Task<BittradeNormalizedCall<IReadOnlyList<OpenOrder>, JsonElement>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default)
    {
        var apiSymbol = await ToApiSymbolAsync(symbol, ct).ConfigureAwait(false);
        var rawCall = await _trading.GetOpenOrdersCallAsync(RawSymbol.From(apiSymbol), _accountId, ct).ConfigureAwait(false);
        var requestMeta = CreateRequest("Bittrade.GetOpenOrders", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.ToString(),
            ["accountId"] = _accountId,
        });

        return CreateCall(rawCall, requestMeta, raw => BittradeTradingMapper.ToOpenOrders(symbol, raw));
    }

    public async Task<BittradeNormalizedCall<OrderStatus, JsonElement>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default)
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

        var rawCall = await _trading.GetOrderCallAsync(RawOrderId.From(orderKey.Value), ct).ConfigureAwait(false);
        var requestMeta = CreateRequest("Bittrade.GetOrder", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.ToString(),
            ["orderKey"] = orderKey.ToString(),
        });

        return CreateCall(rawCall, requestMeta, raw => BittradeTradingMapper.ToOrderStatus(market.ProductCode, raw, key));
    }

    private static BittradeNormalizedRequest CreateRequest(
        string operation,
        IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BittradeNormalizedCall<TOk, JsonElement> CreateCall<TRaw, TOk>(
        BittradeRawCall<TRaw, JsonElement> rawCall,
        BittradeNormalizedRequest request,
        Func<TRaw, TOk> mapper)
    {
        return rawCall.Result switch
        {
            Ok<TRaw, JsonElement> ok => new BittradeNormalizedCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(mapper(ok.Value), ok.StatusCode),
                rawCall.Meta),
            Err<TRaw, JsonElement> err => new BittradeNormalizedCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(err.Error, err.StatusCode),
                rawCall.Meta),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }

    private async Task<string> ToApiSymbolAsync(Symbol symbol, CancellationToken ct)
    {
        var market = await _markets.ResolveAsync(symbol, ct).ConfigureAwait(false);
        return market.ProductCode.Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant();
    }
}
