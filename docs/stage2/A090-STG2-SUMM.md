# A090-STG2-SUMM Stage2 FIX まとめ（get balance）

> 状態: Stage2 は本ドキュメントをもって FIX・凍結済み。以降の変更は Stage3 以降の文書で扱う。

## 1. Stage2 の位置づけ

- Stage1：Public GET（板など）の設計・実装
- **Stage2：Private GET の最初の一歩として `/v1/me/getbalance` を end-to-end で通すステージ**
- Stage3：Private GET の拡張（collateral / positions / executions）および POST Private（注文・キャンセル）へ進む予定

Stage2 は、**「bitFlyer Private API 呼び出しのテンプレートを確立するステージ」**として FIX する。

---

## 2. Stage2 の scope（確定版）

### 2.1 やること（含める範囲）
- Abstractions
  - `Balance` ドメインモデルの定義
  - `IExchangeAccountClient.GetBalancesAsync()` の追加
  - `IExchangeClient` に `IExchangeAccountClient` を継承させる
- Infrastructure
  - `IExchangeClock` / `SystemClock`
  - `IRequestSigner`（bitFlyer 用署名インターフェース）
  - `IRestClient` / `RestClient`（署名付き GET + JSON + E1 エラー処理）
  - `ExchangeApiException`（HTTP エラー共通例外）
- Bitflyer.Private
  - `/v1/me/getbalance` に対応する `BitflyerBalanceResponse` DTO
  - `IBitflyerPrivateApi.GetBalancesAsync`
  - `BitflyerPrivateApi` 実装（RestClient 経由で呼び出し）
- Bitflyer.Adapter
  - `BitflyerExchangeClient.GetBalancesAsync` の実装
  - DTO → Domain 変換（`BitflyerExchangeClient` 内に実装）
- Factory
  - `BitflyerClientFactory.Create(apiKey, apiSecret)` により `IExchangeClient` を組み立て、
    `GetBalancesAsync` が実口座から値を取得できる状態にする

### 2.2 やらないこと（除外範囲）
- Private GET のうち balance 以外の API
  - 証拠金：`/v1/me/getcollateral`
  - ポジション：`/v1/me/getpositions`
  - 注文一覧：`/v1/me/getchildorders`
  - 約定履歴：`/v1/me/getexecutions`
- POST Private API（発注・キャンセル）
  - `/v1/me/sendchildorder`
  - `/v1/me/cancelchildorder`
  - `/v1/me/cancelallchildorders`
- FundingRate / 手数料 / 各種履歴（`getfundingrate`, `gettradingcommission`, `getbalancehistory`, `getcollateralhistory` など）
- エラー処理レベル E2 以降（取引所固有エラーの解釈・リトライ制御）

---

## 3. 成果物（ドキュメントと実装）の整理

### 3.1 Stage2 用ドキュメント
- **A010-STG2-OVER**：Stage2 ゴール定義（get balance）
- **A020-STG2-REQR**：要件定義（get balance）
- **A030-STG2-ARCL**：レイヤ構成（Abstractions / Infrastructure / Bitflyer）
- **A040-STG2-ARCB**：bitFlyer Private API → 抽象層マッピング（get balance）
- **A050-STG2-IMPL**：実装ノート（レイヤ別の実装ポイント）
- **A060-STG2-TSTP**：テスト観点・代表ケース
- **A070-STG2-OPS**：動作確認・運用メモ（Factory + GetBalancesAsync）

### 3.2 コード上の成果物（想定）
- `ExchangeApi.Core`
  - `Balance` record
  - `IExchangeAccountClient`, `IExchangeClient`
- `ExchangeApi.Transport`
  - `IExchangeClock` / `SystemClock`
  - `IRequestSigner` / `BitflyerRequestSigner`
  - `IRestClient` / `RestClient`
  - `ExchangeApiException`
- `ExchangeApi.Adapter.Bitflyer`
  - DTO: `BitflyerBalanceResponse`
  - Private API: `IBitflyerPrivateApi`, `BitflyerPrivateApi`
  - Adapter: `BitflyerExchangeClient.GetBalancesAsync`（DTO → Balance 変換を内包）
  - Factory: `BitflyerClientFactory.Create`

---

## 4. Stage2 の Definition of Done（FIX版）

Stage2 は、以下の条件を満たした時点で「完了」とみなす。

1. **設計面**
   - A010〜A070 の各文書が一貫した内容で揃っている。
   - Abstractions / Infrastructure / Bitflyer のレイヤ構造と依存関係が明確化されている。

2. **実装面**
   - `IExchangeAccountClient.GetBalancesAsync` が実装されている。
   - `/v1/me/getbalance` の呼び出しが、RestClient → Private API → Adapter の流れで一貫して動作する。
   - HTTP エラーが `ExchangeApiException` として扱われる（E1 レベル）。

3. **動作確認面**
   - API Key / Secret を使い、`BitflyerClientFactory.Create` から取得した `IExchangeClient` で
     `GetBalancesAsync` を実行し、実口座の JPY / BTC 残高を取得できる。
   - エラー系（キー不正・タイムスタンプずれ）を再現し、適切に `ExchangeApiException` が発生することを確認している。

4. **テスト面**
   - RestClient / Signer の基本的なユニットテスト（署名・エラー処理）が存在する。
   - DTO → Balance 変換のテストが存在する。
   - BitflyerExchangeClient.GetBalancesAsync のテストが存在し、
     Private API をモックした形で DTO → Domain の流れが検証されている。
   - 本完了条件を満たした時点で Stage2 を FIX（変更凍結）とする。

---

## 5. 制約と今後の見直しポイント（総括）

- Stage2 はあくまで `/v1/me/getbalance` のみを対象としたテンプレ確立ステージである。
- リスト系 GET（注文・約定履歴）や POST Private（注文・キャンセル）では、
  クエリ・ページング・副作用を含むため、
  RestClient インターフェースやエラー設計を見直す可能性がある。
- `Balance` の情報粒度は最小限に留めており、今後の要件に応じてフィールド追加の余地がある。
- エラー処理は E1 レベル（HTTP ステータスによる大まかな分類）にとどめており、
  E2 以降（取引所固有エラーのドメイン例外化・リトライ制御）は Stage3 以降で検討する。

---

## 6. Stage3 への接続

Stage3 では、Stage2 で確立した構造とテンプレートを用いて、
- Private GET の拡張（`getcollateral`, `getpositions`, `getexecutions`）
- 必要に応じた RestClient / Abstractions の拡張
- POST Private API（`sendchildorder`, `cancelchildorder` 等）への着手

を行う予定である。

本ドキュメントは、Stage2 を「FIX」とみなすための最終サマリとして位置づけ、
後続ステージの設計・実装のリファレンスとする。
