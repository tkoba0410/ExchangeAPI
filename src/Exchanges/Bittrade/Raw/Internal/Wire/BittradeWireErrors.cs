using System;
using System.Globalization;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

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

        throw new InvalidOperationException(
            $"{operation}: status={status}, code={errorCode}, msg={message}");
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
        new InvalidOperationException($"{operation}: missing field {field}");

    public static Exception Unexpected(string operation, string field, string value) =>
        new InvalidOperationException($"{operation}: invalid {field}='{value}'");

    public static Exception UnexpectedMessage(string operation, string message) =>
        new InvalidOperationException($"{operation}: {message}");
}
