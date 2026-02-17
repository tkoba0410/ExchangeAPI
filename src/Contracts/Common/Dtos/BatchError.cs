using System;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record BatchError
{
    public BatchError(
        string endpointId,
        BatchErrorKind errorKind,
        string message)
    {
        EndpointId = string.IsNullOrWhiteSpace(endpointId)
            ? throw new ArgumentException("EndpointId is required.", nameof(endpointId))
            : endpointId;
        ErrorKind = errorKind;
        Message = string.IsNullOrWhiteSpace(message)
            ? throw new ArgumentException("Message is required.", nameof(message))
            : message;
    }

    public string EndpointId { get; }
    public BatchErrorKind ErrorKind { get; }
    public string Message { get; }
}
