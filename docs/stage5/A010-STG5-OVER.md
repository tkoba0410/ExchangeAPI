# A010-STG5-OVER
Stage5 Overview（REST Only / bitFlyer 縦スライス検証）

---

## 1. Stage5 の目的（OVERVIEW）
Stage5 は、Stage4 までに定義した抽象 REST API を **bitFlyer 実装で縦に通し、その妥当性を検証するステージ** である。WebSocket/Realtime は本ラインから完全に廃止し、ExchangeAPI は **REST 専用ライブラリ** として構成する。

Stage5 の主目的は次のとおり：
- bitFlyer REST API を用いて、Stage4 抽象インターフェースが **実際に破綻なく機能するか** を確認する。
- LIMIT / STOP / キャンセル / 残高・証拠金・建玉取得など、**代表的なトレードフローを end-to-end で実行できる状態** を作る。
- Stage1〜4 の成果（テンプレート化された HTTP 実装、抽象ドメイン、API 区分など）を **統合し、1 取引所を通して検証する"最初の完成形"** を作る。
- WebSocket が消えることにより、REST の責務が明確化し、抽象レイヤの揺らぎを減らす。

---

## 2. 今までやったこと（Stage1〜4 の成果）
この節では、Stage5 の前提となる **既存成果物** を整理する。

### 2.1 Stage1（Public REST 基本）
- Public API（Ticker / Board / Executions）の実装。
- HTTP クライアント共通基盤の確立（シグネチャ、テンプレート化）。

### 2.2 Stage2（Private GET 基本）
- 認証付き Private GET（残高取得）の実装。
- `ExchangeApiException` を用いた基本的なエラー処理。

### 2.3 Stage3（Private POST 基本 / MARKET 注文）
- MARKET 注文を中心とした Private POST のテンプレート確立。
- Private API の署名方式・共通化の完了。

### 2.4 Stage4（抽象 API の統合設計）
- ExchangeAPI の抽象インターフェースを 6 区分に整理：
  - Market / Trading / Account / Margin / ExchangeInfo / RawApi
- ドメインモデル（OrderRequest / OrderResult / Position / Ticker / Execution など）の統合。
- この抽象 API を土台に「どの取引所もこの形に寄せる」ための設計を確立。

---

## 3. Stage5 でやること（スコープ）
このステージでは、抽象 API を bitFlyer REST 実装で実際に通し、**REST-only の ExchangeAPI の完成形** を作る。

### 3.1 Trading（MARKET / LIMIT / STOP / キャンセル）
- bitFlyer の `/sendchildorder` を抽象 `ITradingApi` にマッピング。
- ORDER_TYPE（MARKET / LIMIT / STOP）の整理と双方向マッピング。
- キャンセル操作（`cancelchildorder` / `cancelallchildorders`）。
- REST ベースでの約定確認フローの確立（Polling）。

### 3.2 Account / Margin（残高・証拠金・建玉）
- `/getbalance` `/getcollateral` `/getpositions` `/getchildorders` `/getexecutions` を抽象 API に統合。
- "口座サマリ（Balance + Collateral + Positions）" を取得する統一フローの確立。

### 3.3 Market（相場系 REST）
- Ticker / Board / Executions の抽象モデルへの正規化。
- 必要であればローソク足（Candles）を追加。

### 3.4 ExchangeInfo / Raw API
- product_code 一覧、ティックサイズ、最小注文数量などを `ExchangeInfo` として提供。
- 抽象化が難しいエンドポイントを RawAPI として整理・公開。

### 3.5 エラー処理とテスト
- bitFlyer 固有エラーを抽象例外にマッピング（認証エラー、残高不足など）。
- DTO → Domain のマッピングテスト。
- 結合テストで代表的トレードフローを通す。

### 3.6 ドキュメント整備
- Stage5 のスコープ・ゴール・やらないことの明確化。
- QuickStart（REST だけで指値→約定確認→決済→履歴取得）。

---

## 4. Stage5 でやらないこと（スコープ外）
ここでは、Stage5 の対象外を明確化する。

### 4.1 WebSocket / Realtime（完全廃止）
- WebSocket は本ラインから完全に削除。
- 今後必要になった場合は **別ステージ・別モジュールとして復活** させる方針。
- 現在の REST-only 設計に影響を与えない。

