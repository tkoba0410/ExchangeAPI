# Review Checklist

## Purpose

本チェックリストは、ExchangeAPI における **PR（Pull Request）時の最終判断装置**として使用する。

ここでの目的は、

* 実装の良し悪しを議論することではなく
* **設計原則・境界・契約が守られているか**を機械的に確認すること

である。

PR 作成者・レビュー担当者の双方が、
**感覚ではなく文書に基づいて判断する**ためのチェック項目とする。

---

## How to Use

* PR 作成者は、提出前に本チェックリストを自己確認する
* レビュー時は、本リストに基づき Yes / No で確認する
* No が含まれる場合は、理由を明示する
* 意図的な例外の場合は、必ず `docs/exceptions.md` に記録する

---

## 必須（Merge 前に必ず確認）

### 境界（Wire/Raw/Normalized/Contracts）

- [ ] Wire が JSON をパースしていない / DTO を返していない（text/bytes のみ）
- [ ] Raw DTO に enum/type（意味型）が混入していない（RawValue の閉集合のみ）
- [ ] Raw が HTTP/WS を直接叩いていない（transport は Wire）
- [ ] Normalized に横断抽象（Contracts 相当）が入っていない
- [ ] Contracts に取引所固有要素が混入していない（必要なら例外台帳へ）

### 例外（Exceptions Ledger）

- [ ] 原則からの逸脱がある場合、`docs/exceptions.md` に **記録がある**（未登録の例外は禁止）

---

## 1. Scope Check

* [ ] この PR は **設計原則・境界・契約**に影響するか

  * [ ] Yes → 以下を続行
  * [ ] No  → 本チェックリストは参考のみで可

---

## 2. Principle Check (TopSpec)

* [ ] 変更内容は `docs/topspec-guide.md` の原則に反していない
* [ ] 新しい原則を、文書化せずにコードへ持ち込んでいない
* [ ] 原則変更が必要な場合、TopSpec に明示的に反映している

---

## 3. Boundary Check (Interfaces)

* [ ] 層の責務を越えた依存が追加されていない
* [ ] Raw / Adapter から Normalized / Public への逆依存がない
* [ ] 層境界で string を直接受け渡していない
* [ ] RawJson / JsonElement が上位層へ漏れていない

---

## 4. Contract Check

* [ ] 公開 DTO の形状・命名が `docs/contracts.md` に準拠している
* [ ] Nullable の導入理由を説明できる
* [ ] Page / Cursor / Limit の意味論を破っていない
* [ ] Response / Result の直返しをしていない（Call-only）

---

## 5. API Inventory Check

* [ ] 新規・変更された外部 API が `docs/endpoints.md` に記載されている
* [ ] Official Reference（公式ドキュメント）が明示されている
* [ ] Internal Mapping（Raw / Normalized）が一致している

---

## 6. Checklist Compliance

* [ ] 新規取引所追加の場合、`new-exchange.md` を確認した
* [ ] 新規エンドポイント追加の場合、`new-endpoint.md` を確認した

---

## 7. Exception Handling

* [ ] 原則・境界・契約からの逸脱は存在しない

  * [ ] 存在する場合、`docs/exceptions.md` に記録した
  * [ ] 理由と影響範囲を説明できる

---

## 8. Anti-Checklist

以下に該当する場合、PR を進めない。

* [ ] 「あとで直す」前提の設計変更
* [ ] 文書化が追いついていないままの境界変更
* [ ] 公式 API 仕様の写経を含む変更（一覧・棚卸し目的のエンドポイント記載を除く）

---

## Authority

本チェックリストは、PR レビュー時の判断において
**コードより優先される基準**とする。

判断に迷った場合は、

* Documentation Policy
* TopSpec Guide
* Contracts / Interfaces

に立ち返り、それでも解決しない場合は
**変更しない選択**を優先する。
