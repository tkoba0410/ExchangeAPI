# A050-STG1-DEV-Guidelines（Stage1 軽量開発方針）

Stage1 における開発を「作業者のとっかかりが良い／成果が見えやすい／重くならない」形で進めるために、既存の OVR / REQ / ARC / SPC を維持しつつも、それらの運用負荷を最小化するための **軽量ガイドライン** を定める。

本ガイドラインは Stage1 のみを対象とし、Stage2 以降で必要に応じて強化される。

---

# 1. 目的
- Stage1 を **開発者が迷わず着手できる軽量フェーズ** にする。
- 「動作するものを早く作る」ことを最優先し、ドキュメント整合や厳格な標準は後回しにする。
- OVR / REQ / ARC / SPC の正典内容は保ちつつ、**運用負荷となる部分を一時的に緩和**する。

---

# 2. 適用範囲
- Stage1（bitFlyer Public REST `getticker` のみ）。
- Abstractions / Adapter / Raw の 3 層。
- Stage2 以降では本ガイドラインは段階的に無効化される。

---

# 3. 軽量化の基本方針（Core Principles）

## 3.1 実装優先（Implementation First）
- ドキュメント整合よりも **まずコードを動かすことを優先**する。
- Ticker が取得できる状態を最速で作る。

## 3.2 PR 必須ルールの緩和
- 初期実装〜試作段階は **PR 不要／個人ブランチでの自由な作業を許可**する。
- まとまった段階で PR 化し、レビューを行う。

## 3.3 Conformance / ADR / キーワード遵守の緩和
- RFC2119（MUST/SHOULD）や Conformance Matrix、ADR の必須適用は Stage1 では免除。
- 重大な設計変更時のみ ADR を作成する。

## 3.4 ドキュメント整合チェックの後回し
- OVR ⇔ REQ ⇔ ARC ⇔ SPC の **整合性チェックは Stage1 完了前の最終段階でまとめて実施**する。
- 作業序盤では整合ズレが生じてもよい。

---

# 4. Stage1 で「必ず守る」最小ルール
Stage1 が軽量であっても、以下 **5 点だけは揺らがせない**。

## 4.1 依存方向（Abstractions ← Adapter ← Raw）
- この依存ルールのみは正典として維持する。

## 4.2 Abstractions の最小構成
- `IExchangeClient`
- `Ticker`
- `Symbols`

## 4.3 bitFlyer Raw API / Raw モデル
- `IBitflyerPublicApi`
- `BitflyerTickerRaw`

## 4.4 symbol ↔ product_code 変換
- "BTC/JPY" ↔ "BTC_JPY"

## 4.5 Ticker の正常取得（Stage1 DoD）
- `GetTickerAsync("BTC/JPY")` が成功すること。

---

# 5. Stage1 で「守らなくてよい」項目
以下は Stage1 では OFF とする。

## 5.1 ドキュメント番号規則の完全遵守
- 形式揺れは許容。整備は Stage1 終盤でまとめて行う。

## 5.2 0900-OVR-COMP の整合性ゼロ要求
- Stage1 では必須にしない。

## 5.3 Transport / Protocol の設計
- Stage1 のスコープ外。議論不要。

## 5.4 高度なログ・メトリクス
- OTel や構造化ログなどは不要。

## 5.5 厳密な TDD 運用
- テストは書くが、手順としての TDD は強制しない。

---

# 6. 作業者のとっかかり改善ガイド
Stage1 に新規参加する作業者が最速で成果を出すためのガイド。

## 6.1 最初の 30 分でやること
1. `IExchangeClient` を見て役割を把握
2. `BitflyerTickerRaw` のフィールドを確認
3. `BitflyerExchangeClient` の雛形を作成
4. HTTP モックテストまたは実通信テストを書いて動かす

→ これで「Ticker が取れた」成果がすぐ見える。

## 6.2 小さなステップで進める
- 1 ステップ = 数分で終わる粒度に分解（Raw モデル作成／API 呼び出し／マッピングなど）。
- 大きい単位の作業は禁止。細粒度を常に維持する。

---

# 7. Stage1 の完了条件（軽量版）
- `GetTickerAsync("BTC/JPY")` が正常動作
- 無効 symbol で例外発生
- Raw → Ticker のマッピングが正しい
- README に使用例がある

ドキュメント整合は最後にまとめて実施すればよい。

---

# 8. Stage2 でこのガイドラインはどう変わるか
- PR 必須ルールが復活する
- Conformance Matrix が必要になる
- ドキュメント整合を常時保つ
- Transport / Protocol が正式導入される

---

# 10. 仕様変更と追加に関する方針（Stage1 Adaptive Change Policy）

Stage1 では「実装速度」と「成果の早期可視化」を最優先とするため、仕様の途中変更・追加を自由に行ってよいものとする。
ただし、変更した内容・理由を最低限の形式で記録することを必須とする。

## 10.1 記録方法（Lightweight Change Notes）
各ドキュメントまたは共通 CHANGELOG.md に、次の形式で記録する。

- 日付
- 変更内容（1〜2行でよい）
- 変更理由（なぜそうしたか）

例：
- 2025-11-22: Raw → Ticker mapping の timestamp を UTC 固定に変更（理由：SPC と統一のため）

## 10.2 ADR の扱い（軽量版）
重大な設計変更のみ ADR として記録するが、Stage1 では簡易形式とする。
1 行で概要と理由を書き、必要に応じて詳細を後から追加してよい。

## 10.3 変更歓迎の原則
Stage1 は「考えながら作る」フェーズであるため、仕様確定を待たずに開発を進めてよい。
仕様の揺れは許容し、Stage2 移行時に統合・整理を行う。

---

# 9. まとめ
- Stage1 を **軽量で進めやすいフェーズ** として扱うための実践的ガイドライン。
- 正典文書は保持しつつ、運用負荷を意図的に下げる。
- とっかかりの良い作業順序と、最小限の拘束だけを残す。

---

