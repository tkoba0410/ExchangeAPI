using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Raw;
using ExchangeApi.Transport.Protocol;

namespace Exchange.Bitflyer.Raw;

/// <summary>
/// bitFlyer Private REST API（情報系）の実装。
/// </summary>
public sealed class BitflyerPrivateApi : IBitflyerPrivateApi
{
    private readonly IRestClient _restClient;

    public BitflyerPrivateApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<IReadOnlyList<string>> GetPermissionsAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetPermissions;
        return _restClient.GetAsync<IReadOnlyList<string>>(path, query: null, cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetBalance;

        return _restClient.GetAsync<IReadOnlyList<BitflyerBalanceResponse>>(
            path,
            query: null,
            cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerPositionResponse>> GetPositionsAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = BitflyerConstants.Paths.GetPositions;
        var query = new Dictionary<string, string?>
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode,
        };

        return _restClient.GetAsync<IReadOnlyList<BitflyerPositionResponse>>(
            path,
            query,
            cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerExecutionPrivateResponse>> GetExecutionsAsync(
        string productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = BitflyerConstants.Paths.GetPrivateExecutions;
        var query = new Dictionary<string, string?>
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode,
            [BitflyerConstants.QueryKeys.ChildOrderId] = childOrderId,
            [BitflyerConstants.QueryKeys.ChildOrderAcceptanceId] = childOrderAcceptanceId,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };

        return _restClient.GetAsync<IReadOnlyList<BitflyerExecutionPrivateResponse>>(
            path,
            query,
            cancellationToken);
    }

    public Task<BitflyerCollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetCollateral;

        return _restClient.GetAsync<BitflyerCollateralResponse>(
            path,
            query: null,
            cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerCollateralAccount>> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetCollateralAccounts;
        return _restClient.GetAsync<IReadOnlyList<BitflyerCollateralAccount>>(path, query: null, cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerChildOrderResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = BitflyerConstants.Paths.GetChildOrders;
        var query = new Dictionary<string, string?>
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode,
            [BitflyerConstants.QueryKeys.ChildOrderState] = childOrderState,
            [BitflyerConstants.QueryKeys.ChildOrderAcceptanceId] = childOrderAcceptanceId,
            [BitflyerConstants.QueryKeys.ChildOrderId] = childOrderId,
            [BitflyerConstants.QueryKeys.ParentOrderId] = parentOrderId,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };

        return _restClient.GetAsync<IReadOnlyList<BitflyerChildOrderResponse>>(
            path,
            query,
            cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerParentOrderResponse>> GetParentOrdersAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderState = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = BitflyerConstants.Paths.GetParentOrders;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
            [BitflyerConstants.QueryKeys.ParentOrderState] = parentOrderState,
        };
        return _restClient.GetAsync<IReadOnlyList<BitflyerParentOrderResponse>>(path, query, cancellationToken);
    }

    public Task<BitflyerParentOrderDetailResponse> GetParentOrderAsync(
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetParentOrder;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.ParentOrderId] = parentOrderId,
            [BitflyerConstants.QueryKeys.ParentOrderAcceptanceId] = parentOrderAcceptanceId,
        };
        return _restClient.GetAsync<BitflyerParentOrderDetailResponse>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetBalanceHistoryAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetBalanceHistory;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.CurrencyCode] = currencyCode,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };

        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetCollateralHistoryAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetCollateralHistory;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query, cancellationToken);
    }

    public Task<JsonElement> GetTradingCommissionAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        const string path = BitflyerConstants.Paths.GetTradingCommission;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode,
        };
        return _restClient.GetAsync<JsonElement>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetAddressesAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetAddresses;
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query: null, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetCoinInsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetCoinIns;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetCoinOutsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetCoinOuts;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.MessageId] = messageId,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetDepositsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetDeposits;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetWithdrawalsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetWithdrawals;
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.MessageId] = messageId,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetBankAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = BitflyerConstants.Paths.GetBankAccounts;
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query: null, cancellationToken);
    }
}
