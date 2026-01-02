# TopSpec.Guide（Core 章対応版 / 案B 物理構成反映版）

> **本書は TopSpec.Core を補足する唯一のガイド文書である。**
> **本書は FIX ではない。**
> **TopSpec.Core と矛盾する場合、常に TopSpec.Core を優先する。**

---

## 分離方針（運用規約）

* TopSpec.Core は **不変の憲法**であり、境界・責務・依存方向・不変条件のみを定義する。
* 本書（Guide）は Core を **理解・実装・運用するための補助文書**である。
* 本書が肥大化した場合、以下のいずれかに該当する内容は **独立文書として分離**する。

**分離トリガー**

1. 時間軸（判断履歴・過去事例・経緯）を持ち始めた
2. 手順・チェックリスト・運用方法が主体になった
3. 特定レイヤ（Adapter / Domain / Error 等）専用の内容になった
4. 章単位で Core 本文より長くなった

分離後、本書には **要約と参照（索引）** のみを残す。

---

## 0. 本書の位置づけと読み方（Core 前提）

* 本書は TopSpec.Core を前提として読む。
* 本書は Core の再解釈や上書きを行わない。
* 本書は「なぜそう決めたか」「どう誤用されやすいか」を記録する。

### 0.1 なぜ Core は短くあるべきか

* 憲法は短く、不変でなければ機能しない。
* 理由・背景・運用を含めた瞬間に、変更圧が Core に流入する。
* したがって Core は **規範のみ** を保持し、理解補助は必ず外部に置く。

Split Candidate: なし（常設）

---

## 1. 決定事項／非決定事項（Core §1 対応）

### 1.1 Core が決めること

* 境界・責務・依存方向・不変条件のみを決定する理由
* 仕様と実装の混線を防ぐための最小決定

### 1.2 Core が決めないこと

* 物理構成・運用・具体例を Core から排除した理由
* それらを Guide に置く必然性

Split Candidate: DecisionLog

---

## 2. 大原則の背景（Core §2 対応）

### 2.1 公開境界を明示する理由

* 境界が曖昧な API は破壊的変更を検知できない

### 2.2 層跨ぎ禁止の意味

* 層を跨ぐ責務混在が引き起こす破壊

### 2.3 不変条件を破壊的変更扱いとする理由

* 不変条件は利用者との契約そのものである

Split Candidate: DecisionLog

---

## 3. FIX 宣言の運用上の意味（Core §3 対応）

* なぜ Core の規則は FIX でなければならないか
* FIX を変更する場合に発生するコスト

Split Candidate: DecisionLog

---

## 4. ゴール／非ゴールの補足（Core §4 対応）

### 4.1 ゴールの解釈

* 本プロジェクトの提供価値には優先順位がある。
  * **第1：取引所ごとの API を前提に、Wire / Raw / Normalized（spec）を第一級の API として提供すること**
  * **第2：複数取引所間で抽象化可能な項目のみを Cross-Exchange（Domain: Contracts / Common）として提供すること**
* 本プロジェクトのゴールは「取引所差異を完全に隠蔽すること」ではない。
  * 取引所にはそれぞれの仕様があり、完全抽象化は不可能／不適切である。
* Cross-Exchange は **抽象化できる範囲に限定**し、意味論を安定させる。
  * 例：資産状況、注文状況 等
* 公開入口（Factory）は、利用者の意図を型で表現する：
  * Exchange-Specific（取引所固有：spec を直接利用）を使いたい
  * Cross-Exchange（抽象化：Domain 契約）を使いたい

### 4.2 非ゴールの再確認

* 「全 endpoint の網羅（完全写像）」は非ゴールである。
  * リポジトリ内に取引所ごとの `spec.md` / `sample.json` 等の写経を置かない方針とも整合する。
* 「取引所間仕様差異の完全抽象化（差異を消すこと）」は非ゴールである。
  * 取引所固有の差異は現実に存在し、完全抽象化は不可能／不適切である。
