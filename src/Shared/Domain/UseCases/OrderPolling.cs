using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Core.Contracts.Errors;

namespace ExchangeApi.Core.UseCases;

/// <summary>
/// 注文状態をポーリングするユースケース。
/// </summary>
public static class OrderPolling
{
    public static async Task<OrderStatus> WaitForOrderAsync(
        ITradingApi api,
        Symbol symbol,
        OrderKey orderKey,
        PollingOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (api is null)
        {
            throw new ArgumentNullException(nameof(api));
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

            try
            {
                latest = await api.GetOrderAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            }
            catch (ExchangeOrderNotFoundException) when (resolvedOptions.NotFoundPolicy == NotFoundPolicy.Continue)
            {
                if (attempt < resolvedOptions.MaxAttempts - 1 && resolvedOptions.Interval > TimeSpan.Zero)
                {
                    await Task.Delay(resolvedOptions.Interval, cancellationToken).ConfigureAwait(false);
                }

                continue;
            }

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
    public NotFoundPolicy NotFoundPolicy { get; init; } = NotFoundPolicy.Continue;

    public static PollingOptions Default { get; } = new(TimeSpan.FromSeconds(1), 30);
}

public enum NotFoundPolicy
{
    Continue,
    StopAsNotFound
}
