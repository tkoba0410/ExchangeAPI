# Contracts API Catalog (Reference)

本書は Contracts 層でサポートする API の一覧（Reference）である。
正本は `docs/contracts/contracts.md` と `src/Contracts/` とする。

## 1. Facade Interfaces

Public/Private は **署名の有無**のみを表す（用途別分類ではない）。

## 2. Public (No Signature)

### 2.1 IMarketDataApi

- `GetTickerCallAsync` -> `Call<GetTickerRequest, Ticker>`
- `GetOrderBookCallAsync` -> `Call<GetOrderBookRequest, OrderBook>`
- `GetMarketExecutionsCallAsync` -> `Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>`
- `GetHistoryKlineCallAsync` -> `Call<GetHistoryKlineRequest, IReadOnlyList<Candlestick>>`
- `GetTickersCallAsync` -> `Call<GetTickersRequest, IReadOnlyList<Ticker>>`
- `GetHistoryTradeCallAsync` -> `Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>`

Source: `src/Contracts/Facade/Interfaces/IMarketDataApi.cs`

### 2.2 IExchangeInfoApi

- `GetExchangeInfoCallAsync` -> `Call<GetExchangeInfoRequest, ExchangeInfo>`
- `GetCurrencysCallAsync` -> `Call<GetCurrencysRequest, IReadOnlyList<string>>`
- `GetTimestampCallAsync` -> `Call<GetTimestampRequest, DateTimeOffset>`

Source: `src/Contracts/Facade/Interfaces/IExchangeInfoApi.cs`

## 3. Private (Signature Required)

### 3.1 ITradingApi

- `PlaceLimitOrderCallAsync` -> `Call<PlaceLimitOrderRequest, OrderResult>`
- `PlaceMarketOrderCallAsync` -> `Call<PlaceMarketOrderRequest, OrderResult>`
- `CancelOrderCallAsync` -> `Call<CancelOrderRequest, CancelResult>`
- `GetOrderCallAsync` -> `Call<GetOrderRequest, OrderStatus>`
- `GetOpenOrdersCallAsync` -> `Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>`

Source: `src/Contracts/Facade/Interfaces/ITradingApi.cs`

### 3.2 IAccountApi

- `GetBalancesCallAsync` -> `Call<GetBalancesRequest, IReadOnlyList<Balance>>`

Source: `src/Contracts/Facade/Interfaces/IAccountApi.cs`

### 3.3 ISpotHistoryApi

- `GetOrdersCallAsync` -> `Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>`
- `GetExecutionsCallAsync` -> `Call<MarketLimitCursorRequest, Page<ExecutionItem>>`

Source: `src/Contracts/Facade/Interfaces/ISpotHistoryApi.cs`

## 4. Notes

- Contracts の公開 API は Call-only であり、I/O は Wire 層のみが行う。
- NotSupported は capability 不足を示す語彙であり、通常制御フローには用いない。
