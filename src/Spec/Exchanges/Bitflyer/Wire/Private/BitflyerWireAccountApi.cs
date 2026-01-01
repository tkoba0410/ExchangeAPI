using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Transport;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal sealed class BitflyerWireAccountApi : IBitflyerWireAccountApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bitflyer;
    private readonly IRestClient _restClient;

    public BitflyerWireAccountApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<WireCall> GetBalancesAsync(
        CancellationToken cancellationToken = default)
        => GetAsync(BitflyerConstants.Paths.GetBalance, query: null, cancellationToken);

    public Task<WireCall> GetExecutionsAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            BitflyerConstants.Paths.GetPrivateExecutions,
            CreateExecutionsQuery(productCode, childOrderId, childOrderAcceptanceId, count, before, after),
            cancellationToken);

    public Task<WireCall> GetPositionsAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        GetAsync(BitflyerConstants.Paths.GetPositions, CreateProductCodeQuery(productCode), cancellationToken);

    public Task<WireCall> GetCollateralAsync(
        CancellationToken cancellationToken = default)
        => GetAsync(BitflyerConstants.Paths.GetCollateral, query: null, cancellationToken);

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
        GetAsync(
            BitflyerConstants.Paths.GetChildOrders,
            CreateChildOrdersQuery(
                productCode,
                childOrderStatusState,
                childOrderAcceptanceId,
                childOrderId,
                parentOrderId,
                count,
                before,
                after),
            cancellationToken);

    public Task<WireCall> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        GetAsync(BitflyerConstants.Paths.GetTradingCommission, CreateProductCodeQuery(productCode), cancellationToken);

    public Task<WireCall> GetPermissionsAsync(
        CancellationToken cancellationToken = default) =>
        GetAsync(BitflyerConstants.Paths.GetPermissions, query: null, cancellationToken);

    public Task<WireCall> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default) =>
        GetAsync(BitflyerConstants.Paths.GetCollateralAccounts, query: null, cancellationToken);

    public Task<WireCall> GetParentOrdersAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderStatusState = null,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            BitflyerConstants.Paths.GetParentOrders,
            CreateParentOrdersQuery(productCode, count, before, after, parentOrderStatusState),
            cancellationToken);

    public Task<WireCall> GetParentOrderAsync(
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            BitflyerConstants.Paths.GetParentOrder,
            CreateParentOrderQuery(parentOrderId, parentOrderAcceptanceId),
            cancellationToken);

    public Task<WireCall> GetBalanceHistoryAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            BitflyerConstants.Paths.GetBalanceHistory,
            CreateBalanceHistoryQuery(currencyCode, count, before, after),
            cancellationToken);

    public Task<WireCall> GetCollateralHistoryAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            BitflyerConstants.Paths.GetCollateralHistory,
            CreateHistoryQuery(count, before, after),
            cancellationToken);

    public Task<WireCall> GetAddressesAsync(
        CancellationToken cancellationToken = default) =>
        GetAsync(BitflyerConstants.Paths.GetAddresses, query: null, cancellationToken);

    public Task<WireCall> GetCoinInsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            BitflyerConstants.Paths.GetCoinIns,
            CreateHistoryQuery(count, before, after),
            cancellationToken);

    public Task<WireCall> GetCoinOutsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            BitflyerConstants.Paths.GetCoinOuts,
            CreateCoinOutsQuery(messageId, count, before, after),
            cancellationToken);

    public Task<WireCall> GetDepositsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            BitflyerConstants.Paths.GetDeposits,
            CreateHistoryQuery(count, before, after),
            cancellationToken);

    public Task<WireCall> GetWithdrawalsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        GetAsync(
            BitflyerConstants.Paths.GetWithdrawals,
            CreateCoinOutsQuery(messageId, count, before, after),
            cancellationToken);

    public Task<WireCall> GetBankAccountsAsync(
        CancellationToken cancellationToken = default) =>
        GetAsync(BitflyerConstants.Paths.GetBankAccounts, query: null, cancellationToken);

    private async Task<WireCall> GetAsync(
        string path,
        IReadOnlyDictionary<string, string?>? query,
        CancellationToken cancellationToken)
    {
        var request = new WireRequest(
            Method: "GET",
            Path: path,
            Query: BuildQuery(query));
        var meta = await _restClient.GetRawAsync(path, query, cancellationToken).ConfigureAwait(false);
        var response = ToWire(meta);
        return new WireCall(request, response, CreateMeta(response));
    }

    private static IReadOnlyDictionary<string, string?> CreateProductCodeQuery(RawProductCode productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        return new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode.Value,
        };
    }

    private static IReadOnlyDictionary<string, string?> CreateExecutionsQuery(
        RawProductCode productCode,
        string? childOrderId,
        string? childOrderAcceptanceId,
        int? count,
        long? before,
        long? after)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        return new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode.Value,
            [BitflyerConstants.QueryKeys.ChildOrderId] = childOrderId,
            [BitflyerConstants.QueryKeys.ChildOrderAcceptanceId] = childOrderAcceptanceId,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };
    }

    private static IReadOnlyDictionary<string, string?> CreateChildOrdersQuery(
        RawProductCode productCode,
        string? childOrderStatusState,
        string? childOrderAcceptanceId,
        string? childOrderId,
        string? parentOrderId,
        int? count,
        long? before,
        long? after)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        return new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode.Value,
            [BitflyerConstants.QueryKeys.ChildOrderStatusState] = childOrderStatusState,
            [BitflyerConstants.QueryKeys.ChildOrderAcceptanceId] = childOrderAcceptanceId,
            [BitflyerConstants.QueryKeys.ChildOrderId] = childOrderId,
            [BitflyerConstants.QueryKeys.ParentOrderId] = parentOrderId,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };
    }

    private static IReadOnlyDictionary<string, string?> CreateParentOrdersQuery(
        RawProductCode productCode,
        int? count,
        long? before,
        long? after,
        string? parentOrderStatusState)
    {
        if (string.IsNullOrWhiteSpace(productCode.Value))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        return new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.ProductCode] = productCode.Value,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
            [BitflyerConstants.QueryKeys.ParentOrderStatusState] = parentOrderStatusState,
        };
    }

    private static IReadOnlyDictionary<string, string?> CreateParentOrderQuery(
        string? parentOrderId,
        string? parentOrderAcceptanceId) =>
        new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.ParentOrderId] = parentOrderId,
            [BitflyerConstants.QueryKeys.ParentOrderAcceptanceId] = parentOrderAcceptanceId,
        };

    private static IReadOnlyDictionary<string, string?> CreateBalanceHistoryQuery(
        string? currencyCode,
        int? count,
        long? before,
        long? after) =>
        new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.CurrencyCode] = currencyCode,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };

    private static IReadOnlyDictionary<string, string?> CreateHistoryQuery(
        int? count,
        long? before,
        long? after) =>
        new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };

    private static IReadOnlyDictionary<string, string?> CreateCoinOutsQuery(
        string? messageId,
        int? count,
        long? before,
        long? after) =>
        new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [BitflyerConstants.QueryKeys.MessageId] = messageId,
            [BitflyerConstants.QueryKeys.Count] = count?.ToString(),
            [BitflyerConstants.QueryKeys.Before] = before?.ToString(),
            [BitflyerConstants.QueryKeys.After] = after?.ToString(),
        };

    private static WireResponse ToWire(HttpResponseMeta meta)
    {
        var headers = meta.Headers is null
            ? null
            : new Dictionary<string, string>(meta.Headers, StringComparer.OrdinalIgnoreCase);
        return new WireResponse(
            Exchange,
            meta.StatusCode,
            meta.Body ?? string.Empty,
            headers);
    }

    private static CallMeta CreateMeta(WireResponse response)
    {
        var elapsed = response.ElapsedMs is { } ms ? TimeSpan.FromMilliseconds(ms) : TimeSpan.Zero;
        var startedAt = DateTimeOffset.UtcNow - elapsed;
        return new CallMeta(startedAt, elapsed, response.RequestId);
    }

    private static string? BuildQuery(IReadOnlyDictionary<string, string?>? query)
    {
        if (query is null || query.Count == 0)
        {
            return null;
        }

        var parts = new List<string>();
        foreach (var (key, value) in query)
        {
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            parts.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
        }

        return parts.Count == 0 ? null : string.Join("&", parts);
    }
}
