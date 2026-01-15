using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Contracts.Common.Errors;

public sealed class ExchangeOrderNotFoundException : ExchangeApiException
{
    public ExchangeCode ExchangeCode { get; }
    public string SymbolValue { get; }
    public string OrderKeyValue { get; }

    public ExchangeOrderNotFoundException(ExchangeCode exchange, string operation, string symbolValue, string orderKeyValue)
        : base($"Order not found: {orderKeyValue}.", exchange: exchange, operation: operation)
    {
        ExchangeCode = exchange;
        SymbolValue = symbolValue;
        OrderKeyValue = orderKeyValue;
    }
}
