# Documentation Index

このディレクトリは、ExchangeAPI プロジェクトにおける  
**設計・契約・運用判断を固定するための文書群**を管理する。

文書の配置・粒度・更新ルールについては、  
以下の文書を正とする。

- [Documentation Policy](./documentation-policy.md)

---

## Design Principles

- [TopSpec Guide](./topspec/guide.md)  
  覆さない設計原則・判断理由（Why）

- [TopSpec Core](./topspec/core.md)  
  全体構造・層構成の要約（Where / What）

---

## Contracts and Boundaries

- [Interfaces](./contracts/interfaces.md)  
  層境界・公開インターフェースの定義

- [Contracts](./contracts/contracts.md)  
  DTO、命名規約、共通契約（Page / Cursor 等）

---

## API Inventory

- [Endpoints (Human-readable)](./inventory/endpoints.md)  
  使用している API の一覧（仕様は記載しない）

- [Endpoints (Machine-readable)](./inventory/endpoints.yaml)  
  上記の機械可読版（将来用途）

---

## Checklists

- [New Exchange](./checklists/new-exchange.md)  
  取引所追加時の確認項目

- [New Endpoint](./checklists/new-endpoint.md)  
  エンドポイント追加時の確認項目

- [Review](./checklists/review.md)  
  PR / レビュー時の自己確認

---

## Exceptions

- [Exceptions Ledger](./exceptions.md)  
  意図的な設計原則逸脱の記録

---

## Notes

- 本ディレクトリ配下の文書は、README に内容を重複させない
- API仕様の詳細は公式ドキュメントを正本とする
- 文書を追加・変更する際は、Documentation Policy に従う
