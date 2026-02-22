# REVIEW-MERGE-CHECK-20260221-stage9

対象: `stage9`  
基準: `docs/process/process.md` 7.2（Merge 前必須チェック）  
実施日: 2026-02-21

---

## 判定

- 総合: **Merge 可**
- 未解消 NG: **0**
- 未解消 Fatal: **0**

---

## 7.2 チェック結果

### A. 層と境界（TopSpec）

- [x] 層呼び出しは隣接層に限定
- [x] Wire は JSON を解釈しない
- [x] `string` の Wire 下流漏れなし
- [x] Raw は lossless（意味確定をしない）
- [x] Raw DTO / RawJson の Contracts 露出なし
- [x] 単一 API 実装内で複数取引所 Raw/Normalized 混在なし

### A2. 非層カテゴリ（TopSpec）

- [x] `Application` は `Contracts.*` 非参照
- [x] Facade Request/Interface を `Application` へ直接流していない
- [x] `Composition` から実装都合の逆流なし

### B. 契約（Contracts）

- [x] 公開 API は Call-only
- [x] Contracts に取引所名・取引所固有要素の混入なし
- [x] DTO 命名/Nullable/Page-Cursor-Limit 規範に違反なし
- [x] 削除した Batch DTO の Breaking Change は CHANGE に記録済み

### C. Inventory（事実一覧）

- [x] 外部 API 追加/変更なし（inventory 追記不要）
- [x] Official Reference 既存整合を維持
- [x] `PresentIn` 語彙整合を維持
- [x] `PresentIn=Raw` / `Normalized` 対応実装整合を維持
- [x] inventory `EndpointId` alias 混入なし

### D. 例外（Decisions）

- [x] 未登録の例外なし（新規例外記録不要）

### E. 物理配置変更時の文書同期

- [x] `<exchange>` 配下構造変更に対して `topspec.md` と `exchange-module-shape.json` を同期更新
- [x] `contracts.md` への物理配置重複記載なし
- [x] `*LayoutParityTests.cs` へ反映し、`dotnet test` で検証済み

---

## 根拠リンク

- 7.2 基準: `docs/process/process.md:178`
- 配置SSOT: `docs/normative/layout/exchange-module-shape.json:1`
- 配置検証: `tests/Common.Tests/Architecture/ExchangeModuleLayoutParityTests.cs:13`
- shape fail-closed 検証: `tests/Common.Tests/Architecture/ExchangeModuleLayoutShapeValidationTests.cs:27`
- Transport設定統一: `src/Transport/Http/TransportConfig.cs:10`
- Transport解決ロジック: `src/Transport/Http/TransportConfigResolver.cs:11`
- RestClient配線: `src/Composition/Bootstrap/Transport/RestClientFactory.cs:15`
- Breaking Change記録（Batch DTO削除）: `docs/process/CHANGE-20260221-contracts-remove-batch-dtos.md:13`
- Breaking Change記録（Transport統一）: `docs/process/CHANGE-20260219-transport-config-unification.md:8`
- CI統制（review lint）: `.github/workflows/ci.yml:20`
- 節目監査サマリ: `docs/process/reviews/REVIEW-SUMMARY-20260221-stage9.md`