### 4.2 複数取引所対応
- bitFlyer 1 取引所のみで抽象 API の妥当性を確認する。
- Binance / Bybit 等は後続ステージに回す。

### 4.3 高度な信頼性・運用（Rate Limit / Retry / CircuitBreaker 等）
- 最低限の例外処理のみを行い、複雑な運用パターンは扱わない。

### 4.4 高度な注文戦略
- IFDOCO / OCO / トレーリングストップなど特殊注文は対象外。
- 必要なら RawAPI または後続で検討する。

### 4.5 ドキュメントの完全仕上げ
- DocFX / Web サイト化などは Stage7 以降に回す。

---

## 5. 完了条件（Definition of Done）
Stage5 は次を満たした時点で完了とみなす：

1. Market / Trading / Account / Margin / ExchangeInfo / RawAPI が **REST-only で実装されている**。
2. BTC/JPY を対象に、以下のフローが抽象 API だけで成功する：
   - 残高取得 → 新規注文（LIMIT or STOP）→ 約定確認（REST）→ 決済 → 履歴取得。
3. DTO ↔ Domain マッピングの単体テストがグリーン。
4. 代表トレードフローの結合テストが実行可能でグリーン。
5. 抽象 REST API が必要十分に安定し、破綻がないことを確認。
6. 本 OVR（A010-STG5-OVER）が整理され、スコープ・目的が明確に定義されている。

---

## 6. 現状のフォルダ構成と課題
本ステージ開始時点の主なフォルダ構成は、概ね次のようになっている：

- `src/ExchangeApi.Contracts`
  - 抽象 API インターフェース群（`ITradingApi`, `IMarketDataApi`, `IAccountApi`, `IMarginAccountApi`, `IExchangeInfoApi` など）
  - ドメイン DTO 群（`Ticker`, `Candlestick`, `Execution`, `OrderRequest`, `OrderResult`, `Balance`, `Collateral`, `Position` など）が `Dtos/` 直下にフラットに配置されている。
- `src/ExchangeApi.Transport`
  - REST 呼び出し用の共通インフラ（`IRestClient`, `RestClient`, `IRequestSigner`, `RestRequest`, `RestResponse` など）
  - HTTP トランスポート（`HttpTransport`）、ポリシー（`IHttpPolicy`）、ログ（`IRestClientLogger`）、時計（`IExchangeClock`）などが責務ごとに配置されている。
- `src/ExchangeApi.Factory`
  - `Credentials/` に認証情報のプロバイダ
  - `Transport/` に `RestClientFactory` 等の組み立てロジック
- `src/adapter/Bitflyer`
  - bitFlyer 向け実装・モデル・Realtime クライアントが 1 フォルダにまとまっている。

この構成はレイヤ分離という観点では概ね次のように整理できる：

- `Contracts` = 抽象 API + ドメイン DTO
- `Transport` = HTTP インフラ
- `Factory`   = クライアント組み立て
- `adapter/Bitflyer` = 取引所ごとの実装

一方で、以下のような課題も存在する：

- `adapter/Bitflyer` 配下に、REST クライアント（`BitflyerPublicApi`, `BitflyerPrivateApi`, `BitflyerRequestSigner`, `BitflyerSigningTransport`）と、
  抽象 API 実装・ファサード（`BitflyerExchangeClient`, `BitflyerExchangeInfoApi`, `BitflyerClientFactory`）、
  Realtime クライアント（`BitflyerRealtimeClient`）、bitFlyer 固有モデル（`Models/Bitflyer*Response.cs`, `BitflyerTickerRaw.cs` 等）が混在しており、
  **Http / Adapter / Realtime / DTO の責務境界が物理的には分離されていない。**
- `ExchangeApi.Contracts/Dtos` 直下に、Market / Trading / Account / Margin / Realtime 関連 DTO がフラットに並んでおり、
  API 区分（Market / Trading / Account / Margin / ExchangeInfo）とフォルダ構成が 1:1 対応していない。
- Realtime / WebSocket に関するインターフェースおよび実装（`IRealtimeMarketDataApi`, `RealtimeTicks`, `BitflyerRealtimeClient` など）が、
  REST 専用ライブラリとする Stage5 方針と齟齬を起こしている。

