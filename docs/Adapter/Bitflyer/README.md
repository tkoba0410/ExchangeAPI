# Bitflyer Adapter 概要

bitFlyer 向けの REST-only 実装（Stage6）の構成と役割をまとめます。

## 構成
- `Apis/`: 抽象 API 実装（Market/Account/Trading/ExchangeInfo）。Raw API 呼び出しを組み立て、DTO にマッピング。
- `RawApi/`: 取引所固有のエンドポイント呼び出し（HTTP パラメータ/レスポンスを bitFlyer 仕様で扱う）。
- `Adapters/`: Raw ⇔ 抽象 DTO のマッピング（例: `BitflyerCommonMapper`, Ticker/Board/Executions マッパ）。
- `Facade/BitflyerExchangeClient`: 抽象インターフェース (`IMarketDataApi` 等) を束ねたクライアント。
- `Facade/BitflyerPublicClient`: Public API（Market/ExchangeInfo のみ）に限定した軽量クライアント。
- `Factory/BitflyerClientFactory`: 署名/RestClient/ポリシー/Raw/Adapters/Facade を組み立てるエントリーポイント（`CreatePublic()` で Public 専用クライアントも生成）。
- `Http/`: 署名や HTTP リクエスト生成の細部。

## 対応範囲（Stage6）
- Market: Ticker/Board/MarketExecutions（歩み値）
- Trading: MARKET/LIMIT/STOP（STOP_LIMIT は TriggerPrice+Price 送信）、キャンセル、ポーリング
- Account/Margin: Balances/Collateral/Positions/AccountExecutions
- ExchangeInfo: BTC/JPY の刻み・最小数量・手数料通貨、定期メンテ情報（04:00-04:10 JST）
- WebSocket: 非対応（REST only）

## 補足
- エラー分類: `IExchangeErrorClassifier` 経由でカテゴリ正規化し、`ExchangeApiException` に集約。
- 手数料: FeeCurrency/FeeType は ExchangeInfo で BTC 徴収/Percentage を設定。実手数料率は適宜更新する想定。
- メンテ情報: 現在は定期メンテを固定値で設定。今後 JSON/外部告知で上書きする余地あり。

## 利用例（Public のみ）
```csharp
// 認証不要のマーケット/ExchangeInfo だけを使いたい場合
var publicClient = BitflyerClientFactory.CreatePublic();
var ticker = await publicClient.GetTickerAsync("BTC/JPY");
var info = await publicClient.GetExchangeInfoAsync();
```
