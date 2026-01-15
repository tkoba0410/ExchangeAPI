> ⚠️ Legacy / 参考資料
> この文書は旧版であり **正本ではない**。実装・判断の拘束力を持たない。
> 正本（Normative）は以下：
> - docs/topspec.md
> - docs/contracts.md
> - docs/process.md
> - docs/exceptions.md
> - docs/endpoints.md
> - docs/inventory-*.md

# 文書設計ノート（参考）

## 0. 本文書の位置づけ（非仕様・参考）

* 本文書は**仕様（Normative）ではなく、文書設計に関する判断と検討結果を残すための参考資料**である。
* 文書量を抑えつつ実装時の揺らぎを止める、という目的のもとで行った設計判断を記録する。
* 本文書自体は規範的拘束力を持たず、仕様判断は `topspec.md` / `contracts.md` を正とする。
* 将来の見直しや第三者理解のために残すが、**日常的な参照や実装判断の根拠として直接用いることは想定しない**。

---

## 1. 文書整備の目的

* 文書量を最小限に抑えること
* 実装時の判断余地（揺らぎ）を最小化すること
* 公式 API 文書を仕様の正本とし、自前仕様の増殖を防ぐこと
* 将来の取引所追加・構成変更に対して破綻しない文書体系を作ること

---

## 2. 到達点（最終的な文書群の全体像）

## 1.5 本計画において「必ず入れる内容」（事前確定事項）

本計画では、以下の内容については**取捨選択の対象とせず、必ず文書群のどこかで表現されるもの**として事前に確定する。

### 1.5.1 構造に関する必須事項

* システムは **Wire / Raw / Normalized / Contracts の4層構成**であること
* 各層の責務と禁止事項が明示されていること
* 層間の依存方向が一意に定まっていること

### 1.5.2 取引所固有・横断の扱い

* 取引所固有要素と取引所横断要素は**どの層にも存在し得る**こと
* 横断要素は Common / Core として集約されること
* 取引所固有要素は取引所単位の名前空間に閉じること

### 1.5.2.1 各層が持つ型（型カテゴリの整理）

本計画では、各層で扱う型を次のカテゴリで整理し、**層ごとに許可される型を限定**する（= 型の持ち込みで揺らぎを作らない）。

* **Wire string**：外部 I/O（HTTP/Query/Path/Header 等）の文字列表現
* **Primitive DTO**：string/number/bool/null/array/object 等のプリミティブで表現された DTO（意味付けをしない）
* **Exchange DTO**：取引所固有の DTO（取引所名を含む、フィールド名も取引所寄り）
* **Abstract DTO**：取引所横断の抽象 DTO（共通の概念・最小集合、Contract 側の型に近い）

### 1.5.2.2 各層の型制約（必ず入れる制約）

* **層の型の統一は、メソッドの in/out（入力/出力）で合わせる**：各層の公開メソッドは、その層で許可された型のみを引数・戻り値に用いる。

* **各層は飛び越えない**：呼び出しは隣接層間（Wire→Raw→Normalized→Contracts）に限定し、上流/下流の層を直接参照・呼び出ししない。

* **抽象DTO（Abstract DTO）の定義元（オーナー）を固定する**：Abstract DTO は横断の共通契約であり、原則として **Contracts 層を定義元**とする（Normalized は Contracts の型を返す）。

* **型カテゴリが機械的に判別できる命名規約を固定する**：

  * Exchange DTO（取引所固有）: 型名または名前空間に取引所名を必須（例: `Bitflyer*`, `Bittrade*`）
  * Abstract/Contract（横断）: 型名・名前空間に取引所名を禁止
  * 層サフィックス（任意だが推奨）: `*Raw`, `*Normalized` 等で所属層を判別可能にする

* **Wire 層**：入口でのみ `string` を受け取り、以降の層へ `string` を流さない（必要なら Parse/OrThrow で明示的に変換）。

* **Raw 層**：外部 JSON の表現を **Primitive DTO / Exchange DTO** として lossless に保持する（意味付け・単位統一・解釈は禁止）。

* **Normalized 層**：意味の確定・統一はすべてここで行い、外向きには **Contracts 定義の Abstract DTO**（または Contract 直結）として公開する。

* **Contracts 層**：完全に横断であり、**Abstract DTO（公開契約）**のみを持つ。取引所名を含む型は置かない。

※ 層をまたぐ型の混入（例：Wire string の下流流入、Raw DTO の Contract 露出、Exchange DTO の Contracts 置き等）は揺らぎの原因として禁止し、必要な例外は Decisions に集約する。

