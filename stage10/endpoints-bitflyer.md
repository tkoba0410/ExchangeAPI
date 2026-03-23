# Stage10 Endpoint Matrix — Bitflyer

本書は、[endpoints-bitflyer.md](/home/tkoba/dev/tkoba0410/ExchangeAPI/docs/inventory/endpoints-bitflyer.md) を入力として作成した Stage10 用の判断表である。  
inventory が事実一覧であるのに対し、本書は Stage10 第1段階の実装・DTO 固定・live test 導入順を管理する。

Stage10 では本書を endpoint 運用正本とし、既存 inventory は import source に留める。  
現在の Stage10 コード配置は本書の従属物であり、判断根拠にはしない。

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
- `ResponseShape`
  - `Object`: top-level object
  - `Array`: top-level array
  - `EmptyOrObject`: empty body または top-level object
  - `TBD`: 後段で確定する
- `WritesState`
  - `Yes`: venue state を変更する
  - `No`: read-only
- `NeedsCleanup`
  - `Yes`: live test 後に cleanup が必要
  - `No`: cleanup 不要
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
  は別途 `stage10.md` の物理構成方針に従う

## Matrix

| EndpointId | Method | Path | Scope | ExposeInProtocol | ExposeInNative | LiveTestPhase | RequestDtoStatus | ResponseDtoStatus | ExpectedStatus | ResponseShape | WritesState | NeedsCleanup | AliasPath | AuthType | OptionalOmissionRule |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| GetMarkets | GET | /v1/getmarkets | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | None | TBD |
| GetBoard | GET | /v1/getboard | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | None | TBD |
| GetTicker | GET | /v1/getticker | public | Yes | Yes | Phase1-Read | Transitional | Transitional | 200 | Object | No | No | /v1/ticker | None | product_code = null は query omitted |
| GetExecutionsPublic | GET | /v1/getexecutions | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | None | TBD |
| GetBoardState | GET | /v1/getboardstate | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | None | TBD |
| GetHealth | GET | /v1/gethealth | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | None | TBD |
| GetFundingRate | GET | /v1/getfundingrate | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | None | TBD |
| GetCorporateLeverage | GET | /v1/getcorporateleverage | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | None | TBD |
| GetChats | GET | /v1/getchats | public | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | None | TBD |
| GetPermissions | GET | /v1/me/getpermissions | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetBalance | GET | /v1/me/getbalance | private | Yes | Yes | Phase1-Read | Transitional | Transitional | 200 | Array | No | No | - | KeySecret | - |
| GetCollateral | GET | /v1/me/getcollateral | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetCollateralAccounts | GET | /v1/me/getcollateralaccounts | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetAddresses | GET | /v1/me/getaddresses | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetCoinIns | GET | /v1/me/getcoinins | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetCoinOuts | GET | /v1/me/getcoinouts | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetBankAccounts | GET | /v1/me/getbankaccounts | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetDeposits | GET | /v1/me/getdeposits | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| Withdraw | POST | /v1/me/withdraw | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | Yes | TBD | - | KeySecret | TBD |
| GetWithdrawals | GET | /v1/me/getwithdrawals | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| SendChildOrder | POST | /v1/me/sendchildorder | private | Yes | Yes | Phase2-Write | Transitional | Transitional | 200 | Object | Yes | Yes | - | KeySecret | minute_to_expire/time_in_force = null omitted, price is conditional |
| SendParentOrder | POST | /v1/me/sendparentorder | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | Yes | TBD | - | KeySecret | TBD |
| CancelChildOrder | POST | /v1/me/cancelchildorder | private | Yes | Yes | Later | Transitional | Transitional | 200 | EmptyOrObject | Yes | No | - | KeySecret | exactly one of child_order_id or child_order_acceptance_id |
| CancelParentOrder | POST | /v1/me/cancelparentorder | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | Yes | TBD | - | KeySecret | TBD |
| CancelAllChildOrders | POST | /v1/me/cancelallchildorders | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | Yes | TBD | - | KeySecret | TBD |
| GetChildOrders | GET | /v1/me/getchildorders | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetParentOrders | GET | /v1/me/getparentorders | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetParentOrder | GET | /v1/me/getparentorder | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetExecutionsPrivate | GET | /v1/me/getexecutions | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetBalanceHistory | GET | /v1/me/getbalancehistory | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetPositions | GET | /v1/me/getpositions | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetCollateralHistory | GET | /v1/me/getcollateralhistory | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |
| GetTradingCommission | GET | /v1/me/gettradingcommission | private | Later | Later | Later | Transitional | Transitional | TBD | TBD | No | No | - | KeySecret | TBD |

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
