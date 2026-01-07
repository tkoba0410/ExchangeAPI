# Contracts Abstract APIs (Interfaces)

この一覧に記載する Contracts 抽象 API は `Call<TRequest,TResponse>` を唯一の返り値とする（Call-only）。
Transport 層（`src/Core/Transport/**`）は対象外。

この文書は `src/Domain/Contracts/Interfaces/` に存在する **抽象化インターフェース**の一覧です。

## Columns

- id: `contracts:<Interface>:<Member>`
- kind: method / property
- member: シグネチャ（末尾 `;` は省略）
- area: client / auth / info / market / trading / account / raw
- source: 実体ファイルパス
- flags: 補足タグ（例: `lossless_hook`）

---

## IExchangeClient

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IExchangeClient:Market | property | IMarketDataApi Market { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:Trading | property | ITradingApi Trading { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:Account | property | IAccountApi Account { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:History | property | ISpotHistoryApi History { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:ExchangeCode | property | ExchangeCode ExchangeCode { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |

---

## IMarketDataApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IMarketDataApi:GetTickerCallAsync | method | Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Interfaces/IMarketDataApi.cs |  |
| contracts:IMarketDataApi:GetOrderBookCallAsync | method | Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Interfaces/IMarketDataApi.cs |  |
| contracts:IMarketDataApi:GetMarketExecutionsCallAsync | method | Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetMarketExecutionsCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Interfaces/IMarketDataApi.cs |  |

---

## ITradingApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:ITradingApi:PlaceLimitOrderCallAsync | method | Task<Call<PlaceLimitOrderRequest, OrderResult>> PlaceLimitOrderCallAsync(Symbol symbol, Side side, Size size, Price price, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:PlaceMarketOrderCallAsync | method | Task<Call<PlaceMarketOrderRequest, OrderResult>> PlaceMarketOrderCallAsync(Symbol symbol, Side side, Size size, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:CancelOrderCallAsync | method | Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:GetOrderCallAsync | method | Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:GetOpenOrdersCallAsync | method | Task<Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>> GetOpenOrdersCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |

---

## IAccountApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IAccountApi:GetBalancesCallAsync | method | Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Interfaces/IAccountApi.cs |  |

---

## ISpotHistoryApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:ISpotHistoryApi:GetOrdersCallAsync | method | Task<Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>> GetOrdersCallAsync(MarketLimitCursorRequest request, CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Interfaces/ISpotHistoryApi.cs |  |
| contracts:ISpotHistoryApi:GetExecutionsCallAsync | method | Task<Call<MarketLimitCursorRequest, Page<ExecutionItem>>> GetExecutionsCallAsync(MarketLimitCursorRequest request, CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Interfaces/ISpotHistoryApi.cs |  |

---

## IExchangeInfoApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IExchangeInfoApi:GetExchangeInfoCallAsync | method | Task<Call<GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(CancellationToken cancellationToken = default) | info | src/Domain/Contracts/Interfaces/IExchangeInfoApi.cs |  |

---

## IExchangeMarketResolver

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IExchangeMarketResolver:ResolveCallAsync | method | Task<Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>> ResolveCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | info | src/Domain/Contracts/Interfaces/IExchangeMarketResolver.cs |  |

---

## IApiCredentialProvider

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IApiCredentialProvider:Get | method | ApiCredentials Get(ExchangeCode exchange, string accountId) | auth | src/Domain/Contracts/Interfaces/IApiCredentialProvider.cs |  |

---

## RawWireAccess

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IHasRawAccess:TryGetRaw | method | bool TryGetRaw<T>(out T raw) where T : class | raw | src/Domain/Contracts/Interfaces/RawWireAccess.cs | lossless_hook |
| contracts:IHasExchangeAccess:TryGetExchange | method | bool TryGetExchange<T>(out T exchange) where T : class | raw | src/Domain/Contracts/Interfaces/RawWireAccess.cs | lossless_hook |
