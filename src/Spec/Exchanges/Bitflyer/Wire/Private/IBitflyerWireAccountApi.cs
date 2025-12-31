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
    Task<WireResponse> GetBalancesAsync(
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetExecutionsAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetPositionsAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetChildOrdersAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetPermissionsAsync(
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetParentOrdersAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderStatusState = null,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetParentOrderAsync(
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetBalanceHistoryAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetCollateralHistoryAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetAddressesAsync(
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetCoinInsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetCoinOutsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetDepositsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetWithdrawalsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default);

    Task<WireResponse> GetBankAccountsAsync(
        CancellationToken cancellationToken = default);
}
