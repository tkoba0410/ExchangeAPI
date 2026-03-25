# Stage10 Endpoint Matrix — Bitflyer

本書は、bitFlyer の Stage10 実装に対する endpoint 運用正本である。  
本書は Stage10 第1段階の実装対象、DTO 固定状況、live test 導入順、endpoint metadata を自己完結に管理する。

現在の Stage10 コード配置は本書の従属物であり、判断根拠にはしない。  
削除済み inventory や他の補助文書を前提にせず、本書自身を endpoint 正本として扱う。

## Values

- `ExposeInProtocol`
  - `Yes`: Stage10 第1段階で `Protocol` 公開面に含める
  - `Later`: Stage10 後段で扱う
- `ExposeInNative`
  - `Yes`: Stage10 第1段階で `Native` 公開面に含める
  - `Later`: Stage10 後段で扱う
- `LiveTestPhase`
  - `Phase1-Read`: 第1段階の read live test 対象
  - `Phase2-Write`: 第2段階の write live test 対象
  - `Later`: 後段導入
- `RequestDtoStatus` / `ResponseDtoStatus`
  - `Transitional`: 最終固定前
  - `Fixed`: 最終固定済み
- `ExpectedStatus`
  - `200`: HTTP 200 を成功とする
  - `TBD`: 後段で確定する
  - `Native` が評価し、`Protocol` は raw status を保持する
- `ResponseShape`
  - `Object`: top-level object
  - `Array`: top-level array
  - `EmptyOrObject`: empty body または top-level object
  - `TBD`: 後段で確定する
- `WritesState`
  - `Yes`: venue state を変更する
  - `No`: read-only
- `CleanupPolicy`
  - `None`: cleanup 不要
  - `Required`: live test 後に cleanup を必須とする
  - `NotSupported`: Stage10 では write live test 対象にしない
- `AliasPath`
  - path alias がある場合はその path を書く
  - なければ `-`
- `AuthType`
  - `None`: 認証不要
  - `KeySecret`: API key / secret による private 認証
  - `TBD`: 後段で確定する
- `OptionalOmissionRule`
  - `-`: omission rule なし
  - `TBD`: 後段で確定する
  - 条件付き omission がある場合は簡潔に記述する

`TBD` の許容条件:

- `ExposeInProtocol = Yes` または `ExposeInNative = Yes` の row に、`ExpectedStatus` / `ResponseShape` / `AuthType` の `TBD` を残さない
- `ExposeInNative = Yes` の row に、`OptionalOmissionRule` の `TBD` を残さない
- `TBD` は `ExposeInProtocol != Yes` かつ `ExposeInNative != Yes` の row にのみ許容する

## Facade + Endpoint Module Rule

- `ExposeInProtocol = Yes`
  - facade に `*CallAsync(...)` の endpoint-level method を公開する
  - 対応する独立 module class を `Protocol/Public|Private/Endpoints/<EndpointName>/` 配下へ置く
- `ExposeInNative = Yes`
  - facade に `*CallAsync(...)` の native call method を公開する
  - 対応する独立 module class を `Native/Public|Private/Endpoints/<EndpointName>/` 配下へ置く
  - request DTO と response DTO は同 endpoint フォルダへ寄せてよい
- matrix は「公開面に出すか」を管理する
  - shared helper の配置
  - converter / validator の細かい file 分割
  は別途 `docs/spec.md` の物理構成方針に従う

## Matrix

