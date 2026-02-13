# Governance — Design Arbitration Rules


## 1. 目的

本書は、本リポジトリにおける**設計判断の裁定ルール（ガバナンス）**を定める。

本書の目的は、

* 設計思想・技術規範の**再定義を行わない**こと
* 取引所追加・改修時に発生しがちな**判断の揺らぎを防止**すること
* 「どこまでを決め、どこからを決めないか」を固定すること

にある。

本書は**技術仕様書ではない**。
技術的な設計規範・層責務・命名規則・派生規則の正本は
**TopSpec（docs/topspec.md）**とする。

---

## 2. 正本（Source of Truth）の階層

設計・実装・文書の判断に迷いが生じた場合、
以下の優先順位に従って裁定する。

1. 各取引所の**公式 API 文書**（最上位の正本）
2. **TopSpec（docs/topspec.md）**（内部技術規範の唯一の正本）
3. **Contracts（docs/contracts/contracts.md）**（公開 API 契約の正本）
4. 本書（governance.md）（設計判断の裁定ルール）
5. inventory 文書（事実の一覧 / Fact）
6. _references 配下の文書（参考資料）

上位の正本と矛盾する記述は、下位文書に存在しても**無効**とする。

---

## 3. 差異の閉じ込め原則

取引所間の差異は、**必ず `src/Exchanges/<Ex>/` 配下に閉じ込める**。

以下は差異の閉じ込め先として許容される。

* `Exchanges.<Ex>.Wire`
* `Exchanges.<Ex>.Raw`
* `Exchanges.<Ex>.Normalized`
* `Exchanges.<Ex>.Adapter`

取引所間差異を理由として、

* 取引所横断層（Contracts / Core / Transport 等）を分岐させること
* API 契約や層責務を歪めること

を**禁止**する。

差異を一般化できる場合のみ、
コード側（共通テンプレート／基底実装／helper）へ昇格させてよい。

---

### 3.1 Endpoint 実装可否に関する裁定

重複・旧版・非機能・version 並立等の理由により、
実装対象が自明でない場合に限り、
最終的な実装可否の判断（inventory の `PresentIn` の確定）は裁定者が行う。

裁定対象は次に限定する。

* 重複している endpoint が存在する場合
* 旧版 / 新版が並立している場合
* 非機能である可能性があるが、公式に明示されていない場合

上記以外の場合、裁定は行わず、
TopSpec および inventory に記載された事実に従う。

裁定結果は、inventory の `PresentIn` により必ず明示されなければならない（非採用は `None`）。

## 4. 公開 API 面に関する禁止原則

本章における「公開 API 面」とは、利用者が直接参照する interface 群を指し、内部実装や内部公開（internal / InternalsVisibleTo）は含まない。

以下は、**公開 API 面（インターフェース／メソッド群）**に対する
判断禁止ルールである。

### 4.1 分類の禁止

* Wire / Raw / Normalized / Adapter の公開 API 面を**意味分類してはならない**。
* MarketData / Trading / Account 等の分類語彙は
  **物理配置・namespace・公開 API に使用してはならない**。
* `Internal` は実装補助のための **非公開置き場**であり、分類軸として扱わない。

### 4.2 Public / Private 分離（必須）

* Wire / Raw / Normalized / Adapter は
  **Public / Private の2区分でのみ分離する**（MUST）。

* Public / Private は「署名有無」を意味する。
* 意味分類は以下に限定して表現する:
  - EndpointTraits
  - Capability
  - Inventory（事実記録）

* 物理構造で意味を表現してはならない。

---

## 5. 文書間衝突時の裁定

文書間に以下のような衝突が発生した場合、
本書は次の判断を強制する。

* TopSpec と他文書が衝突した場合：**TopSpec を優先**
* inventory とコードが乖離した場合：

  * 公式 API 文書に照らして inventory を修正する
  * コード修正はその後に行う
* 文書とコードが乖離した場合：

  * 文書が正本である場合のみコードを修正する

---

## 6. 変更に関する原則

本書に追記してよいのは、次の場合に限る。

* 差異の閉じ込め先が変更される場合
* 公開 API 面に対する**禁止原則**が変更される場合
* 正本階層（裁定順序）が変更される場合

技術的な詳細・実装上の工夫・具体構成は、
**本書に追記してはならない**。

---

## 7. 本書の位置づけ

* 本書は **Normative** である
* ただし本書は「技術仕様」ではなく
  **設計判断の裁定ルール**のみを定める
* 本書と TopSpec の役割が重なる場合、
  **TopSpec を必ず優先**する


## 8. REVIEW 採用ルール（Normative）

本章は `docs/reviews/REVIEW-01`〜`REVIEW-06` のうち、採用済みルールのみを規範化したものである。
`docs/reviews` 配下は引き続き Reference とし、規範判断は本章を正とする。

### 8.1 命名・語彙（REVIEW-01）

- EndpointId は取引所ごとの inventory を正本とし、取引所横断で同名統一を要求しない。
- inventory 主表には正規 EndpointId のみを記載し、別名・重複候補は `Aliases` に分離する。
- `Request/Response` は API 境界 DTO に限定し、`Result` は `CallResult<T>` 用語としてのみ使用する。
- 外部仕様由来 typo は Wire/inventory に閉じ込め、Contracts や API 境界 DTO へ拡散させない。

### 8.2 引数・型（REVIEW-02）

- `CancellationToken` 引数名は `cancellationToken` に固定し、末尾配置・既定値 `= default` を原則とする。
- 新規 endpoint では Raw Request/Response から Normalized へ変換する時点で VO/enum 化を完了させる。
- Normalized 以降で primitive を新規導入してはならない。

### 8.3 実装フロー（REVIEW-03）

- 業務エラー判定は Normalized 層の detector に集約し、`MapOk` の先頭で必ず評価する。
- Mapping 例外は `CallErrorKind.Mapping` に統一し、`Semantic` へ混在させない。
- scalar 変換は「必須は Fail-fast、任意は null 許容（不正フォーマットは Mapping）」を標準とする。
- timestamp 欠損は endpoint ごとの `Required/Optional` ポリシーで固定し、暗黙補完を禁止する。

### 8.4 依存境界（REVIEW-04）

- Adapter は `Normalized.Internal.*` を直接参照してはならない。
- Normalized は `Wire.Constants` / `Wire.Internal` を直接参照してはならない。
- `PublicClient` は entrypoint 専用とし、実オーケストレーションは `MarketApi` 側に集約する。

### 8.5 取引所間並列性（REVIEW-05）

- Adapter Public 境界では Request DTO を保持し、`ExchangeClient/PublicClient -> MarketApi` 委譲を取引所間で統一する。
- Adapter テスト命名は取引所間で同一規約に合わせる。

### 8.6 定数・語彙定義（REVIEW-06）

- HTTP method 等の共通語彙は共通層で定義し、取引所別の文字列直書きを禁止する。
- 監視・CallMeta で使う Layer/Component 語彙は正本を一元化し、直書きを禁止する。
- 認証キー名・仕様 typo・raw lexicon は取引所固有定数として取引所配下に閉じ込める。

