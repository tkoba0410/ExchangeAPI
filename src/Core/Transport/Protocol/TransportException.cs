using System;
using System.Net;

namespace ExchangeApi.Core.Transport.Protocol;

/// <summary>
/// Transport レイヤーで発生した通信・パース等の例外。
/// </summary>
public class TransportException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public string? ErrorCode { get; }
    public TransportErrorCategory? ErrorCategory { get; }

    public TransportException(
        string message,
        HttpStatusCode? statusCode = null,
        string? errorCode = null,
        TransportErrorCategory? errorCategory = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ErrorCategory = errorCategory;
    }
}
