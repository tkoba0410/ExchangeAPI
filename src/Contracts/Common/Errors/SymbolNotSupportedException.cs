namespace ExchangeApi.Contracts.Common.Errors;

/// <summary>
/// クライアントがサポートしていないシンボルが指定されたときの例外。
/// </summary>
public class SymbolNotSupportedException : ExchangeApiException
{
    public string SymbolValue { get; }

    public SymbolNotSupportedException(string symbolValue)
        : base($"Symbol is not supported: '{symbolValue}'.")
    {
        SymbolValue = symbolValue;
    }

    public SymbolNotSupportedException(string symbolValue, Exception innerException)
        : base($"Symbol is not supported: '{symbolValue}'.", innerException)
    {
        SymbolValue = symbolValue;
    }
}