* 「spec（Wire / Raw / Normalized）を Cross-Exchange と同一水準で不変に保つこと」は非ゴールである。
  * spec は公式 API 変更に追随して破壊されうる前提で扱う（利用者が選択して追随する）。

### 4.3 よくある誤用（改訂後の注意点）

* Cross-Exchange に取引所固有フィールドや transport 情報を混入させない。
  * それは §18「domain 内への transport 情報の流入」違反を誘発する。
* Exchange-Specific（spec 直利用）は “何でも OK” ではない。
  * Raw / Exchange への直接アクセスは明示的 opt-in を前提とする（Core §14）。
* 「抽象化できる項目」の一覧は Core に固定せず、運用文書（Ops 等）で管理する。
  * Core は境界・責務・不変条件のみを決める（Core §1）。

Split Candidate: なし

---

## 5. 論理階層の理解補足（Core §5 対応）

### 5.1 spec / domain 境界の思想

* Raw / Normalized を spec に留める理由

### 5.1.1 Wire を 2 段階で運用してよい（共通 Wire / 取引所 Wire）

* Wire は次の 2 種に分離して運用してよい。
  * **共通 Wire**：取引所概念を持たない transport 抽象（Request/Response/Transport 等）
  * **取引所 Wire**：当該取引所の endpoint・署名・組み立て等、WireRequest へ落とすための固有ロジック
* 共通 Wire は **DTO を持たない**（JSON 文字列/バイト列を運ぶのみ）。
* 取引所 Wire も **DTO を持たない**（DTO は Raw に属する）。
* 取引所 Wire は Raw の codec を呼び出してよい（Raw DTO ⇄ JSON 変換を利用するため）。

### 5.1.2 下層は上層の名前空間を参照しない（Core §5「依存方向」）

* 本プロジェクトでは、論理階層の順序に対して **依存方向を固定**する。
  * **下層は上層を参照しない**
  * 許可される参照は **同一層**または **下層**のみ
* 代表例：
  * **Wire は Raw を参照しない**（Wire は transport のみ。型（DTO/VO）を上層に求めない）
  * Raw は Normalized/Contracts を参照しない
  * Contracts は Domain/Composition に依存しない
* 実装上、どうしても型が欲しくなった場合は「上層へ逃がさない」：
  * Wire が必要とする型は Wire 自身（Wire/Common や Exchange/Wire）に定義する
  * Raw の “鏡像 DTO” は Raw に閉じる（Wire へ漏らさない）

### 5.2 Adapter を翻訳関所とする理由

* なぜ判断を Adapter に持ち込まないか

### 5.3 Domain を肥大させない原則

* Domain が持つべき責務と持ってはならない責務

### 5.4 層内 Call 概念（Raw / Normalized）に関する補足

* Raw 層および Normalized 層においても、
  **層内の便宜として Request と Outcome を不可分に扱う概念（Call）を導入してよい**。
* これらの Call 概念は **当該層内でのみ有効**とし、
  Contracts / Domain へ露出してはならない。
* Contracts 層における Call は「公開契約」であり、
  Raw / Normalized 層の Call とは **意味が異なる**。

Split Candidate: AdapterNotes

---

## 6. 公開入口（Factory）の設計意図（Core §6 対応）

* Factory を入口に限定する理由
* 利用意図を型で表現する設計思想

Split Candidate: なし

---

## 7. 公開契約（Contracts）の背景（Core §7–8 対応）

### 7.1 Call / Outcome 採用理由

* Request と結果を不可分に扱う理由

### 7.2 Contracts と Common を分ける理由

* 形と語彙を分離する設計判断

Split Candidate: ErrorDesign / ValueDesign

---

## 8. Cross-Exchange 共通化の思想（Core §11 対応）

### 8.1 なぜ共通化対象を 4 種に限定したか

* Interface / DTO / Type / Error 以外を共通化しない理由

