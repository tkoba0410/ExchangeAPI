# Contracts Boundary Freeze Audit

> Informative only: 本文書は監査ログであり、設計判断の正本ではない。

Date: 2026-02-13
Scope:
- `src/Contracts/Facade/Interfaces/`
- `src/Contracts/**` (public Contracts-facing DTO/record types)

## A. Public Facade Interfaces (`src/Contracts/Facade/Interfaces`)

### `src/Contracts/Facade/Interfaces/IPublicApi.cs`

| Interface | Method Signature | Where `string` appears | Suggested replacement candidate |
|---|---|---|---|
| `IPublicApi` | `GetTickerAsync(TickerRequest, CancellationToken)` | None | - |
| `IPublicApi` | `GetBoardAsync(BoardRequest, CancellationToken)` | None | - |
| `IPublicApi` | `GetExecutionsPublicAsync(ExecutionsPublicRequest, CancellationToken)` | None | - |
| `IPublicApi` | `GetCandlesticksAsync(CandlesticksRequest, CancellationToken)` | None | - |
| `IPublicApi` | `GetExchangeInfoAsync(ExchangeInfoRequest, CancellationToken)` | None | - |

### `src/Contracts/Facade/Interfaces/IPrivateApi.cs`

| Interface | Method Signature | Where `string` appears | Suggested replacement candidate |
|---|---|---|---|
| `IPrivateApi` | `OrderLimitAsync(OrderLimitRequest, CancellationToken)` | None | - |
| `IPrivateApi` | `CancelOrderAsync(CancelOrderRequest, CancellationToken)` | None | - |
| `IPrivateApi` | `GetBalanceAsync(BalanceRequest, CancellationToken)` | None | - |
| `IPrivateApi` | `GetOrdersAsync(OrdersRequest, CancellationToken)` | None | - |
| `IPrivateApi` | `GetExecutionsPrivateAsync(ExecutionsPrivateRequest, CancellationToken)` | None | - |

### `src/Contracts/Facade/Interfaces/IExchangeMarketResolver.cs`

| Interface | Method Signature | Where `string` appears | Suggested replacement candidate |
|---|---|---|---|
| `IExchangeMarketResolver` | `ResolveCallAsync(ResolveExchangeMarketRequest, CancellationToken)` | None | - |

### `src/Contracts/Facade/Interfaces/IExchangeClient.cs`

| Interface | Member Signature | Where `string` appears | Suggested replacement candidate |
|---|---|---|---|
| `IExchangeClient` | `IPublicApi? Public { get; }` | None | - |
| `IExchangeClient` | `IPrivateApi? Private { get; }` | None | - |

Result for interfaces: **No `string` exposure found** in public Facade interface method parameters/return types (including generic wrappers in signatures).

## B. Other Public Contracts-Facing DTO/Record Types (`src/Contracts/**`)

### `src/Contracts/Common/Dtos/PeriodDto.cs`

| Type | Public Member | Where `string` appears | Suggested replacement candidate |
|---|---|---|---|
| `PeriodDto` (record) | Primary constructor: `PeriodDto(string Code)` | DTO constructor/property | `Period` (existing primitive in `src/Primitives/DomainCommon/Types/Period.cs`) |

Result for DTO/records: **1 public string exposure found** (`PeriodDto.Code`).
