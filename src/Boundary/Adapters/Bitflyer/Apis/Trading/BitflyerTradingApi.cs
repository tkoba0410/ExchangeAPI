using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ContractSide = ExchangeApi.Common.Enums.Side;
using ExchangeApi.Exchanges.Bitflyer.Adapter;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Trading;

/// <summary>
/// bitFlyer の Trading API 実装（REST）。
/// </summary>
internal sealed class BitflyerTradingApi : ITradingApi
{
    private readonly IBitflyerNormalizedTradingApi _tradingApi;
    private readonly ExchangeCode _exchange;

    public BitflyerTradingApi(
        IBitflyerNormalizedTradingApi tradingApi,
        ExchangeCode exchange = ExchangeCode.Bitflyer)
    {
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
        _exchange = exchange;
    }

    public Task<OrderResult> PlaceLimitOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Limit(symbol, side, size, price), cancellationToken);

    public Task<OrderResult> PlaceMarketOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        CancellationToken cancellationToken = default) =>
        PlaceOrderInternal(OrderRequest.Market(symbol, side, size), cancellationToken);

    public Task<OrderResult> PlaceStopOrderAsync(
        Symbol symbol,
        ContractSide side,
        Size size,
        Price triggerPrice,
        CancellationToken cancellationToken = default) =>
        throw new ExchangeFeatureNotSupportedException(ExchangeCode.Bitflyer, "StopOrder");

    private async Task<OrderResult> PlaceOrderInternal(
        OrderRequest request,
        CancellationToken cancellationToken)
    {
        var operation = BitflyerOperations.Trading.PlaceOrder;
        try
        {
            return await _tradingApi.PlaceOrderAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer sendchildorder API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<CancelResult> CancelOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Trading.CancelOrder;
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            return await _tradingApi.CancelOrderAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer cancelchildorder API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Trading.GetOpenOrders;
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            return await _tradingApi.GetOpenOrdersAsync(symbol, cancellationToken).ConfigureAwait(false);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getchildorders API.",
                exchange: _exchange,
                operation: operation,
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<OrderStatus> GetOrderAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var operation = BitflyerOperations.Trading.GetOrder;
        if (symbol.IsEmpty)
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }

        try
        {
            return await _tradingApi.GetOrderAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
        }
        catch (SymbolNotSupportedException)
        {
            throw;
        }
        catch (TransportException ex)
        {
            throw BitflyerErrorMapper.FromTransportException(ex, _exchange, operation);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchange, operation);
        }
    }

}
