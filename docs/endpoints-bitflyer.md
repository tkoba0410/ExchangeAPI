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

## 時刻フィールド一覧

bitFlyer の timestamp field は offset なし文字列が多く、API 文書上も timezone 記述が揃っていない。  
そのため、timestamp については `仕様書の状態` と `実装の状態` を分けて管理する。

Stage10 の working hypothesis:

- bitFlyer は内部時刻を概ね UTC 基準で管理していると仮定する
- 口座変動履歴の `trade_date` は日本向け確認用の JST 例外とみなす
- timezone undocumented な no-offset timestamp は、反証が出るまで UTC と仮定して decode 境界で UTC 正規化する

- `仕様状態`
  - `UTC documented`: bitFlyer API 文書に UTC と明記あり
  - `JST documented`: bitFlyer API 文書に JST と明記あり
  - `Timezone undocumented`: bitFlyer API 文書に timezone 明記なし
- `実装状態`
  - `Generic parse, not normalized`: `DateTimeOffset.TryParse(..., DateTimeStyles.None, ...)` に依存し、timezone 解釈を固定していない
  - `JST->UTC normalized`: offset なし値を JST として解釈し、内部正本では UTC に正規化済み
  - `Documented UTC normalized`: UTC documented field を UTC として明示解釈し、内部正本では UTC に正規化済み
  - `Hypothesized UTC normalized`: timezone undocumented field を UTC working hypothesis で解釈し、内部正本では UTC に正規化済み
- `確認状態`
  - `Documented`: bitFlyer API 文書に timezone 記述がある
  - `Observed`: live test / live log で実値観測あり
  - `Documented + Observed`: 文書記述と live 観測の両方あり
  - `Unverified`: 文書記述も live 観測も正本に未反映

| Endpoint | Field | 仕様状態 | 実装状態 | 確認状態 | 備考 |
| --- | --- | --- | --- | --- | --- |
| GetTicker | `timestamp` | UTC documented | Documented UTC normalized | Documented + Observed | API 文書に「UTC（協定世界時）」明記あり |
| GetFundingRate | `next_funding_rate_settledate` | UTC documented | Documented UTC normalized | Documented + Observed | API 文書に「UTC（協定世界時）」明記あり |
| GetBalanceHistory | `event_date` | UTC documented | Documented UTC normalized | Documented + Observed | API 文書に「UTC（協定世界時）」明記あり |
| GetBalanceHistory | `trade_date` | JST documented | JST->UTC normalized | Documented + Observed | API 文書に「JST（日本標準時, UTC+9）」明記あり |
| GetExecutionsPublic | `exec_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetExecutionsPrivate | `exec_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetCorporateLeverage | `current_startdate` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetCorporateLeverage | `next_startdate` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetChats | `date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetCollateral | `margin_call_due_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | endpoint 専用 optional timestamp parser を使わず shared UTC 仮説 parser を使う |
| GetCoinIns | `event_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetCoinOuts | `event_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetDeposits | `event_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetWithdrawals | `event_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetChildOrders | `expire_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetChildOrders | `child_order_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetParentOrders | `expire_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetParentOrders | `parent_order_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetParentOrder | `expire_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetPositions | `open_date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |
| GetCollateralHistory | `date` | Timezone undocumented | Hypothesized UTC normalized | Observed | response 例のみで timezone 記述なし |

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
- 引数一覧

| Parameter | Location | Required | Omission | Type/Domain | Notes |
| --- | --- | --- | --- | --- | --- |
| `product_code` | query | No | `null` のとき omitted | string | omitted 時は bitFlyer 側既定値 `BTC_JPY` |
| `count` | query | No | `null` のとき omitted | positive int | paging |
| `before` | query | No | `null` のとき omitted | positive long | paging |
| `after` | query | No | `null` のとき omitted | positive long | paging |
| `child_order_state` | query | No | `null` のとき omitted | enum | child order status filter |
| `child_order_id` | query | No | `null` のとき omitted | string | entity id filter |
| `child_order_acceptance_id` | query | No | `null` のとき omitted | string | acceptance id filter |
| `parent_order_id` | query | No | `null` のとき omitted | string | parent linkage filter |
- enum一覧

