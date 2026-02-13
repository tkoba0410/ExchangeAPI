# Exceptions Ledger

## Purpose

本ドキュメントは、ExchangeAPI プロジェクトにおける **設計原則・境界・契約からの意図的な逸脱（例外）** を記録するための台帳である。

例外は「失敗」や「妥協」を正当化するものではなく、
**制約・現実・外部要因を踏まえた判断結果を、説明責任のために記録するもの**である。

本書の目的は、

* なぜ例外が存在するのかを将来に説明できるようにすること
* 同じ議論を繰り返さないこと
* 将来的な解消候補を可視化すること

である。

---

## Principles

* 例外は **必ず文書化する**
* 例外は **最小範囲**に閉じる
* 例外は **理由・影響範囲・将来対応案**を伴う
* 例外を見つけたら、原則を破る前にまずここを確認する

「文書化されていない例外」は存在しないものとみなす。

---

## When to Register an Exception

以下に該当する場合、例外として本台帳に登録する。

* TopSpec Guide の原則に反する実装を行う場合
* Interfaces で定義された境界を越える必要がある場合
* Contracts の形状・意味論を守れない場合
* RawJson や Exchange 固有要素を上位層に露出せざるを得ない場合
* 将来的に解消予定だが、現時点では避けられない場合
* Adapter の共通 Call 骨格を適用できず、個別 `try/catch` 実装を維持する場合
* 共通 `Operations` 正本を使用できず、取引所別 `Operations.cs` に固有ラベルを追加する場合

---

## Registration Rules

* 1 例外 = 1 エントリとする
* 例外は **具体的な対象**に紐づける（API / 型 / 層 / 機能）
* 曖昧な表現や一般論は禁止する
* 恒久対応ではない場合、その旨を明示する

---

## Entry Format

各例外は、以下の形式で記載する。

```md
## <Short Title>

### Summary
<何が原則から外れているかを一文で>

### Reason
<なぜ避けられなかったのか>

### Affected Area
- Exchange:
- Layer:
- API / Type:

### Impact
<設計・利用者・将来拡張への影響>

### Mitigation
<被害を抑えるために行っている対策>

### Future Plan
<将来的に解消できる可能性があるか。ある場合は条件>

### Status
- [ ] Temporary
- [ ] Accepted
- [ ] To be removed
```

---

## Example

```md
## Bitflyer ParentOrder RawJson Exposure

### Summary
Normalized 内部で RawJson を保持している。

### Reason
公式 API の構造上、lossless な情報保持が必要なため。

### Affected Area
- Exchange: bitFlyer
- Layer: Normalized
- API / Type: ParentOrder

### Impact
Public API には露出しないが、内部構造が複雑化する。

### Mitigation
RawJson は Normalized 内部に閉じ、Adapter 以外へは露出しない。

### Future Plan
公式 API の仕様変更、または情報取捨選択が可能になった段階で解消を検討する。

### Status
- [x] Accepted
```

---

## Normalized Mapper Throws During Mapping (Bitflyer/Bittrade)

### Summary
Normalized の一部 Mapper に例外ベースの旧実装が残っているが、主要パスは Try 系で CallError に変換している。

### Reason
既存の Mapper が例外ベースで書かれており、Try 系への全面移行が段階的であるため。

### Affected Area
- Exchange: bitFlyer / Bittrade
- Layer: Normalized
- API / Type: *Mapper / *Normalizer（旧 API）

### Impact
旧 API が使われた場合に例外が発生する可能性があるが、主要パスは Try 系で CallError 化している。

### Mitigation
主要パスは Try 系で CallError 化。MapOk でも例外を捕捉して CallError に変換する。

### Future Plan
旧 API の使用箇所を完全に排除し、例外ベースの実装を整理・削除する。

### Status
- [x] Temporary

---

## Normalized Suffix Coexistence (Bitflyer/Bittrade)

### Summary
`*Normalized` 接尾辞は原則不使用だが、既存 DTO 群に接尾辞付き名称が残っている。

### Reason
命名ルール更新前に導入された型が広範囲で参照されており、段階移行を採用しているため。

### Affected Area
- Exchange: bitFlyer / Bittrade
- Layer: Normalized
- API / Type: `*Normalized` 命名の DTO（例: `ExecutionAccountNormalized`, `PositionNormalized`）

### Impact
新規型との命名統一時に、接尾辞の要否判断が再発する可能性がある。

### Mitigation
新規 DTO は `docs/naming-rules.md` の「原則不使用・衝突時のみ許可」に従う。
既存 DTO は API 境界直結化の修正時に順次整理する。

### Future Plan
参照影響が局所化した単位で、接尾辞を外すか、衝突理由を個別記録して恒久化するかを裁定する。

### Status
- [x] Temporary

---

## Authority

本台帳は、例外的判断に関する **唯一の正本**である。

PR レビュー時に例外が指摘された場合、

* 本台帳に記録されているか
* 新規登録が必要か

を必ず確認すること。

文書化されていない例外は、
**原則違反として扱う。**
