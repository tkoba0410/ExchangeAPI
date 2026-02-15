# Stage9

## Stage9 の目的

Stage9 は、実装とテストの混線を解消し、回帰保証の土台を確立するフェーズである。

Exchange 拡張や機能拡張ではなく、構造の整理と保証体系の整備に集中する。

---

## Stage9 の範囲

### 9-1 ExchangeInfo（概念）の廃止

ExecutionContext の塊としての ExchangeInfo 依存を解消する。

* Facade は薄い入力（ClientOptions + 必要最小限の Credentials）で呼び出せる状態にする
* 取引所差分は部品（Signer / Canonicalizer / EndpointCatalog 等）として Core に残す
* 複数アカウント管理・環境選択・secrets 管理は Core から分離する

---

### 9-2 テストのための実装

テストが検査可能となる最小限の実装を整備する。

* 静的試験を可能にするためのフックや部品の整理
* Fake Transport 等によるネットワーク不要の検査基盤
* 組み立て結果（request / 署名 / 整形）の検査可能化

---

### 9-3 テスト

整備された基盤の上でテスト体系を確立する。

* CI 常時実行の静的テスト
* 手動実行の動的テスト（段階導入）

---

### 9-4 レビュー体系の整理

Stage9 の作業単位に対応したレビュー観点と運用導線を整理する。

* レビュー観点の明確化
* PR テンプレートとの整合
* docs/reviews 体系の整理

---

## Stage9 の非目的

Stage9 では以下を行わない。

* 新規 Exchange 追加
* 既存 Exchange の endpoint 拡張
* Contracts の意味拡張
* 実地データ大量取り込み運用の確立
* Trading の実地呼び出しの本格運用

---

各 9-x の詳細（DoD・成果物・手順）は、着手時点で個別に詰めるものとする。
