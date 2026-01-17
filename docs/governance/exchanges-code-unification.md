# 取引所間のコード統一方針（Exchanges 配下スコープ）

## 0. 目的

本書は、取引所間の実装差異を減らし、**`src/Exchanges/<ExchangeName>/` 配下のコードに統一感（横方向の一貫性）**を与えるための最小規約である。

* 本書の対象は **`src/Exchanges/<Ex>/` 配下のみ**とする。
* `src/Exchanges/` 配下以外（取引所横断実装）は、本書の対象外とし、**本書では決めない／触れない**。
* 取引所固有仕様の正本は **各取引所の公式 API 文書**とする。

> 本書の役割：
>
> * 「Exchanges 配下で差異をどこに閉じ込め、どう同形に収束させるか」だけを固定する。
> * 取引所横断実装（Core/Contracts/Transport 等）の設計判断は別系統の文書・議論へ分離する。

## 1. 正本（Source of Truth）

### 1.1 取引所仕様

* 取引所固有の仕様（endpoint の意味、項目、制約、エラーなど）は **公式 API 文書を正本**とする。
* 本リポジトリは、取引所固有仕様について **endpoint 一覧**のみを保持し、詳細仕様の転記（spec.md / sample.json 等）は行わない。

### 1.2 取引所間統一

* 取引所間の統一規約は **コード（共通型・参照関係・テンプレ）を正本**とする。
* reference 実装として **bitFlyer** を指定するが、bitFlyer の“出来”に依存しないよう、統一は **共通コードへの吸収**で達成する。

## 2. 整合性の判定基準（合格条件）

次の 4 点を満たせば「文書とコードは整合している」と判定する。

1. **層責務**（wire/raw/normalized/contracts/transport）が矛盾なく条文化されている
2. **参照方向**が層責務と矛盾しない（矛盾は csproj 参照で物理的に不可能）
3. **公開 API 形状**（例：Call-only、transport 除外）が共通コード／参照関係により固定されている
4. “揺らぎ” が発生した場合、第一選択は **共通コード（テンプレ／基底／helper）への吸収**であり、文書追記は最小で済んでいる

> 重要：本書は Level3（横統一）を「説明」しない。横統一はコードで実現し、文書はその所在と判定基準のみを固定する。

## 3. 文書に書く／書かない

### 3.1 書いてよい（必要最小）

* 解釈が分岐しうる点（揺らぎの源）の条文化
* 例外（transport 除外など）の明示
* 統一規約の所在（どの共通コードが正本か）
* 物理配置／論理配置／名前空間（後述）

### 3.2 書かない（禁止）

* 公式 API 文書の転記（endpoint 詳細、フィールド羅列、サンプル JSON、チュートリアル）
* 取引所ごとの spec.md / sample.json

## 4. 物理配置・論理配置・名前空間（差異は Exchanges 配下に限定する）

### 4.1 結論

**取引所間差異は `src/Exchanges/<ExchangeName>/` 配下にのみ存在する。**

* `src/Core/` / `src/Contracts/` / `src/Primitives/` / `src/Transport/` は
  **取引所差を持たない横断レイヤ**とし、差異を持ち込まない。
* 取引所追加・仕様差対応により変更が許容されるのは
  **`src/Exchanges/<Ex>/` 配下のみ**である。

この前提を崩さないため、物理配置・論理配置・名前空間は
「差異を閉じ込める箱」として **Exchanges 配下にのみ自由度**を与える。

### 4.2 差異の閉じ込め原則

* 取引所差異は必ず以下のいずれかに閉じ込める：

  * `Exchanges.<Ex>.Wire`
  * `Exchanges.<Ex>.Raw`
  * `Exchanges.<Ex>.Normalized`
  * `Exchanges.<Ex>.Adapter`
* 取引所差異を理由に、`Core` / `Contracts` / `Primitives` の
  **public API を分岐・肥大させてはならない**。
* 差異を横断層に吸収する場合は、
  **仕様差を一般化した形（テンプレ／helper／基底）**としてのみ許可する。

> 判断基準：
>
> * 「bitFlyer だけ」「Bittrade だけ」の if 分岐が必要なら、それは Exchanges 配下の責務。
> * 複数取引所に共通化できる形に昇格できた時のみ、Core へ移動する。

