# Stage 1 統合ミニマム仕様書（ステージ非依存文書を Stage1 粒度へ縮退した版）

本ドキュメントは、プロジェクト全体で使われる包括的な設計・規範文書を **「Stage 1（bitFlyer Public REST / Ticker のみ）」に必要な粒度**まで圧縮したものである。Stage1 以外の要素（Protocol 層、Transport 拡張、複数取引所、認証 API、Board/OrderBook、複雑なエラー設計など）は含めない。

---

# 1. 目的（Purpose）
Stage 1 の目的は、Exchange API Library の基盤となる **取引所非依存の Abstractions** と、**bitFlyer Public REST の Ticker 取得**を最小限で提供することである。

- 開発者が `GetTickerAsync("BTC/JPY")` を呼び出して Ticker を取得できる
- 将来の Stage（認証、WS、複数取引所）へ破綻なく拡張できる
- Stage1 では "ミニマムで揺らがない層構造" の確立が主眼

---

# 2. スコープ（Scope）
## 2.1 対象（In Scope）
- bitFlyer Public REST `GET /v1/getticker` のみ
- Abstractions 層の最小構成（IExchangeClient / Ticker / Symbols）
- bitFlyer Adapter（Raw モデル＋マッピング）
- symbol ↔ product_code の静的変換

## 2.2 対象外（Out of Scope）
- 認証 REST
- WebSocket
- Board / Balance / Order / Position
- 複数取引所
- 高度な Transport（Retry, RateLimit, CircuitBreaker）
- Result 型ベースのエラー表現

---

# 3. レイヤ構造（Stage1 縮退モデル）
Stage1 は以下 3 層のみを用いる。

```
Abstractions (IExchangeClient, DTOs)
        ↑
Adapter (BitflyerExchangeClient, IBitflyerPublicApi)
        ↑
Raw Model (BitflyerTickerRaw)
```

- Abstractions は上位で依存先なし
- Adapter は Abstractions と Raw に依存
- Raw は取引所固有仕様の写像で、Adapter 内部でのみ利用

依存方向は必ず一方向（逆依存禁止）。

正典は **4 層構造（Abstractions → Adapters → Protocol → Transport）** であるが、Stage1 のスコープは bitFlyer Public REST `getticker` のみに限定されるため、Protocol / Transport は導入せず、**Abstractions / Adapter / Raw の 3 層のみを実装する縮退構造**を採用する。

---

# 4. Abstractions 仕様（最小）
## 4.1 インターフェース
```
Task<Ticker> GetTickerAsync(string symbol, CancellationToken ct = default)
```

- symbol は "BASE/QUOTE" の大文字・スラッシュ形式（例："BTC/JPY"）
- 無効な形式は ArgumentException

## 4.2 DTO（Ticker）
- Symbol : string
- BestBid : decimal
- BestAsk : decimal
- LastTradedPrice : decimal
- TimestampUtc : DateTime (UTC)

## 4.3 Symbols
- `Symbols.BtcJpy = "BTC/JPY"`

---

# 5. Adapter 仕様（bitFlyer）
## 5.1 Raw モデル
bitFlyer `GET /v1/getticker` のレスポンスを欠損なく保持するクラスを定義する。フィールドは bitFlyer 公式ドキュメント（https://lightning.bitflyer.com/docs?lang=ja#get-ticker）に準拠し、少なくとも以下を含める。
- `product_code`
- `timestamp`
- `tick_id`
- `best_bid`
- `best_ask`
- `best_bid_size`
- `best_ask_size`
- `total_bid_depth`
- `total_ask_depth`
- `ltp`
- `volume`
- `volume_by_product`

公式レスポンスに追加のフィールドが将来追加された場合は、Raw モデルに安全に拡張してよい。

## 5.2 Raw API
```
Task<BitflyerTickerRaw> GetTickerRawAsync(string productCode, CancellationToken ct)
```