* **層の型の統一は、メソッドの in/out（入力/出力）で合わせる**：各層の公開メソッドは、その層で許可された型のみを引数・戻り値に用いる。

* **各層は飛び越えない**：呼び出しは隣接層間（Wire→Raw→Normalized→Contracts）に限定し、上流/下流の層を直接参照・呼び出ししない。

* **Wire 層**：入口でのみ `string` を受け取り、以降の層へ `string` を流さない（必要なら Parse/OrThrow で明示的に変換）。

* **Raw 層**：外部 JSON の表現を **Primitive DTO / Exchange DTO** として lossless に保持する（意味付け・単位統一・解釈は禁止）。

* **Normalized 層**：意味の確定・統一はすべてここで行い、外向きには **Abstract DTO**（または Contract 直結）として公開する。

* **Contracts 層**：完全に横断であり、**Abstract DTO（公開契約）**のみを持つ。取引所名を含む型は置かない。

※ 層をまたぐ型の混入（例：Wire string の下流流入、Raw DTO の Contract 露出、Exchange DTO の Contracts 置き等）は揺らぎの原因として禁止し、必要な例外は Decisions に集約する。（例：Wire string の下流流入、Raw DTO の Contract 露出、Exchange DTO の Contracts 置き等）は揺らぎの原因として禁止し、必要な例外は Decisions に集約する。

### 1.5.3 物理構成（src）の位置づけ

* `src/` 配下の物理ディレクトリ構成は**仕様の一部（正本）**として扱うこと
* 文書と物理構成が食い違った場合は、原則として**文書側を修正対象**とすること
* 物理構成は詳細な全ツリーではなく、**揺らぎを止めるための最小骨格（skeleton）と配置規則**として文書化すること

#### 1.5.3.1 最小骨格（skeleton）

以下のディレクトリ骨格が `src/` 直下に存在し、以後の実装は必ずこのいずれかに所属する。

```
src/
  Wire/
  Raw/
  Normalized/
  Contracts/
```

#### 1.5.3.2 固有／横断の配置規則（各層に共通）

各層配下では、取引所横断要素と取引所固有要素を次の規則で分離する。

```
src/<Layer>/
  Common/        # cross-exchange
  <Exchange>/    # exchange-specific (e.g. Bitflyer, Bittrade)
```

※ 例外（この規則に従えない配置）が発生した場合は、構造説明を増やすのではなく **Decisions に理由と差分を記録**する。

#### 1.5.3.3 物理構成の扱い（運用ルール）

* 文書側のツリー図は上記 skeleton までに限定する（個別サブフォルダやファイル一覧は書かない）
* 物理構成を変更する場合は、Phase 2 で Normative との整合を取り、必要なら Decisions を更新する

### 1.5.4 仕様の正本と責務分離

* 各取引所の API 仕様の正本は**公式 API 文書**であること
* 自前文書では API の意味的仕様を再定義しないこと
* 文書は拘束・索引・裁定・運用に役割を限定すること

### 1.5.5 文書設計の思想

* 文書は**量を増やすことではなく、判断余地を消すこと**を目的とする
* 説明よりも MUST / NG / 構造 / テストを優先する
* 例外は必ず Decisions に集約する

### 2.1 文書群の完成形

```
docs/
  index.md            # 入口・思想・ナビゲーション
  topspec.md          # Normative（全体憲法）
  contracts.md        # Normative（横断 Contract）
  decisions.md        # 例外台帳
  process.md          # 運用・編集規律
  endpoints/
    README.md         # Inventory 書式定義
    bitflyer.md
    bittrade.md
```

### 2.2 基本原則

* Normative 文書は **2 本まで**とする
* 仕様の正本は **各取引所の公式 API 文書**である
* **src 配下の物理構成は仕様の一部（正本）**として扱う
* 説明文よりも **拘束（MUST/NG）・構造・テスト**を優先する

---

## 3. 文書の役割定義（増殖を防ぐ枠組み）

| 種別        | 役割      | 書いてよい内容   | 書いてはいけない内容 |
| --------- | ------- | --------- | ---------- |
| Normative | 揺らぎを止める | MUST / NG | 背景説明・長文例   |
| Inventory | 探させる    | 一覧・リンク    | 仕様解釈・意味付け  |
| Decisions | 例外を閉じる  | 原則からの逸脱理由 | 一般ルール      |
| Process   | 運用を固定   | 手順・判断順    | 仕様本文       |