| EndpointId | Method | Path | Scope | ExposeInProtocol | ExposeInNative | LiveTestPhase | RequestDtoStatus | ResponseDtoStatus | ExpectedStatus | ResponseShape | WritesState | CleanupPolicy | AliasPath | AuthType | OptionalOmissionRule |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| GetMarkets | GET | /v1/getmarkets | public | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | /v1/markets | None | - |
| GetBoard | GET | /v1/getboard | public | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Object | No | None | - | None | product_code = null は query omitted |
| GetTicker | GET | /v1/getticker | public | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Object | No | None | /v1/ticker | None | product_code = null は query omitted |
| GetExecutionsPublic | GET | /v1/getexecutions | public | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | /v1/executions | None | optional query params omitted when null; product_code omitted => BTC_JPY default |
| GetBoardState | GET | /v1/getboardstate | public | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Object | No | None | - | None | product_code = null は query omitted |
| GetHealth | GET | /v1/gethealth | public | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Object | No | None | - | None | product_code = null は query omitted |
| GetFundingRate | GET | /v1/getfundingrate | public | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Object | No | None | - | None | product_code required |
| GetCorporateLeverage | GET | /v1/getcorporateleverage | public | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Object | No | None | - | None | - |
| GetChats | GET | /v1/getchats | public | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | None | from_date = null は query omitted |
| GetPermissions | GET | /v1/me/getpermissions | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | - |
| GetBalance | GET | /v1/me/getbalance | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | - |
| GetCollateral | GET | /v1/me/getcollateral | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Object | No | None | - | KeySecret | - |
| GetCollateralAccounts | GET | /v1/me/getcollateralaccounts | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | - |
| GetAddresses | GET | /v1/me/getaddresses | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | - |
| GetCoinIns | GET | /v1/me/getcoinins | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | optional query params omitted when null |
| GetCoinOuts | GET | /v1/me/getcoinouts | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | optional query params omitted when null |
| GetBankAccounts | GET | /v1/me/getbankaccounts | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | - |
| GetDeposits | GET | /v1/me/getdeposits | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | optional query params omitted when null |
| Withdraw | POST | /v1/me/withdraw | private | Yes | Yes | Later | Transitional | Transitional | 200 | Object | Yes | NotSupported | - | KeySecret | currency_code/bank_account_id/amount/code required |
| GetWithdrawals | GET | /v1/me/getwithdrawals | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | optional query params omitted when null |
| SendChildOrder | POST | /v1/me/sendchildorder | private | Yes | Yes | Phase2-Write | Fixed | Fixed | 200 | Object | Yes | Required | - | KeySecret | minute_to_expire/time_in_force = null omitted, price is conditional |
| SendParentOrder | POST | /v1/me/sendparentorder | private | Yes | Yes | Phase2-Write | Fixed | Fixed | 200 | Object | Yes | Required | - | KeySecret | order_method/minute_to_expire/time_in_force = null omitted; parameter fields are conditionally omitted |
| CancelChildOrder | POST | /v1/me/cancelchildorder | private | Yes | Yes | Phase2-Write | Fixed | Fixed | 200 | EmptyOrObject | Yes | None | - | KeySecret | exactly one of child_order_id or child_order_acceptance_id |
| CancelParentOrder | POST | /v1/me/cancelparentorder | private | Yes | Yes | Phase2-Write | Fixed | Fixed | 200 | EmptyOrObject | Yes | None | - | KeySecret | exactly one of parent_order_id or parent_order_acceptance_id |
| CancelAllChildOrders | POST | /v1/me/cancelallchildorders | private | Yes | Yes | Phase2-Write | Fixed | Fixed | 200 | EmptyOrObject | Yes | None | - | KeySecret | product_code required |
| GetChildOrders | GET | /v1/me/getchildorders | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | optional query params omitted when null; product_code omitted => BTC_JPY default |
| GetParentOrders | GET | /v1/me/getparentorders | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | optional query params omitted when null |
| GetParentOrder | GET | /v1/me/getparentorder | private | Yes | Yes | Phase2-Write | Fixed | Fixed | 200 | Object | No | None | - | KeySecret | exactly one of parent_order_id or parent_order_acceptance_id |
| GetExecutionsPrivate | GET | /v1/me/getexecutions | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | product_code required; optional query params omitted when null |
| GetBalanceHistory | GET | /v1/me/getbalancehistory | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | optional query params omitted when null |
| GetPositions | GET | /v1/me/getpositions | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | - |
| GetCollateralHistory | GET | /v1/me/getcollateralhistory | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Array | No | None | - | KeySecret | optional query params omitted when null |
| GetTradingCommission | GET | /v1/me/gettradingcommission | private | Yes | Yes | Phase1-Read | Fixed | Fixed | 200 | Object | No | None | - | KeySecret | product_code required |

## Initial Rule