## 5. Exchanges 配下の物理配置（Physical Layout）

### 5.1 取引所フォルダの必須サブ構成（固定）

`src/Exchanges/<Ex>/` は次のサブ構成を必ず持つ。

* `Wire/` : HTTP 境界（I/O 表現・プリミティブ中心）
* `Raw/` : 外部 JSON の表現を型へ（lossless / semantic-free）
* `Normalized/` : 意味確定・統一（enum/value object はここ）
* `Adapter/` : 公開契約（Contracts）への写像

> ここで固定するのは「層の箱」だけ。具体のクラスはテンプレ（共通コード）で誘導する。

### 5.2 取引所追加時の変更範囲

* 取引所追加・取引所固有差の対応は **`src/Exchanges/<Ex>/` 配下のみ**で完結させる。
* `src/Exchanges/` 配下以外に差異が漏れる場合、それは「差異の一般化（テンプレ／helper化）」が必要なサインであり、別系統の設計判断として扱う。

## 6. 論理配置（Logical Layout）と責務

### 6.1 Wire（境界）

* 入出力の表現（文字列・数値・null・命名差）を扱う
* OK: 型変換（string/number 混在吸収）、null 許容、名前の写像
* NG: 意味の確定（単位換算、通貨ペア統一、注文種別の解釈、デフォルト補完）

### 6.2 Raw（表現の型写像）

* **lossless / semantic-free**：外部 JSON の“表現”を型へ落とすだけ
* OK: そのまま保持（RawJson を含む）、表現差の吸収
* NG: 単位換算、時刻単位統一、通貨ペア統一、売買方向/注文種別の解釈

### 6.3 Normalized（意味確定）

* 取引所差を統一し、意味を確定する（ここで初めて“共通の意味”になる）
* enum/value object を用いて、契約面がぶれないようにする

### 6.4 Adapter（契約への写像）

* Normalized を Contracts へ写像する
* Contracts は「利用者に見せる形」なので、取引所差の露出を避ける

### 6.5 Contracts（公開契約）

* 取引所横断の最終公開面
* 取引所固有表現（RawJson 等）を露出しない

## 7. 名前空間（Namespace）方針（Exchanges 配下のみ）

### 7.1 基本

* **物理配置と namespace は一致**させる（探索可能性・衝突回避）
* 取引所固有：`Exchanges.<Ex>.<Layer>...`

例：

* `src/Exchanges/Bitflyer/Raw/...` → `Exchanges.Bitflyer.Raw.*`
* `src/Exchanges/Bittrade/Normalized/...` → `Exchanges.Bittrade.Normalized.*`

### 7.2 差異の閉じ込め（4 箇所に限定）

仕様差は「層を跨がずに」次のいずれかへ閉じ込める。

1. `Exchanges.<Ex>.Wire`
2. `Exchanges.<Ex>.Raw`
3. `Exchanges.<Ex>.Normalized`
4. `Exchanges.<Ex>.Adapter`

> ルール：仕様差を `Exchanges.<Ex>.*` 以外へ持ち出さない。

## 8. 横統一をコードで担保するための運用（文書は最小で済ませる）

### 8.1 揺らぎの扱い

* “揺らぎ” を発見したら、第一選択は **共通コード（Core）に吸収**する。

  * 例：limit 適用、エラー分類、RawJson の扱い、Closed/Unknown の扱い
* 文書は「吸収先（どの共通コードが正本か）」のみ追記し、詳細説明はしない。

### 8.2 reference 実装（bitFlyer）の位置付け

* bitFlyer は “例” ではなく、最初にテンプレを満たした実装である。
* 新取引所は bitFlyer を模倣するのではなく、**共通テンプレに乗る**ことで同形に収束させる。

## 9. 例外（Exception Policy）

* `src/Exchanges/<Ex>/` 配下に閉じ込められない差異が発生した場合、
  本書に例外として追記しない。
* その差異は「一般化してテンプレ／helper／基底へ昇格できるか」の検討対象であり、別系統の設計判断として扱う。

## 10. 変更ルール（文書肥大の防止）

* 文書に追記してよいのは「2つ以上の解釈が成立し、実装が分岐しうる」場合に限る。
* それ以外は **コード（共通テンプレ）へ吸収**し、文書は所在だけを最小記録する。
