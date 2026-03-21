# Stage10（ゴール / 第1段階）

最終更新: 2026-03-20  
対象ブランチ: `stage10`

## 1. 位置づけ

Stage10 は、Stage9 で固定した構造・品質軸・レビュー運用を前提に、
「実環境で実用に耐えるか」を確認するライブ検証フェーズである。  
目的は機能追加ではなく、bitFlyer 実装の実運用上の不確実性を
Wire / Raw / Normalized の層別に減らすことにある。

Stage9 で固定した事項（品質軸、深度モデル、重大度運用、最終ゲート）は
本 Stage でも前提条件として維持し、再交渉しない。

運用正本（Process SSOT）:
- `docs/process/review-framework.md`
- `docs/process/codex-review-runbook.md`
- `docs/process/process.md`
- `stage9.md`

---

## 2. Stage10 のゴール

- bitFlyer の REST 実装について、実ネットワーク・実資格情報を使った live test を成立させる
- live 実行失敗時に、Wire / Raw / Normalized のどこで崩れたかを切り分けられるようにする
- Public GET / Private GET / 限定 Private POST の証跡を揃え、実用投入可否を判断できる状態にする
- Contracts 層の live test は本段階では扱わず、bitFlyer 固有層の検証を優先する

---

## 3. 第1段階のスコープ

### In Scope

- Exchange: `bitFlyer` のみ
- Layers: `Wire` / `Raw` / `Normalized`
- `Live Public GET`
- `Live Private GET`
- `Live Private POST` は以下の 2 endpoint のみ
  - `/v1/me/sendchildorder`
  - `/v1/me/cancelchildorder`

### Out of Scope

- `Contracts` 層の live test
- `Bittrade`
- WebSocket / streaming
- `sendparentorder` / `cancelparentorder` / `cancelallchildorders` / `withdraw` など、上記 2 本以外の Private POST
- デフォルト CI 経路への live test の常設組み込み

---

## 4. 対象 endpoint

### Public GET（コア）

- `GetTicker`
- `GetBoard`
- `GetExecutionsPublic`

### Private GET（コア）

- `GetBalance`
- `GetChildOrders`
- `GetExecutionsPrivate`

### Private POST（必須）

- `SendChildOrder`
- `CancelChildOrder`

### 拡張候補（第1段階の枠内で後追い可）

- `GetHealth`
- `GetBoardState`
- `GetPermissions`
- `GetCollateral`
- `GetCollateralAccounts`
- `GetPositions`
- その他、`docs/inventory/endpoints-bitflyer.md` 上で `PresentIn = Wire, Raw, Normalized` の GET endpoint

---

## 5. 検証単位

- `Wire`: method / path / query / body / auth 前提と live HTTP 応答を確認する
- `Raw`: DTO デシリアライズ成功と endpoint 単位の応答整合を確認する
- `Normalized`: 市場解決、意味変換、ID 変換、Call 結果の整合を確認する

Private POST は単発確認ではなく、次のライフサイクル試験として扱う。

1. `SendChildOrder` で `child_order_acceptance_id` を取得する
2. `GetChildOrders` で対象注文が見えることを確認する
3. `CancelChildOrder` で取消する
4. `GetChildOrders` で `ACTIVE` から消えることを確認する

---

## 6. 安全運用ルール

- live test は bitFlyer の専用テスト口座で実施する
- 資格情報はリポジトリに置かず、既存のテンプレート運用に従う
- POST は最小数量・現物市場・即時約定しにくい指値を原則とする
- 成行注文は禁止する
- `SendChildOrder` 後に `CancelChildOrder` できなかった注文は blocker とし、放置して close しない
- live test は明示 opt-in 実行とし、通常の `dotnet test` / CI 既定経路には混ぜない
- 実行ログには API key / secret / 秘密鍵 / 平文資格情報を残さない
- 実行ログはサニタイズ済み JSONL として保存し、order/account 系 identifier は pseudonymize する

---

## 7. 実装成果物

- bitFlyer live test 用の専用 test project
- `Wire` / `Raw` / `Normalized` ごとに実行可能な live test セット
- `Public GET` / `Private GET` / `Private POST` を `Trait(Category / Flow / Layer)` で分離実行できる仕組み
- live test 実行に必要な環境変数、資格情報導線、実行手順の文書
- 実行結果を保存できる証跡フォーマット
  - `docs/process/reviews/templates/STAGE10-LIVE-EVIDENCE.md`
