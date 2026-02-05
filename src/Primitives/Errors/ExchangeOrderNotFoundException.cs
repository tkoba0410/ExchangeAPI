namespace ExchangeApi.Primitives.Errors;

public sealed class ExchangeOrderNotFoundException : ExchangeApiException
{
    public string SymbolValue { get; }
    public string OrderKeyValue { get; }

    public ExchangeOrderNotFoundException(string operation, string symbolValue, string orderKeyValue)
        : base($"Order not found: {orderKeyValue}.", operation: operation)
    {
        SymbolValue = symbolValue;
        OrderKeyValue = orderKeyValue;
    }
}
