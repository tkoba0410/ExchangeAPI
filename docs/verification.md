# ExchangeAPI Verification Policy

最終更新: 2026-04-24  
位置づけ: verification 正本

本書は、endpoint ごとの API 契約分類をもとに、live / manual verification の扱いを決めるための正本である。

`docs/spec.md` は library 共通の test 契約を定義する。  
`docs/endpoints-bitflyer.md` / `docs/endpoints-binance.md` は endpoint metadata の正本を定義する。  
本書はそれらを入力にして、実行時の危険度、配置、証跡管理を決める。

## 1. 基本方針

- verification の判断は endpoint ごとに行う
- API 契約分類と verification 運用分類を混ぜない
- `public/private`, `read/write`, `CleanupPolicy`, `LiveTestPhase` は endpoint matrix の責務とする
- `safe/tolerable/dangerous`, `repo/local`, evidence 管理は本書の責務とする
- `tests/` は deterministic な契約固定を中心に保つ
- live / manual verification の本体は `tests/` または `verification/` へ置く
- 実行結果、artifact、log、手動確認メモは `local/evidence/` へ集約する

## 2. 判定軸

endpoint matrix から読む API 契約軸:

- `Scope`
  - `public`
  - `private`
- `WritesState`
  - `No`
  - `Yes`
- `CleanupPolicy`
  - `None`
  - `Required`
  - `NotSupported`
- `LiveTestPhase`
  - `Phase1-Read`
  - `Phase2-Write`
  - `Later`
- `AuthType`
  - `None`
  - `KeySecret`
- `ResponseShape`

verification 側で決める運用軸:

- `RiskClass`
  - `safe`
  - `tolerable`
  - `dangerous`
- `ExecutionPlace`
  - `tests`
  - `verification`
- `EvidencePlace`
  - `local/evidence/static`
  - `local/evidence/verification`
  - `local/evidence/local-live`
  - `local/evidence/test-operation`
- `Evidence`
  - 不要
  - 任意
  - 必須
- `Automation`
  - CI deterministic
  - opt-in live
  - manual only

## 3. RiskClass

### safe

`safe` は、実行しても venue state や資産状態を変更しない verification である。

該当条件:

- public read endpoint
- private read endpoint
- market data / account snapshot / order history / execution history などの参照系
- 認証は必要でも、注文、キャンセル、入出金、残高変動を起こさない endpoint

扱い:

- deterministic test は `tests/` に置く
- live verification は global opt-in と必要な credentials 条件で実行してよい
- evidence は原則任意で、残す場合は `local/evidence/` 配下へ置く
- MCP inspection read tool の候補にしてよい

### tolerable

`tolerable` は、venue state を変更するが、影響範囲を限定でき、cleanup または安全条件で実害を抑えられる verification である。

該当条件:

- 最小数量、最小影響で実行できる注文系
- cleanup endpoint が存在する
- dedicated marker と preflight で実行条件を限定できる
- 失敗時の影響が局所的で、利用者が許容判断できる

最低条件:

- global live opt-in に加えて endpoint group 専用 marker を要求する
- 対象 product / symbol を runbook または test code で固定する
- request は venue の最小数量またはそれに準じる最小影響値を使う
- cleanup endpoint がある場合は同じ verification 内で cleanup を必ず試みる
- cleanup failure は silent ignore せず、実行結果と evidence に残す
- 通常 preflight と `run-safe-live-tests.sh` には含めない

扱い:

- CI 既定実行には含めない
- `verification/` に runbook または live verification code を置く
- 実行には global opt-in、credentials、dedicated marker を要求する
- cleanup 手順と失敗時の扱いを同じ verification に含める
- evidence は原則 `local/evidence/verification/` または `local/evidence/local-live/` に残す

### dangerous

`dangerous` は、影響範囲が広い、cleanup ができない、資産移動を伴う、または誤実行時の実害が大きい verification である。

該当条件:

- 入金、出金
- 全キャンセルなど、対象範囲が広い操作
- cleanup 不可の state-changing endpoint
- success path を実行すると資産移動や不可逆な状態変化が起きる endpoint

扱い:

- CI 既定実行には含めない
- 通常の opt-in live test に含めない
- 原則 manual only とする
- negative contract だけを確認できる場合でも dedicated marker と runbook を要求する
- evidence は `local/evidence/verification/` または `local/evidence/test-operation/` に残す

## 4. 決定手順

endpoint ごとの live / manual verification は、次の順で決める。

1. endpoint matrix で `Scope`, `WritesState`, `CleanupPolicy`, `LiveTestPhase`, `AuthType` を確認する。
2. `WritesState = No` なら `safe` とする。
3. `WritesState = Yes` かつ `CleanupPolicy = Required` なら、cleanup と最小影響条件を確認して `tolerable` にできるか判断する。
4. `WritesState = Yes` かつ `CleanupPolicy = NotSupported` なら、原則 `dangerous` とする。
5. `CleanupPolicy = None` でも、実行対象が広い write endpoint は `dangerous` に上げる。
6. `dangerous` endpoint は success path の live automation を避け、必要なら negative contract / manual verification に限定する。
7. 決定した `RiskClass` に応じて test 本体を `tests/` または `verification/` へ置く。
8. 実行結果、artifact、log、手動確認メモを残す場合は `local/evidence/` 配下の phase 別 directory へ置く。

dangerous endpoint の negative contract は success path と別扱いにする。negative contract は、資産移動や不可逆操作が発生しないことを request 条件で説明でき、dedicated marker と runbook を持つ場合だけ実行してよい。

## 5. 物理構成

`tests/`:

