# ExchangeInfo メモ

取引所やシンボルの刻み値・数量制約・手数料・メンテ状況・サポート状況を表す DTO についての指針。`ExchangeMarketInfo` を正とし、`SymbolMeta` は廃止。

## ExchangeInfo / ExchangeMarketInfo
- `ExchangeInfo.Markets` に銘柄ごとのメタ情報を格納。
- フィールド: `MinSize`/`MaxSize`/`MinNotional`/`PriceIncrement`/`SizeIncrement`/`MakerFeeRate`/`TakerFeeRate`/`FeeCurrency`/`FeeType` はバリデーションや約定後の精算に関するヒント。欠損時は取引所デフォルトに従う。
- `MakerFeeRate`/`TakerFeeRate`: 例 0.001 = 0.1%。リベートを表す負の値も許容。取得できない場合は null。
- `FeeCurrency`: 手数料を徴収する通貨コード。null は「約定通貨で徴収」（受け取り側が減る）を意味する。例: bitFlyer は BTC/JPY で BTC 徴収なので `"BTC"` を設定。
- `FeeType`: Percentage または Flat（1注文あたり固定額）。現状は Percentage 利用が主。別トークン割引や特典は将来拡張で表現する前提。
- `IsSupported=false` は「取引所には存在するが、このライブラリでは未サポート」を示す。`StatusNote` に理由を記載できる。
- 取引所がシンボル名と product_code を分けている場合、`Symbol` は抽象（例: `BTC/JPY`）、`ProductCode` は取引所仕様（例: `BTC_JPY`）を入れる。

## 交換所全体のメタ情報
- `ExchangeRateLimits` で呼び出し制限を表現（`RequestsPerMinute`/`OrdersPerMinute`）。取れない場合は null。
- `ExchangeMaintenance` でメンテ情報を表現（`Status` = Normal/Planned/Unplanned, `PlannedUntil`, `Message`）。告知を取得できない場合は null のままにする。

## 運用例
- 起動時に `IExchangeInfoApi` で最新の `ExchangeInfo` を取得し、ローカルキャッシュを作る。取得できない場合はアプリ設定で与える静的値を使う。
- `OrderRequest` 送信前に刻み値・数量制約をチェックし、違反時は `ArgumentException` などで早期に弾く。