- 現行 Stage10 実装では `GetMarkets`、`GetBoard`、`GetTicker`、`GetExecutionsPublic`、`GetBoardState`、`GetHealth`、`GetFundingRate`、`GetCorporateLeverage`、`GetChats`、`GetPermissions`、`GetBalance`、`GetCollateral`、`GetCollateralAccounts`、`GetAddresses`、`GetCoinIns`、`GetCoinOuts`、`GetBankAccounts`、`GetDeposits`、`Withdraw`、`GetWithdrawals`、`GetChildOrders`、`GetParentOrders`、`GetParentOrder`、`GetExecutionsPrivate`、`GetBalanceHistory`、`GetPositions`、`GetCollateralHistory`、`GetTradingCommission`、`SendChildOrder`、`SendParentOrder`、`CancelChildOrder`、`CancelParentOrder`、`CancelAllChildOrders` を library 公開面に含める
- read path の live test は、public は条件なし、private read は認証可能なら実行する
- `SendChildOrder` と `CancelChildOrder` は `Phase2-Write`、`CancelAllChildOrders` は dedicated marker と `BTC_JPY` preflight empty check を持つ `Phase2-Write` とする
- `GetMarkets`、`GetTicker`、`GetBalance`、`GetCollateral`、`GetCollateralAccounts`、`GetTradingCommission` は first wave として `Fixed` に上げる
- `GetBoard`、`GetExecutionsPublic`、`GetBoardState`、`GetHealth`、`GetFundingRate`、`GetCorporateLeverage`、`GetChats`、`GetAddresses`、`GetBankAccounts` は second wave として `Fixed` に上げる
- `GetPermissions`、`GetCoinIns`、`GetCoinOuts`、`GetDeposits`、`GetWithdrawals`、`GetChildOrders`、`GetParentOrders`、`GetExecutionsPrivate`、`GetBalanceHistory`、`GetPositions`、`GetCollateralHistory` は third wave として `Fixed` に上げる
- `SendChildOrder` と `CancelChildOrder` は non-fill lifecycle を前提に fourth wave として `Fixed` に上げる
- `SendParentOrder`、`GetParentOrder`、`CancelParentOrder` は parent non-fill lifecycle を前提に fifth wave として `Fixed` に上げる
- `CancelAllChildOrders` は `BTC_JPY` 専用 safety gate と preflight を前提に sixth wave として `Fixed` に上げる
- `Withdraw` は cleanup 不可のため `Fixed` に上げず、wrong-code による negative live contract のみを許容する
  - current normative では non-success HTTP status を `Http` と扱うため、negative status は child protocol body で確認する
- `Phase1-Read` に含めた read endpoint は third wave までで `Fixed` に上げ切る
- `Later` の read と write を含む残りの実装済み endpoint は、引き続き `Transitional` のまま段階的に固定する

## Implementation Order

- `GetMarkets` で public top-level array response の基準形を作る
- `GetTicker` で public object response の基準形を作る
- `GetBoard` を追加し、public object with nested array response の形を固定する
- `GetExecutionsPublic` を追加し、public paging/filter array response の形を固定する
- `GetBalance` を追加し、private top-level array response の基準形を作る
- `GetCollateral` / `GetCollateralAccounts` を追加し、private object と private array の空 request read endpoint を固定する
- `GetChildOrders` / `GetExecutionsPrivate` / `GetCollateralHistory` を追加し、paging/filter を持つ private read endpoint の形を固定する
- `GetPositions` を追加し、required query を持つ private read endpoint の形を固定する
- `GetTradingCommission` を追加し、required query + object response の単純 private read endpoint を固定する
- `SendChildOrder` / `CancelChildOrder` を追加し、body encode を持つ write endpoint の形を固定する
- `CancelAllChildOrders` を追加し、body encode + `Unit` response の destructive write endpoint を固定する

## Current Implemented Endpoint Contracts

実装済み endpoint の公開有無と contract metadata の正本は matrix とする。以下は first wave と代表例の exact contract を示す。