※ 上記いずれにも該当しない文書は作成しない。

---

## 4. 既存文書の取り扱い方針（作業前判断）

### 4.1 基本姿勢

* 内容の良し悪しは判断基準にしない
* **役割の重複は統合または削除**とする
* 「便利そう」「詳しい」は存続理由にならない

### 4.2 機械的な判定手順

1. Normative に該当するか
2. Inventory に該当するか
3. Decisions に該当するか
4. Process に該当するか
   → いずれにも当てはまらない場合は削除対象

---

## 5. 作業フェーズ（実施順が重要）

### Phase 1：Normative の確定（最優先）

**目的**：揺らぎの発生源を先に封じる

* topspec.md

  * 4 層構成
  * 各層の責務と禁止事項
  * 取引所固有 / 横断の原則
  * src 構成が正本であることの宣言
* contracts.md

  * Call-only 原則
  * 公開 API 境界
  * 取引所非依存の保証

※ この段階では Guide や説明文書は参照しない

---

### Phase 2：src 構成との整合確認

**目的**：文書と実体の矛盾を解消する

* src/ 配下の物理構成を確認
* topspec の原則との不整合を洗い出す
* 修正対象は原則として docs 側
* 構造変更が必要な場合は Decisions 候補として記録

---

### Phase 3：Inventory の整理

**目的**：説明を書かずに探せる状態を作る

* Inventory は endpoints のみに限定
* 書式ルールを README.md に集約
* 既存 inventory 文書を機械的に移設
* 仕様説明文は追加しない

---

### Phase 4：例外の隔離（Decisions）

**目的**：例外の散在と増殖を防ぐ

* Normative に適合しない点のみを記録
* 「現状こうなっている」という事実列挙は行わない
* 原則との差分と理由のみを書く

---

### Phase 5：Process の統合

**目的**：今後文書が増えない状態を作る

* documentation-policy / review-checklist 等を統合
* 文書追加の判断順序を明文化
* 非 Normative 文書であることを明示

---

### Phase 6：index.md の作成（最後）

**目的**：全体の入口を整える

* 文書群の思想
* 各文書の役割と参照先
* 「迷った場合の参照順序」

※ index.md は最初に書かない

---

## 6. 本文書を役割完了とみなす条件

* 文書構成および設計思想が `index.md` / `topspec.md` / `contracts.md` に反映されている
* src 構成と Normative 文書の整合が取れている
* 抽象DTOの定義元、型制約、例外運用などの重要判断が Normative に条文化されている

※ 上記を満たした後も、本文書は**参考資料として保持**される。

---

## 7. 本計画書の価値

* 「考えながら整備する」状態を終わらせる
* 判断を前倒しで確定させる
* 文書整備を**作業**に落とす

---

## 8. 現状からの移行計画（参考・非仕様）

本章は、既存文書体系から新しい文書構成（TopSpec / Contracts 中心）へ移行するための  
**一時的な判断整理** を目的とする。

本章の内容は仕様（Normative）ではなく、移行完了後は更新対象としない。

---

### 8.1 移行の前提

* 新たに定義された `topspec.md` / `contracts.md`（最小条文化・日本語）を **正（canonical）** とする
* 既存文書は段階的に参照対象から外す
* Inventory（endpoints）および Decisions（exceptions）は継続利用する
* 移行期間中であっても、新規実装は常に **新 TopSpec / Contracts 準拠** とする

---

### 8.2 フェーズ定義

#### Phase A：並行期間

* 旧 TopSpec / 新 TopSpec が共存する
* 新規コードは新 TopSpec / Contracts にのみ従う
* 既存コードは原則として変更しない
* 旧文書に基づく新規実装は禁止する

#### Phase B：切替期間

* 旧 TopSpec / boundaries / guide を Reference 扱いに変更する
* Normative として参照されるのは新 TopSpec / Contracts のみとする
* 移行を理由とした規範緩和や例外追加は禁止する

#### Phase C：収束

* 旧 TopSpec / boundaries / guide を `docs/_references/` 配下へ移動する
* `index.md` を新文書構成前提で再生成する
* Normative 文書は TopSpec / Contracts の 2 本のみとする

---

### 8.3 移行期における禁止事項

* 移行を理由に Normative の MUST / MUST NOT を弱めてはならない
* 移行中の判断や暫定対応を Decisions に記録してはならない
* 旧文書の内容を温存する目的で、新規 Normative 文書を追加してはならない
