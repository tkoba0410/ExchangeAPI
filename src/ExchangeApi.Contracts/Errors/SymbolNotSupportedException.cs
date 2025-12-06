namespace ExchangeApi.Contracts.Errors;

/// <summary>
/// クライアントがサポートしていないシンボルが指定されたときの例外。
/// </summary>
public class SymbolNotSupportedException : ExchangeApiException
{
    public string Symbol { get; }

    public SymbolNotSupportedException(string symbol)
        : base($"Symbol is not supported: '{symbol}'.")
    {
        Symbol = symbol;
    }

    public SymbolNotSupportedException(string symbol, Exception innerException)
        : base($"Symbol is not supported: '{symbol}'.", innerException)
    {
        Symbol = symbol;
    }
}
