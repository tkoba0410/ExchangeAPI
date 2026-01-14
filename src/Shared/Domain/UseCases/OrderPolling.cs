using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Shared.Domain.UseCases;

/// <summary>
/// 注文状態をポーリングするユースケース。
/// </summary>
public static class OrderPolling
{
    public static async Task<Call<GetOrderRequest, OrderStatus>> WaitForOrderAsync(
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

        Call<GetOrderRequest, OrderStatus>? latest = null;

        for (var attempt = 0; attempt < resolvedOptions.MaxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            latest = await api.GetOrderCallAsync(symbol, orderKey, cancellationToken).ConfigureAwait(false);
            if (latest.Result is CallResult<OrderStatus>.Err err)
            {
                if (err.Error.Exception is ExchangeOrderNotFoundException &&
                    resolvedOptions.NotFoundPolicy == NotFoundPolicy.Continue)
                {
                    if (attempt < resolvedOptions.MaxAttempts - 1 && resolvedOptions.Interval > TimeSpan.Zero)
                    {
                        await Task.Delay(resolvedOptions.Interval, cancellationToken).ConfigureAwait(false);
                    }

                    continue;
                }

                return latest;
            }

            if (latest.Result is CallResult<OrderStatus>.Ok ok &&
                IsTerminal(ok.Response.Status))
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
