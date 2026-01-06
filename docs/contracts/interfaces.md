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

## IAccountApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IAccountApi:GetAccountExecutionsCallAsync | method | Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>> GetAccountExecutionsCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Interfaces/IAccountApi.cs |  |
| contracts:IAccountApi:GetBalancesCallAsync | method | Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Interfaces/IAccountApi.cs |  |

---

## IApiCredentialProvider

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IApiCredentialProvider:Get | method | ApiCredentials Get(ExchangeCode exchange, string accountId) | auth | src/Domain/Contracts/Interfaces/IApiCredentialProvider.cs |  |

---

## IExchangeClient

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IExchangeClient:ExchangeCode | property | ExchangeCode ExchangeCode { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:Account | property | IAccountApi Account { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:Info | property | IExchangeInfoApi Info { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:MarginAccount | property | IMarginAccountApi? MarginAccount { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:Market | property | IMarketDataApi Market { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:Trading | property | ITradingApi Trading { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:TradingMarket | property | ITradingMarketApi? TradingMarket { get; } | client | src/Domain/Contracts/Interfaces/IExchangeClient.cs |  |

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

## IHasExchangeAccess

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IHasExchangeAccess:TryGetExchange | method | bool TryGetExchange<T>(out T exchange) where T : class | raw | src/Domain/Contracts/Interfaces/RawWireAccess.cs | lossless_hook |

---

## IHasRawAccess

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IHasRawAccess:TryGetRaw | method | bool TryGetRaw<T>(out T raw) where T : class | raw | src/Domain/Contracts/Interfaces/RawWireAccess.cs | lossless_hook |

---

## IMarginAccountApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IMarginAccountApi:GetCollateralCallAsync | method | Task<Call<GetCollateralRequest, Collateral>> GetCollateralCallAsync(CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Interfaces/IMarginAccountApi.cs |  |
| contracts:IMarginAccountApi:GetOpenPositionsCallAsync | method | Task<Call<GetOpenPositionsRequest, IReadOnlyList<Position>>> GetOpenPositionsCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Interfaces/IMarginAccountApi.cs |  |

---

## IMarketDataApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IMarketDataApi:GetMarketExecutionsCallAsync | method | Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetMarketExecutionsCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Interfaces/IMarketDataApi.cs |  |
| contracts:IMarketDataApi:GetOrderBookCallAsync | method | Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Interfaces/IMarketDataApi.cs |  |
| contracts:IMarketDataApi:GetTickerCallAsync | method | Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Interfaces/IMarketDataApi.cs |  |

---

## ITradingApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:ITradingApi:CancelOrderCallAsync | method | Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:GetOrderCallAsync | method | Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:GetOrdersCallAsync | method | Task<Call<GetOrdersRequest, IReadOnlyList<OpenOrder>>> GetOrdersCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:PlaceLimitOrderCallAsync | method | Task<Call<PlaceLimitOrderRequest, OrderResult>> PlaceLimitOrderCallAsync(Symbol symbol, Side side, Size size, Price price, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:PlaceMarketOrderCallAsync | method | Task<Call<PlaceMarketOrderRequest, OrderResult>> PlaceMarketOrderCallAsync(Symbol symbol, Side side, Size size, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:PlaceStopOrderCallAsync | method | Task<Call<PlaceStopOrderRequest, OrderResult>> PlaceStopOrderCallAsync(Symbol symbol, Side side, Size size, Price triggerPrice, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:GetParentOrdersCallAsync | method | Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrder>>> GetParentOrdersCallAsync(Symbol symbol, string? parentOrderId = null, string? parentOrderAcceptanceId = null, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:GetParentOrderCallAsync | method | Task<Call<GetParentOrderRequest, ParentOrderDetail>> GetParentOrderCallAsync(Symbol symbol, string? parentOrderId = null, string? parentOrderAcceptanceId = null, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Interfaces/ITradingApi.cs |  |
