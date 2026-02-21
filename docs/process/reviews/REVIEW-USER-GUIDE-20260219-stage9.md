# REVIEW-USER-GUIDE-20260219-stage9

`docs/process/reviews/templates/REVIEW-USER-GUIDE.md` に基づく監査結果（利用者導線の現状評価）。
本書は実施時点の語彙（例: `NonFatal` / `重大度 (F番号明示)`）を保持する履歴ログであり、語彙Lint対象外とする（`docs/process/reviews/README.md` 参照）。

---

## 0. 対象

* PR番号: （監査のみ）
* ガイド範囲:
  * `README.md`
  * `docs/index.md`
  * `docs/process/public-surface.md`
  * `docs/process/templates/README.md`
  * `docs/normative/contracts/overview.md`（安定保証境界の根拠）
* 想定読者: Bot / 高度利用 / 初見
* 想定環境: .NET（詳細はガイド側で明記すべき）
* 変更概要: User Guide 導線の評価（テンプレ新設に伴う初回適用）
* 対象ブランチ: `stage9`
* 実施日: 2026-02-19

---

## 1. 判定サマリ

| 観点 | 判定 | 重大度 (F番号明示) | CI化可否 | 備考 |
| --- | --- | --- | --- | --- |
| 初回成功導線（Quickstart） | OK | NonFatal | 一部可 | README に Contracts/Normalized の最短例（コマンド+期待結果）を追加 |
| 認証/秘密情報の扱い | OK | NonFatal | 一部可 | テンプレ運用で平文秘匿を明示 |
| 失敗時の対処（Troubleshooting） | OK | NonFatal | 可 | index から resilience / templates / public-surface に誘導 |
| 安定保証境界の明示 | OK | NonFatal | 可 | Contracts only / Normalized follow が明示 |
| コピペ実行性（再現可能性） | OK | NonFatal | 一部可 | Contracts は認証不要でコピペ可。Normalized は secret 運用が前提 |
| SSOT参照/保守容易性 | OK | NonFatal | 可 | SSOTへリンク中心で写経を避けている |

---

## 2. 指摘一覧（重大順）

`[OK] README に「最初の1コール」（Contracts/Normalized の2パス）を追加 - README.md:13 - NonFatal - REVIEW-USER-GUIDE（初回成功導線）`
`[OK] 目的別の最短導線（読了パス）を追加 - docs/index.md:28 - NonFatal - REVIEW-USER-GUIDE（初回成功導線）`
`[OK] 安定保証境界（Contracts only）が明示されている - README.md:117 - NonFatal - REVIEW-USER-GUIDE（安定保証境界）`
`[OK] Normalized 利用は追従前提（互換保証外）であることが明示されている - docs/index.md:73 - NonFatal - REVIEW-USER-GUIDE（安定保証境界）`
`[OK] 公開面の利用レベルと入口（CreateClient / CreateContractClient）が明示されている - docs/process/public-surface.md:14 - NonFatal - REVIEW-USER-GUIDE（安定保証境界/導線）`
`[OK] 資格情報テンプレ運用で「平文/秘密鍵を置かない」方針が明示されている - docs/process/templates/README.md:3 - NonFatal - REVIEW-USER-GUIDE（認証/秘密情報）`
`[OK] 失敗時の対処（429/timeout/認証）の最小導線を追加 - docs/index.md:77 - NonFatal - REVIEW-USER-GUIDE（Troubleshooting）`
`[OK] コピペ実行できる最小コード（Contracts/Normalized の選択例）を追加 - README.md:28 - NonFatal - REVIEW-USER-GUIDE（コピペ実行性）`

---

## 3. 改善案（最小 / 実施済み）

1. `README.md` に「最初の1コール」導線を追加（Contracts版 + Normalized版の2パス、secretはplaceholder）
2. `docs/index.md` に「利用目的別の最短読了パス」を追加（Bot/高度利用/安定重視）
3. 429/timeout/認証失敗の最小 Troubleshooting を `docs/index.md` から `resilience.md` へ誘導（写経しない）

---

## 4. 最終結論

* OK（初見導線を追加し、SSOT/安全性を維持したまま「最初の1コール」まで到達可能）
