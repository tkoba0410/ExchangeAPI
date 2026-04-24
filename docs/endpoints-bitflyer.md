# Bitflyer Endpoint Matrix

最終更新: 2026-04-22  
位置づけ: bitFlyer venue ledger

本書は、bitFlyer venue の endpoint metadata、公開範囲、固定状況を管理する現行正本である。  
library 共通原則は [`docs/spec.md`](./spec.md) を参照し、本書では bitFlyer 固有の matrix と補助台帳だけを扱う。

現在のコード配置は本書の従属物であり、判断根拠にはしない。  
削除済み inventory や他の補助文書を前提にせず、本書自身を endpoint 正本として扱う。

注記:

- 本文中に残る `Stage10` は履歴ラベルであり、現行の優先順位は文書体系ガイドに従う
- 実装順や代表 contract 例は [`docs/archive/endpoint-history-and-examples.md`](./archive/endpoint-history-and-examples.md) に切り出して管理する

## Values

- `ExposeInProtocol`
  - `Yes`: 現行 bitFlyer slice の `Protocol` 公開面に含める
  - `Later`: 現行 slice ではまだ公開しない
- `ExposeInNative`
  - `Yes`: 現行 bitFlyer slice の `Native` 公開面に含める
  - `Later`: 現行 slice ではまだ公開しない
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
  - `NotSupported`: 現行 slice では write live test 対象にしない
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

## Vocabulary Notes

- `ProductCode`, `CurrencyCode`, `AccountType`, `ReasonCode` のような string field について、known values を `Vocabulary` project の `public static class` + `public const string` として置いてよい
- これらの known values は convenience 用であり、closed set や exhaustive inventory の正本として扱わない
- enum 化対象に上がっていない string field の validation は、known values 定数ではなく endpoint contract を正本とする

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
| Withdraw | POST | /v1/me/withdraw | private | Yes | Yes | Phase2-Write | Fixed | Fixed | 200 | Object | Yes | NotSupported | - | KeySecret | currency_code/bank_account_id/amount/code required |
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

## Current Rule

- 現行 library 公開面では `GetMarkets`、`GetBoard`、`GetTicker`、`GetExecutionsPublic`、`GetBoardState`、`GetHealth`、`GetFundingRate`、`GetCorporateLeverage`、`GetChats`、`GetPermissions`、`GetBalance`、`GetCollateral`、`GetCollateralAccounts`、`GetAddresses`、`GetCoinIns`、`GetCoinOuts`、`GetBankAccounts`、`GetDeposits`、`Withdraw`、`GetWithdrawals`、`GetChildOrders`、`GetParentOrders`、`GetParentOrder`、`GetExecutionsPrivate`、`GetBalanceHistory`、`GetPositions`、`GetCollateralHistory`、`GetTradingCommission`、`SendChildOrder`、`SendParentOrder`、`CancelChildOrder`、`CancelParentOrder`、`CancelAllChildOrders` を含める
- live test の repo 共通 opt-in ルールは `docs/spec.md` の Test 契約を正本とする
- bitFlyer private read live test は credentials source を解決できる場合にのみ実行する
- `SendChildOrder` と `CancelChildOrder` は `Phase2-Write`、`CancelAllChildOrders` は dedicated marker と `BTC_JPY` preflight empty check を持つ `Phase2-Write` とする
- `GetMarkets`、`GetTicker`、`GetBalance`、`GetCollateral`、`GetCollateralAccounts`、`GetTradingCommission` は first wave として `Fixed` に上げる
- `GetBoard`、`GetExecutionsPublic`、`GetBoardState`、`GetHealth`、`GetFundingRate`、`GetCorporateLeverage`、`GetChats`、`GetAddresses`、`GetBankAccounts` は second wave として `Fixed` に上げる
- `GetPermissions`、`GetCoinIns`、`GetCoinOuts`、`GetDeposits`、`GetWithdrawals`、`GetChildOrders`、`GetParentOrders`、`GetExecutionsPrivate`、`GetBalanceHistory`、`GetPositions`、`GetCollateralHistory` は third wave として `Fixed` に上げる
- `SendChildOrder` と `CancelChildOrder` は non-fill lifecycle を前提に fourth wave として `Fixed` に上げる
- `SendParentOrder`、`GetParentOrder`、`CancelParentOrder` は parent non-fill lifecycle を前提に fifth wave として `Fixed` に上げる
- `CancelAllChildOrders` は `BTC_JPY` 専用 safety gate と preflight を前提に sixth wave として `Fixed` に上げる
- `Withdraw` は cleanup 不可のため success live write target には含めないが、dedicated negative live contract を前提に seventh wave として `Fixed` に上げる
- `Phase1-Read` に含めた read endpoint は third wave までで `Fixed` に上げ切る
- `Later` の read と write を含む残りの実装済み endpoint は、引き続き `Transitional` のまま段階的に固定する

## 時刻フィールド一覧

bitFlyer の timestamp field は offset なし文字列が多く、API 文書上も timezone 記述が揃っていない。  
そのため、timestamp については `仕様書の状態` と `実装の状態` を分けて管理する。

現行の working hypothesis:

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

## Representative Contract Notes

実装済み endpoint の公開有無と contract metadata の正本は matrix とする。  
代表 contract 例、実装順、旧 bootstrap 文脈は [`docs/archive/endpoint-history-and-examples.md`](./archive/endpoint-history-and-examples.md) を参照する。

- bitFlyer venue ledger では endpoint ごとの exact contract を本文へ展開しない
- facade/request/response の代表例や旧実装順は archive 側の履歴文書で保持する
