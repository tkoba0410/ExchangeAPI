# Contracts API Catalog (Reference)

本書は Contracts 層でサポートする API の一覧（Reference）である。
正本は `docs/contracts/contracts.md` と `src/Contracts/` とする。

## 1. Facade Interfaces

### 1.1 IMarketDataApi

- `GetTickerCallAsync` -> `Call<GetTickerRequest, Ticker>`
- `GetOrderBookCallAsync` -> `Call<GetOrderBookRequest, OrderBook>`
- `GetMarketExecutionsCallAsync` -> `Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>`
- `GetHistoryKlineCallAsync` -> `Call<GetHistoryKlineRequest, IReadOnlyList<Candlestick>>`
- `GetTickersCallAsync` -> `Call<GetTickersRequest, IReadOnlyList<Ticker>>`
- `GetHistoryTradeCallAsync` -> `Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>`

Source: `src/Contracts/Facade/Interfaces/IMarketDataApi.cs`

### 1.2 ITradingApi

- `PlaceLimitOrderCallAsync` -> `Call<PlaceLimitOrderRequest, OrderResult>`
- `PlaceMarketOrderCallAsync` -> `Call<PlaceMarketOrderRequest, OrderResult>`
- `CancelOrderCallAsync` -> `Call<CancelOrderRequest, CancelResult>`
- `GetOrderCallAsync` -> `Call<GetOrderRequest, OrderStatus>`
- `GetOpenOrdersCallAsync` -> `Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>`

Source: `src/Contracts/Facade/Interfaces/ITradingApi.cs`

### 1.3 IAccountApi

- `GetBalancesCallAsync` -> `Call<GetBalancesRequest, IReadOnlyList<Balance>>`

Source: `src/Contracts/Facade/Interfaces/IAccountApi.cs`

### 1.4 IExchangeInfoApi

- `GetExchangeInfoCallAsync` -> `Call<GetExchangeInfoRequest, ExchangeInfo>`
- `GetCurrencysCallAsync` -> `Call<GetCurrencysRequest, IReadOnlyList<string>>`
- `GetTimestampCallAsync` -> `Call<GetTimestampRequest, DateTimeOffset>`

Source: `src/Contracts/Facade/Interfaces/IExchangeInfoApi.cs`

### 1.5 ISpotHistoryApi

- `GetOrdersCallAsync` -> `Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>`
- `GetExecutionsCallAsync` -> `Call<MarketLimitCursorRequest, Page<ExecutionItem>>`

Source: `src/Contracts/Facade/Interfaces/ISpotHistoryApi.cs`

## 2. Notes

- Contracts の公開 API は Call-only であり、I/O は Wire 層のみが行う。
- NotSupported は capability 不足を示す語彙であり、通常制御フローには用いない。
