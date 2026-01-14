# TopSpec（統合版）

本書は ExchangeAPI リポジトリの **最上位規範（Normative）** である。  
ここに記載された MUST / MUST NOT は拘束力を持ち、他の文書・コメント・慣習よりも優先される。

---

## 1. 目的

- 文書量を増やさずに、実装時の判断余地（揺らぎ）を最小化する。
- 層の責務、境界、データ形状、依存方向を固定し、取引所追加・変更時も迷いなく維持できるようにする。

---

## 2. 正本と権威（Authority）

### 2.1 正本
- 各取引所の API 仕様の正本は **公式 API 文書**である（MUST）。
- 本リポジトリは公式仕様の意味的仕様を再定義しない（MUST NOT）。

### 2.2 文書の優先順位
1. TopSpec（本書）
2. Contracts（公開契約の規範）
3. Decisions / Exceptions（例外台帳）
4. Inventory / Endpoints（索引）
5. その他の文書（Guide / Policy / Checklist など）

---

## 3. 層モデル（Layer Model）

### 3.1 必須の 4 層
本システムは次の **4 層構成**を必須とする（MUST）。

```
Wire → Raw → Normalized → Contracts
```

### 3.2 層を跨ぐ呼び出しの禁止
- 依存・呼び出しは **隣接層間に限定**する（MUST）。
- 層を飛び越えた参照・呼び出しは禁止する（MUST NOT）。

### 3.3 Adapter の位置づけ
- Adapter は「層」ではなく、**Normalized（取引所内の意味確定）→ Contracts（横断契約）への翻訳境界**である。
- Adapter は Contracts を実装・返却するが、Contracts の意味論を変更してはならない（MUST NOT）。

---

## 4. 層の責務と禁止事項

### 4.1 Wire
**責務（MUST）**
- 外部 I/O（HTTP Path / Query / Header / Body など）の入口となる。
- 入口の文字列・バイト列を、下流に流してよい形へ変換する。

**禁止（MUST NOT）**
- JSON の意味解釈を行う。
- Wire 以外の層へ `string` をそのまま流す。

### 4.2 Raw
**責務（MUST）**
- 外部 JSON の表現を **lossless** に保持する（型混在・欠損・null を含む）。

**禁止（MUST NOT）**
- 単位換算・時刻統一・売買方向/注文種別の解釈などの意味確定。
- 意味のあるデフォルト補完。

### 4.3 Normalized
**責務（MUST）**
- 取引所内で意味を確定し、表現差分を統一する（正規化）。

**禁止（MUST NOT）**
- Raw DTO（外部表現）を公開面へ露出する。

### 4.4 Contracts
**責務（MUST）**
- 取引所横断の公開契約（語彙・型）を提供する。

**禁止（MUST NOT）**
- 取引所名を含む型・名前空間を定義する。
- 取引所固有差分を契約に持ち込む。

---

## 5. 境界ポリシー（Boundary Rules）

### 5.1 Call-only
- 公開 API の返却形式は **Call** に統一する（MUST）。
- Response DTO を直接返してはならない（MUST NOT）。

### 5.2 型安全（in/out の固定）
- 層の型の統一は、**メソッドの in/out（入力/出力）**で合わせる（MUST）。
- 各層の公開メソッドは、その層で許可された型のみを in/out に用いる（MUST）。

---

## 6. データ形状（Data Shape）

### 6.1 型カテゴリ
- **Wire string**：外部 I/O 由来の文字列表現
- **Primitive DTO**：プリミティブ型のみで構成された DTO（意味付けをしない）
- **Exchange DTO**：取引所固有の DTO（取引所名を含む）
- **Abstract DTO**：取引所横断の抽象 DTO（公開契約）

### 6.2 層ごとの許可型
- Wire：Wire string（入口のみ）
- Raw：Primitive DTO / Exchange DTO（lossless）
- Normalized：Exchange DTO（意味確定済み）および Contracts への変換材料
- Contracts：Abstract DTO（公開契約）のみ

### 6.3 命名規約（機械判定）
- Exchange DTO は型名または名前空間に取引所名を必須とする（MUST）。
- 横断型（Abstract/Contract）は型名・名前空間に取引所名を含めてはならない（MUST NOT）。

---

## 7. 固有／横断（Variation）

- 横断要素は配置として Shared に集約する（MUST）。
- 固有要素は Exchanges/<Exchange> に閉じる（MUST）。
- Contracts は最上位の共通（公開契約）として独立する（MUST）。

---

## 8. 物理構成（src）

### 8.1 正本宣言
- `src/` 配下の物理ディレクトリ構成は **仕様の一部（正本）**である（MUST）。
- 文書と物理構成が矛盾した場合は、原則として文書を修正する（MUST）。

### 8.2 最小骨格（Skeleton）

```
src/
  Shared/
  Exchanges/
    <ExchangeName>/
      Wire/
      Raw/
      Normalized/
      Adapter/
  Contracts/
```

### 8.3 配置規範

取引所固有コードは常に `src/Exchanges/<Exchange>/...` に配置する。  
取引所横断の実装基盤は常に `src/Shared/...` に配置する。  
公開契約（Contracts）は `src/Contracts/...` に配置する（最上位の共通であり Shared には含めない）。  
この規範に例外を設けてはならない。

### 8.4 物理は Exchange 主導 / アセンブリは Layer 主導

物理構成は Exchange 主導とする。  
アセンブリ（csproj）は Layer 主導とし、`src/Exchanges/*/<Layer>` を束ねる。  
例外は禁止する。

### 8.5 Namespace 規則（例外禁止）

すべての C# ソースコードは物理ディレクトリ構成と一致する namespace を持たなければならない。  
例外は禁止する。

例:
- src/Exchanges/Bitflyer/Wire/... → ExchangeApi.Exchanges.Bitflyer.Wire...
- src/Shared/Transport/... → ExchangeApi.Shared.Transport...
- src/Contracts/... → ExchangeApi.Contracts...

---

## 9. Transport

Transport（HTTP/JSON/Retry 等の横断的通信基盤）は Shared の責務とする。  
Transport は `src/Shared/Transport/` に配置する。  
Wire は Transport を内包しない。例外は禁止する。

---

## 10. RawJson の扱い

- RawJson は Raw の結果として保持し、必要に応じて下流へ伝播してよい（MUST）。
- RawJson の解釈（意味確定）は Normalized 以降でのみ行う（MUST）。

---

## 11. 契約の進化（Interface Evolution）

- 公開契約（Contracts）の破壊的変更は原則禁止し、必要な場合は互換レイヤまたはメジャーバージョンで表現する（MUST）。
- 境界（in/out 型、Call-only、層ジャンプ禁止）を破る変更は **例外**として扱い、Decisions に理由を記録しなければならない（MUST）。

---

## 12. 例外

- 本書の規範に従えない場合は Decisions / Exceptions に理由を記録しなければならない（MUST）。
- 例外として認められるのは次の場合に限る。
  - 公式 API 仕様による不可避な制約
  - 後方互換性維持のための必要性
  - セキュリティ・性能・法令上の理由