- product_code は "BTC_JPY" 形式
- HTTP 通信、JSON パース、Content-Type の扱いを含む

## 5.3 マッピング
- Raw → Ticker の変換を `BitflyerExchangeClient` が行う
- Timestamp は UTC 正規化
- Volume/Size 系は Stage1 Ticker には含めない

## 5.4 symbol ↔ product_code
- "BTC/JPY" ↔ "BTC_JPY"（静的マッピング）
- 対応しない symbol は ArgumentException

---

# 6. 例外ポリシー（Stage1 縮退版）
- **入力エラー**：`ArgumentException` 系を使用する。
- **API / 内部エラー**：`ExchangeApiException` を用いる。

Stage1 は例外ベースでシンプルに扱い、Result 型などのエラーモデル拡張は対象外とする。

---

# 7. HTTP / Transport ポリシー（最小）
- `HttpClient` は DI から供給し、使い捨てしない。
- タイムアウトは 5〜10 秒を推奨とする。
- `CancellationToken` は `SendAsync` に伝播する。
- `User-Agent` を明示的に設定する。

本章および前章の方針は、OVR 8 章および REQ の NFR-2/3 と整合する Stage1 共通ポリシーとして扱う。

---

# 8. ファイル構成（Stage1 最小）
```
src/
  ExchangeApi.Abstractions/
    IExchangeClient.cs
    Models/Ticker.cs
    Symbols.cs

  ExchangeApi.Bitflyer/
    IBitflyerPublicApi.cs
    Models/BitflyerTickerRaw.cs
    BitflyerExchangeClient.cs
```

テストはそれぞれに分離（Abstractions.Tests / Bitflyer.Tests）。

---

# 9. Stage 1 完了条件（DoD）
- Ticker の取得が成功する（実 API または HTTP モック）
- 無効 symbol で例外が出る
- Raw → Ticker のマッピングが正しい
- README に簡易使用例が載っている（GetTickerAsync）
- ドキュメントが docs/ に配置されている

---

# 10. まとめ

---

# 11. 依存禁止ルール（MUST）
レイヤ間の依存方向および禁止ルールの正典は `A030-STG1-ARC-MinimalArchitecture.md` に定義される。

---

# 12. Stage1 DoD（強化版）
次の条件を満たした場合、Stage1 を完了とする。

## 12.1 機能
- `GetTickerAsync("BTC/JPY")` が正常動作する
- Raw → Ticker マッピングが仕様通り
- 無効 symbol で ArgumentException が発生

## 12.2 テスト
- Bitflyer.Tests にて **実通信または HTTP モックテスト 1 ケース以上必須**
- Abstractions.Tests では DTO・引数検証テストを実施

## 12.3 監査性
- README に使用例（Ticker の取得）が掲載される
- docs/ ディレクトリに本仕様書が配置されている

---

# 13. Stage2+ への拡張の伏線（参考）
本仕様は Stage1 専用だが、以下の追加が想定されている。

- Transport 層（Retry / CircuitBreaker / RateLimit）
- Protocol 層（署名生成、時刻同期、Nonce など）
- 認証 REST（Balance / Order）
- WebSocket（Board / Executions）
- 複数取引所（Binance / Bybit）

これらを追加しても Stage1 の 3 層構造はそのまま維持される。
本書は、Stage を考慮しない包括的な文書群を **Stage1 専用の最小構成**に縮退したものである。設計原則を壊さず、今後の Stage2+ の拡張にも耐える「薄いが揺らぎのない基盤」となる。

---

# 14. 改訂履歴

| 版 | 日付 | 内容 |
|----|------|------|
| v1.1.1 | 2025-05-05 | HTTP/例外ポリシーが OVR/REQ と整合することを明示し、改訂履歴を更新。 |
| v1.1.0 | 2025-05-05 | Stage1 の縮退構造と依存ルール参照の明文化、Raw フィールドの公式化、通信/例外ポリシーの整理を実施。 |

