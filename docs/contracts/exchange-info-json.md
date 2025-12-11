# ExchangeInfo JSON 仕様（案）

`ExchangeInfo` を JSON で外部化するためのスキーマ案。複数取引所をサポートする場合は取引所ごとにファイルを分ける（例: `configs/exchangeinfo/bitflyer.json`）。`IExchangeInfoApi` の JSON 実装でデシリアライズし、必要に応じて従来実装でフォールバックする。

## サンプル
```jsonc
{
  "markets": [
    {
      "symbol": "BTC/JPY",
      "productCode": "BTC_JPY",
      "type": "Spot",
      "priceIncrement": 1,
      "sizeIncrement": 0.001,
      "minSize": 0.001,
      "maxSize": null,
      "minNotional": null,
      "makerFeeRate": 0.001,
      "takerFeeRate": 0.002,
      "feeCurrency": "BTC",
      "feeType": "Percentage", // or "Flat"
      "isSupported": true,
      "statusNote": "bitFlyer Lightning BTC/JPY"
    }
  ],
  "features": {
    "supportsWebSocket": false,
    "supportsMargin": true,
    "supportsStopOrder": true,
    "supportsParentOrder": true,
    "supportsCandlestick": false,
    "supportsOrderBookDelta": false,
    "supportsRealtimeExecutions": false,
    "supportsWithdraw": false
  },
  "rateLimits": {
    "requestsPerMinute": 500,
    "ordersPerMinute": 100
  },
  "maintenance": {
    "status": "Planned", // Normal | Planned | Unplanned
    "plannedUntil": "2025-01-01T04:10:00Z",
    "message": "Daily maintenance 04:00-04:10 JST"
  },
  "version": "1.0",
  "lastUpdated": "2025-01-01T00:00:00Z",
  "notes": "Static config for bitFlyer Stage6"
}
```

## フィールド仕様
- `markets` (必須): `ExchangeMarketInfo` 相当。`symbol`/`productCode`/`type` は必須。数値フィールドは null で「取引所デフォルト」を意味する。
- 手数料: `makerFeeRate`/`takerFeeRate` は 0.001 = 0.1%、負の値でリベートも許容。`feeCurrency` は手数料通貨（null は約定通貨）。`feeType` は `Percentage` または `Flat`（1注文あたり固定額）。
- `features` (任意): `ExchangeFeatureFlags` に対応。欠損時は null。
- `rateLimits` (任意): `requestsPerMinute`/`ordersPerMinute`。欠損時は null。
- `maintenance` (任意): `status` = Normal/Planned/Unplanned、`plannedUntil`（UTC）、`message`。欠損時は null。
- `version`/`lastUpdated`/`notes` (任意): 運用メタデータ。バージョン管理や更新確認に利用。

## 推奨運用
- ファイル分割: `configs/exchangeinfo/{exchange}.json` に配置。環境ごとに `*.local.json` を用意し、後者を上書きとしてマージする。
- ロード順序: JSON →（失敗時）従来の固定値実装にフォールバックする `IExchangeInfoApi` ラッパーを用意すると安全。
- 検証: 起動時に `symbol/productCode/type` の必須検証と `feeType` の値域チェックを行う。CI でスキーマチェックを入れると事故が減る。
- キャッシュ: 必要ならファイル更新時刻を見て再読み込みする。`version/lastUpdated` をログに出すと運用で確認しやすい。
