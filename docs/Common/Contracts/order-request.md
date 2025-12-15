# OrderRequest メモ

`OrderRequest` は抽象的な注文 DTO。Transport/Adapter はここでのパラメータを取引所固有のリクエストにマッピングする。

## 必須パラメータ組み合わせ
- MARKET: `Size` のみ必須。
- LIMIT: `Size` + `Price` が必須。
- STOP: `Size` + `TriggerPrice` が必須（`Price` は不要）。
- STOP_LIMIT: `Size` + `TriggerPrice` + `Price` が必須。
- `TimeInForce` は LIMIT/STOP_LIMIT のみ有効。取引所が非対応の場合は無視またはエラーに正規化する。

## バリデーションヒント
- `PriceIncrement` / `SizeIncrement` / `MinSize` / `MaxSize` / `MinNotional` は任意フィールド。`ExchangeInfo` から値を流し、利用側の事前チェックに使う。
- 手数料の精算通貨を考慮する場合、`ExchangeMarketInfo.FeeCurrency`（null=約定通貨、明示時はその通貨）と `FeeType`（Percentage/Flat、負値でリベートを表現可）を参照し、必要なら注文前に約定後の受取額を試算する。
- 欠損している場合は取引所デフォルトに従うことを意味する。アダプターは存在する値のみをバリデーションに用いる。
- `ClientOrderId` は重複不可を想定。未対応の取引所は無視するか、適切なエラーに正規化する。

## 推奨ヘルパ
- 利用側で簡易チェックを行う拡張メソッド（例: `ValidateAgainst(ExchangeMarketInfo market)`）を用意すると、アダプター固有のエラーを呼び出し前に防ぎやすい。
