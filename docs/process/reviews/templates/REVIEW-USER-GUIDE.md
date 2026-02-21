# REVIEW-USER-GUIDE

本レビューは User Guide（利用開始 / 実行例 / セットアップ / 失敗時の対処）に基づく確認を行う。

重大度定義は `PROJECT-FATAL-DEFINITION.md` を参照する。
重大度は `Severity` と `FatalClass` の 2 軸で記録すること。
`Severity=Fatal` の場合は `FatalClass=F1〜F5` を明示すること。

---

## 0. 対象

* PR番号:
* ガイド範囲（例: `docs/guides/*` / `README.md`）:
* 想定読者（例: Bot / 高度利用 / 初見）:
* 想定環境（OS/SDK/言語）:
* 変更概要:

---

## 1. 判定サマリ

| 観点 | 判定 | Severity (Fatal/High/Medium/Low/Nit) | FatalClass (F1-F5/None) | CI化可否 | 備考 |
| --- | --- | --- | --- | --- | --- |
| 初回成功導線（Quickstart） |  |  |  |  |  |
| 認証/秘密情報の扱い |  |  |  |  |  |
| 失敗時の対処（Troubleshooting） |  |  |  |  |  |
| 安定保証境界の明示 |  |  |  |  |  |
| コピペ実行性（再現可能性） |  |  |  |  |  |
| SSOT参照/保守容易性 |  |  |  |  |  |

---

## 2. 観点詳細

### 初回成功導線（Quickstart）

* 判定基準: 初見利用者が最短で「1回叩ける」まで到達できる
* OK条件:
  * 前提条件（SDK/環境変数/権限）が明記されている
  * 最初の 1 コールが具体的（入力/出力/期待結果がある）
  * `Contracts` / `Normalized` のどちらを使うかが導線として明確
* NG条件:
  * リンク集のみで、最初の実行手順がない
  * 正本/参照が混線し、どこを読めばよいか判断不能
* 不合格例: README が規範への導線のみで、実行例が存在しない
* 該当Fatal: F2（SSOT逸脱がある場合）/ F3（公開契約と矛盾する導線を提示した場合）
* 修正方針: 手順本文はガイドに置き、規範本文は SSOT へリンクする（写経しない）

### 認証/秘密情報の扱い

* 判定基準: 利用開始手順が secret を安全に扱い、平文露出/コミット誘導がない
* OK条件:
  * secret（APIキー/シークレット/秘密鍵）をリポジトリ管理下に置かない手順になっている
  * ログ出力・例外メッセージに secret を含めない注意がある
  * テンプレは `*.template.*` / `*.example.*` のみを前提とし、実ファイルの配置を要求しない
* NG条件:
  * secret を `docs/` 配下に置く / コミットする / 共有することを誘導している
  * コマンド例に secret 値の貼り付けを要求している
* 不合格例: `.env`/JSON に secret を直書きしてコミットする手順
* 該当Fatal: F4（secret露出・秘匿性破壊）
* 修正方針: `docs/process/process.md` と `docs/process/templates/` の運用に寄せる

### 失敗時の対処（Troubleshooting）

* 判定基準: 典型的な失敗が想定され、復旧手順が安全に記述されている
* OK条件:
  * 認証失敗/429/タイムアウト等の典型失敗の扱いが書かれている
  * 失敗時の判断（再試行/停止/待機）が契約と矛盾しない
* NG条件:
  * エラーを握りつぶす / 無制限リトライなど、運用上危険な手順を推奨している
  * 契約（resilience）と矛盾する対処が書かれている
* 不合格例: 429 を無視して即時再送し続ける推奨
* 該当Fatal: F5（重大な信頼性欠陥を誘発する記述）/ F2（契約と矛盾する場合）
* 修正方針: `docs/normative/contracts/resilience.md` の契約に合わせて記述する

### 安定保証境界の明示

* 判定基準: 何が「安定（互換保証）」で何が「追従前提」かが誤解なく伝わる
* OK条件:
  * 公開安定 API が Contracts のみであることを明示している
  * Normalized の利用は可能だが互換保証外（追従前提）であることを明示している
* NG条件:
  * Normalized を「安定 API」として扱う説明になっている
  * `Wire/Raw/Adapter/Internal` を外部利用導線に含めている
* 不合格例: 「Normalized は安定だからこれに依存してよい」と断言している
* 該当Fatal: F2（SSOT逸脱）/ F3（公開契約破壊に繋がる場合）
* 修正方針: `docs/normative/contracts/overview.md` と `docs/process/public-surface.md` を正本参照する

### コピペ実行性（再現可能性）

* 判定基準: サンプルがコピペで実行でき、結果が検証可能
* OK条件:
  * サンプルは最小依存で成立し、前提と期待結果がある
  * Placeholder（`<API_KEY>` 等）を使い、実値の貼り付けを要求しない
* NG条件:
  * コンパイル不能/依存不明/出力が曖昧で、成功/失敗の判定ができない
* 不合格例: 断片コードのみで、必要な using/参照がない
* 該当Fatal: （原則 NonFatal。SSOT逸脱/secret露出を含む場合は F2/F4）
* 修正方針: 最小コード + 実行コマンド + 期待結果を 1 セットにする

### SSOT参照/保守容易性

* 判定基準: ガイドが SSOT を侵食せず、更新乖離を起こしにくい
* OK条件:
  * 規範（MUST/MUST NOT）本文をガイドに複製していない（リンクで参照）
  * 破壊的変更がある場合、CHANGE に誘導される
* NG条件:
  * ガイド側が事実上の正本になり、Normative/Process と矛盾する
* 不合格例: ガイドに独自ルールが増殖し、SSOT更新なしで運用される
* 該当Fatal: F2（SSOT逸脱）
* 修正方針: ルール本文は正本へ移し、ガイドは導線に徹する

---

## 3. CI自動化候補

* ガイド文書に「安定保証境界（Contracts only / Normalized follow）」が明示されているか検査
* ガイド文書に `Wire/Raw/Adapter/Internal` を導線として記載していないか検査
* docs 変更時の CHANGE 更新漏れ検査（breaking change の場合）

---

## 4. 関連Normative / 判例

* docs/process/public-surface.md
* docs/normative/contracts/overview.md
* docs/normative/contracts/contracts.md
* docs/normative/contracts/resilience.md
* docs/process/process.md（7.2）
* docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md

---

## 5. 最終結論

* OK / 要修正 / NG
