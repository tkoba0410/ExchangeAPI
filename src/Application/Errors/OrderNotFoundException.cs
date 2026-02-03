using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Application.Errors;

public sealed class OrderNotFoundException : Exception
{
    public string Operation { get; }
    public Symbol Symbol { get; }
    public OrderKey OrderKey { get; }

    public OrderNotFoundException(
        string operation,
        Symbol symbol,
        OrderKey orderKey,
        Exception? inner = null)
        : base($"Order not found: {operation} {symbol} {orderKey}", inner)
    {
        Operation = operation;
        Symbol = symbol;
        OrderKey = orderKey;
    }
}
