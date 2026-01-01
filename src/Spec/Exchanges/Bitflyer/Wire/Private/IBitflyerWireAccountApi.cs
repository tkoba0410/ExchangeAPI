using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

public interface IBitflyerWireAccountApi
{
    Task<WireCall> GetBalancesAsync(
        CancellationToken cancellationToken = default);

    Task<WireCall> GetExecutionsAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetPositionsAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetCollateralAsync(
        CancellationToken cancellationToken = default);

    Task<WireCall> GetChildOrdersAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetPermissionsAsync(
        CancellationToken cancellationToken = default);

    Task<WireCall> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default);

    Task<WireCall> GetParentOrdersAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderStatusState = null,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetParentOrderAsync(
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetBalanceHistoryAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetCollateralHistoryAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetAddressesAsync(
        CancellationToken cancellationToken = default);

    Task<WireCall> GetCoinInsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetCoinOutsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetDepositsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetWithdrawalsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetBankAccountsAsync(
        CancellationToken cancellationToken = default);
}
