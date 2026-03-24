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
| GetMarkets | GET | /v1/getmarkets | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | None | TBD |
| GetBoard | GET | /v1/getboard | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | None | TBD |
| GetTicker | GET | /v1/getticker | public | Yes | Yes | Phase1-Read | Transitional | Transitional | 200 | Object | No | None | /v1/ticker | None | product_code = null は query omitted |
| GetExecutionsPublic | GET | /v1/getexecutions | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | None | TBD |
| GetBoardState | GET | /v1/getboardstate | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | None | TBD |
| GetHealth | GET | /v1/gethealth | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | None | TBD |
| GetFundingRate | GET | /v1/getfundingrate | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | None | TBD |
| GetCorporateLeverage | GET | /v1/getcorporateleverage | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | None | TBD |
| GetChats | GET | /v1/getchats | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | None | TBD |
| GetPermissions | GET | /v1/me/getpermissions | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetBalance | GET | /v1/me/getbalance | private | Yes | Yes | Phase1-Read | Transitional | Transitional | 200 | Array | No | None | - | KeySecret | - |
| GetCollateral | GET | /v1/me/getcollateral | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetCollateralAccounts | GET | /v1/me/getcollateralaccounts | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetAddresses | GET | /v1/me/getaddresses | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetCoinIns | GET | /v1/me/getcoinins | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetCoinOuts | GET | /v1/me/getcoinouts | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetBankAccounts | GET | /v1/me/getbankaccounts | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetDeposits | GET | /v1/me/getdeposits | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| Withdraw | POST | /v1/me/withdraw | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | Yes | NotSupported | - | KeySecret | TBD |
| GetWithdrawals | GET | /v1/me/getwithdrawals | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| SendChildOrder | POST | /v1/me/sendchildorder | private | Yes | Yes | Phase2-Write | Transitional | Transitional | 200 | Object | Yes | Required | - | KeySecret | minute_to_expire/time_in_force = null omitted, price is conditional |
| SendParentOrder | POST | /v1/me/sendparentorder | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | Yes | Required | - | KeySecret | TBD |
| CancelChildOrder | POST | /v1/me/cancelchildorder | private | Yes | Yes | Phase2-Write | Transitional | Transitional | 200 | EmptyOrObject | Yes | None | - | KeySecret | exactly one of child_order_id or child_order_acceptance_id |
| CancelParentOrder | POST | /v1/me/cancelparentorder | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | Yes | None | - | KeySecret | TBD |
| CancelAllChildOrders | POST | /v1/me/cancelallchildorders | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | Yes | None | - | KeySecret | TBD |
| GetChildOrders | GET | /v1/me/getchildorders | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetParentOrders | GET | /v1/me/getparentorders | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetParentOrder | GET | /v1/me/getparentorder | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetExecutionsPrivate | GET | /v1/me/getexecutions | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetBalanceHistory | GET | /v1/me/getbalancehistory | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetPositions | GET | /v1/me/getpositions | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetCollateralHistory | GET | /v1/me/getcollateralhistory | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |
| GetTradingCommission | GET | /v1/me/gettradingcommission | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | None | - | KeySecret | TBD |

## Initial Rule

- Stage10 第1段階では `GetTicker`、`GetBalance`、`SendChildOrder` を先行し、その後 `CancelChildOrder` を追加実装対象に含める
- `GetTicker` と `GetBalance` は read path のため `Phase1-Read` とする
- `SendChildOrder` は write path のため `Phase2-Write` とする
- 初版では DTO 固定前のため、全 endpoint の `RequestDtoStatus` / `ResponseDtoStatus` は `Transitional` から開始する

## Implementation Order

- まず `GetTicker` を `Protocol` / `Native` 両方の template endpoint として新構成へ移す
- 次に `GetBalance` を同じ形で移し、top-level array 契約の扱いを固定する
- その後 `SendChildOrder` を移し、request encode が強い write endpoint の形を固定する
- `CancelChildOrder` は注文 lifecycle 補助 endpoint としてその後に追従させる
- `Composition` の wiring 変更は、少なくとも `GetTicker` と `GetBalance` の module 形が固まった後に行う

## Initial Endpoint Contracts

初期 4 endpoint の exact contract は以下とする。

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
