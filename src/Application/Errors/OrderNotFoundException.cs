using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Application.Errors;

public sealed class OrderNotFoundException : Exception
{
    public ExchangeCode Exchange { get; }
    public string Operation { get; }
    public Symbol Symbol { get; }
    public OrderKey OrderKey { get; }

    public OrderNotFoundException(
        ExchangeCode exchange,
        string operation,
        Symbol symbol,
        OrderKey orderKey,
        Exception? inner = null)
        : base($"Order not found: {exchange} {operation} {symbol} {orderKey}", inner)
    {
        Exchange = exchange;
        Operation = operation;
        Symbol = symbol;
        OrderKey = orderKey;
    }
}
