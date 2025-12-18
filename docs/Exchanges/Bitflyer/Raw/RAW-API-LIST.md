# bitFlyer RAW API List

Exchange.Bitflyer アダプターで利用している主要な REST エンドポイント（2025-01 時点）。

## Public API
- `GET /v1/ticker` (別名 `/v1/getticker`)
  - 用途: ティッカー取得
  - 対応コード: `BitflyerMarketApi.GetTickerAsync` → `IBitflyerPublicApi.GetTickerRawAsync`
- `GET /v1/board` (別名 `/v1/getboard`)
  - 用途: 板情報取得
  - 対応コード: `BitflyerMarketApi.GetOrderBookAsync` → `IBitflyerPublicApi.GetBoardRawAsync`
- `GET /v1/executions`
  - 用途: 市場全体の約定（歩み値）取得
  - 対応コード: `BitflyerMarketApi.GetMarketExecutionsAsync` → `IBitflyerPublicApi.GetExecutionsRawAsync`
- `GET /v1/markets`
  - 用途: 取扱い銘柄一覧
  - 対応コード: `BitflyerExchangeInfoApi` 経由で利用
- その他の公開情報（ヘルスチェック/ボード状態など）
  - `GET /v1/gethealth`, `GET /v1/getboardstate`, `GET /v1/getfunding_rate` などは Raw に用意（現状アダプターでは未使用）。

## Private API（Account / Margin）
- `GET /v1/me/getbalance`
  - 用途: 残高取得
  - 対応コード: `BitflyerAccountApi.GetBalancesAsync`, `BitflyerMarginApi.GetBalancesAsync`
- `GET /v1/me/getcollateral`
  - 用途: 証拠金取得
  - 対応コード: `BitflyerMarginApi.GetCollateralAsync`
- `GET /v1/me/getpositions`
  - 用途: 建玉一覧取得
  - 対応コード: `BitflyerMarginApi.GetOpenPositionsAsync`
- `GET /v1/me/getexecutions`
  - 用途: 口座約定履歴取得
  - 対応コード: `BitflyerAccountApi.GetAccountExecutionsAsync`, `BitflyerMarginApi.GetAccountExecutionsAsync`
- `GET /v1/me/getchildorders`
  - 用途: 子注文一覧取得
  - 対応コード: `BitflyerTradingApi.GetOrdersAsync`

## Private API（Trading）
- `POST /v1/me/sendchildorder`
  - 用途: 子注文発注（成行/指値）
  - 対応コード: `BitflyerTradingApi.PlaceLimitOrderAsync`, `PlaceMarketOrderAsync`
- `POST /v1/me/cancelchildorder`
  - 用途: 子注文キャンセル
  - 対応コード: `BitflyerTradingApi.CancelOrderAsync`
- `POST /v1/me/cancelallchildorders`
  - Raw に存在（全キャンセル）。現行アダプターでは未使用。
- 親注文（OCO/IFDOCOなど）
  - `POST /v1/me/sendparentorder`, `POST /v1/me/cancelparentorder`, `GET /v1/me/getparentorders`, `GET /v1/me/getparentorder`
  - Raw に定義済み。現行アダプターでは未使用。

## 未実装/NotSupported
- ローソク足（candlesticks）: Public REST 経由では未サポート。`GetCandlesticksAsync` は NotSupported を返す。
- WebSocket API: このアダプターでは扱わない（REST のみ）。

## 補足
- product_code 例: `BTC_JPY`, `ETH_JPY`, `FX_BTC_JPY`
- Public API はパラメータ `product_code` を大文字スネークケースで指定。
- Private API は認証ヘッダが必要（ACCESS-KEY / ACCESS-TIMESTAMP / ACCESS-SIGN / ACCESS-NONCE）。
- エラー時は bitFlyer 固有のコードを `ExchangeApiException` にマッピングして返す。