| Field | Allowed Values | Meaning / Notes |
| --- | --- | --- |
| `child_order_state` | `ACTIVE`, `COMPLETED`, `CANCELED`, `EXPIRED`, `REJECTED` | filter domain |
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

### Withdraw

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> WithdrawCallAsync(string bodyJson, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<WithdrawRequest, WithdrawResponse>> WithdrawCallAsync(WithdrawRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `CurrencyCode: string`
  - `BankAccountId: long`
  - `Amount: decimal`
  - `Code: string`
- request rule
  - `CurrencyCode` 必須
  - `CurrencyCode` は現行実装では `JPY` のみ許容
  - `BankAccountId` は正数
  - `Amount` は正数
  - `Code` 必須
- 引数一覧

| Parameter | Location | Required | Omission | Type/Domain | Notes |
| --- | --- | --- | --- | --- | --- |
| `currency_code` | body | Yes | omitted 不可 | string | 現行実装では `JPY` のみ |
| `bank_account_id` | body | Yes | omitted 不可 | positive long | 出金先口座 id |
| `amount` | body | Yes | omitted 不可 | positive decimal | 出金額 |
| `code` | body | Yes | omitted 不可 | string | 二段階認証コード |
- enum一覧

| Field | Allowed Values | Meaning / Notes |
| --- | --- | --- |
| `currency_code` | `JPY` | 現行実装の許容値 |
- response DTO
  - `MessageId: string`
- response note
  - `200` かつ `message_id` があれば成功
  - `200` かつ負の `status` を持つ error body は `Semantic`
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
- 引数一覧

| Parameter | Location | Required | Omission | Type/Domain | Notes |
| --- | --- | --- | --- | --- | --- |
| `product_code` | body | Yes | omitted 不可 | string | venue product |
| `child_order_type` | body | Yes | omitted 不可 | enum | `LIMIT` / `MARKET` |
| `side` | body | Yes | omitted 不可 | enum | `BUY` / `SELL` |
| `price` | body | Conditional | `null` のとき omitted | decimal | `LIMIT` では必須、`MARKET` では omitted |
| `size` | body | Yes | omitted 不可 | positive decimal | order size |
| `minute_to_expire` | body | No | `null` のとき omitted | positive int | expiration minutes |
| `time_in_force` | body | No | `null` のとき omitted | enum | execution policy |
- enum一覧

| Field | Allowed Values | Meaning / Notes |
| --- | --- | --- |
| `child_order_type` | `LIMIT`, `MARKET` | child order type |
| `side` | `BUY`, `SELL` | order side |
| `time_in_force` | `GTC`, `IOC`, `FOK` | execution policy |
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
- 引数一覧

| Parameter | Location | Required | Omission | Type/Domain | Notes |
| --- | --- | --- | --- | --- | --- |
| `product_code` | body | Yes | omitted 不可 | string | venue product |
| `child_order_id` | body | Conditional | `null` のとき omitted | string | entity id |
| `child_order_acceptance_id` | body | Conditional | `null` のとき omitted | string | acceptance id |

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
- 引数一覧

| Parameter | Location | Required | Omission | Type/Domain | Notes |
| --- | --- | --- | --- | --- | --- |
| `order_method` | body | No | `null` のとき omitted | enum | omitted 時は `SIMPLE` 扱い |
| `minute_to_expire` | body | No | `null` のとき omitted | positive int | expiration minutes |
| `time_in_force` | body | No | `null` のとき omitted | enum | execution policy |
| `parameters` | body | Yes | omitted 不可 | array | `order_method` に対応する件数が必要 |
- `parameters[]` 一覧

| Parameter | Location | Required | Omission | Type/Domain | Notes |
| --- | --- | --- | --- | --- | --- |
| `product_code` | body.parameters[] | Yes | omitted 不可 | string | venue product |
| `condition_type` | body.parameters[] | Yes | omitted 不可 | enum | execution condition |
| `side` | body.parameters[] | Yes | omitted 不可 | enum | `BUY` / `SELL` |
| `price` | body.parameters[] | Conditional | `null` のとき omitted | decimal | `LIMIT` / `STOP_LIMIT` で必須 |
| `size` | body.parameters[] | Yes | omitted 不可 | positive decimal | order size |
| `trigger_price` | body.parameters[] | Conditional | `null` のとき omitted | decimal | `STOP` / `STOP_LIMIT` で必須 |
| `offset` | body.parameters[] | Conditional | `null` のとき omitted | long | `TRAIL` で必須 |
- enum一覧

| Field | Allowed Values | Meaning / Notes |
| --- | --- | --- |
| `order_method` | `SIMPLE`, `IFD`, `OCO`, `IFDOCO` | parent order method |
| `time_in_force` | `GTC`, `IOC`, `FOK` | execution policy |
| `parameters[].condition_type` | `LIMIT`, `MARKET`, `STOP`, `STOP_LIMIT`, `TRAIL` | parameter condition |
| `parameters[].side` | `BUY`, `SELL` | order side |
- response DTO
  - `ParentOrderAcceptanceId: string`
- `ExpectedStatus = 200`
- `ResponseShape = Object`

### GetParentOrders

- `Protocol` facade
  - `Task<Call<ProtocolRequest, ProtocolResponse>> GetParentOrdersCallAsync(string? productCode, int? count, long? before, long? after, string? parentOrderState, CancellationToken cancellationToken = default)`
- `Native` facade
  - `Task<Call<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> GetParentOrdersCallAsync(GetParentOrdersRequest request, CancellationToken cancellationToken = default)`
- request DTO
  - `ProductCode: string?`
  - `Count: int?`
  - `Before: long?`
  - `After: long?`
  - `ParentOrderState: string?`
- request rule
  - `ProductCode = null` のとき query omitted
  - `Count`、`Before`、`After` は指定時に正数
  - `ParentOrderState` は `ACTIVE` / `COMPLETED` / `CANCELED` / `EXPIRED` / `REJECTED` のいずれか
  - `null` の optional query は omitted
- 引数一覧

| Parameter | Location | Required | Omission | Type/Domain | Notes |
| --- | --- | --- | --- | --- | --- |
| `product_code` | query | No | `null` のとき omitted | string | venue product filter |
| `count` | query | No | `null` のとき omitted | positive int | paging |
| `before` | query | No | `null` のとき omitted | positive long | paging |
| `after` | query | No | `null` のとき omitted | positive long | paging |
| `parent_order_state` | query | No | `null` のとき omitted | enum | parent order status filter |
- enum一覧

| Field | Allowed Values | Meaning / Notes |
| --- | --- | --- |
| `parent_order_state` | `ACTIVE`, `COMPLETED`, `CANCELED`, `EXPIRED`, `REJECTED` | filter domain |
- response DTO
  - top-level array
  - `GetParentOrders.Item`
    - `Id: long`
    - `ParentOrderId: string`
    - `ProductCode: string`
    - `Side: string`
    - `ParentOrderType: string`
    - `Price: decimal`
    - `AveragePrice: decimal`
    - `Size: decimal`
    - `ParentOrderState: string`
    - `ExpireDate: DateTimeOffset`
    - `ParentOrderDate: DateTimeOffset`
    - `ParentOrderAcceptanceId: string`
    - `OutstandingSize: decimal`
    - `CancelSize: decimal`
    - `ExecutedSize: decimal`
    - `TotalCommission: decimal`
- `ExpectedStatus = 200`
- `ResponseShape = Array`

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
- 引数一覧

| Parameter | Location | Required | Omission | Type/Domain | Notes |
| --- | --- | --- | --- | --- | --- |
| `product_code` | body | Yes | omitted 不可 | string | venue product |
| `parent_order_id` | body | Conditional | `null` のとき omitted | string | entity id |
| `parent_order_acceptance_id` | body | Conditional | `null` のとき omitted | string | acceptance id |
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
- 引数一覧

| Parameter | Location | Required | Omission | Type/Domain | Notes |
| --- | --- | --- | --- | --- | --- |
| `product_code` | body | Yes | omitted 不可 | string | venue product |
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
