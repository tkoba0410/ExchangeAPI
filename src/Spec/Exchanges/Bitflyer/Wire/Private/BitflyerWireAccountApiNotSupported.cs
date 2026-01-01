using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal sealed class BitflyerWireAccountApiNotSupported : IBitflyerWireAccountApi
{
    private static NotSupportedException NotSupported() =>
        new("Bitflyer wire account is not supported.");

    public Task<WireCall> GetBalancesAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetExecutionsAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetPositionsAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetCollateralAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetChildOrdersAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetPermissionsAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetParentOrdersAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderStatusState = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetParentOrderAsync(
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetBalanceHistoryAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetCollateralHistoryAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetAddressesAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetCoinInsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetCoinOutsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetDepositsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetWithdrawalsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireCall> GetBankAccountsAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();
}