Split Candidate: DecisionLog

---

## 9. 責務分離と横断関心（Core §12–13 対応）

* Contracts / Common / Domain の役割分離
* 横断関心を Domain に集約する理由

Split Candidate: DomainNotes

---

## 10. Raw / Exchange 明示アクセスの背景（Core §14 対応）

* opt-in を強制する理由
* 調査用途と業務用途の分離

Split Candidate: Operations

---

## 11. 依存方向の設計理由（Core §15 対応）

* なぜ依存方向を固定する必要があるか
* 境界破壊が起きる典型パターン

Split Candidate: AdapterNotes

---

## 12. 不変条件の補足解説（Core §16 対応）

* 後方互換を絶対条件とする理由
* Success / Failure 排他の意味

Split Candidate: ValueDesign

---

## 13. 禁止事項の背景（Core §18 対応）

* 各禁止事項が防いでいる事故

Split Candidate: AntiPatterns

---

## 14. 変更の扱い（運用補足）

* Core 変更を原則破壊的とみなす理由
* 変更理由・影響範囲を明示する運用

Split Candidate: DecisionLog

---

## 15. 物理構成と正本管理（Core 補完章 / 案B）

> 本章は **非規範**である。
> 本章は Core の論理境界（spec / domain / boundary / composition）を、
> **安全に運用できる物理構成へ写像するための参照情報**を提供する。
>
> 本章を根拠として設計判断を完結させてはならない。
> 迷った場合は常に TopSpec.Core の境界・依存方向・禁止事項に立ち戻る。

### 15.1 物理構成を規範にしない理由

* 物理構成（フォルダ・プロジェクト分割）は、言語、CI、mono-repo / multi-repo、組織規模により変化する。
* 物理構成を規範化すると、分割戦略の変更が **破壊的変更**へ誤転化しやすい。
* 規範とすべき対象は常に **責務・依存方向・不変条件**である。

### 15.2 案Bの目的（論理写像の最優先）

案Bは、Core §5 の論理階層を「誤読しにくい形」で物理配置に写像する。

* **Wire / Raw / Normalized は spec 側に閉じる**（domain から触れない）
* **Adapter は境界（Boundary）として分離**する（Exchange 実装の一部に見せない）
* **Contracts / Common / Domain(Behavior) を混在させない**
* **Composition を依存の頂点**として明示する

### 15.3 src/ 以下の推奨物理構成（案B）

以下は「比較的安全に境界を保ちやすい」構成例であり、必須ではない。

```text
src/
  Core/                          # 取引所概念を持たない実行基盤

  Spec/                          # spec層（domainを認識しない）
    Wire/                        # 共通 Wire（transport 抽象 / DTO は持たない）
      Common/
    Exchanges/
      <Exchange>/
        Wire/                    # 取引所 Wire（endpoint/署名/組立 / DTO は持たない）
        Raw/                     # 鏡像DTO（spec）
        Normalized/              # 取引所内正規化DTO（spec）

  Boundary/                      # 翻訳関所（境界）
    Adapters/
      <Exchange>/                # spec → contracts の翻訳のみ

  Domain/                        # domain層（specを認識しない）
    Contracts/                   # 公開契約（I/O構造）
    Common/                      # 共通語彙（Value / Type / Error / Parsing）
    Behavior/                    # 横断ふるまい（CoreのDomain）

  Composition/                   # DI / Factory / 組み立て
```

#### 15.3.1 Wire / Raw / Normalized / Adapter の境界（よくある誤解）

* Wire/Raw/Normalized は **spec 層の都合**で導入してよい（Core §5）。
* Wire は **JSON 文字列（またはバイト列）の transport payload を運ぶだけ**であり、DTO を持たない。
* Raw は **鏡像 DTO と codec**（JSON payload ⇄ DTO）を担ってよい。
* Normalized は **単独取引所内**の正規化に限定し、取引所横断の抽象化を目的としない。
* Contracts は **取引所横断の抽象化 DTO**であり、transport 情報（path/header/query/json string 等）を持たない。
* Adapter は **翻訳関所**であり、判断・事業ロジックの置き場ではない。
* Domain は横断的ふるまいであり、「再利用フォルダ」ではない。

