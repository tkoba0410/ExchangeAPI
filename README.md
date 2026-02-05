# ExchangeAPI

ExchangeAPI は、複数の暗号資産取引所 API を扱うための実装基盤です。

本 README は **設計判断・仕様説明を含みません**。  
判断の正本は `docs/` 配下の文書を参照してください。

※ 本 README は導線のみを提供し、
※ 内容理解には `docs/` 配下の正本を参照すること。

---

## Docs（必ずここから読む）

### 規範（Normative）

- **TopSpec**（最上位規範・禁止事項・層モデル）  
  `docs/topspec.md`

- **Contracts**（横断契約・公開 API の形状）  
  `docs/contracts/contracts.md`

---

### 運用（Process）

- **Process**（文書化・レビュー・例外運用）  
  `docs/process.md`

---

### 例外（Decisions）

- **Exceptions Ledger**（原則からの逸脱と理由）  
  `docs/exceptions.md`

---

### 一覧（Inventory / Fact）

- **Endpoints Inventory**  
  `docs/inventory/`

- **Contracts Inventory（SSOT）**  
  `docs/inventory/endpoints-contracts.md`  
  Contracts の実装対象は `endpoints-contracts.md` を正とする。

- **Exchange Inventories**  
  `docs/inventory/endpoints-bitflyer.md`  
  `docs/inventory/endpoints-bittrade.md`

---

## Reference

過去の設計文書・補足資料は `docs/_references/` に保管されています。  
これらは **Normative ではありません**。
