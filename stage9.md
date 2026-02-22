# Stage9（現況）

最終更新: 2026-02-22  
対象ブランチ: `stage9`

## 1. 位置づけ

Stage9 は、機能拡張フェーズではなく、構造と運用の安定化フェーズである。  
目的は「変更を止めること」ではなく、「変更を統治可能にすること」にある。

正本:
- `docs/process/review-framework.md`
- `docs/process/codex-review-runbook.md`
- `docs/process/process.md`

---

## 2. 現在の判定（2026-02-21 時点）

- Stage9 完了宣言（基準判定日: 2026-02-19）: **可**（`docs/process/reviews/STAGE9-FINAL-DECISION-20260219.md`）
- 補足: 2026-02-19 判定時点では未解消 NonFatal が 3 件（`docs/process/reviews/STAGE9-FINAL-DECISION-20260219.md`）。
- 節目レビュー（L3相当）: **PASS**（`NG=0 / Fatal=0`）
  - `docs/process/reviews/REVIEW-SUMMARY-20260221-stage9.md`
- 最終レビュー（Merge前 7.2）: **Merge 可**（`NG=0 / Fatal=0`）
  - `docs/process/reviews/REVIEW-MERGE-CHECK-20260221-stage9.md`

---

## 3. Stage9で固定した事項

- 品質軸は 7 本で固定（Boundary / Consistency / Contracts / Reliability / Security / DX / Change）
- 深度モデルは固定（L1 常設 / L2 トリガ / L3 節目）
- 重大度モデルは固定（`Severity` + `FatalClass`）
- 破壊的変更は `docs/process/CHANGE-*.md` への記録を必須化
- 文書運用は `process.md` の 7.2 最終チェックで機械的に確認

---

## 4. 直近で確定した安定化項目（2026-02-21）

- Exchange 配下構造の機械可読 SSOT を導入  
  `docs/normative/layout/exchange-module-shape.json`
- 形状パリティ検証と fail-closed 検証をテスト化  
  `tests/Common.Tests/Architecture/ExchangeModuleLayoutParityTests.cs`  
  `tests/Common.Tests/Architecture/ExchangeModuleLayoutShapeValidationTests.cs`
- Transport 設定を排他的 `TransportConfig` に統一  
  `src/Transport/Http/TransportConfig.cs`  
  `src/Transport/Http/TransportConfigResolver.cs`
- レビュー運用lintを CI 常設化  
  `scripts/ci/lint-review-axis-alignment.sh`  
  `scripts/ci/lint-reference-review-status.sh`  
  `.github/workflows/ci.yml`

---

## 5. DoD 達成状況

- [x] `review-framework.md` が確立している
- [x] レビュー運用（L1/L2）が runbook と CI で機能している
- [x] 構造安定化対象の未解消 NG/Fatal がない（2026-02-21 監査）
- [x] 破壊的変更は CHANGE 記録が残っている
- [x] Stage 締めレビュー（L3相当）が実施済み

---

## 6. 非固定事項（継続進化）

- 実装詳細（DTO/ValueObject、内部構造、エラーモデル細分化）
- レビュー資産の改善（テンプレ運用、補助監査）
- DX 改善

上記は、固定済み品質軸と運用ルールの範囲で継続的に更新する。

---

## 7. 廃止条件（Sunset）

Stage 文書（`stage*.md`）は初回リリース前の暫定文書。  
`v1.0.0` 時点で本書を `docs/archive/` へ移動し、以後の追跡は `docs/process/revision-history.md` に統合する。
