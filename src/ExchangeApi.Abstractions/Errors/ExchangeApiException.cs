namespace ExchangeApi.Abstractions.Errors;

/// <summary>
/// Exchange API 関連のエラーの基本クラス。
/// </summary>
public class ExchangeApiException : Exception
{
    public ExchangeApiException()
    {
    }

    public ExchangeApiException(string message)
        : base(message)
    {
    }

    public ExchangeApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
