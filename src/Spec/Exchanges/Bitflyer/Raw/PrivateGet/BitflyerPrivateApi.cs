using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;

/// <summary>
/// bitFlyer Private REST API（情報系）の実装。
/// </summary>
public sealed class BitflyerPrivateApi : IBitflyerPrivateApi
{
    private readonly IWireTransport _wire;

    public BitflyerPrivateApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<IReadOnlyList<string>> GetPermissionsAsync(
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BitflyerEndpoints.GetPermissions(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<string>>(
                response.Json,
                "Bitflyer.GetPermissions");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetPermissions", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BitflyerEndpoints.GetBalances(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<BalanceResponse>>(
                response.Json,
                "Bitflyer.GetBalances");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetBalances", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BitflyerEndpoints.GetPositions(productCode), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<PositionResponse>>(
                response.Json,
                "Bitflyer.GetPositions");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetPositions", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(
                BitflyerEndpoints.GetExecutions(
                    productCode,
                    childOrderId,
                    childOrderAcceptanceId,
                    count,
                    before,
                    after),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ExecutionPrivateResponse>>(
                response.Json,
                "Bitflyer.GetExecutions");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetExecutions", response.StatusCode, response.Json);
    }

    public async Task<CollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BitflyerEndpoints.GetCollateral(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<CollateralResponse>(
                response.Json,
                "Bitflyer.GetCollateral");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetCollateral", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<CollateralAccount>> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BitflyerEndpoints.GetCollateralAccounts(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<CollateralAccount>>(
                response.Json,
                "Bitflyer.GetCollateralAccounts");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.GetCollateralAccounts",
            response.StatusCode,
            response.Json);
    }

    public async Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(
                BitflyerEndpoints.GetChildOrders(
                    productCode,
                    childOrderStatusState,
                    childOrderAcceptanceId,
                    childOrderId,
                    parentOrderId,
                    count,
                    before,
                    after),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ChildOrderResponse>>(
                response.Json,
                "Bitflyer.GetChildOrders");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetChildOrders", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<ParentOrderResponse>> GetParentOrdersAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderStatusState = null,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(
                BitflyerEndpoints.GetParentOrders(
                    productCode,
                    count,
                    before,
                    after,
                    parentOrderStatusState),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ParentOrderResponse>>(
                response.Json,
                "Bitflyer.GetParentOrders");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetParentOrders", response.StatusCode, response.Json);
    }

    public async Task<ParentOrderDetailResponse> GetParentOrderAsync(
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(
                BitflyerEndpoints.GetParentOrder(parentOrderId, parentOrderAcceptanceId),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<ParentOrderDetailResponse>(
                response.Json,
                "Bitflyer.GetParentOrder");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetParentOrder", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetBalanceHistoryAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(
                BitflyerEndpoints.GetBalanceHistory(currencyCode, count, before, after),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetBalanceHistory");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.GetBalanceHistory",
            response.StatusCode,
            response.Json);
    }

    public async Task<JsonElement> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BitflyerEndpoints.GetTradingCommission(productCode), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<JsonElement>(
                response.Json,
                "Bitflyer.GetTradingCommission");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.GetTradingCommission",
            response.StatusCode,
            response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetCollateralHistoryAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(
                BitflyerEndpoints.GetCollateralHistory(count, before, after),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetCollateralHistory");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.GetCollateralHistory",
            response.StatusCode,
            response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetAddressesAsync(
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BitflyerEndpoints.GetAddresses(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetAddresses");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetAddresses", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetCoinInsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BitflyerEndpoints.GetCoinIns(count, before, after), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetCoinIns");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetCoinIns", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetCoinOutsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(
                BitflyerEndpoints.GetCoinOuts(messageId, count, before, after),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetCoinOuts");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetCoinOuts", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetDepositsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(
                BitflyerEndpoints.GetDeposits(count, before, after),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetDeposits");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetDeposits", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetWithdrawalsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(
                BitflyerEndpoints.GetWithdrawals(messageId, count, before, after),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetWithdrawals");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetWithdrawals", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetBankAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BitflyerEndpoints.GetBankAccounts(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetBankAccounts");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetBankAccounts", response.StatusCode, response.Json);
    }

    public async Task<BitflyerRawCall<IReadOnlyList<string>, JsonElement>> GetPermissionsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BitflyerEndpoints.GetPermissions(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetPermissions", new Dictionary<string, string?>());
        return CreateCall<IReadOnlyList<string>>(request, wireCall, "Bitflyer.GetPermissions");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<BalanceResponse>, JsonElement>> GetBalancesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BitflyerEndpoints.GetBalances(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetBalances", new Dictionary<string, string?>());
        return CreateCall<IReadOnlyList<BalanceResponse>>(request, wireCall, "Bitflyer.GetBalances");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<PositionResponse>, JsonElement>> GetPositionsCallAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BitflyerEndpoints.GetPositions(productCode), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetPositions", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
        });
        return CreateCall<IReadOnlyList<PositionResponse>>(request, wireCall, "Bitflyer.GetPositions");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<ExecutionPrivateResponse>, JsonElement>> GetExecutionsCallAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BitflyerEndpoints.GetExecutions(
                    productCode,
                    childOrderId,
                    childOrderAcceptanceId,
                    count,
                    before,
                    after),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetExecutions", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
            ["childOrderId"] = childOrderId,
            ["childOrderAcceptanceId"] = childOrderAcceptanceId,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        });
        return CreateCall<IReadOnlyList<ExecutionPrivateResponse>>(request, wireCall, "Bitflyer.GetExecutions");
    }

    public async Task<BitflyerRawCall<CollateralResponse, JsonElement>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BitflyerEndpoints.GetCollateral(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetCollateral", new Dictionary<string, string?>());
        return CreateCall<CollateralResponse>(request, wireCall, "Bitflyer.GetCollateral");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<CollateralAccount>, JsonElement>> GetCollateralAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BitflyerEndpoints.GetCollateralAccounts(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetCollateralAccounts", new Dictionary<string, string?>());
        return CreateCall<IReadOnlyList<CollateralAccount>>(request, wireCall, "Bitflyer.GetCollateralAccounts");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<ChildOrderResponse>, JsonElement>> GetChildOrdersCallAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BitflyerEndpoints.GetChildOrders(
                    productCode,
                    childOrderStatusState,
                    childOrderAcceptanceId,
                    childOrderId,
                    parentOrderId,
                    count,
                    before,
                    after),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetChildOrders", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
            ["childOrderStatusState"] = childOrderStatusState,
            ["childOrderAcceptanceId"] = childOrderAcceptanceId,
            ["childOrderId"] = childOrderId,
            ["parentOrderId"] = parentOrderId,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        });
        return CreateCall<IReadOnlyList<ChildOrderResponse>>(request, wireCall, "Bitflyer.GetChildOrders");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<ParentOrderResponse>, JsonElement>> GetParentOrdersCallAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderStatusState = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BitflyerEndpoints.GetParentOrders(
                    productCode,
                    count,
                    before,
                    after,
                    parentOrderStatusState),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetParentOrders", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
            ["parentOrderStatusState"] = parentOrderStatusState,
        });
        return CreateCall<IReadOnlyList<ParentOrderResponse>>(request, wireCall, "Bitflyer.GetParentOrders");
    }

    public async Task<BitflyerRawCall<ParentOrderDetailResponse, JsonElement>> GetParentOrderCallAsync(
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BitflyerEndpoints.GetParentOrder(parentOrderId, parentOrderAcceptanceId),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetParentOrder", new Dictionary<string, string?>
        {
            ["parentOrderId"] = parentOrderId,
            ["parentOrderAcceptanceId"] = parentOrderAcceptanceId,
        });
        return CreateCall<ParentOrderDetailResponse>(request, wireCall, "Bitflyer.GetParentOrder");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<JsonElement>, JsonElement>> GetBalanceHistoryCallAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BitflyerEndpoints.GetBalanceHistory(currencyCode, count, before, after),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetBalanceHistory", new Dictionary<string, string?>
        {
            ["currencyCode"] = currencyCode,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        });
        return CreateCall<IReadOnlyList<JsonElement>>(request, wireCall, "Bitflyer.GetBalanceHistory");
    }

    public async Task<BitflyerRawCall<JsonElement, JsonElement>> GetTradingCommissionCallAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BitflyerEndpoints.GetTradingCommission(productCode), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetTradingCommission", new Dictionary<string, string?>
        {
            ["productCode"] = productCode.Value,
        });
        return CreateCall<JsonElement>(request, wireCall, "Bitflyer.GetTradingCommission");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<JsonElement>, JsonElement>> GetCollateralHistoryCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BitflyerEndpoints.GetCollateralHistory(count, before, after),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetCollateralHistory", new Dictionary<string, string?>
        {
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        });
        return CreateCall<IReadOnlyList<JsonElement>>(request, wireCall, "Bitflyer.GetCollateralHistory");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<JsonElement>, JsonElement>> GetAddressesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BitflyerEndpoints.GetAddresses(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetAddresses", new Dictionary<string, string?>());
        return CreateCall<IReadOnlyList<JsonElement>>(request, wireCall, "Bitflyer.GetAddresses");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<JsonElement>, JsonElement>> GetCoinInsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BitflyerEndpoints.GetCoinIns(count, before, after),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetCoinIns", new Dictionary<string, string?>
        {
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        });
        return CreateCall<IReadOnlyList<JsonElement>>(request, wireCall, "Bitflyer.GetCoinIns");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<JsonElement>, JsonElement>> GetCoinOutsCallAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BitflyerEndpoints.GetCoinOuts(messageId, count, before, after),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetCoinOuts", new Dictionary<string, string?>
        {
            ["messageId"] = messageId,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        });
        return CreateCall<IReadOnlyList<JsonElement>>(request, wireCall, "Bitflyer.GetCoinOuts");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<JsonElement>, JsonElement>> GetDepositsCallAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BitflyerEndpoints.GetDeposits(count, before, after),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetDeposits", new Dictionary<string, string?>
        {
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        });
        return CreateCall<IReadOnlyList<JsonElement>>(request, wireCall, "Bitflyer.GetDeposits");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<JsonElement>, JsonElement>> GetWithdrawalsCallAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BitflyerEndpoints.GetWithdrawals(messageId, count, before, after),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetWithdrawals", new Dictionary<string, string?>
        {
            ["messageId"] = messageId,
            ["count"] = count?.ToString(),
            ["before"] = before?.ToString(),
            ["after"] = after?.ToString(),
        });
        return CreateCall<IReadOnlyList<JsonElement>>(request, wireCall, "Bitflyer.GetWithdrawals");
    }

    public async Task<BitflyerRawCall<IReadOnlyList<JsonElement>, JsonElement>> GetBankAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BitflyerEndpoints.GetBankAccounts(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bitflyer.GetBankAccounts", new Dictionary<string, string?>());
        return CreateCall<IReadOnlyList<JsonElement>>(request, wireCall, "Bitflyer.GetBankAccounts");
    }

    private static BitflyerRawRequest CreateRequest(
        string operation,
        IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BitflyerRawCall<TOk, JsonElement> CreateCall<TOk>(
        BitflyerRawRequest request,
        WireCall call,
        string context)
    {
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            var ok = BitflyerRawJson.DeserializeOrThrow<TOk>(response.Json, context);
            return new BitflyerRawCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(ok, response.StatusCode),
                call.Meta);
        }

        if (BitflyerRawJson.TryDeserialize<JsonElement>(response.Json, out var error, out _))
        {
            return new BitflyerRawCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(error!, response.StatusCode),
                call.Meta);
        }

        return new BitflyerRawCall<TOk, JsonElement>(
            request,
            new Err<TOk, JsonElement>(default, response.StatusCode),
            call.Meta);
    }

    private Task<WireCall> SendAsync(WireRequest request, CancellationToken ct) =>
        _wire.SendAsync(ExchangeCode.Bitflyer, request, ct);
}