- 自動保存される sanitized live log
  - `artifacts/live-logs/bitflyer/<run-id>/run.json`
  - `artifacts/live-logs/bitflyer/<run-id>/events.jsonl`

### 実行例

- 全 live test
  `dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --nologo --verbosity minimal`
- `Public GET` のみ
  `dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --filter "Category=Live&Flow=PublicGet"`
- `Private GET` のみ
  `dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --filter "Category=Live&Flow=PrivateGet"`
- `Private POST` のみ
  `dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --filter "Category=Live&Flow=PrivatePost"`
- `Normalized` 層のみ
  `dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --filter "Category=Live&Layer=Normalized"`

---

## 8. DoD

- bitFlyer live test 専用プロジェクトが追加され、通常テストから分離されている
- `Public GET` コア 3 本が `Wire` / `Raw` / `Normalized` の各層で成功する
- `Private GET` コア 3 本が `Wire` / `Raw` / `Normalized` の各層で成功する
- `SendChildOrder` / `CancelChildOrder` のライフサイクル試験が成功する
- ライフサイクル試験後に未取消の `ACTIVE` 注文が残らない
- 失敗時ログで endpoint / layer / HTTP status / exchange error を追跡できる
- 実行ごとの sanitized request / response / error ログ保存先を証跡へ残せる
- 資格情報運用と実行手順が文書化されている
- `Contracts` が本段階では未着手であることが文書上明示されている

---

## 9. 完了判定

Stage10 第1段階の完了は、上記 DoD を満たし、
live test 証跡とレビュー結果に基づき Maintainer が可否を裁定する。

Public / Private GET の拡張候補が未完でも、
コア対象と Private POST ライフサイクルが安定し、
未解消 blocker がなければ第1段階は close できる。

---

## 10. 実施状況（2026-03-20）

- bitFlyer 専用 live test project を追加済み
- `Public GET` 3 本、`Private GET` 3 本、`SendChildOrder -> GetChildOrders -> CancelChildOrder` のライフサイクル試験を `Wire` / `Raw` / `Normalized` で実装済み
- 実行単位は `Trait(Category=Live, Flow=PublicGet|PrivateGet|PrivatePost, Layer=Wire|Raw|Normalized)` で分離可能
- 2026-03-20 に以下で live 実行し、`21 passed / 0 failed / 0 skipped` を確認済み  
  `EXCHANGEAPI_BITFLYER_LIVE=1 EXCHANGEAPI_BITFLYER_LIVE_ALLOW_POST=1 EXCHANGEAPI_BITFLYER_LIVE_ORDER_SIDE=BUY EXCHANGEAPI_BITFLYER_LIVE_ORDER_SIZE=0.001 EXCHANGEAPI_BITFLYER_LIVE_ORDER_PRICE=9000000 dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --nologo --verbosity minimal`
- live 検証で発見した本体修正として、private auth timestamp のミリ秒化、および `CancelChildOrder` 系の `200 + empty body` 成功処理を反映済み
- live test の認証導線は direct env と既存 age 資格情報運用の両方をサポートし、`~/.config/exchangeapi/...` を既定値として使える
- live test 実行時は sanitized request / response / error ログを `artifacts/live-logs/bitflyer/<run-id>/` へ自動保存する
- normalized の child order DTO は、`OrderKey` のような派生キーを持たず、API返り値由来の `AcceptanceId` / `ExchangeOrderId` のみを保持する
- normalized の DTO は child order だけでなく、`Ticker` / `Board` / `GetExecutionsPrivate` / `GetTradingCommission` についても API返り値ベースへ整理済み
- `Contracts` 境界では、必要な場合に限って `AcceptanceId` 優先で `OrderKey` / `OrderId` を再構成する

---

## 11. 廃止条件（Sunset）

Stage 文書（`stage*.md`）は初回リリース前の暫定文書。  
`v1.0.0` 時点で本書を `docs/archive/` へ移動し、以後の追跡は `docs/process/revision-history.md` に統合する。