#### 15.3.2 依存方向の運用（推奨チェック）

* `using` / 参照の静的チェックで、以下を禁止する：
  * `Spec/Wire/**` が `Spec/Exchanges/**/Raw/**` を参照
  * `Spec/Exchanges/**/Raw/**` が `Normalized/**` や `Contracts/**` を参照
* CI では依存方向違反を **ビルド失敗**にするのが望ましい。

### 15.4 依存方向を CI で強制する（最小セット）

物理構成は補助輪である。境界違反はレビューではなく CI で検出されるべきである。

最低限、次の依存制約を機械的に検査する。

* `Domain/**` → `Spec/**` を禁止
* `Spec/**` → `Domain/**` を禁止
* `Boundary/Adapters/**` → `Spec/**` を許可（Adapter が spec を読む）
* `Boundary/Adapters/**` → `Domain/Contracts/**` を許可（翻訳先）
* `Composition/**` → 全依存を許可（組み立て頂点）

（推奨）`Spec/**` → `Boundary/**` を禁止（境界の一方向性を保つ）

### 15.5 正本（source of truth）の扱い

* 取引所 API 仕様の正本は **公式 API 文書**のみとする。
* リポジトリ内に、取引所ごとの `spec.md` や `sample.json` 等の鏡像を置いてはならない。
* ただし、開発者が迷わないために **endpoint 一覧（索引）**のみを文書として保持してよい。

  * endpoint 一覧は仕様ではなく索引である。
  * 実装時は常に公式文書へ遷移し、公式の記述を正として採用する。

### 15.6 endpoint 一覧（索引）に含める最小情報

endpoint 一覧は「仕様の代替」ではなく「入口」である。

* グループ（Public / Private 等）
* Method / Path（1行）
* 目的（短い説明）
* 公式文書への参照（リンクまたは参照ID）

### 15.7 よくある誤用（物理構成が原因の事故）

* Adapter が肥大化し、Domain を吸い込み始める（境界が実装本体化する）
* Common に何でも集約し、境界が消える
* Wire/Raw/Normalized を Domain だと誤認し、契約を混線させる

Split Candidate: PhysicalLayout（構成例・CI・運用が増えた場合は独立文書へ）

---

## 16. 失敗の意味と責務帰属（運用視点補足）

* **Converter の失敗**は、公式 API 文書と入力データの不整合、
  もしくは取引所仕様の破壊を示す。
* **Mapper の失敗**は、意味解釈不能または前提条件違反を示す。
* **Adapter の失敗**は、Contracts への翻訳責務の失敗であり、
  実装側の問題として扱う。

これらの失敗は、同一の失敗として扱ってはならない。

Split Candidate: ErrorDesign

---

## 17. 附則：将来拡張として検討済みの論点

本章は規範ではない。
過去に検討されたが、憲法（Core）には含めなかった論点を記録する。

* エラー正規化方針
* ページングの共通表現
* レート制限の扱い
* キャンセル／タイムアウト

これらは将来、必要に応じて独立文書として詳細化してよい。

Split Candidate: DecisionLog

---

## 18. 分離された文書一覧（索引）

* TopSpec.DecisionLog.md : 設計判断の履歴
* TopSpec.AdapterNotes.md : Adapter 専用補足
* TopSpec.DomainNotes.md : Domain 設計補足
* TopSpec.PhysicalLayout.md : 物理構成詳細
* TopSpec.ErrorDesign.md : エラー設計詳細
* TopSpec.ValueDesign.md : Value 設計詳細
* TopSpec.Operations.md : 運用・CI
* TopSpec.AntiPatterns.md : 事故・誤用集

Split Candidate: なし（索引章として常設）
