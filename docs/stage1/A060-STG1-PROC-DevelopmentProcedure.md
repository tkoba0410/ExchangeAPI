# Stage1 開発手順書（改訂版 / Rev.A）
本手順書は、Stage1 の実装を **最速で成果が見えやすく、作業開始のとっかかりが良い形**に最適化したものである。  
OVR/REQ/ARC/SPC（正典）および A050-STG1-DEV-Guidelines（軽量方針）を統合し、**柔軟な仕様変更を許容しつつ、最低限の記録を必須**とする運用とした。

---

# 1. この手順書の目的
- Stage1 の開発を **軽く・速く・分かりやすく** 進められるようにする。
- とっかかりのよい最小ステップを提示し、停止ポイントを減らす。
- 途中の仕様変更や追加は自由にしつつ、**変更記録の確実化**を行う。

---

# 2. 前提とする文書
- OVR（0120-OVR-INTR）
- REQ（0210-REQ-STG1-AbstractionsMVP）
- ARC（0310-ARC-STG1-MinimalArchitecture）
- SPC（Stage1-minimal-spec）
- A050-STG1-DEV-Guidelines（軽量開発方針）

これらの正典を尊重しつつ、**運用の重さは最小化**する。

---

# 3. Stage1 のゴール（DoD）
1. `GetTickerAsync("BTC/JPY")` が正常動作する。
2. Raw → Ticker のマッピングが仕様通りに行われる。
3. 無効 symbol で ArgumentException が発生する。
4. README に使用例（Ticker取得）が載る。
5. ドキュメント（OVR/REQ/ARC/SPC）は Stage1 終盤で整合を揃える。

---

# 4. 開発の進め方（大枠）
Stage1 では **実装優先**でよく、ドキュメント整合は後回し。  
以下の順で作業すれば、最短で成果が見える。

### 4.1 作業順序（最短）
1. `IExchangeClient` を作る（Abstractions）
2. `Ticker` DTO を作る（Abstractions）
3. `Symbols.BtcJpy` を作る（Abstractions）
4. `BitflyerTickerRaw` を作る（Adapter/Raw）
5. `IBitflyerPublicApi` を作る（Adapter）
6. `BitflyerExchangeClient` を作って Raw → Ticker をマッピングする（Adapter）
7. モック or 実通信で `GetTickerAsync("BTC/JPY")` を通す

---

# 5. 詳細手順（1ステップ＝数分で完了）

## Step 1: Abstractions を最小で作る
- `IExchangeClient`
- `Ticker`（BestBid/BestAsk/LastTradedPrice/TimestampUtc）
- `Symbols.BtcJpy`

**ポイント:**  
複雑な DTO 拡張は禁止。Stage1 は Ticker の最小情報のみ。

---

## Step 2: bitFlyer Raw モデルを作る
- 公式 API のレスポンスをそのまま写す
- 欠損なく全て含む（Raw 専用なので自由）
- Abstractions を参照しないこと（依存方向維持）

**目的:**  
後で拡張しやすくしつつ、実装の見通しを良くする。

---

## Step 3: Raw API（IBitflyerPublicApi）を書く
- `GetTickerRawAsync(productCode)` を定義
- HttpClient・JSONパースの簡易版でOK
- タイムアウト・User-Agent・CT は軽く対応

**制限:**  
Transport/Protocol の議論は不要（Stage1スコープ外）。

---

## Step 4: ExchangeClient マッピングを作る
- symbol ↔ product_code
- Raw → Ticker を正規化（UTC）
- エラー処理（ArgumentException / ExchangeApiException）

**注意:**  
Stage1 は Result 型や複雑なエラー設計は禁止。

---

## Step 5: テスト（軽量版）
- Abstractions.Tests → DTO/引数検証
- Bitflyer.Tests → Raw API + マッピング
- モック通信 or 実通信どちらでも良い

**テスト駆動は必須ではない**（A050 に基づく）。

