using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bittrade.RawApi;

public sealed record BittradeAccountsResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<BittradeAccount>? Data);

public sealed record BittradeAccount(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("subtype")] string? SubType,
    [property: JsonPropertyName("state")] string State);

public sealed record BittradeBalancesResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] BittradeBalanceData? Data);

public sealed record BittradeBalanceData(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("list")] IReadOnlyList<BittradeBalanceEntry> List);

public sealed record BittradeBalanceEntry(
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("balance")] string Balance);

public sealed record BittradePlaceOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] long OrderId);

public sealed record BittradeCancelOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] string OrderId);

public sealed record BittradeOpenOrdersResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<BittradeOrderSummary>? Data);

public sealed record BittradeOrderDetailResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] BittradeOrderDetail? Data);

public sealed record BittradeOrderDetail(
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

public sealed record BittradeOrderSummary(
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
