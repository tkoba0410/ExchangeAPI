using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Common.Interfaces;

namespace ExchangeApi.Common.UseCases;

/// <summary>
/// 注文状態をポーリングするユースケース。
/// </summary>
public static class OrderPolling
{
    public static async Task<OrderStatus> WaitForOrderAsync(
        ITradingApi api,
        Symbol symbol,
        string orderId,
        PollingOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (api is null)
        {
            throw new ArgumentNullException(nameof(api));
        }

        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        var resolvedOptions = options ?? PollingOptions.Default;

        if (resolvedOptions.MaxAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "MaxAttempts must be greater than zero.");
        }

        if (resolvedOptions.Interval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Interval must be non-negative.");
        }

        OrderStatus? latest = null;

        for (var attempt = 0; attempt < resolvedOptions.MaxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            latest = await api.GetOrderAsync(symbol, orderId, cancellationToken).ConfigureAwait(false);

            if (IsTerminal(latest.Status))
            {
                return latest;
            }

            if (attempt < resolvedOptions.MaxAttempts - 1 && resolvedOptions.Interval > TimeSpan.Zero)
            {
                await Task.Delay(resolvedOptions.Interval, cancellationToken).ConfigureAwait(false);
            }
        }

        return latest ?? throw new InvalidOperationException("Polling did not return any order status.");
    }

    private static bool IsTerminal(OrderState status) =>
        status is OrderState.Completed or OrderState.Canceled or OrderState.Expired or OrderState.Rejected;
}

public sealed record PollingOptions(TimeSpan Interval, int MaxAttempts)
{
    public static PollingOptions Default { get; } = new(TimeSpan.FromSeconds(1), 30);
}