これらにより、

- 初見で Bitflyer 実装の責務境界を把握しづらい
- Domain / Transport / Adapter / Realtime の物理構造が直感的でない
- ドキュメント（Market / Trading / Account / Margin / ExchangeInfo 区分）とフォルダ構成の対応が弱い

といった "ぱっと見での分かりづらさ" に繋がっている。

---

## 7. Stage5 で目指すフォルダ構成（REST Only / 責務明確化）
Stage5 では、REST-only ライブラリとしてのシンプルさと可読性を最大化するため、次の構成を標準とする（Bitflyer は Http→RawApi→Adapters→Apis→Facade→Factory に整理済み。詳細は `docs/stage5/STRUCTURE-OPTIMAL.md` を参照）。

```text
src/
  ExchangeApi.Contracts
    Contracts/
      ITradingApi.cs
      IMarketDataApi.cs
      IAccountApi.cs
      IMarginAccountApi.cs
      IExchangeInfoApi.cs
    Dtos/
      Market/
        Ticker.cs
        Board.cs
        Execution.cs
        Candlestick.cs
      Trading/
        OrderRequest.cs
        OrderResult.cs
        OpenOrder.cs
        OrderStatus.cs
        TimeInForce.cs
        OrderSide.cs
        OrderType.cs
      Account/               ※ Margin も Account 配下に統合
        Balance.cs
        Collateral.cs
        Position.cs
      ExchangeInfo/
        ExchangeInfo.cs
      Common/
        Symbol.cs など共通型

  ExchangeApi.Transport
    Protocol/
      IRestClient.cs
      RestClient.cs
      RestRequest.cs
      RestResponse.cs
      IRequestSigner.cs
    Transport/
      HttpTransport.cs
    Policy/
      IHttpPolicy.cs
    Logging/
      IRestClientLogger.cs
    Time/
      IExchangeClock.cs

  ExchangeApi.Factory
    Credentials/
      ICredentialsProvider.cs
    Transport/
      RestClientFactory.cs

  adapter/
    Bitflyer
      Http/
        BitflyerPublicApi.cs
        BitflyerPrivateApi.cs
        BitflyerRequestSigner.cs
        BitflyerSigningTransport.cs
        Models/
          Bitflyer*Request.cs
          Bitflyer*Response.cs
          BitflyerTickerRaw.cs

      Adapters/
        TradingMapper.cs
        MarketMapper.cs
        AccountMapper.cs
        MarginMapper.cs
      ExchangeInfoMapper.cs

      Trading/
        BitflyerTradingApi.cs      // ITradingApi 実装
      Market/
        BitflyerMarketDataApi.cs   // IMarketDataApi 実装
      Account/
        BitflyerAccountApi.cs      // IAccountApi 実装
      Margin/
        BitflyerMarginAccountApi.cs// IMarginAccountApi 実装
      ExchangeInfo/
        BitflyerExchangeInfoApi.cs // IExchangeInfoApi 実装
      RawApi/
        BitflyerRawApiClient.cs     // 抽象に載せないエンドポイントをラップ
```

### フォルダ構成における重要な原則
1. **Domain (`ExchangeApi.Contracts`) と Bitflyer 実装 (`adapter/Bitflyer`) を明確に分離する。**
2. **API 区分（Market / Trading / Account / Margin / ExchangeInfo）とフォルダ構造を 1:1 で対応させる。**
3. **DTO と Domain のマッピングは `Adapters` 層に閉じ込め、Http 層に混在させない。**
4. **REST 呼び出し（`Http`）と業務ロジック（`Trading` / `Market` / `Account` 等の実装）を物理的に分ける。**
5. **Realtime / WebSocket 関連コード（インターフェース・DTO・実装・テスト）は、Stage5 時点ではライブラリ本体から完全に除去する。**
6. **"ぱっと見で責務が理解できる構造" を最優先とし、1 フォルダ＝1 責務となるよう多層化しすぎない。**

---

## 8. Stage6 以降への接続


- REST-only の安定した土台を確立し、複数取引所展開が容易な状態にする。
- 必要に応じて、Realtime/WS を **独立モジュール** として再導入する判断材料とする。
- Stage6 では「運用・信頼性強化」や「抽象 API の追加仕様」を扱う可能性がある。
