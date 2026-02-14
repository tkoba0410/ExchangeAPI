# Stage 8（完了判定）

## Goal

TopSpec / Contracts / Governance / Process / Exceptions / Inventory の役割が重複せず、設計判断の参照先が常に一意で、README と _references が判断を含まない状態を完成させる。

## DoD（Yes/No）

- [ ] README が導線のみで、SSOT指定や判断文が存在しない（→ docs/index.md / docs/process.md に寄せる）
- [ ] docs/index.md が Exceptions を「逸脱台帳/決定記録」として案内し、エラー分類とは混同しない
- [ ] docs/process.md の文書カテゴリ定義と docs 配下の実ファイルが矛盾しない
- [ ] `docs/_references/` が Normative を匂わせる表現を持たない（informative である旨が明確）
- [ ] Inventory（`docs/inventory/endpoints-*.md`）が Fact のみで、判断文を含まない
