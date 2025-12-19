# MIGRATION

ExchangeApi の破壊的変更（OrderKey導入）に伴う移行ガイドです。

## Breaking changes summary
- `ITradingApi.GetOrderAsync(Symbol, string)` を廃止し、`GetOrderAsync(Symbol, OrderKey)` に変更
- `ITradingApi.CancelOrderAsync(Symbol, string)` を廃止し、`CancelOrderAsync(Symbol, OrderKey)` に変更
- `OpenOrder.OrderId` を廃止し、`OpenOrder.Key` に変更
- `OrderStatus.OrderAcceptanceId` を廃止し、`OrderStatus.Key` に変更
- bitFlyer の not found が `Completed` ではなく `ExchangeOrderNotFoundException` になる

## Replacement table (old -> new)
- `GetOrderAsync(Symbol, string orderId)` -> `GetOrderAsync(Symbol, OrderKey orderKey)`
- `CancelOrderAsync(Symbol, string orderId)` -> `CancelOrderAsync(Symbol, OrderKey orderKey)`
- `OpenOrder.OrderId` -> `OpenOrder.Key`
- `OrderStatus.OrderAcceptanceId` -> `OrderStatus.Key`

## Code examples

### Before -> After (GetOrder / Cancel)
```csharp
// Before
var status = await trading.GetOrderAsync(symbol, orderId);
await trading.CancelOrderAsync(symbol, orderId);

// After
var key = new OrderKey(OrderIdKind.AcceptanceId, orderId);
var status = await trading.GetOrderAsync(symbol, key);
await trading.CancelOrderAsync(symbol, key);
```

### Before -> After (OpenOrder / OrderResult)
```csharp
// Before
var result = await trading.PlaceMarketOrderAsync(symbol, Side.Buy, 0.01m);
var status = await OrderPolling.WaitForOrderAsync(trading, symbol, result.OrderId);

// After
var result = await trading.PlaceMarketOrderAsync(symbol, Side.Buy, 0.01m);
var status = await OrderPolling.WaitForOrderAsync(trading, symbol, result.Key);
```

## not found behavior
- 旧: bitFlyer の not found を `Completed` として扱っていました。
- 新: `ExchangeOrderNotFoundException` を投げます。
- ポーリングは `NotFoundPolicy.Continue` / `NotFoundPolicy.StopAsNotFound` で挙動を制御できます。

## Notes
- `OpenOrder.Key` / `OrderResult.Key` は `GetOrderAsync` / `CancelOrderAsync` / `OrderPolling` にそのまま渡せます。