---

# 6. 仕様変更・追加の扱い（Adaptive Policy）
Stage1 は **仕様変更をいつでも歓迎する**。  
ただし **変更記録だけは必須**とする。

## 6.1 記録フォーマット（Lightweight Change Notes）
```
- 日付:
- 変更内容:
- 変更理由:
```
複数文書にまたがる場合は CHANGELOG.md にまとめてもよい。

## 6.2 ADR（軽量版）
重大な変更は ADR として 1 行で記録可。  
詳細は後から追加でよい。

---

# 7. 作業者のとっかかり改善のためのチェックリスト
- 動くところから始める（Raw API → Mapping）
- ドキュメントの整合を気にしすぎない
- コミット粒度は小さく（1ステップ＝数分）
- 仕様は途中で変わってもよい（ただし記録する）
- 依存方向だけは絶対に守る

---

# 8. Stage1 で禁止されていること
- Transport 層の本格実装
- Protocol 層の議論
- 複雑なエラーモデル導入
- 早期の文書完全整合（後回し）
- 大きな作業塊（細粒度に分解する）

---

# 9. Stage1 完了後に行うこと
- ドキュメント整合（OVR/REQ/ARC/SPC）
- 変更記録の統合
- Stage2 へ向けた構造整理（Transport/Protocol の導入準備）

---

# 11. デメリットへの軽い言及
Stage1 手順は「軽さ・速度・柔軟性」を最優先とするため、以下のような軽度のリスクを内包する：
- 設計の抜け漏れが発生しやすい（後で補完可能）
- 実装者ごとの品質揺れが出やすい
- 文書整合の回収が Stage1 終盤に集中する
- 小さなステップで進めない場合に破綻しやすい

これらは **軽量開発のメリットとトレードオフ** であり、Stage2 で段階的に規律を戻すことで解消する。

---

# 12. Stage2 へのスムーズ移行ガイド（Transition Guide）
Stage2 では Transport / Protocol / 認証 / 複数取引所 対応が始まり、規律が強化される。  
その際、Stage1 から自然に移行できるよう次をガイドラインとする。

## 12.1 復活する運用ルール
- PR 必須 / レビュー必須 の完全復活
- Conformance Matrix（要求ID ⇔ テスト ⇔ 実装）のリンク再開
- 文書整合（OVR / REQ / ARC / SPC）の常時維持
- RFC2119 表現の遵守強化（仕様の明確性向上）

## 12.2 Stage1 の成果の再整理
- Raw モデルのフィールドを正式ドキュメントに反映
- Mapping 仕様を REQ/SPC に編入
- symbol ↔ product_code の仕組みを共通化ドキュメントへ昇格

## 12.3 Transport / Protocol 導入準備
- HttpClient ラッパ（Transport）導入のため Adapter の境界を明確化
- Nonce / timestamp / signature など Protocol 要素の設計開始
- 例外体系（ExchangeApiException）を拡張する場合は ADR 化

## 12.4 実装者への負荷を軽減する移行ステップ
- Stage1 で実装されたコードを例に「正しいレイヤリング」教材を作成
- サンプル PR を準備し、レビュー基準を共有
- Transport / Protocol の導入は小ステップで行い、段階的に責務を移す

## 12.5 チーム向けメッセージ
> Stage1 は“試作フェーズ”として軽量化し、  
> Stage2 は“製品フェーズ”として強度を高める。
>
> この移行は段階的に行い、Stage1 の柔軟性は失わず、  
> 正典の体系にスムーズに合流させる。

---

# 10. まとめ
Stage1 の目的は **「軽く、早く、動かす」**ことである。  
設計は必要最小限、実装は素早く、変更はいつでも歓迎。  
ただし将来に備え、**記録だけは必ず残す**。  
この手順書は、正典文書と軽量ガイドラインの中間として、実際の手を動かす開発者が迷わず進めるための実践的ガイドである。

