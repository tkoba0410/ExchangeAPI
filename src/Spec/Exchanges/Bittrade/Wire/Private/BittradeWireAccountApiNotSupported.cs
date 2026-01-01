using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

internal sealed class BittradeWireAccountApiNotSupported : IBittradeWireAccountApi
{
    private static NotSupportedException NotSupported() =>
        new("Bittrade wire account is not supported.");

    public Task<WireCall> GetAccountsAsync(CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetAccountBalanceAsync(string accountId, CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetOpenOrdersAsync(RawSymbol symbol, string accountId, CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetOrderMatchResultsAsync(RawOrderId orderId, CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetOrdersAsync(
        RawSymbol symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetMatchResultsAsync(
        RawSymbol? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetDepositWithdrawsAsync(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetRetailOrdersAsync(
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();
}
