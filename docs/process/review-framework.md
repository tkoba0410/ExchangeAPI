# Review Framework

本書は Stage9 におけるレビュー体系の運用正本（Process SSOT）である。

Stage9 で確立した本体系を、運用可能な形で継続維持することを目的とする。
本書は `docs/process/` 配下で管理し、運用結果に応じて継続的に更新する。
Stage9 終了判定および無限レビューループ抑止の詳細規則は
`docs/stage_9_close_policy.md` を参照する。

本体系は業界標準を参考にしつつ、
ExchangeAPI の規模・目的に合わせて軽量化して再構成している。

---

# 参照標準

* Google Engineering Practices（Code Review）
* ISO/IEC 25010（Software Quality Model）
* ATAM（Architecture Tradeoff Analysis Method）
* Microsoft REST API Guidelines
* Google API Design Guide
* Diátaxis Documentation Framework
* OWASP ASVS / Secure Coding Guidelines

これらを完全適用することは目的としない。
本体系は「統治可能性」を目的とする。

---

# 軽量化方針

* 単一リポジトリ中心である
* 動的テストは導入しない
* 静的テストは最小限
* 破壊的変更は許容する
* Bot は別リポジトリで実装する

したがって、委員会方式や網羅的監査は行わない。
PRベースの軽量レビューを基本とする。

---

# 品質軸（Quality Axes）

品質軸は以下の7本に固定する。

1. Boundary
2. Consistency
3. Contracts
4. Reliability
5. Security
6. DX
7. Change

新しい観点を追加する場合は、まず既存軸にマッピングできるかを検討する。

---

# Structure Stability Contract（崩さない対象）

## Boundary

* 層責務の混線禁止
* 依存方向逆流禁止
* Core の exchange 非依存
* 横断的情報塊の復活禁止

## Consistency

* EndpointId 起点の命名維持
* 同概念の分裂禁止
* 定数 / enum 統一
* Cross-exchange parity 維持

## Contracts

* public surface 最小化維持
* Try / OrThrow 統一維持
* string 流入禁止維持
* DTO / ValueObject 境界維持

## Reliability

* Expected / Unexpected 分離維持
* 429 / timeout / partial failure 分離
* 再試行安全性維持
* 診断可能性維持

## Security

* 署名 / Canonicalize 整合維持
* 秘密情報ログ出力禁止
* nonce / 時刻依存安全性維持

## DX

* 自然な利用形態維持
* 誤用しにくい設計維持
* エラー時の次行動提示

## Change

* 破壊的変更記録必須
* 移行方法提示必須
* Bot 影響明示

---

# 深度モデル

L1: 常設（毎PR）
L2: 条件付き（トリガ制）
L3: 節目（Stage締め）

注記:

* 本体系では 7 軸を定義し、各軸に対応するテンプレートを用意する。
* テンプレートは運用結果に応じて更新されうる（ただし品質軸の追加は行わない）。

---

# L1 常設確認

* Boundary違反がない
* 命名・構造の揺れがない
* public surface増減確認
* 秘密情報ログ出力がない

---

# L2 トリガ

以下に該当する場合、対応レビューを実施する。

* Contracts / public 変更
* Signer / 認証 / Canonicalize 変更
* Error / Retry / 429 / timeout 変更
* Normalized / Cross-exchange 構造変更
* Breaking change を含む
* 文書構造 / SSOT 変更（補助監査: `REVIEW-DOCS`）
* ユーザ向けガイド（Quickstart / HowTo / Examples）変更（補助監査: `REVIEW-USER-GUIDE`）

注記:

* `REVIEW-DOCS` / `REVIEW-USER-GUIDE` は 7 品質軸の補助監査であり、軸件数集計には含めない。
* 利用者向け文書のみの変更は `REVIEW-USER-GUIDE` を主軸とし、DX は原則トリガしない。
* 軸別テンプレートへのマッピング詳細は `docs/process/codex-review-runbook.md` を正とする。

---

# L3 節目レビュー

* Boundary 再評価
* public surface 棚卸し
* DX 確認
* エラーモデル再点検
* 文書構造総整理

---

# 実施フロー

1. すべての PR で L1 を確認
2. L2 トリガを判定
3. 該当する場合は L2 レビュー（軸レビュー + 必要に応じて補助監査）を実施
4. Stage 終了前に L3 を実施
5. Stage 終了判定は `docs/stage_9_close_policy.md` の必須条件・証跡要件で裁定する

---

# 文書とコードの収束判定（Convergence Gate）

レビュー往復の長期化を防ぐため、文書とコードの収束地点を次で固定する。

1. 変更ごとに `収束アンカー` を 1 つ定義する（正本文書 1 つ + 主実装 1 箇所）。
2. 文書とコードが乖離した場合の修正方向は `docs/normative/governance.md` の正本優先順で決める。
3. 正本が Normative の場合、コードを正本へ合わせる。Process/Reference のみが乖離している場合、文書を現実装へ合わせる。
4. 収束完了条件は「対象スコープに未解消 NG がない」「未確定メモを残さない」の 2 点とする。
5. 再レビューは前回 NG 指摘のみを対象とし、新規指摘は重大回帰に限定する。
6. 同一論点が 3 回目のレビューでも収束しない場合、レビューを継続せず裁定（Normative 更新 / `exceptions` 記録 / `CHANGE-*` 記録）に切り替える。

注記:

* Stage9 クローズ時の適用詳細は `docs/stage_9_close_policy.md` 9 章を正とする。

---

本体系は固定的な品質保証制度ではない。
変更を制御可能にするための統治構造である。
