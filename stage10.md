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

---

## 7. 実装成果物

- bitFlyer live test 用の専用 test project
- `Wire` / `Raw` / `Normalized` ごとに実行可能な live test セット
- `Public GET` / `Private GET` / `Private POST` を分離実行できるフィルタまたは同等の仕組み
- live test 実行に必要な環境変数、資格情報導線、実行手順の文書
- 実行結果を保存できる証跡フォーマット

---

## 8. DoD

- bitFlyer live test 専用プロジェクトが追加され、通常テストから分離されている
- `Public GET` コア 3 本が `Wire` / `Raw` / `Normalized` の各層で成功する
- `Private GET` コア 3 本が `Wire` / `Raw` / `Normalized` の各層で成功する
- `SendChildOrder` / `CancelChildOrder` のライフサイクル試験が成功する
- ライフサイクル試験後に未取消の `ACTIVE` 注文が残らない
- 失敗時ログで endpoint / layer / HTTP status / exchange error を追跡できる
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

## 10. 廃止条件（Sunset）

Stage 文書（`stage*.md`）は初回リリース前の暫定文書。  
`v1.0.0` 時点で本書を `docs/archive/` へ移動し、以後の追跡は `docs/process/revision-history.md` に統合する。