- unit test
- endpoint module contract test
- adapter contract test
- migration lock test
- deterministic replay test

`verification/`:

- live / manual verification code
- runbook
- scenario
- replay input template
- endpoint ごとの実行条件説明

`local/evidence/`:

- `static/`
  - build / unit / integration test の追加証跡
- `verification/`
  - 実 API 接続前の検証、replay、限定 verification の証跡
- `local-live/`
  - ローカル環境で実 API に近い形で確認した証跡
- `test-operation/`
  - 実運用に近い継続稼働や operator 手順に基づく確認証跡

各 evidence run の標準構成:

```text
local/evidence/<phase>/<yyyymmdd>-<label>/
  runtime/
    artifacts/
    logs/
  notes/
```

- `runtime/artifacts/` には `summary.json`, `cycle.json`, sanitize 済み replay artifact などを置く
- `runtime/logs/` には protocol debug log、adapter log、verification 実行 log などを置く
- `notes/` には operator の判断、異常、再現手順、気づきなどを置く
- `local/evidence/` 配下は repository の正本ではない
- credentials、署名値、API key / secret を evidence に含めてはならない

`local/app/` は ExchangeAPI には導入しない。  
ExchangeAPI は library repo であり、通常実行アプリの I/O 正本を持たないためである。

## 6. 初期 Endpoint Inventory

本表は verification 判断の初期台帳である。  
API 契約の正本は endpoint matrix であり、本表は live / manual verification の扱いだけを固定する。

### bitFlyer

| Endpoint | API Contract | RiskClass | Verification Handling |
| --- | --- | --- | --- |
| GetMarkets | public read | safe | opt-in live read |
| GetBoard | public read | safe | opt-in live read |
| GetTicker | public read | safe | opt-in live read |
| GetExecutionsPublic | public read | safe | opt-in live read |
| GetBoardState | public read | safe | opt-in live read |
| GetHealth | public read | safe | opt-in live read |
| GetFundingRate | public read | safe | opt-in live read |
| GetCorporateLeverage | public read | safe | opt-in live read |
| GetChats | public read | safe | opt-in live read |
| GetPermissions | private read | safe | opt-in live read with credentials |
| GetBalance | private read | safe | opt-in live read with credentials |
| GetCollateral | private read | safe | opt-in live read with credentials |
| GetCollateralAccounts | private read | safe | opt-in live read with credentials |
| GetAddresses | private read | safe | opt-in live read with credentials |
| GetCoinIns | private read | safe | opt-in live read with credentials |
| GetCoinOuts | private read | safe | opt-in live read with credentials |
| GetBankAccounts | private read | safe | opt-in live read with credentials |
| GetDeposits | private read | safe | opt-in live read with credentials |
| GetWithdrawals | private read | safe | opt-in live read with credentials |
| GetChildOrders | private read | safe | opt-in live read with credentials |
| GetParentOrders | private read | safe | opt-in live read with credentials |
| GetParentOrder | private read by id | safe | known-id manual read; automated coverage is currently paired with parent-order write lifecycle |
| GetExecutionsPrivate | private read | safe | opt-in live read with credentials |
| GetBalanceHistory | private read | safe | opt-in live read with credentials |
| GetPositions | private read | safe | opt-in live read with credentials |
| GetCollateralHistory | private read | safe | opt-in live read with credentials |
| GetTradingCommission | private read | safe | opt-in live read with credentials |
| SendChildOrder | private write, cleanup required | tolerable | dedicated opt-in live with cleanup |
| SendParentOrder | private write, cleanup required | tolerable | dedicated opt-in live with cleanup |
| CancelChildOrder | private write, targeted cleanup/action | tolerable | paired with order lifecycle verification |
| CancelParentOrder | private write, targeted cleanup/action | tolerable | paired with parent order lifecycle verification |
| CancelAllChildOrders | private write, broad cancellation | dangerous | manual or dedicated marker with strict preflight |
| Withdraw | private write, asset movement | dangerous | no success live automation; wrong-code negative contract only with dedicated marker and runbook |

### Binance

| Endpoint | API Contract | RiskClass | Verification Handling |
| --- | --- | --- | --- |
| GetKlines | public read | safe | opt-in live read with closed window comparison |

## 7. LiveTests との関係

既存の `LiveTests` は削除対象ではない。  
役割は、real venue との drift detection と protocol/native parity の確認である。

ただし、`LiveTests` だけを verification の正本にしない。  
endpoint ごとの危険度、manual 実行条件、evidence 保存先は本書と `verification/` 側で管理する。  
`LiveTests` の実行結果を後から確認したい場合は、端末出力や CI 結果そのものではなく、必要な範囲を `local/evidence/static/` または `local/evidence/verification/` に整理して残す。

safe live verification をまとめて実行する場合は、次を使う。

```bash
bash scripts/run-safe-live-tests.sh
```

この script は `EXCHANGEAPI_RUN_LIVE_TESTS=1` を一時設定し、Binance live tests、bitFlyer read live tests、MCP server live tests を実行する。
credentials は `local/credentials/credential-profile.json` から解決する。API key 読み込みに環境変数は使わない。
credentials が未設定の場合、bitFlyer private read と MCP private read は skip される。
write 系 live test は dedicated marker の有無にかかわらず safe runner から除外する。

## 8. MCP との関係

MCP は read-only 情報取得を広く支援してよい。  
ただし、MCP tool 追加可否は `RiskClass = safe` を前提に判断する。

MCP で扱わないもの:

- 注文
- キャンセル
- 入金
- 出金
- その他の state-changing operation

private read endpoint は、認証を要しても `safe` であれば inspection read tool の候補にできる。
