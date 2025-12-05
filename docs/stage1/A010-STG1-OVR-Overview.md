# A010-STG1-OVR-Overview（Stage1 正典 / 全面改訂版）

本書は Exchange API Library **Stage1 の唯一の正典（Single Source of Truth）** として、
プロジェクトの目的・構造・依存方向・設計原則を定義する。Stage1 は bitFlyer Public REST
`getticker` のみを対象とする **最小実装フェーズ** であり、Stage2（認証・WebSocket・複数取引所・Protocol/Transport 拡張）へ自然に発展できる基盤を提供する。

---

# 1. 文書の位置づけ（正典 / Canonical Document）

* 本書は Stage1 全文書（REQ / ARC / SPC / DEV / PROC）を統括する**上位規範**である。
* 矛盾が発生した場合は **本書が優先（MUST）**。
* 設計・依存方向・構造に関する定義は Stage2 以降でも継続して有効となる。

---

# 2. プロジェクトの目的（Purpose）

Exchange API Library の目的は、複数の暗号資産取引所 API を横断して扱うための
**取引所非依存の契約境界（Boundary）** を定義し、その周囲に **実装モジュール（Technical Modules）** を配置することで、
堅牢で拡張可能な統一 API を提供することである。

Stage1 の具体的目的：

1. **Abstractions（契約境界）** を確立する
2. bitFlyer Public REST `getticker` を利用した **Ticker 取得**を最小構成で実現する
3. Stage2 での Transport / Protocol / 複数取引所拡張に耐えられる構造を形成する

---

# 3. スコープ（Stage1 Scope）

## 3.1 対象（In Scope）

* Abstractions（IExchangeClient / Ticker / Symbols）
* bitFlyer Adapter（BitflyerExchangeClient）
* Raw モデル（BitflyerTickerRaw）
* REST 呼び出しのための最小限の技術モジュール（RestClient / HttpTransport）
* symbol ↔ product_code の静的変換

## 3.2 対象外（Out of Scope → Stage2 で扱う）

* 認証 REST（Balance / Order / Position）
* WebSocket（Board / Executions / Realtime Ticker）
* 複数取引所（Binance / Bybit など）
* Transport の高度機能（Retry / RateLimit / CircuitBreaker）
* Protocol の高度機能（署名生成・timestamp/nonce）
* Result 型・複雑なエラー階層

---

# 4. 基本構造（Boundary + Technical Modules）

Exchange API Library は「層構造」ではなく、**Boundary を中心に技術モジュールを接続する構造**を採用する。これにより柔軟な拡張が可能になる。

```
                ┌─────────────────────────┐
                │  Boundary / Abstractions │
                │  (Interfaces + DTOs)      │
                └────────────▲──────────────┘
                             │
              ┌──────────────┼───────────────┐
              │              │               │
┌──────────────┴──────┐ ┌──────┴────────┐ ┌───────────────┴───────┐
│ Adapter (bitFlyer)   │ │ Protocol (REST) │ │ Transport (HTTP Client) │
└──────────────────────┘ └────────────────┘ └──────────────────────────┘
```

Stage1 ではこのうち：

* Adapter（bitFlyer）
* Protocol（REST の最小実装）
* Transport（HttpClient を包む最小実装）
  だけを利用する。  
  ※ Stage4 以降の命名: ExchangeApi.Core（旧 Abstractions）、ExchangeApi.Transport（旧 Infrastructure/Protocol/Transport）、ExchangeApi.Adapter.Bitflyer（旧 Adapter）、ExchangeApi.Factory（旧 Orchestration）

### 4.1 Boundary（Abstractions）

* `IExchangeClient` / Ticker / Symbols
* **依存先ゼロ（MUST）**
* 取引所固有の仕様・HTTP 実装を含まない

### 4.2 Technical Modules（Adapters / Protocol / Transport）

* Abstractions に従って実装される
* 下位への依存は許可される（Adapter → Protocol → Transport）
* Boundary への逆依存は禁止（MUST NOT）

### 4.3 Raw Models（特定取引所の写像）

* bitFlyer の JSON レスポンスを欠損なく保持
* Adapter 内部専用

---

# 5. Stage1 のプロジェクト構成（実装準拠）

実装済みの Stage1 では次の構成となっている：

* `ExchangeApi.Core`（旧 ExchangeApi.Core）… Boundary（依存なし）
* `ExchangeApi.Transport`（旧 ExchangeApi.Transport）… Protocol + Transport（技術モジュール）
* `ExchangeApi.Adapter.Bitflyer`（旧 ExchangeApi.Adapter.Bitflyer）… bitFlyer Adapter（Core / Transport に依存）
* `ExchangeApi.Factory`（旧 ExchangeApi.Factory）… 資格情報や組み立て用の上位層（Stage2 以降で利用）

---

# 6. 依存方向ルール（正典 / MUST）

Stage1〜Stage2 を通じて **不変の依存原則**：

```
Core (旧 Abstractions)
        ↑
Adapter.Bitflyer / Transport
```

禁止（MUST NOT）：

* Abstractions → Adapter / Protocol / Transport
* Raw → Abstractions
* Raw → Adapter

許可（MUST）：

* Adapter → Abstractions
* Adapter → Protocol / Transport
* Protocol → Transport

Stage1 の実コードはこれらに準拠している。

---

# 7. Stage1 設計原則（Design Principles）

## 7.1 シンプルさ優先

* 「Ticker が取れること」を最優先とし、必要最小限の構造に限定する

## 7.2 Boundary の安定性

* Boundary は将来バージョンでも継続して利用される中心である

## 7.3 モジュールの段階的拡張

* Transport / Protocol は Stage2 で正式に拡張
* Stage1 は GET + JSON の必要最小限のみを使用

## 7.4 Anti-Corruption

* Raw モデルは外部 API の構造をそのまま保持し、Boundary を汚染しない

---

# 8. Stage1 通信・例外ポリシー

* `HttpClient` は DI から供給（使い捨て禁止）
* `User-Agent` を明示設定
* `CancellationToken` を REST 呼び出しへ伝播
* 無効 symbol → `ArgumentException` または `SymbolNotSupportedException`
* API/内部エラー → `ExchangeApiException`

---

# 9. 想定利用者

* 外部 OSS 利用者
* ライブラリ実装者
* 金融・システム開発者

---

# 10. Stage1 Definition of Done

1. `GetTickerAsync("BTC/JPY")` が正常動作する
2. Raw → Ticker のマッピングが正しい
3. 無効 symbol で例外が発生する
4. README に使用例が掲載される
5. OVR / REQ / ARC / SPC の整合が取れている

---

# 11. Stage2 への発展

Stage2 では次を拡張対象とする：

* Transport の強化（Retry / RateLimit / CircuitBreaker）
* Protocol の強化（署名・timestamp/nonce）
* 認証 REST（Balance / Order）
* WebSocket（Board / Executions / Streams）
* 複数取引所（Binance / Bybit 等）

Stage1 の構造は Stage2 でもそのまま利用できる。

---

# 12. 改訂履歴

| 版      | 日付         | 内容                         |
| ------ | ---------- | -------------------------- |
| v2.0.0 | 2025-11-XX | 全面改訂。Stage2 以降を前提とした構造へ整理。 |

---
本ドキュメント（A010）は Stage1 の最終ゴールを示すものであり、
A000-STG1-GOAL-Vision と連携して Stage1 の目的と到達点を規定する。
