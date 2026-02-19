# REVIEW-USER-GUIDE-20260219-stage9

`docs/process/reviews/templates/REVIEW-USER-GUIDE.md` に基づく監査結果（利用者導線の現状評価）。

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
| 初回成功導線（Quickstart） | 要修正 | NonFatal | 一部可 | リンク中心で「最初の1コール」までの手順がない |
| 認証/秘密情報の扱い | OK | NonFatal | 一部可 | テンプレ運用で平文秘匿を明示 |
| 失敗時の対処（Troubleshooting） | 要修正 | NonFatal | 可 | 典型失敗と復旧手順がガイドとして未整備 |
| 安定保証境界の明示 | OK | NonFatal | 可 | Contracts only / Normalized follow が明示 |
| コピペ実行性（再現可能性） | 要修正 | NonFatal | 一部可 | コピペで動く最小コード/コマンドがない |
| SSOT参照/保守容易性 | OK | NonFatal | 可 | SSOTへリンク中心で写経を避けている |

---

## 2. 指摘一覧（重大順）

`[要修正] README と index がリンク中心で、最初の1コール（コマンド+期待結果）の導線がない - README.md:6 - NonFatal - REVIEW-USER-GUIDE（初回成功導線）`
`[要修正] 推奨読了順はあるが、実行可能な最短手順（例: 3分セットアップ）が定義されていない - docs/index.md:122 - NonFatal - REVIEW-USER-GUIDE（初回成功導線）`
`[OK] 安定保証境界（Contracts only）が明示されている - README.md:15 - NonFatal - REVIEW-USER-GUIDE（安定保証境界）`
`[OK] Normalized 利用は追従前提（互換保証外）であることが明示されている - docs/index.md:63 - NonFatal - REVIEW-USER-GUIDE（安定保証境界）`
`[OK] 公開面の利用レベルと入口（CreateClient / CreateContractClient）が明示されている - docs/process/public-surface.md:14 - NonFatal - REVIEW-USER-GUIDE（安定保証境界/導線）`
`[OK] 資格情報テンプレ運用で「平文/秘密鍵を置かない」方針が明示されている - docs/process/templates/README.md:3 - NonFatal - REVIEW-USER-GUIDE（認証/秘密情報）`
`[要修正] 失敗時の対処（認証失敗/429/timeout）の判断フローがガイドとして未整備 - docs/index.md:50 - NonFatal - REVIEW-USER-GUIDE（Troubleshooting）`
`[要修正] コピペ実行できる最小コード（Contracts/Normalized の選択例）が存在しない - README.md:1 - NonFatal - REVIEW-USER-GUIDE（コピペ実行性）`

---

## 3. 改善案（最小）

1. `README.md` に「最初の1コール」導線を追加（Contracts版 + Normalized版の2パス、secretはplaceholder）
2. `docs/index.md` に「利用目的別の最短読了パス」を追加（Bot/高度利用/安定重視）
3. 429/timeout/認証失敗の最小 Troubleshooting を `docs/index.md` から `resilience.md` へ誘導（写経しない）

---

## 4. 最終結論

* 要修正（初見導線の不足。SSOT/安全性は維持できている）

