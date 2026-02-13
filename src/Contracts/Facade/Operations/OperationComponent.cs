using System;

namespace ExchangeApi.Contracts.Facade.Operations;

public static class OperationComponent
{
    public static string WithExchange(string exchange, string contractOperation)
    {
        if (string.IsNullOrWhiteSpace(exchange))
        {
            throw new ArgumentException("Exchange must be non-empty.", nameof(exchange));
        }

        if (string.IsNullOrWhiteSpace(contractOperation))
        {
            throw new ArgumentException("Operation must be non-empty.", nameof(contractOperation));
        }

        return exchange + "." + contractOperation;
    }
}
