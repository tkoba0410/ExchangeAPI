# Contracts API Catalog (Reference)

本書は Contracts 層でサポートする API の一覧（Reference）である。
正本は `docs/contracts/contracts.md` と `src/Contracts/` とする。

## 1. Facade Interfaces

Public/Private は **署名の有無**のみを表す（用途別分類ではない）。

### 1.1 命名ルール（参照実装と例外）

- 代表 API 名は **Bitflyer の命名を参照**する。
- ただし **Bitflyer準拠で挙動が著しく歪む場合は独自命名を採用**する。
  - 特に **注文系（Order）** は取引所差が大きいため、独自命名を許容する。
- Bitflyer 名は **拘束ではなく参照**であり、Contracts の取引所非依存性を優先する。
- Contracts の公開 API 面の分類は **Public / Private のみ**とし、意味分類語彙を公開 I/F 名称や namespace に使用してはならない。
- 用途別の意味分類は **上位レイヤのラッパにのみ許容**する。

## 2. Public (No Signature)

### 2.1 IPublicApi

- `GetTickerCallAsync` -> `Call<GetTickerRequest, Ticker>`
- `GetOrderBookCallAsync` -> `Call<GetOrderBookRequest, OrderBook>`
- `GetMarketExecutionsCallAsync` -> `Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>`
- `GetHistoryKlineCallAsync` -> `Call<GetHistoryKlineRequest, IReadOnlyList<Candlestick>>`
- `GetTickersCallAsync` -> `Call<GetTickersRequest, IReadOnlyList<Ticker>>`
- `GetHistoryTradeCallAsync` -> `Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionMarket>>`
- `GetExchangeInfoCallAsync` -> `Call<GetExchangeInfoRequest, ExchangeInfo>`
- `GetCurrencysCallAsync` -> `Call<GetCurrencysRequest, IReadOnlyList<CurrencyCode>>`
- `GetTimestampCallAsync` -> `Call<GetTimestampRequest, DateTimeOffset>`

Source: `src/Contracts/Facade/Interfaces/IPublicApi.cs`

## 3. Private (Signature Required)

### 3.1 IPrivateApi

- `PlaceLimitOrderCallAsync` -> `Call<PlaceLimitOrderRequest, OrderResult>`
- `PlaceMarketOrderCallAsync` -> `Call<PlaceMarketOrderRequest, OrderResult>`
- `CancelOrderCallAsync` -> `Call<CancelOrderRequest, CancelResult>`
- `GetOrderCallAsync` -> `Call<GetOrderRequest, OrderStatus>`
- `GetOpenOrdersCallAsync` -> `Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>`
- `GetBalancesCallAsync` -> `Call<GetBalancesRequest, IReadOnlyList<Balance>>`
- `GetOrdersCallAsync` -> `Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>`
- `GetExecutionsCallAsync` -> `Call<MarketLimitCursorRequest, Page<ExecutionItem>>`

Source: `src/Contracts/Facade/Interfaces/IPrivateApi.cs`

## 4. Notes

- Contracts の公開 API は Call-only であり、I/O は Wire 層のみが行う。
- NotSupported は capability 不足を示す語彙であり、通常制御フローには用いない。
