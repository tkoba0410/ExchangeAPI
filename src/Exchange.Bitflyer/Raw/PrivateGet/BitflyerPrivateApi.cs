using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Adapter.Bitflyer;

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
        const string path = "/v1/me/getpermissions";
        return _restClient.GetAsync<IReadOnlyList<string>>(path, query: null, cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getbalance";

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

        const string path = "/v1/me/getpositions";
        var query = new Dictionary<string, string?>
        {
            ["product_code"] = productCode,
        };

        return _restClient.GetAsync<IReadOnlyList<BitflyerPositionResponse>>(
            path,
            query,
            cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerExecutionResponse>> GetExecutionsAsync(
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

        const string path = "/v1/me/getexecutions";
        var query = new Dictionary<string, string?>
        {
            ["product_code"] = productCode,
            ["child_order_id"] = childOrderId,
            ["child_order_acceptance_id"] = childOrderAcceptanceId,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        };

        return _restClient.GetAsync<IReadOnlyList<BitflyerExecutionResponse>>(
            path,
            query,
            cancellationToken);
    }

    public Task<BitflyerCollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getcollateral";

        return _restClient.GetAsync<BitflyerCollateralResponse>(
            path,
            query: null,
            cancellationToken);
    }

    public Task<IReadOnlyList<BitflyerCollateralAccount>> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getcollateralaccounts";
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

        const string path = "/v1/me/getchildorders";
        var query = new Dictionary<string, string?>
        {
            ["product_code"] = productCode,
            ["child_order_state"] = childOrderState,
            ["child_order_acceptance_id"] = childOrderAcceptanceId,
            ["child_order_id"] = childOrderId,
            ["parent_order_id"] = parentOrderId,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        };

        return _restClient.GetAsync<IReadOnlyList<BitflyerChildOrderResponse>>(
            path,
            query,
            cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetParentOrdersAsync(
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

        const string path = "/v1/me/getparentorders";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["product_code"] = productCode,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
            ["parent_order_state"] = parentOrderState,
        };
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query, cancellationToken);
    }

    public Task<JsonElement> GetParentOrderAsync(
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getparentorder";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["parent_order_id"] = parentOrderId,
            ["parent_order_acceptance_id"] = parentOrderAcceptanceId,
        };
        return _restClient.GetAsync<JsonElement>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetBalanceHistoryAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getbalancehistory";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["currency_code"] = currencyCode,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        };

        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetCollateralHistoryAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getcollateralhistory";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
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

        const string path = "/v1/me/gettradingcommission";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["product_code"] = productCode,
        };
        return _restClient.GetAsync<JsonElement>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetAddressesAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getaddresses";
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query: null, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetCoinInsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getcoinins";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
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
        const string path = "/v1/me/getcoinouts";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["message_id"] = messageId,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        };
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetDepositsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getdeposits";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
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
        const string path = "/v1/me/getwithdrawals";
        var query = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["message_id"] = messageId,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        };
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query, cancellationToken);
    }

    public Task<IReadOnlyList<JsonElement>> GetBankAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        const string path = "/v1/me/getbankaccounts";
        return _restClient.GetAsync<IReadOnlyList<JsonElement>>(path, query: null, cancellationToken);
    }
}
