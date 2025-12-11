using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bittrade.RawApi;

internal sealed record BittradeAccountsResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<BittradeAccount>? Data);

internal sealed record BittradeAccount(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("subtype")] string? SubType,
    [property: JsonPropertyName("state")] string State);

internal sealed record BittradeBalancesResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] BittradeBalanceData? Data);

internal sealed record BittradeBalanceData(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("list")] IReadOnlyList<BittradeBalanceEntry> List);

internal sealed record BittradeBalanceEntry(
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("balance")] string Balance);

internal sealed record BittradePlaceOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] long OrderId);

internal sealed record BittradeCancelOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] string OrderId);

internal sealed record BittradeOpenOrdersResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<BittradeOrderSummary>? Data);

internal sealed record BittradeOrderDetailResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] BittradeOrderDetail? Data);

internal sealed record BittradeOrderDetail(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("account-id")] string AccountId,
    [property: JsonPropertyName("amount")] string Amount,
    [property: JsonPropertyName("price")] string? Price,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("client-order-id")] string? ClientOrderId,
    [property: JsonPropertyName("created-at")] long CreatedAt,
    [property: JsonPropertyName("finished-at")] long? FinishedAt,
    [property: JsonPropertyName("field-amount")] string FilledAmount,
    [property: JsonPropertyName("field-cash-amount")] string FilledCashAmount,
    [property: JsonPropertyName("field-fees")] string Fees);

internal sealed record BittradeOrderSummary(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("account-id")] string AccountId,
    [property: JsonPropertyName("amount")] string Amount,
    [property: JsonPropertyName("price")] string? Price,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("client-order-id")] string? ClientOrderId,
    [property: JsonPropertyName("created-at")] long CreatedAt,
    [property: JsonPropertyName("field-amount")] string FilledAmount);