### GetMarkets

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetMarketsCallAsync(CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>> GetMarketsCallAsync(GetMarketsRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `GetMarketsRequest` は空 DTO
  - JSON body なし
- response DTO
  - top-level array
  - `GetMarkets.Item`
    - `ProductCode: string`
    - `MarketType: string`
- `ExpectedStatus = 200`
- `ResponseShape = Array`

### GetBoard

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetBoardCallAsync(string? productCode, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(GetBoardRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string?`
  - JSON body なし
  - `ProductCode = null` のとき query omitted
- response DTO
  - `MidPrice: decimal`
  - `Bids: IReadOnlyList<GetBoardLevel>`
  - `Asks: IReadOnlyList<GetBoardLevel>`
  - `GetBoardLevel`
    - `Price: decimal`
    - `Size: decimal`
- `ExpectedStatus = 200`
- `ResponseShape = Object`

### GetExecutionsPublic

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetExecutionsCallAsync(string? productCode, int? count, long? before, long? after, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>> GetExecutionsCallAsync(GetExecutionsPublicRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string?`
  - `Count: int?`
  - `Before: long?`
  - `After: long?`
- request rule
  - `ProductCode = null` のとき query omitted で bitFlyer の既定値 `BTC_JPY`
  - `Count`、`Before`、`After` は指定時に正数
  - optional query は `null` のとき omitted
- response DTO
  - top-level array
  - `GetExecutionsPublic.Item`
    - `Id: long`
    - `Side: string`
    - `Price: decimal`
    - `Size: decimal`
    - `ExecDate: DateTimeOffset`
    - `BuyChildOrderAcceptanceId: string`
    - `SellChildOrderAcceptanceId: string`
- `ExpectedStatus = 200`
- `ResponseShape = Array`

### GetTicker

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetTickerCallAsync(string? productCode, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(GetTickerRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string?`
  - JSON body なし
  - `ProductCode = null` のとき query omitted
- response DTO
  - `ProductCode: string`
  - `State: string`
  - `Timestamp: DateTimeOffset`
  - `TickId: long`
  - `BestBid: decimal`
  - `BestAsk: decimal`
  - `BestBidSize: decimal`
  - `BestAskSize: decimal`
  - `TotalBidDepth: decimal`
  - `TotalAskDepth: decimal`
  - `MarketBidSize: decimal`
  - `MarketAskSize: decimal`
  - `Ltp: decimal`
  - `Volume: decimal`
  - `VolumeByProduct: decimal`
- `ExpectedStatus = 200`
- `ResponseShape = Object`

### GetBalance

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetBalanceCallAsync(CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(GetBalanceRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `GetBalanceRequest` は空 DTO
  - JSON body なし
- response DTO
  - top-level array
  - `GetBalance.Item`
    - `CurrencyCode: string`
    - `Amount: decimal`
    - `Available: decimal`
- `ExpectedStatus = 200`
- `ResponseShape = Array`

### GetCollateral

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetCollateralCallAsync(CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(GetCollateralRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `GetCollateralRequest` は空 DTO
  - JSON body なし
- response DTO
  - `Collateral: decimal`
  - `OpenPositionPnl: decimal`
  - `RequireCollateral: decimal`
  - `KeepRate: decimal`
  - `MarginCallAmount: decimal?`
  - `MarginCallDueDate: DateTimeOffset?`
- `ExpectedStatus = 200`
- `ResponseShape = Object`

### GetCollateralAccounts

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetCollateralAccountsCallAsync(CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> GetCollateralAccountsCallAsync(GetCollateralAccountsRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `GetCollateralAccountsRequest` は空 DTO
  - JSON body なし
- response DTO
  - top-level array
  - `GetCollateralAccounts.Item`
    - `CurrencyCode: string`
    - `Amount: decimal`
- `ExpectedStatus = 200`
- `ResponseShape = Array`

### GetChildOrders

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetChildOrdersCallAsync(string? productCode, int? count, long? before, long? after, string? childOrderState, string? childOrderId, string? childOrderAcceptanceId, string? parentOrderId, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetChildOrdersCallAsync(GetChildOrdersRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string?`
  - `Count: int?`
  - `Before: long?`
  - `After: long?`
  - `ChildOrderState: string?`
  - `ChildOrderId: string?`
  - `ChildOrderAcceptanceId: string?`
  - `ParentOrderId: string?`
- request rule
  - `ProductCode = null` のとき query omitted で bitFlyer の既定値 `BTC_JPY`
  - `Count`、`Before`、`After` は指定時に正数
  - `ChildOrderState` は `ACTIVE` / `COMPLETED` / `CANCELED` / `EXPIRED` / `REJECTED` のいずれか
  - それ以外の optional query は `null` のとき omitted
- response DTO
  - top-level array
  - `GetChildOrders.Item`
    - `Id: long`
    - `ChildOrderId: string`
    - `ProductCode: string`
    - `Side: string`
    - `ChildOrderType: string`
    - `Price: decimal`
    - `AveragePrice: decimal`
    - `Size: decimal`
    - `ChildOrderState: string`
    - `ExpireDate: DateTimeOffset`
    - `ChildOrderDate: DateTimeOffset`
    - `ChildOrderAcceptanceId: string`
    - `OutstandingSize: decimal`
    - `CancelSize: decimal`
    - `ExecutedSize: decimal`
    - `TotalCommission: decimal`
    - `TimeInForce: string`
- `ExpectedStatus = 200`
- `ResponseShape = Array`

### GetExecutionsPrivate

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetExecutionsCallAsync(string productCode, int? count, long? before, long? after, string? childOrderId, string? childOrderAcceptanceId, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> GetExecutionsCallAsync(GetExecutionsRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string`
  - `Count: int?`
  - `Before: long?`
  - `After: long?`
  - `ChildOrderId: string?`
  - `ChildOrderAcceptanceId: string?`
- request rule
  - `ProductCode` 必須
  - `Count`、`Before`、`After` は指定時に正数
  - optional query は `null` のとき omitted
- response DTO
  - top-level array
  - `GetExecutions.Item`
    - `Id: long`
    - `ChildOrderId: string`
    - `Side: string`
    - `Price: decimal`
    - `Size: decimal`
    - `Commission: decimal`
    - `ExecDate: DateTimeOffset`
    - `ChildOrderAcceptanceId: string`
- `ExpectedStatus = 200`
- `ResponseShape = Array`

### GetPositions

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetPositionsCallAsync(string productCode, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsCallAsync(GetPositionsRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string`
  - `FX_BTC_JPY` のみ許容する
  - JSON body なし
- response DTO
  - top-level array
  - `GetPositions.Item`
    - `ProductCode: string`
    - `Side: string`
    - `Price: decimal`
    - `Size: decimal`
    - `Commission: decimal`
    - `SwapPointAccumulate: decimal`
    - `RequireCollateral: decimal`
    - `OpenDate: DateTimeOffset`
    - `Leverage: decimal`
    - `Pnl: decimal`
    - `Sfd: decimal`
- `ExpectedStatus = 200`
- `ResponseShape = Array`

### GetCollateralHistory

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetCollateralHistoryCallAsync(int? count, long? before, long? after, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> GetCollateralHistoryCallAsync(GetCollateralHistoryRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `Count: int?`
  - `Before: long?`
  - `After: long?`
- request rule
  - `Count`、`Before`、`After` は指定時に正数
  - `null` のとき omitted
- response DTO
  - top-level array
  - `GetCollateralHistory.Item`
    - `Id: long`
    - `CurrencyCode: string`
    - `Change: decimal`
    - `Amount: decimal`
    - `ReasonCode: string`
    - `Date: DateTimeOffset`
- `ExpectedStatus = 200`
- `ResponseShape = Array`

### GetTradingCommission

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetTradingCommissionCallAsync(string productCode, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionCallAsync(GetTradingCommissionRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string`
  - `ProductCode` 必須
  - JSON body なし
- response DTO
  - `CommissionRate: decimal`
- `ExpectedStatus = 200`
- `ResponseShape = Object`

### SendChildOrder

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> SendChildOrderCallAsync(string bodyJson, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(SendChildOrderRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string`
  - `ChildOrderType: string`
  - `Side: string`
  - `Price: decimal?`
  - `Size: decimal`
  - `MinuteToExpire: int?`
  - `TimeInForce: string?`
- request rule
  - `ChildOrderType = LIMIT` のとき `Price` 必須
  - `ChildOrderType = MARKET` のとき `Price` omitted
  - `MinuteToExpire = null` のとき body omitted
  - `TimeInForce = null` のとき body omitted
- response DTO
  - `ChildOrderAcceptanceId: string`
- `ExpectedStatus = 200`
- `ResponseShape = Object`

### CancelChildOrder

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> CancelChildOrderCallAsync(string bodyJson, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<CancelChildOrderRequest, Unit>> CancelChildOrderCallAsync(CancelChildOrderRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string`
  - `ChildOrderId: string?`
  - `ChildOrderAcceptanceId: string?`
- request rule
  - `ChildOrderId` と `ChildOrderAcceptanceId` は exactly one
- response DTO
  - `Unit`
  - empty body または `{}` を成功扱いにしてよい
- `ExpectedStatus = 200`
- `ResponseShape = EmptyOrObject`

### SendParentOrder

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> SendParentOrderCallAsync(string bodyJson, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderCallAsync(SendParentOrderRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `OrderMethod: string?`
  - `MinuteToExpire: int?`
  - `TimeInForce: string?`
  - `Parameters: IReadOnlyList<SendParentOrderParameter>`
  - `SendParentOrderParameter`
    - `ProductCode: string`
    - `ConditionType: string`
    - `Side: string`
    - `Price: decimal?`
    - `Size: decimal`
    - `TriggerPrice: decimal?`
    - `Offset: long?`
- request rule
  - `OrderMethod = null` のとき `SIMPLE` として扱う
  - `MinuteToExpire = null` のとき body omitted
  - `TimeInForce = null` のとき body omitted
  - `Parameters` は method に対応する件数を満たす
  - `LIMIT` は `Price` 必須
  - `MARKET` は `Price` / `TriggerPrice` / `Offset` omitted
  - `STOP` は `TriggerPrice` 必須
  - `STOP_LIMIT` は `Price` と `TriggerPrice` 必須
  - `TRAIL` は `Offset` 必須
- response DTO
  - `ParentOrderAcceptanceId: string`
- `ExpectedStatus = 200`
- `ResponseShape = Object`

### GetParentOrder

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetParentOrderCallAsync(string? parentOrderId, string? parentOrderAcceptanceId, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderCallAsync(GetParentOrderRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ParentOrderId: string?`
  - `ParentOrderAcceptanceId: string?`
- request rule
  - `ParentOrderId` と `ParentOrderAcceptanceId` は exactly one
- response DTO
  - `Id: long`
  - `ParentOrderId: string`
  - `OrderMethod: string`
  - `ExpireDate: DateTimeOffset`
  - `TimeInForce: string`
  - `Parameters: IReadOnlyList<GetParentOrderParameter>`
  - `ParentOrderAcceptanceId: string`
  - `GetParentOrderParameter`
    - `ProductCode: string`
    - `ConditionType: string`
    - `Side: string`
    - `Price: decimal`
    - `Size: decimal`
    - `TriggerPrice: decimal`
    - `Offset: decimal`
- `ExpectedStatus = 200`
- `ResponseShape = Object`

### CancelParentOrder

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> CancelParentOrderCallAsync(string bodyJson, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<CancelParentOrderRequest, Unit>> CancelParentOrderCallAsync(CancelParentOrderRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string`
  - `ParentOrderId: string?`
  - `ParentOrderAcceptanceId: string?`
- request rule
  - `ParentOrderId` と `ParentOrderAcceptanceId` は exactly one
- response DTO
  - `Unit`
  - empty body または `{}` を成功扱いにしてよい
- `ExpectedStatus = 200`
- `ResponseShape = EmptyOrObject`

### CancelAllChildOrders

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> CancelAllChildOrdersCallAsync(string bodyJson, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<CancelAllChildOrdersRequest, Unit>> CancelAllChildOrdersCallAsync(CancelAllChildOrdersRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string`
- request rule
  - `ProductCode` 必須
- response DTO
  - `Unit`
  - response body は decode しない
- `ExpectedStatus = 200`
- `ResponseShape = EmptyOrObject`
- write live test rule
  - dedicated marker `local/bitflyer-live-cancel-all-enabled` を要求する
  - `BTC_JPY` に固定して実行する
  - 実行前に `GetChildOrders(product_code=BTC_JPY, child_order_state=ACTIVE)` が empty であることを要求する
  - test が作成した deep limit child orders だけを `CancelAllChildOrders(BTC_JPY)` の対象とし、残留時は individual cancel cleanup を試みる
