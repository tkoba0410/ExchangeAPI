using ExchangeApi.Common.Types;

namespace ExchangeApi.Core.Contracts.Errors;

/// <summary>
/// クライアントがサポートしていないシンボルが指定されたときの例外。
/// </summary>
public class SymbolNotSupportedException : ExchangeApiException
{
    public Symbol Symbol { get; }

    public SymbolNotSupportedException(Symbol symbol)
        : base($"Symbol is not supported: '{symbol}'.")
    {
        Symbol = symbol;
    }

    public SymbolNotSupportedException(Symbol symbol, Exception innerException)
        : base($"Symbol is not supported: '{symbol}'.", innerException)
    {
        Symbol = symbol;
    }
}
