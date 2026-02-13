using System;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record BatchError
{
    public BatchError(
        string exchange,
        string endpointId,
        BatchErrorKind errorKind,
        string message)
    {
        Exchange = string.IsNullOrWhiteSpace(exchange)
            ? throw new ArgumentException("Exchange is required.", nameof(exchange))
            : exchange;
        EndpointId = string.IsNullOrWhiteSpace(endpointId)
            ? throw new ArgumentException("EndpointId is required.", nameof(endpointId))
            : endpointId;
        ErrorKind = errorKind;
        Message = string.IsNullOrWhiteSpace(message)
            ? throw new ArgumentException("Message is required.", nameof(message))
            : message;
    }

    public string Exchange { get; }
    public string EndpointId { get; }
    public BatchErrorKind ErrorKind { get; }
    public string Message { get; }
}
