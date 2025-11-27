# DTO-Candlestick-MAP Candlestick マッピング仕様（複数取引所想定）

## 1. 抽象 DTO 要約
- DTO 名: `Candlestick`
- 必須フィールド:
  - `Symbol` : 抽象シンボル（例: BTC/JPY）
  - `Timescale` : 足種別（例: 1m, 5m, 1h など）
  - `OpenTime` : 足の開始時刻（UTC, DateTimeOffset）
  - `CloseTime` : 足の終了時刻（UTC, DateTimeOffset）
  - `Open`, `High`, `Low`, `Close` : 価格（quote 通貨建て, decimal）
  - `Volume` : 取引量（base 通貨単位, decimal）
  - `IsFinal` : 足が確定済みかどうか（リアルタイム更新時に使用）
- 任意フィールド（取れれば埋める、無ければ null）:
  - `QuoteVolume` : quote 通貨建ての出来高
  - `NumberOfTrades` : 足に含まれる約定件数

## 2. 正規化ルール
- 時刻: 取引所の epoch/ISO を UTC の `DateTimeOffset` に正規化し、`OpenTime` を足の開始、`CloseTime` を足の終了として扱う（返されない場合は Timescale から導出可）。
- 価格: 受信値を decimal で保持。丸めが必要なら利用側で行う。
- Volume: base 通貨単位を標準。取引所が quote ボリュームを返す場合は `QuoteVolume` に入れ、`Volume` は base に限定する。
- Timescale: 取引所の粒度表記（1m, 5m, 1h など）を抽象の列挙/文字列にマップ。未対応粒度は例外またはスキップ。

## 3. 取引所別マッピング
| 取引所 | エンドポイント/フィールド | 備考 | 実装状況 |
| --- | --- | --- | --- |
| Binance（参考） | `/api/v3/klines` → [open time, open, high, low, close, volume, ...] | `open time` ms epoch (UTC)、シンボル大文字連結。quote volume, trades も返す。 | 未実装 |
| BitBank（参考） | `/v1/candlestick/{pair}/{timescale}` → `ohlcv` 配列（[open, high, low, close, volume, timestamp]） | `timestamp` ms epoch (UTC)、`pair` を Symbol に変換。 | 未実装 |
| BitTrade（参考） | （未調査） | 実仕様要確認。 | 未実装 |
| BTCBOX（参考） | （未調査） | 実仕様要確認。 | 未実装 |
| CoinCheck（参考） | `/api/candles/{pair}/{period}` → `opened_at`, `high`, `low`, `close`, `volume` | `opened_at` epoch (UTC)。period は 1min/5min 等。 | 未実装 |
| GMOコイン（参考） | `/public/v1/klines` → `openTime`, `open`, `high`, `low`, `close`, `volume` | `openTime` ISO8601 (UTC)。`symbol` を Symbol に変換。 | 未実装 |
| bitFlyer（参考） | 公式OHLCVなし（Board/Tickerのみ）。外部の約成から集計が必要。 | 現状は未対応（自前集計）。 | 未実装 |
| （TBD） | - | 追加取引所の実装時に追記。 | - |

## 4. 注意点
- 足の開始/終了どちらを返すかは取引所差がある。抽象では「開始時刻」を採用し、必要なら終了時刻は利用側で計算する。
- Volume が base か quote か取引所で異なることがあるため、実装時に必ず確認する。
- タイムスケールの命名が取引所ごとに異なる（例: 1m, 1min, 60, 60s）。列挙へのマッピング表を別途管理すること。
- 取引所に公式OHLCVがない場合（例: bitFlyer）は、抽象API `ListCandlesticksAsync` で未サポートを例外として返す方針。呼び出し側は例外ハンドリングまたは事前のCapabilities確認で対処すること。
