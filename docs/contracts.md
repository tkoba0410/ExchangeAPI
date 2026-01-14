# Contracts（横断契約）

本書は、本リポジトリにおける **公開契約（Contract）** を定義する最上位規範（Normative）である。  
ここに記載された MUST / MUST NOT は拘束力を持ち、実装都合よりも優先される。

---

## 1. 目的

- Contracts 層は、取引所をまたいで共通に利用できる **公開 API の契約**を提供する（MUST）。
- Contracts は **振る舞い（Behavior）ではなく、型・形状・意味論（Shape/Semantics）** を定義する（MUST）。
- 網羅性よりも安定性を優先し、必要最小限のみを追加する（MUST）。

---

## 2. 適用範囲

- 本書は `src/Contracts/` 配下の型・インターフェイス・公開 API 仕様に適用される（MUST）。
- Contracts は **取引所非依存**でなければならない（MUST）。

---

## 3. 取引所非依存（横断性）

- Contracts の型・名前空間・識別子に **取引所名を含めてはならない**（MUST NOT）。
- Contracts に置いてよいのは、取引所横断の **Abstract DTO / Request / Response / Error / Result** のみである（MUST）。
- 取引所固有の差分は Contracts で表現してはならない（MUST NOT）。差分は Normalized/Adapter 側で吸収し、必要なら Decisions（Exceptions）に記録する（MUST）。

---

## 4. API 返却形式（Call-only）

- 公開 API は **Call を唯一の返却形式**とする（MUST）。
- Response（DTO）を直接返してはならない（MUST NOT）。
- Call は **成功/失敗/メタ情報**を一体として表現できなければならない（MUST）。

---

## 5. エラーとメタ情報

- エラーは Call のメタ情報（例：CallMeta）に集約する（MUST）。
- 例外（throw）は、エントリポイント（例：OrThrow / ParseOrThrow 等）以外では使用してはならない（MUST NOT）。
- エラー表現は **分類レベル**（例：通信失敗 / 認証失敗 / 業務的失敗）までに留め、詳細コード体系を契約に含めない（MUST）。
- リトライ可否・HTTP ステータス・レート制限などの運用情報は、必要に応じて Call のメタで表現する（MUST）。

---

## 6. 型の所有権

- Abstract DTO（公開契約の型）は **Contracts 層が定義元（オーナー）**である（MUST）。
- Normalized 層は Contracts の型を返す。Normalized 独自の抽象型を公開してはならない（MUST NOT）。

---

## 7. 層境界の型制約

- Contracts の公開メソッド（インターフェイス）は、Contracts で許可された型のみを in/out に用いる（MUST）。
- Raw DTO / Exchange DTO / Wire string を Contracts の in/out に含めてはならない（MUST NOT）。

---

## 8. 命名規約

### 8.1 DTO 命名

- 型名は **名詞 + Context** を基本とする（MUST）。
  例：`OrderSnapshot`, `ExecutionHistoryItem`
  （※例は規範ではなく参考）
- 意味の異なる DTO を suffix だけで区別してはならない（MUST NOT）。
- Raw / Normalized / Adapter の区別を型名 suffix で表現してはならない（MUST NOT）。区別は namespace / フォルダで行う（MUST）。

### 8.2 プロパティ命名

- 公開プロパティは PascalCase とする（MUST）。
- 略語は一般的なもののみ使用する（例：Id, Url 等）（MUST）。
- 取引所固有の語彙をそのまま転記してはならない（MUST NOT）。

---

## 9. Nullable / Optional ポリシー

- Nullable を許可するのは、次の場合に限る（MUST）。
  - 公式 API 上、常に欠落する可能性がある
  - 取引所によって意味が異なる
  - 将来的な拡張余地として意図的に空を許容する
- Nullable は設計上の判断であり、「たまたま無い」ことを理由にしてはならない（MUST NOT）。
- 常に存在する概念は Non-nullable とする（MUST）。
- 値が不明な場合は Nullable に逃がさず、別の状態・型で表現する（MUST）。

---

## 10. Page / Cursor / Limit 契約

- Page は「1回の取得結果」を表す（MUST）。
- 戻り値の件数と要求した limit は区別される（MUST）。
  - Limit は「要求」であり、保証ではない（MUST）
  - 実際に適用された limit は Meta 情報として保持する（MUST）
- Cursor はページング状態を表す **opaque な値**とする（MUST）。
  Cursor の内部構造や生成方法は契約に含めない（MUST NOT）。

---

## 11. 互換性（Evolution / Breaking Change）

- Contracts の変更は利用者に影響するため、後方互換性を原則維持する（MUST）。
- 破壊的変更は禁止する（MUST NOT）。
- 破壊的変更が必要な場合は、互換レイヤ（新旧共存）またはメジャーバージョンで表現し、Decisions（Exceptions）に理由を記録する（MUST）。

---

## 12. Anti-Rules（禁止事項）

- 公式 API 仕様の写経をしてはならない（MUST NOT）。
- フィールド単位の詳細説明を契約に含めてはならない（MUST NOT）。
- 取引所ごとの差分説明を契約に含めてはならない（MUST NOT）。
- 実装都合による一時的 DTO を公開してはならない（MUST NOT）。

---

## 13. 例外の扱い

- 本書の規範に従えない場合は Decisions（Exceptions）に理由を記録しなければならない（MUST）。
- 例外として認められるのは、次の場合に限る（MUST）。
  - 公式 API 仕様による不可避な制約
  - 後方互換性維持のための必要性
  - セキュリティ・性能・法令上の理由
