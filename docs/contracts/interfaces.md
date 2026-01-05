# Contracts Abstract APIs (Interfaces)

この文書は `src/Domain/Contracts/Contracts/Interfaces/` に存在する **抽象化インターフェース**の一覧です。

## Columns

- id: `contracts:<Interface>:<Member>`
- kind: method / property
- member: シグネチャ（末尾 `;` は省略）
- area: client / auth / info / market / trading / account / raw
- source: 実体ファイルパス
- flags: 補足タグ（例: `call_variant`, `lossless_hook`）

---

## IAccountApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IAccountApi:GetAccountExecutionsAsync | method | Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Contracts/Interfaces/IAccountApi.cs |  |
| contracts:IAccountApi:GetAccountExecutionsCallAsync | method | Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>> GetAccountExecutionsCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Contracts/Interfaces/IAccountApi.cs | call_variant |
| contracts:IAccountApi:GetBalancesAsync | method | Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Contracts/Interfaces/IAccountApi.cs |  |
| contracts:IAccountApi:GetBalancesCallAsync | method | Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Contracts/Interfaces/IAccountApi.cs | call_variant |

---

## IApiCredentialProvider

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IApiCredentialProvider:Get | method | ApiCredentials Get(ExchangeCode exchange, string accountId) | auth | src/Domain/Contracts/Contracts/Interfaces/IApiCredentialProvider.cs |  |

---

## IExchangeClient

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IExchangeClient:ExchangeCode | property | ExchangeCode ExchangeCode { get; } | client | src/Domain/Contracts/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:Account | property | IAccountApi Account { get; } | client | src/Domain/Contracts/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:Info | property | IExchangeInfoApi Info { get; } | client | src/Domain/Contracts/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:MarginAccount | property | IMarginAccountApi? MarginAccount { get; } | client | src/Domain/Contracts/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:Market | property | IMarketDataApi Market { get; } | client | src/Domain/Contracts/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:Trading | property | ITradingApi Trading { get; } | client | src/Domain/Contracts/Contracts/Interfaces/IExchangeClient.cs |  |
| contracts:IExchangeClient:TradingMarket | property | ITradingMarketApi? TradingMarket { get; } | client | src/Domain/Contracts/Contracts/Interfaces/IExchangeClient.cs |  |

---

## IExchangeInfoApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IExchangeInfoApi:GetExchangeInfoAsync | method | Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) | info | src/Domain/Contracts/Contracts/Interfaces/IExchangeInfoApi.cs |  |
| contracts:IExchangeInfoApi:GetExchangeInfoCallAsync | method | Task<Call<GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(CancellationToken cancellationToken = default) | info | src/Domain/Contracts/Contracts/Interfaces/IExchangeInfoApi.cs | call_variant |

---

## IExchangeMarketResolver

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IExchangeMarketResolver:ResolveAsync | method | Task<ExchangeMarketInfo> ResolveAsync(Symbol symbol, CancellationToken cancellationToken = default) | info | src/Domain/Contracts/Contracts/Interfaces/IExchangeMarketResolver.cs |  |

---

## IHasExchangeAccess

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IHasExchangeAccess:TryGetExchange | method | bool TryGetExchange<T>(out T exchange) where T : class | raw | src/Domain/Contracts/Contracts/Interfaces/RawWireAccess.cs | lossless_hook |

---

## IHasRawAccess

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IHasRawAccess:TryGetRaw | method | bool TryGetRaw<T>(out T raw) where T : class | raw | src/Domain/Contracts/Contracts/Interfaces/RawWireAccess.cs | lossless_hook |

---

## IMarginAccountApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IMarginAccountApi:GetCollateralAsync | method | Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Contracts/Interfaces/IMarginAccountApi.cs |  |
| contracts:IMarginAccountApi:GetCollateralCallAsync | method | Task<Call<GetCollateralRequest, Collateral>> GetCollateralCallAsync(CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Contracts/Interfaces/IMarginAccountApi.cs | call_variant |
| contracts:IMarginAccountApi:GetOpenPositionsAsync | method | Task<IReadOnlyList<Position>> GetOpenPositionsAsync(Symbol symbol, CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Contracts/Interfaces/IMarginAccountApi.cs |  |
| contracts:IMarginAccountApi:GetOpenPositionsCallAsync | method | Task<Call<GetOpenPositionsRequest, IReadOnlyList<Position>>> GetOpenPositionsCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | account | src/Domain/Contracts/Contracts/Interfaces/IMarginAccountApi.cs | call_variant |

---

## IMarketDataApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:IMarketDataApi:GetCandlestickColumnsAsync | method | Task<CandlestickColumnar> GetCandlestickColumnsAsync(Symbol symbol, Timescale timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Contracts/Interfaces/IMarketDataApi.cs |  |
| contracts:IMarketDataApi:GetCandlestickColumnsCallAsync | method | Task<Call<GetCandlestickColumnsRequest, CandlestickColumnar>> GetCandlestickColumnsCallAsync(Symbol symbol, Timescale timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Contracts/Interfaces/IMarketDataApi.cs | call_variant |
| contracts:IMarketDataApi:GetCandlesticksAsync | method | Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(Symbol symbol, Timescale timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Contracts/Interfaces/IMarketDataApi.cs |  |
| contracts:IMarketDataApi:GetCandlesticksCallAsync | method | Task<Call<GetCandlesticksRequest, IReadOnlyList<Candlestick>>> GetCandlesticksCallAsync(Symbol symbol, Timescale timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Contracts/Interfaces/IMarketDataApi.cs | call_variant |
| contracts:IMarketDataApi:GetMarketExecutionsAsync | method | Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, ExecutionQuery? query = null, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Contracts/Interfaces/IMarketDataApi.cs |  |
| contracts:IMarketDataApi:GetMarketExecutionsCallAsync | method | Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetMarketExecutionsCallAsync(Symbol symbol, ExecutionQuery? query = null, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Contracts/Interfaces/IMarketDataApi.cs | call_variant |
| contracts:IMarketDataApi:GetOrderBookAsync | method | Task<OrderBook> GetOrderBookAsync(Symbol symbol, OrderBookDepth depth = OrderBookDepth.Default, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Contracts/Interfaces/IMarketDataApi.cs |  |
| contracts:IMarketDataApi:GetOrderBookCallAsync | method | Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(Symbol symbol, OrderBookDepth depth = OrderBookDepth.Default, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Contracts/Interfaces/IMarketDataApi.cs | call_variant |
| contracts:IMarketDataApi:GetTickerAsync | method | Task<Ticker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Contracts/Interfaces/IMarketDataApi.cs |  |
| contracts:IMarketDataApi:GetTickerCallAsync | method | Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | market | src/Domain/Contracts/Contracts/Interfaces/IMarketDataApi.cs | call_variant |

---

## ITradingApi

| id | kind | member | area | source | flags |
| -- | ---- | ------ | ---- | ------ | ----- |
| contracts:ITradingApi:CancelOrderAsync | method | Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:CancelOrderCallAsync | method | Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs | call_variant |
| contracts:ITradingApi:GetOrderAsync | method | Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:GetOrderCallAsync | method | Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs | call_variant |
| contracts:ITradingApi:GetOrdersAsync | method | Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:GetOrdersCallAsync | method | Task<Call<GetOrdersRequest, IReadOnlyList<OpenOrder>>> GetOrdersCallAsync(Symbol symbol, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs | call_variant |
| contracts:ITradingApi:PlaceLimitOrderAsync | method | Task<OrderResult> PlaceLimitOrderAsync(Symbol symbol, Side side, Size size, Price price, TimeInForce? timeInForce = null, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:PlaceLimitOrderCallAsync | method | Task<Call<PlaceLimitOrderRequest, OrderResult>> PlaceLimitOrderCallAsync(Symbol symbol, Side side, Size size, Price price, TimeInForce? timeInForce = null, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs | call_variant |
| contracts:ITradingApi:PlaceMarketOrderAsync | method | Task<OrderResult> PlaceMarketOrderAsync(Symbol symbol, Side side, Size size, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:PlaceMarketOrderCallAsync | method | Task<Call<PlaceMarketOrderRequest, OrderResult>> PlaceMarketOrderCallAsync(Symbol symbol, Side side, Size size, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs | call_variant |
| contracts:ITradingApi:PlaceStopOrderAsync | method | Task<OrderResult> PlaceStopOrderAsync(Symbol symbol, Side side, Size size, Price triggerPrice, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs |  |
| contracts:ITradingApi:PlaceStopOrderCallAsync | method | Task<Call<PlaceStopOrderRequest, OrderResult>> PlaceStopOrderCallAsync(Symbol symbol, Side side, Size size, Price triggerPrice, CancellationToken cancellationToken = default) | trading | src/Domain/Contracts/Contracts/Interfaces/ITradingApi.cs | call_variant |
