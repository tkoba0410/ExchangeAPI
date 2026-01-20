using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Application.Errors;
using ExchangeApi.Application.Interfaces;
using ExchangeApi.Application.Trading;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Composition.Adapters.Application;

public sealed class TradingApiOrderQueryAdapter : IOrderQueryApi
{
    private readonly ITradingApi _tradingApi;

    public TradingApiOrderQueryAdapter(ITradingApi tradingApi)
    {
        _tradingApi = tradingApi ?? throw new ArgumentNullException(nameof(tradingApi));
    }

    public async Task<Call<GetOrderQuery, OrderStatusSnapshot>> GetOrderCallAsync(
        GetOrderQuery request,
        CancellationToken cancellationToken = default)
    {
        var call = await _tradingApi.GetOrderCallAsync(
            request.Symbol,
            request.OrderKey,
            cancellationToken).ConfigureAwait(false);

        return MapCall(request, call);
    }

    private static Call<GetOrderQuery, OrderStatusSnapshot> MapCall(
        GetOrderQuery request,
        Call<Contracts.Facade.Requests.GetOrderRequest, OrderStatus> call)
    {
        return call.Result switch
        {
            CallResult<OrderStatus>.Ok ok => Ok(request, call, ok.Response),
            CallResult<OrderStatus>.Err err => Err(request, call, err.Error),
            _ => Unknown(request, call)
        };
    }

    private static Call<GetOrderQuery, OrderStatusSnapshot> Ok(
        GetOrderQuery request,
        Call<Contracts.Facade.Requests.GetOrderRequest, OrderStatus> call,
        OrderStatus status)
    {
        var meta = new CallMeta(
            Layer: "Composition",
            Component: "TradingApiOrderQueryAdapter",
            EndpointId: call.Meta.EndpointId,
            Tags: null,
            Children: new[] { call.Id });

        var mapped = new OrderStatusSnapshot(
            ProductCode: status.ProductCode,
            Key: status.Key,
            Status: status.Status,
            ExecutedSize: status.ExecutedSize,
            OutstandingSize: status.OutstandingSize,
            Price: status.Price,
            AveragePrice: status.AveragePrice);

        return new Call<GetOrderQuery, OrderStatusSnapshot>(
            Id: CallId.New(),
            StartedAt: call.StartedAt,
            Duration: call.Duration,
            Request: request,
            Result: new CallResult<OrderStatusSnapshot>.Ok(mapped),
            Meta: meta);
    }

    private static Call<GetOrderQuery, OrderStatusSnapshot> Err(
        GetOrderQuery request,
        Call<Contracts.Facade.Requests.GetOrderRequest, OrderStatus> call,
        CallError error)
    {
        var meta = new CallMeta(
            Layer: "Composition",
            Component: "TradingApiOrderQueryAdapter",
            EndpointId: call.Meta.EndpointId,
            Tags: null,
            Children: new[] { call.Id });

        var mappedError = MapError(error);
        return new Call<GetOrderQuery, OrderStatusSnapshot>(
            Id: CallId.New(),
            StartedAt: call.StartedAt,
            Duration: call.Duration,
            Request: request,
            Result: new CallResult<OrderStatusSnapshot>.Err(mappedError),
            Meta: meta);
    }

    private static Call<GetOrderQuery, OrderStatusSnapshot> Unknown(
        GetOrderQuery request,
        Call<Contracts.Facade.Requests.GetOrderRequest, OrderStatus> call)
    {
        var meta = new CallMeta(
            Layer: "Composition",
            Component: "TradingApiOrderQueryAdapter",
            EndpointId: call.Meta.EndpointId,
            Tags: null,
            Children: new[] { call.Id });

        var error = new CallError(CallErrorKind.Unknown, "Facade call returned unknown result.");
        return new Call<GetOrderQuery, OrderStatusSnapshot>(
            Id: CallId.New(),
            StartedAt: call.StartedAt,
            Duration: call.Duration,
            Request: request,
            Result: new CallResult<OrderStatusSnapshot>.Err(error),
            Meta: meta);
    }

    private static CallError MapError(CallError error)
    {
        return error;
    }
}
