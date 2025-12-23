using System;
using System.Globalization;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;

namespace ExchangeApi.Exchanges.Bittrade.Wire;

internal static class BittradeWireErrors
{
    public static void RequireOk(
        string? status,
        string? errorCode,
        string? message,
        string operation)
    {
        if (status is null)
        {
            return;
        }

        if (string.Equals(status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw new ExchangeApiException(
            $"{operation}: status={status}, code={errorCode}, msg={message}",
            exchange: ExchangeCode.Bittrade,
            operation: operation,
            errorCategory: ExchangeErrorCategory.Request);
    }

    public static DateTimeOffset ParseTimestampOrThrow(string? value, string operation, string field)
    {
        if (value is null)
        {
            throw Unexpected(operation, field, "<missing>");
        }

        if (DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var dto))
        {
            return dto;
        }

        throw Unexpected(operation, field, value);
    }

    public static Exception Missing(string operation, string field) =>
        new ExchangeApiException(
            $"{operation}: missing field {field}",
            exchange: ExchangeCode.Bittrade,
            operation: operation,
            errorCategory: ExchangeErrorCategory.Unknown);

    public static Exception Unexpected(string operation, string field, string value) =>
        new ExchangeApiException(
            $"{operation}: invalid {field}='{value}'",
            exchange: ExchangeCode.Bittrade,
            operation: operation,
            errorCategory: ExchangeErrorCategory.Unknown);

    public static Exception UnexpectedMessage(string operation, string message) =>
        new ExchangeApiException(
            $"{operation}: {message}",
            exchange: ExchangeCode.Bittrade,
            operation: operation,
            errorCategory: ExchangeErrorCategory.Unknown);
}
