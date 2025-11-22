using System;

namespace ExchangeApi.Abstractions
{
    /// <summary>
    /// Exchange API 呼び出し時のエラーを表す例外。
    /// Stage1 では最小限の情報のみ持つ。
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
}
